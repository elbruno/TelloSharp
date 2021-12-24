using System.Text;
using static TelloSharp.Messages;

namespace TelloSharp
{
    public class Tello
    {
        private UdpUser _client;
        private FlyData _state;
        private Messages _messages;
        public FlightData fd = new();
        private FileInternal fileTemp;
        private DateTime lastMessageTime;//for connection timeouts.
        private readonly UdpListener videoServer = new UdpListener(6040);


        private short _wifiStrength = 0;
        private short _controlSequence = 1;
        public bool _connected = false;

        public event EventHandler<TelloStateEventArgs> OnUpdate;
        public event EventHandler<ConnectionState> OnConnection;

        public string picPath;      //todo redo this. 
        public string picFilePath;  //todo redo this. 
        public int picMode = 0;     //pic or vid aspect ratio.

        public int iFrameRate = 5;  //How often to ask for iFrames in 50ms. Ie 2=10x 5=4x 10=2xSecond 5 = 4xSecond

        private ushort sequence = 1;

        public enum ConnectionState
        {
            Disconnected,
            Connecting,
            Connected,
            Paused,
            UnPausing
        }

        public ConnectionState _connectionState = ConnectionState.Disconnected;
        private CancellationTokenSource _cancelTokens = new CancellationTokenSource();
        public CancellationTokenSource CancelTokens { get => _cancelTokens; set => _cancelTokens = value; }

        public FlyData State => _state;

        public Tello()
        {
            _state = new FlyData(this);
            _messages = new Messages();
            _client = new UdpUser();
        }

        public void TakeOff()
        {
            var pkt = _messages.NewPacketAsBytes(PacketType.ptSet, MessageTypes.msgDoTakeoff, _controlSequence++, 0);
            _client.Send(pkt);
        }

        public void StopSmartVideo(SmartVideoCmd cmd)
        {
            var pkt = _messages.NewPacket(PacketType.ptSet, MessageTypes.msgDoSmartVideo, _controlSequence++, 1);
            pkt.payload[0] = (byte)cmd;
            var buffer = _messages.PacketToBuffer(pkt);
            _client.Send(buffer);
        }

        public void Hover()
        {
            ctrlRx = 0;
            ctrlRy = 0;
            ctrlLx = 0;
            ctrlLy = 0;
            SendStickUpdate();
        }

        /// <summary>
        /// StartSmartVideo
        /// </summary>
        /// <param name="cmd"></param>
        public void StartSmartVideo(SmartVideoCmd cmd)
        {
            var pkt = _messages.NewPacket(PacketType.ptSet, MessageTypes.msgDoSmartVideo, _controlSequence++, 1);
            pkt.payload[0] = (byte)((byte)cmd | 0x01);
            var buffer = _messages.PacketToBuffer(pkt);
            _client.Send(buffer);
        }

        public void ThrowTakeOff()
        {
            var pkt = _messages.NewPacketAsBytes(PacketType.ptGet, MessageTypes.msgDoThrowTakeoff, _controlSequence++, 0);
            _client.Send(pkt);
        }

        public void Land()
        {
            var pkt = _messages.NewPacket(PacketType.ptSet, MessageTypes.msgDoLand, _controlSequence++, 1);
            pkt.payload[0] = 0; // see StopLanding() for use of this field
            var buffer = _messages.PacketToBuffer(pkt);
            _client.Send(buffer);
        }

        public void StopLanding()
        {
            var pkt = _messages.NewPacket(PacketType.ptSet, MessageTypes.msgDoLand, _controlSequence++, 1);
            pkt.payload[0] = 1;
            var buffer = _messages.PacketToBuffer(pkt);
            _client.Send(buffer);
        }

        public void PalmLanding()
        {
            var pkt = _messages.NewPacket(PacketType.ptSet, MessageTypes.msgDoPalmLand, _controlSequence++, 1);
            pkt.payload[0] = 0;
            var buffer = _messages.PacketToBuffer(pkt);
            _client.Send(buffer);
        }

        public void Bounce()
        {
            var pkt = _messages.NewPacket(PacketType.ptSet, MessageTypes.msgDoBounce, _controlSequence++, 1);

            if (ctrlBouncing)
            {
                pkt.payload[0] = 0x31;
                ctrlBouncing = false;
            }
            else
            {
                pkt.payload[0] = 0x30;
                ctrlBouncing = true;
            }

            var buffer = _messages.PacketToBuffer(pkt);
            _client.Send(buffer);
        }

        private void SendStickUpdate()
        {
            var pkt = new Packet();

            // populate the command packet fields we need
            pkt.header = msgHdr;
            pkt.toDrone = true;
            pkt.packetType = PacketType.ptData2;
            pkt.messageID = MessageTypes.msgSetStick;
            pkt.sequence = 0;
            pkt.payload = new byte[11];
            _controlSequence = 0;

            // This packing of the joystick data is just vile...
            int packedAxes = jsInt16ToTello(ctrlRx) & 0x07ff;
            packedAxes |= (jsInt16ToTello(ctrlRy) & 0x07ff) << 11;
            packedAxes |= (jsInt16ToTello(ctrlLy) & 0x07ff) << 22;
            packedAxes |= (jsInt16ToTello(ctrlLx) & 0x07ff) << 33;

            if (ctrlSportsMode)
            {
                packedAxes |= 1 << 44;
            }

            pkt.payload[0] = (byte)packedAxes;
            pkt.payload[1] = (byte)(packedAxes >> 8);
            pkt.payload[2] = (byte)(packedAxes >> 16);
            pkt.payload[3] = (byte)(packedAxes >> 24);
            pkt.payload[4] = (byte)(packedAxes >> 32);
            pkt.payload[5] = (byte)(packedAxes >> 40);

            var now = DateTime.Now;
            pkt.payload[6] = (byte)now.Hour;
            pkt.payload[7] = (byte)now.Minute;
            pkt.payload[8] = (byte)now.Second;

            var ms = DateTimeOffset.Now.ToUnixTimeSeconds() / 1000000;
            pkt.payload[9] = (byte)(ms & 0xff);
            pkt.payload[10] = (byte)(ms >> 8);

            var buffer = _messages.PacketToBuffer(pkt);
            _client.Send(buffer);
        }


        private int jsInt16ToTello(short sv)
        {
            // sv is in range -32768 to 32767, we need 660 to 1388 where 0 => 1024
            //return uint64((sv / 90) + 1024)
            // Changed this as new info (Oct 18) suggests range should be 364 to 1684...
            return (int)(sv / 49.672 + 1024);
        }

        private void UpdateSticks(StickMessage message)
        {
            ctrlRx = message.Rx;
            ctrlRy = message.Ry;
            ctrlLx = message.Lx;
            ctrlLy = message.Ly;
        }

        private void SendDateTime()
        {
            var pkt = new Packet();

            // populate the command packet fields we need
            pkt.header = msgHdr;
            pkt.toDrone = true;
            pkt.packetType = PacketType.ptData1;
            pkt.messageID = MessageTypes.msgSetDateTime;
            pkt.payload = new byte[11];

            _controlSequence++;
            pkt.sequence = _controlSequence;

            pkt.payload = new byte[15];
            pkt.payload[0] = 0;

            var now = DateTime.Now;
            pkt.payload[1] = (byte)now.Year;
            pkt.payload[2] = (byte)((byte)now.Year >> 8);
            pkt.payload[3] = (byte)now.Month;
            pkt.payload[4] = (byte)((byte)now.Month >> 8);
            pkt.payload[5] = (byte)now.Day;
            pkt.payload[6] = (byte)((byte)now.Day >> 8);
            pkt.payload[7] = (byte)now.Hour;
            pkt.payload[8] = (byte)((byte)now.Hour >> 8);
            pkt.payload[9] = (byte)now.Minute;
            pkt.payload[10] = (byte)((byte)now.Minute >> 8);
            pkt.payload[11] = (byte)now.Second;
            pkt.payload[12] = (byte)((byte)now.Second >> 8);

            var ms = DateTimeOffset.Now.ToUnixTimeSeconds() / 1000000;
            pkt.payload[13] = (byte)ms;
            pkt.payload[14] = (byte)((byte)ms >> 8);

            var buffer = _messages.PacketToBuffer(pkt);
            _client.Send(buffer);
        }

        public void Forward(int pct)
        {
            short speed = 0;

            if (pct > 0)
            {
                speed = (short)(pct * 327); // /100 * 32767
            }
            UpdateSticks(new StickMessage() { Rx = 0, Ry = speed, Lx = 0, Ly = 0 });
        }

        public void Backward(int pct)
        {
            short speed = 0;

            if (pct > 0)
            {
                speed = (short)(pct * 327); // /100 * 32767
            }
            UpdateSticks(new StickMessage() { Rx = 0, Ry = (short)-speed, Lx = 0, Ly = 0 });
        }

        public void Left(int pct)
        {
            short speed = 0;

            if (pct > 0)
            {
                speed = (short)(pct * 327); // /100 * 32767
            }
            UpdateSticks(new StickMessage() { Rx = (short)-speed, Ry = 0, Lx = 0, Ly = 0 });
        }

        public void Right(int pct)
        {
            short speed = 0;

            if (pct > 0)
            {
                speed = (short)(pct * 327); // /100 * 32767
            }
            UpdateSticks(new StickMessage() { Rx = speed, Ry = 0, Lx = 0, Ly = 0 });
        }

        public void Up(int pct)
        {
            short speed = 0;

            if (pct > 0)
            {
                speed = (short)(pct * 327); // /100 * 32767
            }
            UpdateSticks(new StickMessage() { Rx = 0, Ry = 0, Lx = 0, Ly = speed });
        }

        public void Down(int pct)
        {
            short speed = 0;

            if (pct > 0)
            {
                speed = (short)(pct * 327); // /100 * 32767
            }
            UpdateSticks(new StickMessage() { Rx = 0, Ry = 0, Lx = 0, Ly = (short)-speed });
        }

        public void ClockWise(int pct)
        {
            short speed = 0;

            if (pct > 0)
            {
                speed = (short)(pct * 327); // /100 * 32767
            }
            UpdateSticks(new StickMessage() { Rx = 0, Ry = 0, Lx = speed, Ly = 0 });
        }

        public void AntiClockWise(int pct)
        {
            short speed = 0;

            if (pct > 0)
            {
                speed = (short)(pct * 327); // /100 * 32767
            }
            UpdateSticks(new StickMessage() { Rx = 0, Ry = 0, Lx = (short)-speed, Ly = 0 });
        }

        public void SetSportsMode(bool sports)
        {
            ctrlSportsMode = sports;
        }

        public void SetFastMode()
        {
            SetSportsMode(true);
        }

        public void SetSlowMode()
        {
            SetSportsMode(false);
        }

        public void GetLowBatteryThreshold()
        {
            var pkt = _messages.NewPacketAsBytes(PacketType.ptGet, MessageTypes.msgQueryLowBattThresh, _controlSequence++, 0);
            _client.Send(pkt);
        }

        public void GetMaxHeight()
        {
            var pkt = _messages.NewPacketAsBytes(PacketType.ptGet, MessageTypes.msgQueryHeightLimit, _controlSequence++, 0);
            _client.Send(pkt);
        }

        public void GetSSID()
        {
            var pkt = _messages.NewPacketAsBytes(PacketType.ptGet, MessageTypes.msgQuerySSID, _controlSequence++, 0);
            _client.Send(pkt);
        }

        public void GetVersion()
        {
            var pkt = _messages.NewPacketAsBytes(PacketType.ptGet, MessageTypes.msgQueryVersion, _controlSequence++, 0);
            _client.Send(pkt);
        }

        public void SetLowBatteryThreshold(byte thr)
        {
            var pkt = _messages.NewPacket(PacketType.ptSet, MessageTypes.msgSetLowBattThresh, _controlSequence++, 1);
            pkt.payload[0] = thr;
            _client.Send(_messages.PacketToBuffer(pkt));
        }

        public void RequestIFrame()
        {
            byte[]? iframePacket = new byte[] { 0xcc, 0x58, 0x00, 0x7c, 0x60, 0x25, 0x00, 0x00, 0x00, 0x6c, 0x95 };
            _client.Send(iframePacket);
        }

        public void QueryUnk(int cmd)
        {
            byte[]? packet = new byte[] { 0xcc, 0x58, 0x00, 0x7c, 0x48, 0xff, 0x00, 0x06, 0x00, 0xe9, 0xb3 };
            packet[5] = (byte)cmd;
            SetPacketSequence(packet);
            SetPacketCRC(packet);
            _client.Send(packet);
        }

        public void QueryAttAngle()
        {
            byte[]? packet = new byte[] { 0xcc, 0x58, 0x00, 0x7c, 0x48, 0x59, 0x10, 0x06, 0x00, 0xe9, 0xb3 };
            SetPacketSequence(packet);
            SetPacketCRC(packet);
            _client.Send(packet);
        }

        public void queryMaxHeight()
        {
            byte[]? packet = new byte[] { 0xcc, 0x58, 0x00, 0x7c, 0x48, 0x56, 0x10, 0x06, 0x00, 0xe9, 0xb3 };
            SetPacketSequence(packet);
            SetPacketCRC(packet);
            _client.Send(packet);
        }

        public void setAttAngle(float angle)
        {
            //                                          crc    typ  cmdL  cmdH  seqL  seqH  ang1  ang2 ang3  ang4  crc   crc
            byte[]? packet = new byte[] { 0xcc, 0x78, 0x00, 0x27, 0x68, 0x58, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x5b, 0xc5 };

            //payload
            byte[] bytes = BitConverter.GetBytes(angle);
            packet[9] = bytes[0];
            packet[10] = bytes[1];
            packet[11] = bytes[2];
            packet[12] = bytes[3];

            SetPacketSequence(packet);
            SetPacketCRC(packet);

            _client.Send(packet);

            QueryAttAngle();//refresh
        }

        public void SetEis(int value)
        {
            //                                          crc    typ  cmdL  cmdH  seqL  seqH  valL  crc   crc
            byte[]? packet = new byte[] { 0xcc, 0x60, 0x00, 0x27, 0x68, 0x24, 0x00, 0x09, 0x00, 0x00, 0x5b, 0xc5 };
            //payload
            packet[9] = (byte)(value & 0xff);

            SetPacketSequence(packet);
            SetPacketCRC(packet);

            _client.Send(packet);
        }

        public void Flip(FlipType dir)
        {
            var pkt = _messages.NewPacket(PacketType.ptFlip, MessageTypes.msgDoFlip, _controlSequence++, 1);
            pkt.payload[0] = (byte)dir;
            var buffer = _messages.PacketToBuffer(pkt);
            _client.Send(buffer);


            //                                          crc    typ  cmdL  cmdH  seqL  seqH  dirL  crc   crc
            //byte[]? packet = new byte[] { 0xcc, 0x60, 0x00, 0x27, 0x70, 0x5c, 0x00, 0x09, 0x00, 0x00, 0x5b, 0xc5 };

            //payload
            //packet[9] = (byte)(dir & 0xff);
            //SetPacketSequence(packet);
            //SetPacketCRC(packet);
            //_client.Send(packet);
        }

        public void SetJpgQuality(int quality)
        {
            //                                          crc    typ  cmdL  cmdH  seqL  seqH  quaL  crc   crc
            byte[]? packet = new byte[] { 0xcc, 0x60, 0x00, 0x27, 0x68, 0x37, 0x00, 0x09, 0x00, 0x00, 0x5b, 0xc5 };

            //payload
            packet[9] = (byte)(quality & 0xff);

            SetPacketSequence(packet);
            SetPacketCRC(packet);

            _client.Send(packet);
        }

        public void SetEV(int ev)
        {
            //                                          crc    typ  cmdL  cmdH  seqL  seqH  evL  crc   crc
            byte[]? packet = new byte[] { 0xcc, 0x60, 0x00, 0x27, 0x68, 0x34, 0x00, 0x09, 0x00, 0x00, 0x5b, 0xc5 };

            byte evb = (byte)(ev - 9);//Exposure goes from -9 to +9
                                      //payload
            packet[9] = evb;

            SetPacketSequence(packet);
            SetPacketCRC(packet);

            _client.Send(packet);
        }

        public void SetVideoBitRate(int rate)
        {
            //                                          crc    typ  cmdL  cmdH  seqL  seqH  rateL  crc   crc
            byte[]? packet = new byte[] { 0xcc, 0x60, 0x00, 0x27, 0x68, 0x20, 0x00, 0x09, 0x00, 0x00, 0x5b, 0xc5 };

            //payload
            packet[9] = (byte)rate;

            SetPacketSequence(packet);
            SetPacketCRC(packet);

            _client.Send(packet);
        }

        public void SetVideoDynRate(int rate)
        {
            //                                          crc    typ  cmdL  cmdH  seqL  seqH  rateL  crc   crc
            byte[]? packet = new byte[] { 0xcc, 0x60, 0x00, 0x27, 0x68, 0x21, 0x00, 0x09, 0x00, 0x00, 0x5b, 0xc5 };

            //payload
            packet[9] = (byte)rate;

            SetPacketSequence(packet);
            SetPacketCRC(packet);

            _client.Send(packet);
        }

        public void SetVideoRecord(int n)
        {
            //                                          crc    typ  cmdL  cmdH  seqL  seqH  nL  crc   crc
            byte[]? packet = new byte[] { 0xcc, 0x60, 0x00, 0x27, 0x68, 0x32, 0x00, 0x09, 0x00, 0x00, 0x5b, 0xc5 };

            //payload
            packet[9] = (byte)n;

            SetPacketSequence(packet);
            SetPacketCRC(packet);

            _client.Send(packet);
        }

        /*TELLO_CMD_SWITCH_PICTURE_VIDEO
        49 0x31
        0x68
        switching video stream mode
        data: u8 (1=video, 0=photo)
        */
        public void SetPicVidMode(int mode)
        {
            //                                          crc    typ  cmdL  cmdH  seqL  seqH  modL  crc   crc
            byte[]? packet = new byte[] { 0xcc, 0x60, 0x00, 0x27, 0x68, 0x31, 0x00, 0x00, 0x00, 0x00, 0x5b, 0xc5 };

            picMode = mode;

            //payload
            packet[9] = (byte)(mode & 0xff);

            SetPacketSequence(packet);
            SetPacketCRC(packet);

            _client.Send(packet);
        }

        public void TakePicture()
        {
            //                                         crc   typ  cmdL  cmdH  seqL  seqH  crc   crc
            byte[]? packet = new byte[] { 0xcc, 0x58, 0x00, 0x7c, 0x68, 0x30, 0x00, 0x06, 0x00, 0xe9, 0xb3 };
            SetPacketSequence(packet);
            SetPacketCRC(packet);
            _client.Send(packet);
            Console.WriteLine("PIC START");
        }

        public void SendAckFilePiece(byte endFlag, ushort fileId, uint pieceId)
        {
            //                                          crc    typ  cmdL  cmdH  seqL  seqH  byte  nL    nH    n2L                     crc   crc
            byte[]? packet = new byte[] { 0xcc, 0x90, 0x00, 0x27, 0x50, 0x63, 0x00, 0xf0, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x5b, 0xc5 };

            packet[9] = endFlag;
            packet[10] = (byte)(fileId & 0xff);
            packet[11] = (byte)((fileId >> 8) & 0xff);

            packet[12] = ((byte)(int)(0xFF & pieceId));
            packet[13] = ((byte)(int)(pieceId >> 8 & 0xFF));
            packet[14] = ((byte)(int)(pieceId >> 16 & 0xFF));
            packet[15] = ((byte)(int)(pieceId >> 24 & 0xFF));

            SetPacketSequence(packet);
            SetPacketCRC(packet);

            _client.Send(packet);
        }

        public void SendAckFileSize()
        {
            var pkt = _messages.NewPacketAsBytes(PacketType.ptData1, MessageTypes.msgFileSize, _controlSequence++, 1);
            _client.Send(pkt);            
        }

        public void SendAckFileDone(int size)
        {
            //                                          crc    typ  cmdL  cmdH  seqL  seqH  fidL  fidH  size  size  size  size  crc   crc
            byte[]? packet = new byte[] { 0xcc, 0x88, 0x00, 0x24, 0x48, 0x64, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x5b, 0xc5 };

            //packet[9] = (byte)(fileid & 0xff);
            //packet[10] = (byte)((fileid >> 8) & 0xff);

            packet[11] = ((byte)(0xFF & size));
            packet[12] = ((byte)(size >> 8 & 0xFF));
            packet[13] = ((byte)(size >> 16 & 0xFF));
            packet[14] = ((byte)(size >> 24 & 0xFF));
            SetPacketSequence(packet);
            SetPacketCRC(packet);

            _client.Send(packet);
        }

        public void SendAckLog(byte[] id)
        {
            var pkt = _messages.NewPacket(PacketType.ptData1, MessageTypes.msgLogHeader, _controlSequence++, 3);
            pkt.payload[1] = id[0];
            pkt.payload[2] = id[1];
            _client.Send(_messages.PacketToBuffer(pkt));

            //                                          crc    typ  cmdL  cmdH  seqL  seqH  unk   idL   idH   crc   crc
            //byte[]? packet = new byte[] { 0xcc, 0x70, 0x00, 0x27, 0x50, 0x50, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x5b, 0xc5 };

            //byte[]? ba = BitConverter.GetBytes(cmd);
            //packet[5] = ba[0];
            //packet[6] = ba[1];

            //ba = BitConverter.GetBytes(id);
            //packet[10] = ba[0];
            //packet[11] = ba[1];

            //SetPacketSequence(packet);
            //SetPacketCRC(packet);

            //_client.Send(packet);
        }

        //this might not be working right 
        public void SendAckLogConfig(short cmd, ushort id, int n2)
        {
            //                                          crc    typ  cmdL  cmdH  seqL  seqH  unk   idL   idH   n2L   n2H   n2L   n2H   crc   crc
            byte[]? packet = new byte[] { 0xcc, 0xd0, 0x00, 0x27, 0x88, 0x50, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x5b, 0xc5 };

            byte[]? ba = BitConverter.GetBytes(cmd);
            packet[5] = ba[0];
            packet[6] = ba[1];

            ba = BitConverter.GetBytes(id);
            packet[10] = ba[0];
            packet[11] = ba[1];

            packet[12] = ((byte)(0xFF & n2));
            packet[13] = ((byte)(n2 >> 8 & 0xFF));
            packet[14] = ((byte)(n2 >> 16 & 0xFF));
            packet[15] = ((byte)(n2 >> 24 & 0xFF));

            //ba = BitConverter.GetBytes(n2);
            //packet[12] = ba[0];
            //packet[13] = ba[1];
            //packet[14] = ba[2];
            //packet[15] = ba[3];

            SetPacketSequence(packet);
            SetPacketCRC(packet);

            _client.Send(packet);
        }

        private void SetPacketSequence(byte[] packet)
        {
            packet[7] = (byte)(sequence & 0xff);
            packet[8] = (byte)((sequence >> 8) & 0xff);
            sequence++;
        }

        private void SetPacketCRC(byte[] packet)
        {
            Crc.CalcUCRC(packet, 4);
            Crc.CalcCrc(packet, packet.Length);
        }

        public void SetEIS(int eis)
        {
        }

        public void SetXAxis(float[] axis)
        {
            //joyAxis = axis.Take(5).ToArray(); ;
            //joyAxis[4] = axis[7];
            //joyAxis[3] = axis[11];
        }

        private void Disconnect()
        {
            _cancelTokens.Cancel();
            _connected = false;

            if (_connectionState != ConnectionState.Disconnected)
            {
                OnConnection?.Invoke(this, ConnectionState.Disconnected);
            }

            _connectionState = ConnectionState.Disconnected;
        }

        private void ConnectClient()
        {
            _client = UdpUser.ConnectTo("192.168.10.1", 8889);

            _connectionState = ConnectionState.Connecting;
            OnConnection?.Invoke(this, _connectionState);

            byte[] connectPacket = Encoding.UTF8.GetBytes("conn_req:\x00\x00");
            connectPacket[connectPacket.Length - 2] = 0x96;
            connectPacket[connectPacket.Length - 1] = 0x17;
            _client.Send(connectPacket);
        }

        //Pause connections. Used by aTello when app paused.
        public void ConnectionSetPause(bool bPause)
        {
            //NOTE only pause if connected and only unpause (connect) when paused.
            if (bPause && _connectionState == ConnectionState.Connected)
            {
                _connectionState = ConnectionState.Paused;
                OnConnection?.Invoke(this, _connectionState);
            }
            else if (bPause == false && _connectionState == ConnectionState.Paused)
            {
                //NOTE:send unpause and not connection event
                OnConnection?.Invoke(this, ConnectionState.UnPausing);
                _connectionState = ConnectionState.Connected;
            }
        }

        private byte[] picbuffer = new byte[3000 * 1024];
        private bool[] picChunkState;
        private bool[] picPieceState;
        private uint picBytesRecived;
        private uint picBytesExpected;
        private uint picExtraPackets;
        public bool picDownloading;
        private ushort maxPieceNum = 0;

        private void StartListeners()
        {
            _cancelTokens = new CancellationTokenSource();
            CancellationToken token = _cancelTokens.Token;
            _state = new FlyData(this);


            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    try
                    {
                        if (token.IsCancellationRequested)//handle canceling thread.
                        {
                            break;
                        }

                        Received received = await _client.Receive();
                        lastMessageTime = DateTime.Now;

                        if (_connectionState == ConnectionState.Connecting)
                        {
                            if (received.Message.StartsWith("conn_ack"))
                            {
                                _connected = true;
                                _connectionState = ConnectionState.Connected;
                                OnConnection?.Invoke(this, _connectionState);

                                StartHeartbeat();
                                RequestIFrame();
                                Console.WriteLine("Tello Connected!");
                                continue;
                            }
                        }

                        var pkt = _messages.BufferToPacket(received.bytes);

                        switch (pkt.messageID)
                        {
                            case MessageTypes.msgFileSize:
                                picFilePath = picPath + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".jpg";
                                Messages.FileInfo fi = _messages.PayloadToFileInfo(pkt.payload);
                                fileTemp.fID = fi.fId;
                                fileTemp.filetype = fi.fileType;
                                fileTemp.expectedSize = fi.FileSize;
                                fileTemp.accumSize = 0;
                                fileTemp.filePieces = new FilePiece[1024];
                                SendAckFileSize();
                                break;
                            case MessageTypes.msgFileData:
                                int start = 9;
                                ushort fileNum = BitConverter.ToUInt16(received.bytes, start);
                                start += 2;
                                uint pieceNum = BitConverter.ToUInt32(received.bytes, start);
                                start += 4;
                                uint seqNum = BitConverter.ToUInt32(received.bytes, start);
                                start += 4;
                                ushort size = BitConverter.ToUInt16(received.bytes, start);
                                start += 2;

                                maxPieceNum = (ushort)Math.Max(pieceNum, maxPieceNum);
                                if (!picChunkState[seqNum])
                                {
                                    Array.Copy(received.bytes, start, picbuffer, seqNum * 1024, size);
                                    picBytesRecived += size;
                                    picChunkState[seqNum] = true;

                                    for (int p = 0; p < picChunkState.Length / 8; p++)
                                    {
                                        bool done = true;
                                        for (int s = 0; s < 8; s++)
                                        {
                                            if (!picChunkState[(p * 8) + s])
                                            {
                                                done = false;
                                                break;
                                            }
                                        }
                                        if (done && !picPieceState[p])
                                        {
                                            picPieceState[p] = true;
                                            SendAckFilePiece(0, fileNum, (uint)p);
                                        }
                                    }
                                    if (picFilePath != null && picBytesRecived >= picBytesExpected)
                                    {
                                        picDownloading = false;

                                        SendAckFilePiece(1, 0, maxPieceNum);
                                        SendAckFileDone((int)picBytesExpected);

                                        OnUpdate?.Invoke(this, new TelloStateEventArgs(State, 100));

                                        Console.WriteLine("\nDONE PN:" + pieceNum + " max: " + maxPieceNum);

                                        //Save raw data minus sequence.
                                        using (FileStream? stream = new FileStream(picFilePath, FileMode.Append))
                                        {
                                            stream.Write(picbuffer, 0, (int)picBytesExpected);
                                        }
                                    }
                                }
                                else
                                {
                                    picExtraPackets++;//for debugging.
                                }
                                break;
                            case MessageTypes.msgFlightStatus:
                                var tmpFd = _messages.PayloadToFlightData(pkt.payload);
                                // not all fields are sent...
                                fd.BatteryCritical = tmpFd.BatteryCritical;
                                fd.BatteryLow = tmpFd.BatteryLow;
                                fd.BatteryMilliVolts = tmpFd.BatteryMilliVolts;
                                fd.BatteryPercentage = tmpFd.BatteryPercentage;
                                fd.BatteryState = tmpFd.BatteryState;
                                fd.CameraState = tmpFd.CameraState;
                                fd.DownVisualState = tmpFd.DownVisualState;
                                fd.DroneFlyTimeLeft = tmpFd.DroneFlyTimeLeft;
                                fd.DroneHover = tmpFd.DroneHover;
                                fd.EastSpeed = tmpFd.EastSpeed;
                                fd.ElectricalMachineryState = tmpFd.ElectricalMachineryState;
                                fd.EmOpen = tmpFd.EmOpen;
                                fd.ErrorState = tmpFd.ErrorState;
                                fd.FactoryMode = tmpFd.FactoryMode;
                                fd.Flying = tmpFd.Flying;
                                fd.FlyMode = tmpFd.FlyMode;
                                fd.FlyTime = tmpFd.FlyTime;
                                fd.FrontIn = tmpFd.FrontIn;
                                fd.FrontLSC = tmpFd.FrontLSC;
                                fd.FrontOut = tmpFd.FrontOut;
                                fd.GravityState = tmpFd.GravityState;
                                fd.Height = tmpFd.Height;
                                fd.ImuCalibrationState = tmpFd.ImuCalibrationState;
                                fd.ImuState = tmpFd.ImuState;
                                fd.NorthSpeed = tmpFd.NorthSpeed;
                                fd.OnGround = tmpFd.OnGround;
                                fd.OutageRecording = tmpFd.OutageRecording;
                                fd.PowerState = tmpFd.PowerState;
                                fd.PressureState = tmpFd.PressureState;
                                fd.ThrowFlyTimer = tmpFd.ThrowFlyTimer;
                                fd.VerticalSpeed = (short)-tmpFd.VerticalSpeed; // seems to be inverted
                                fd.WindState = tmpFd.WindState;
                                break;
                            case MessageTypes.msgLightStrength:
                                fd.LightStrength = pkt.payload[0];
                                fd.LightStrengthUpdated = DateTime.Now;
                                break;
                            case MessageTypes.msgLogConfig:
                                break;
                            case MessageTypes.msgLogHeader:
                                SendAckLog(pkt.payload.Take(2).ToArray());
                                break;
                            case MessageTypes.msgLogData:
                                ParseLogPacket(pkt.payload);
                                break;
                            case MessageTypes.msgQueryHeightLimit:
                                fd.MaxHeight = pkt.payload[1];
                                break;
                            case MessageTypes.msgQueryLowBattThresh:
                                fd.LowBatteryThreshold = pkt.payload[1];
                                break;
                            case MessageTypes.msgQuerySSID:
                                fd.SSID = pkt.payload.Skip(2).ToString();
                                break;
                            case MessageTypes.msgQueryVersion:
                                fd.Version = pkt.payload.Skip(1).ToString();
                                break;
                            case MessageTypes.msgQueryVideoBitrate:
                                fd.VideoBitrate = (VBR)pkt.payload[0];
                                break;
                            case MessageTypes.msgSetDateTime:
                                SendDateTime();
                                break;
                            case MessageTypes.msgSetLowBattThresh:
                                break;
                            case MessageTypes.msgSmartVideoStatus:
                                break;
                            case MessageTypes.msgSwitchPicVideo:
                                break;
                            case MessageTypes.msgWifiStrength:
                                fd.WifiStrength = pkt.payload[0];
                                break;
                            default:
                                break;

                        }
                        OnUpdate?.Invoke(this, new TelloStateEventArgs(State, pkt.messageID));
                    }
                    catch (Exception eex)
                    {
                        Console.WriteLine("Receive thread error:" + eex.Message);
                        Disconnect();
                        break;
                    }
                }
            }, token);
        }

       

        private void SendFileDone(short fID, object accumSize)
        {
            var pkt = _messages.NewPacket(PacketType.ptGet, MessageTypes.msgFileDone, _controlSequence++, 6);
            pkt.payload[0] = (byte)fID;
            pkt.payload[1] = (byte)((byte)fID >> 8);
            pkt.payload[2] = (byte)accumSize;
            pkt.payload[3] = (byte)((byte)accumSize >> 8);
            pkt.payload[4] = (byte)((byte)accumSize >> 16);
            pkt.payload[5] = (byte)((byte)accumSize >> 24);
            _client.Send(_messages.PacketToBuffer(pkt));
        }

        private void SendFileAckPiece(byte done, short fID, uint pieceNum)
        {
            var pkt = _messages.NewPacket(PacketType.ptData1, MessageTypes.msgFileData, _controlSequence++, 7);
            pkt.payload[0] = done;
            pkt.payload[1] = (byte)fID;
            pkt.payload[2] = (byte)(fID >> 8);
            pkt.payload[3] = (byte)pieceNum;
            pkt.payload[4] = (byte)(pieceNum >> 8);
            pkt.payload[5] = (byte)((byte)pieceNum >> 16);
            pkt.payload[6] = (byte)((byte)pieceNum >> 24);
            _client.Send(_messages.PacketToBuffer(pkt));
        }

        private void ParseLogPacket(byte[] data)
        {
            var pos = 1;

            if (data.Length < 2)
            {
                return;
            }

            for (int i = 0; i < data.Length - 6; i++)
            {
                if (data[pos] != logRecordSeparator)
                {
                    break;
                }
            }

            var recLen = data[pos + 1] + data[pos + 2] << 8;
            var logRecType = data[pos + 4] + data[pos + 5] << 8;

            var xorBuf = new byte[256];
            var xorVal = data[pos + 6];

            switch (logRecType)
            {
                case (int)LogRecTypes.logRecNewMVO:
                    for (var i = 0; i < recLen && pos + i < data.Length; i++)
                    {
                        xorBuf[i] = (byte)(data[pos + i] ^ xorVal);

                    }
                    var offset = 10;
                    var flags = data[offset + 76];

                    if ((flags & LogValidVe.logValidVelX) != 0)
                    {
                        fd.MVO.VelocityX = (short)(xorBuf[offset + 2] + xorBuf[offset + 3] << 8);
                    }

                    if ((flags & LogValidVe.logValidVelY) != 0)
                    {
                        fd.MVO.VelocityY = (short)(xorBuf[offset + 4] + xorBuf[offset + 5] << 8);
                    }

                    if ((flags & LogValidVe.logValidVelZ) != 0)
                    {
                        fd.MVO.VelocityZ = (short)(-xorBuf[offset + 6] + xorBuf[offset + 7] << 8);
                    }

                    if ((flags & LogValidVe.logValidPosY) != 0 && (flags & LogValidVe.logValidPosX) != 0 && (flags & LogValidVe.logValidPosZ) != 0)
                    {
                        fd.MVO.PositionY = BitConverter.ToSingle(xorBuf.Skip(offset + 8).Take(offset + 13).ToArray());
                        fd.MVO.PositionX = BitConverter.ToSingle(xorBuf.Skip(offset + 12).Take(offset + 17).ToArray());
                        fd.MVO.PositionZ = BitConverter.ToSingle(xorBuf.Skip(offset + 16).Take(offset + 21).ToArray());
                    }
                    break;

                case (int)LogRecTypes.logRecIMU:

                    for (var i = 0; i < recLen && pos + i < data.Length; i++)
                    {
                        xorBuf[i] = (byte)(data[pos + i] ^ xorVal);
                    }
                    offset = 10;

                    fd.IMU.QuaternionW = BitConverter.ToSingle(xorBuf.Skip(offset + 48).Take(offset + 53).ToArray());
                    fd.IMU.QuaternionX = BitConverter.ToSingle(xorBuf.Skip(offset + 52).Take(offset + 57).ToArray());
                    fd.IMU.QuaternionY = BitConverter.ToSingle(xorBuf.Skip(offset + 56).Take(offset + 61).ToArray());
                    fd.IMU.QuaternionZ = BitConverter.ToSingle(xorBuf.Skip(offset + 60).Take(offset + 65).ToArray());
                    fd.IMU.Temperature = (short)(xorBuf[offset + 106] + xorBuf[offset + 107] << 8 / 100);

                    fd.IMU.Yaw = QuatToYawDeg(fd.IMU.QuaternionX, fd.IMU.QuaternionY, fd.IMU.QuaternionZ, fd.IMU.QuaternionW);
                    pos += recLen;
                    break;
                default:
                    break;
            }
        }



        private short QuatToYawDeg(float qX, float qY, float qZ, float qW)
        {
            const float degree = (float)(Math.PI / 180.0);
            var qqX = (int)qX;
            var qqY = (int)qY;
            var qqZ = (int)qZ;
            var qqW = (int)qW;
            var sqY = qqY * qqY;
            var sqZ = qqZ * qqZ;

            var sinY = 2.0 * (qqW * qqZ + qqX * qqY);
            var cosY = 1.0 - 2 * (sqY + sqZ);

            var yaw = (short)(Math.Round(Math.Atan2(sinY, cosY) / degree));

            return yaw;
        }

        public delegate float[] GetControllerDeligate();
        public GetControllerDeligate? GetControllerCallback;

        private void StartHeartbeat()
        {
            CancellationToken token = _cancelTokens.Token;

            Func<Task> function = Heartbeat(token);
            Task.Factory.StartNew(function, token);
        }

        private Func<Task> Heartbeat(CancellationToken token)
        {
            return async () =>
            {
                int tick = 0;
                while (true)
                {

                    try
                    {
                        if (token.IsCancellationRequested)
                        {
                            break;
                        }

                        if (_connectionState == ConnectionState.Connected)
                        {
                            SendControllerUpdate();

                            tick++;
                            if ((tick % iFrameRate) == 0)
                            {
                                RequestIFrame();
                            }
                        }
                        Thread.Sleep(40);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Heatbeat error:" + ex.Message);
                        if (ex.Message.StartsWith("Access denied") && _connectionState != ConnectionState.Paused)
                        {
                            Console.WriteLine("Heatbeat: access denied and not paused:" + ex.Message);
                            Disconnect();
                            break;
                        }

                        //Denied means app paused
                        if (!ex.Message.StartsWith("Access denied"))
                        {
                            Disconnect();
                            break;
                        }
                    }
                }
            };
        }

        public void Connect()
        {
            _ = Task.Factory.StartNew(async () =>
              {
                  TimeSpan timeout = new TimeSpan(3000);//3 second connection timeout
                  while (true)
                  {
                      try
                      {
                          switch (_connectionState)
                          {
                              case ConnectionState.Disconnected:
                                  ConnectClient();
                                  lastMessageTime = DateTime.Now;
                                  StartListeners();
                                  break;
                              case ConnectionState.Connecting:
                              case ConnectionState.Connected:
                                  TimeSpan elapsed = DateTime.Now - lastMessageTime;
                                  if (elapsed.Seconds > 30)
                                  {
                                      Console.WriteLine("Connection timeout :");
                                      Disconnect();
                                  }
                                  break;
                              case ConnectionState.Paused:
                                  lastMessageTime = DateTime.Now;
                                  break;
                          }
                          Thread.Sleep(100);
                      }
                      catch (Exception ex)
                      {
                          Console.WriteLine("Connection thread error:" + ex.Message);
                      }
                  }
              });
        }

        public class ControllerState
        {
            public float Rx { get; set; }
            public float Ry { get; set; }
            public float Lx { get; set; }
            public float Ly { get; set; }
            public int Speed { get; set; }
            public double DeadBand { get; set; } = 0.15D;

            public void SetAxis(float lx, float ly, float rx, float ry)
            {
                Rx = rx;
                Ry = ry;
                Lx = lx;
                Ly = ly;
            }
            public void SetSpeedMode(int mode)
            {
                Speed = mode;
            }
        }

        public ControllerState _controllerState = new ControllerState();
        public ControllerState _autoPilotControllerState = new ControllerState();
        private bool ctrlBouncing;
        private bool ctrlSportsMode;
        private short ctrlRx;
        private short ctrlRy;
        private short ctrlLy;
        private short ctrlLx;

        public float Clamp(float value, float min, float max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        public void SendControllerUpdate()
        {
            if (!_connected)
            {
                return;
            }

            float boost = 0.0f;
            if (_controllerState.Speed > 0)
            {
                boost = 1.0f;
            }

            float rx = _controllerState.Rx;
            float ry = _controllerState.Ry;
            float lx = _controllerState.Lx;
            float ly = _controllerState.Ly;

            if (true)
            {
                rx = Clamp(rx + _autoPilotControllerState.Rx, -1.0f, 1.0f);
                ry = Clamp(ry + _autoPilotControllerState.Ry, -1.0f, 1.0f);
                lx = Clamp(lx + _autoPilotControllerState.Lx, -1.0f, 1.0f);
                ly = Clamp(ly + _autoPilotControllerState.Ly, -1.0f, 1.0f);
            }

            byte[]? packet = CreateJoyPacket(rx, ry, lx, ly, boost);
            try
            {
                _client.Send(packet);
            }
            catch (Exception)
            {
            }
        }

        private readonly Dictionary<int, string> cmdIdLookup = new Dictionary<int, string>
            {
                { 26, "Wifi" },//2 bytes. Strength, Disturb.
                { 53, "Light" },//1 byte?
                { 86, "FlyData" },
                { 4176, "Data" },//wtf?
            };

        //Create joystick packet from floating point axis.
        //Center = 0.0. 
        //Up/Right =1.0. 
        //Down/Left=-1.0. 
        private byte[] CreateJoyPacket(float fRx, float fRy, float fLx, float fLy, float speed)
        {
            //template joy packet.
            byte[]? packet = new byte[] { 0xcc, 0xb0, 0x00, 0x7f, 0x60, 0x50, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x12, 0x16, 0x01, 0x0e, 0x00, 0x25, 0x54 };

            short axis1 = (short)(660.0F * fRx + 1024.0F);//RightX center=1024 left =364 right =-364
            short axis2 = (short)(660.0F * fRy + 1024.0F);//RightY down =364 up =-364
            short axis3 = (short)(660.0F * fLy + 1024.0F);//LeftY down =364 up =-364
            short axis4 = (short)(660.0F * fLx + 1024.0F);//LeftX left =364 right =-364
            short axis5 = (short)(660.0F * speed + 1024.0F);//Speed. 

            if (speed > 0.1f)
            {
                axis5 = 0x7fff;
            }

            long packedAxis = ((long)axis1 & 0x7FF) | (((long)axis2 & 0x7FF) << 11) | ((0x7FF & (long)axis3) << 22) | ((0x7FF & (long)axis4) << 33) | ((long)axis5 << 44);
            packet[9] = ((byte)(int)(0xFF & packedAxis));
            packet[10] = ((byte)(int)(packedAxis >> 8 & 0xFF));
            packet[11] = ((byte)(int)(packedAxis >> 16 & 0xFF));
            packet[12] = ((byte)(int)(packedAxis >> 24 & 0xFF));
            packet[13] = ((byte)(int)(packedAxis >> 32 & 0xFF));
            packet[14] = ((byte)(int)(packedAxis >> 40 & 0xFF));

            //Add time info.		
            DateTime now = DateTime.Now;
            packet[15] = (byte)now.Hour;
            packet[16] = (byte)now.Minute;
            packet[17] = (byte)now.Second;
            packet[18] = (byte)(now.Millisecond & 0xff);
            packet[19] = (byte)(now.Millisecond >> 8);

            Crc.CalcUCRC(packet, 4);//Not really needed.            
            Crc.CalcCrc(packet, packet.Length);

            return packet;
        }

        public class FlyData
        {
            private int flyMode;
            private int height;
            private int verticalSpeed;
            private int flySpeed;
            private int eastSpeed;
            private int northSpeed;
            private int flyTime;
            private bool flying;

            private bool downVisualState;
            private bool droneHover;
            private bool eMOpen;
            private bool onGround;
            private bool pressureState;

            private bool batteryCritical;
            private int batteryPercentage;
            private bool batteryLow;
            private bool batteryLower;
            private bool batteryState;
            private bool powerState;
            private int droneBatteryLeft;
            private int droneFlyTimeLeft;

            private int cameraState;
            private int electricalMachineryState;
            private bool factoryMode;
            private bool frontIn;
            private bool frontLSC;
            private bool frontOut;
            private bool gravityState;
            private int imuCalibrationState;
            private bool imuState;
            private int lightStrength;
            private bool outageRecording;
            private int smartVideoExitMode;
            private int temperatureHeight;
            private int throwFlyTimer;
            private int wifiDisturb;
            private int wifiStrength;
            private bool windState;

            private float velX;
            private float velY;
            private float velZ;

            private float posX;
            private float posY;
            private float posZ;
            private float posUncertainty;

            private float velN;
            private float velE;
            public float velD;

            private float quatX;
            private float quatY;
            private float quatZ;
            private float quatW;

            private readonly Tello tello;

            public int FlyMode { get => flyMode; set => flyMode = value; }
            public int Height { get => height; set => height = value; }
            public int VerticalSpeed { get => verticalSpeed; set => verticalSpeed = value; }
            public int FlySpeed { get => flySpeed; set => flySpeed = value; }
            public int EastSpeed { get => eastSpeed; set => eastSpeed = value; }
            public int NorthSpeed { get => northSpeed; set => northSpeed = value; }
            public int FlyTime { get => flyTime; set => flyTime = value; }
            public bool Flying { get => flying; set => flying = value; }
            public bool DownVisualState { get => downVisualState; set => downVisualState = value; }
            public bool DroneHover { get => droneHover; set => droneHover = value; }
            public bool EMOpen { get => eMOpen; set => eMOpen = value; }
            public bool OnGround { get => onGround; set => onGround = value; }
            public bool PressureState { get => pressureState; set => pressureState = value; }
            public int BatteryPercentage { get => batteryPercentage; set => batteryPercentage = value; }
            public bool BatteryLow { get => batteryLow; set => batteryLow = value; }
            public bool BatteryLower { get => batteryLower; set => batteryLower = value; }
            public bool BatteryState { get => batteryState; set => batteryState = value; }
            public bool PowerState { get => powerState; set => powerState = value; }
            public int DroneBatteryLeft { get => droneBatteryLeft; set => droneBatteryLeft = value; }
            public int DroneFlyTimeLeft { get => droneFlyTimeLeft; set => droneFlyTimeLeft = value; }
            public int CameraState { get => cameraState; set => cameraState = value; }
            public int ElectricalMachineryState { get => electricalMachineryState; set => electricalMachineryState = value; }
            public bool FactoryMode { get => factoryMode; set => factoryMode = value; }
            public bool FrontIn { get => frontIn; set => frontIn = value; }
            public bool FrontLSC { get => frontLSC; set => frontLSC = value; }
            public bool FrontOut { get => frontOut; set => frontOut = value; }
            public bool GravityState { get => gravityState; set => gravityState = value; }
            public int ImuCalibrationState { get => imuCalibrationState; set => imuCalibrationState = value; }
            public bool ImuState { get => imuState; set => imuState = value; }
            public int LightStrength { get => lightStrength; set => lightStrength = value; }
            public bool OutageRecording { get => outageRecording; set => outageRecording = value; }
            public int SmartVideoExitMode { get => smartVideoExitMode; set => smartVideoExitMode = value; }
            public int TemperatureHeight { get => temperatureHeight; set => temperatureHeight = value; }
            public int ThrowFlyTimer { get => throwFlyTimer; set => throwFlyTimer = value; }
            public int WifiDisturb { get => wifiDisturb; set => wifiDisturb = value; }
            public int WifiStrength { get => wifiStrength; set => wifiStrength = value; }
            public bool WindState { get => windState; set => windState = value; }
            public float VelX { get => velX; set => velX = value; }
            public float VelY { get => velY; set => velY = value; }
            public float VelZ { get => velZ; set => velZ = value; }
            public float PosX { get => posX; set => posX = value; }
            public float PosY { get => posY; set => posY = value; }
            public float PosZ { get => posZ; set => posZ = value; }
            public float PosUncertainty { get => posUncertainty; set => posUncertainty = value; }
            public float VelN { get => velN; set => velN = value; }
            public float VelE { get => velE; set => velE = value; }
            public float QuatX { get => quatX; set => quatX = value; }
            public float QuatY { get => quatY; set => quatY = value; }
            public float QuatZ { get => quatZ; set => quatZ = value; }
            public float QuatW { get => quatW; set => quatW = value; }
            public bool BatteryCritical { get => batteryCritical; set => batteryCritical = value; }

            public FlyData(Tello tello)
            {
                this.tello = tello;
            }

            public void Set(byte[] data)
            {
                int index = 0;
                height = (short)(data[index] | (data[index + 1] << 8)); index += 2;
                northSpeed = (short)(data[index] | (data[index + 1] << 8)); index += 2;
                eastSpeed = (short)(data[index] | (data[index + 1] << 8)); index += 2;
                FlySpeed = ((int)Math.Sqrt(Math.Pow(northSpeed, 2.0D) + Math.Pow(eastSpeed, 2.0D)));
                verticalSpeed = (short)(data[index] | (data[index + 1] << 8)); index += 2;
                flyTime = data[index] | (data[index + 1] << 8); index += 2;

                imuState = (data[index] >> 0 & 0x1) == 1 ? true : false;
                pressureState = (data[index] >> 1 & 0x1) == 1 ? true : false;
                downVisualState = (data[index] >> 2 & 0x1) == 1 ? true : false;
                powerState = (data[index] >> 3 & 0x1) == 1 ? true : false;
                batteryState = (data[index] >> 4 & 0x1) == 1 ? true : false;
                gravityState = (data[index] >> 5 & 0x1) == 1 ? true : false;
                windState = (data[index] >> 7 & 0x1) == 1 ? true : false;
                index += 1;

                imuCalibrationState = data[index]; index += 1;
                batteryPercentage = data[index]; index += 1;
                droneFlyTimeLeft = data[index] | (data[index + 1] << 8); index += 2;
                droneBatteryLeft = data[index] | (data[index + 1] << 8); index += 2;

                //index 17
                flying = (data[index] >> 0 & 0x1) == 1 ? true : false;
                onGround = (data[index] >> 1 & 0x1) == 1 ? true : false;
                eMOpen = (data[index] >> 2 & 0x1) == 1 ? true : false;
                droneHover = (data[index] >> 3 & 0x1) == 1 ? true : false;
                outageRecording = (data[index] >> 4 & 0x1) == 1 ? true : false;
                batteryLow = (data[index] >> 5 & 0x1) == 1 ? true : false;
                batteryLower = (data[index] >> 6 & 0x1) == 1 ? true : false;
                factoryMode = (data[index] >> 7 & 0x1) == 1 ? true : false;
                index += 1;

                FlyMode = data[index]; index += 1;
                throwFlyTimer = data[index]; index += 1;
                cameraState = data[index]; index += 1;

                electricalMachineryState = data[index]; index += 1;

                frontIn = (data[index] >> 0 & 0x1) == 1 ? true : false;//22
                frontOut = (data[index] >> 1 & 0x1) == 1 ? true : false;
                frontLSC = (data[index] >> 2 & 0x1) == 1 ? true : false;
                index += 1;
                temperatureHeight = (data[index] >> 0 & 0x1);//23            
                wifiStrength = tello._wifiStrength;//Wifi str comes in a cmd.
            }

            public void ParseLog(byte[] data)
            {
                int pos = 0;

                while (pos < data.Length - 2)//-2 for CRC bytes at end of packet.
                {
                    if (data[pos] != 'U')
                    {
                        break;
                    }
                    byte len = data[pos + 1];
                    if (data[pos + 2] != 0)
                    {
                        break;
                    }

                    ushort id = BitConverter.ToUInt16(data, pos + 4);
                    byte[]? xorBuf = new byte[256];
                    byte xorValue = data[pos + 6];
                    switch (id)
                    {
                        case 0x1d:
                            for (int i = 0; i < len; i++)
                            {
                                xorBuf[i] = (byte)(data[pos + i] ^ xorValue);
                            }

                            int index = 10;
                            velX = BitConverter.ToInt16(xorBuf, index); index += 2;
                            velY = BitConverter.ToInt16(xorBuf, index); index += 2;
                            velZ = BitConverter.ToInt16(xorBuf, index); index += 2;
                            posX = BitConverter.ToSingle(xorBuf, index); index += 4;
                            posY = BitConverter.ToSingle(xorBuf, index); index += 4;
                            posZ = BitConverter.ToSingle(xorBuf, index); index += 4;
                            posUncertainty = BitConverter.ToSingle(xorBuf, index) * 10000.0f; index += 4;
                            break;
                        case 0x0800://2048 imu
                            for (int i = 0; i < len; i++)
                            {
                                xorBuf[i] = (byte)(data[pos + i] ^ xorValue);
                            }

                            int index2 = 10 + 48;//44 is the start of the quat data.
                            quatW = BitConverter.ToSingle(xorBuf, index2); index2 += 4;
                            quatX = BitConverter.ToSingle(xorBuf, index2); index2 += 4;
                            quatY = BitConverter.ToSingle(xorBuf, index2); index2 += 4;
                            quatZ = BitConverter.ToSingle(xorBuf, index2);
                            index2 = 10 + 76;//Start of relative velocity
                            velN = BitConverter.ToSingle(xorBuf, index2); index2 += 4;
                            velE = BitConverter.ToSingle(xorBuf, index2); index2 += 4;
                            velD = BitConverter.ToSingle(xorBuf, index2);
                            break;

                    }
                    pos += len;
                }
            }

            public double[] ToEuler()
            {
                float qX = quatX;
                float qY = quatY;
                float qZ = quatZ;
                float qW = quatW;

                double sqW = qW * qW;
                double sqX = qX * qX;
                double sqY = qY * qY;
                double sqZ = qZ * qZ;
                double yaw = 0.0;
                double roll = 0.0;
                double pitch = 0.0;
                double[] retv = new double[3];
                double unit = sqX + sqY + sqZ + sqW; // if normalised is one, otherwise
                                                     // is correction factor
                double test = qW * qX + qY * qZ;
                if (test > 0.499 * unit)
                {
                    // singularity at north pole
                    yaw = 2 * Math.Atan2(qY, qW);
                    pitch = Math.PI / 2;
                    roll = 0;
                }
                else if (test < -0.499 * unit)
                {
                    // singularity at south pole
                    yaw = -2 * Math.Atan2(qY, qW);
                    pitch = -Math.PI / 2;
                    roll = 0;
                }
                else
                {
                    yaw = Math.Atan2(2.0 * (qW * qZ - qX * qY), 1.0 - 2.0 * (sqZ + sqX));
                    roll = Math.Asin(2.0 * test / unit);
                    pitch = Math.Atan2(2.0 * (qW * qY - qX * qZ), 1.0 - 2.0 * (sqY + sqX));
                }

                retv[0] = pitch;
                retv[1] = roll;
                retv[2] = yaw;
                return retv;
            }

            public string GetLogHeader()
            {
                StringBuilder sb = new StringBuilder();
                foreach (System.Reflection.FieldInfo property in GetType().GetFields())
                {
                    sb.Append(property.Name);
                    sb.Append(",");
                }
                sb.AppendLine();
                return sb.ToString();
            }

            public string GetLogLine()
            {
                StringBuilder sb = new StringBuilder();
                foreach (System.Reflection.FieldInfo property in GetType().GetFields())
                {
                    if (property.FieldType == typeof(bool))
                    {
                        if (!(bool)property.GetValue(this))
                        {
                            sb.Append("0");
                        }
                        else
                        {
                            sb.Append("1");
                        }
                    }
                    else
                    {
                        sb.Append(property.GetValue(this));
                    }

                    sb.Append(",");
                }
                sb.AppendLine();
                return sb.ToString();
            }

            public override string ToString()
            {
                StringBuilder sb = new();
                int count = 0;
                foreach (System.Reflection.PropertyInfo property in GetType().GetProperties())
                {
                    sb.Append(property.Name);
                    sb.Append(": ");
                    sb.Append(property.GetValue(this));
                    if (count++ % 2 == 1)
                    {
                        sb.Append(Environment.NewLine);
                    }
                    else
                    {
                        sb.Append("      ");
                    }
                }

                return sb.ToString();
            }
        }
    }
}