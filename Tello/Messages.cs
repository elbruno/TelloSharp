namespace TelloSharp
{
    public class Messages
    {
        public const byte msgHdr = 0xcc; // 204

        // packet is our internal representation of the messages passed to/from the Tello
        internal struct Packet
        {
            public byte header;
            public short size13;
            public byte crc8;
            public bool fromDrone;  // the following 4 fields are encoded in a single byte in the raw packet
            public bool toDrone;
            public short packetType;
            public short packetSubtype;
            public short messageID;
            public short sequence;
            public byte[] payload;
            public short crc16;
        }

        public const short minPktSize = 11; // smallest possible raw packet

        public struct PacketType
        {
            // tello packet types, 3 and 7 currently unknown
            public const short ptExtended = 0;
            public const short ptGet = 1;
            public const short ptData1 = 2;
            public const short ptData2 = 4;
            public const short ptSet = 5;
            public const short ptFlip = 6;
        }

        // Tello message IDs
        public struct MessageTypes
        {
            public const byte msgDoConnect = 0x0001; // 1
            public const byte msgConnected = 0x0002; // 2
            public const byte msgQuerySSID = 0x0011; // 17
            public const byte msgSetSSID = 0x0012; // 18
            public const byte msgQuerySSIDPass = 0x0013; // 19
            public const byte msgSetSSIDPass = 0x0014; // 20
            public const byte msgQueryWifiRegion = 0x0015; // 21
            public const byte msgSetWifiRegion = 0x0016; // 22
            public const byte msgWifiStrength = 0x001a; // 26
            public const byte msgSetVideoBitrate = 0x0020; // 32
            public const byte msgSetDynAdjRate = 0x0021; // 33
            public const byte msgEisSetting = 0x0024; // 36
            public const byte msgQueryVideoSPSPPS = 0x0025; // 37
            public const byte msgQueryVideoBitrate = 0x0028; // 40
            public const byte msgDoTakePic = 0x0030; // 48
            public const byte msgSwitchPicVideo = 0x0031; // 49
            public const byte msgDoStartRec = 0x0032; // 50
            public const byte msgExposureVals = 0x0034; // 52 (Get or set?)
            public const byte msgLightStrength = 0x0035; // 53
            public const byte msgQueryJPEGQuality = 0x0037; // 55
            public const byte msgError1 = 0x0043; // 67
            public const byte msgError2 = 0x0044; // 68
            public const byte msgQueryVersion = 0x0045; // 69
            public const byte msgSetDateTime = 0x0046;// 70
            public const byte msgQueryActivationTime = 0x0047; // 71
            public const byte msgQueryLoaderVersion = 0x0049; // 73
            public const byte msgSetStick = 0x0050;// 80
            public const byte msgDoTakeoff = 0x0054; // 84
            public const byte msgDoLand = 0x0055;// 85
            public const byte msgFlightStatus = 0x0056;// 86
            public const byte msgSetHeightLimit = 0x0058;// 88
            public const byte msgDoFlip = 0x005c;// 92
            public const byte msgDoThrowTakeoff = 0x005d; // 93
            public const byte msgDoPalmLand = 0x005e; // 94
            public const byte msgFileSize = 0x0062; // 98
            public const byte msgFileData = 0x0063; // 99
            public const byte msgFileDone = 0x0064; // 100
            public const byte msgDoSmartVideo = 0x0080; // 128
            public const byte msgSmartVideoStatus = 0x0081; // 129
            public const short msgLogHeader = 0x1050;// 4176
            public const short msgLogData = 0x1051;// 4177
            public const short msgLogConfig = 0x1052;// 4178
            public const short msgDoBounce = 0x1053;// 4179
            public const short msgDoCalibration = 0x1054; // 4180
            public const short msgSetLowBattThresh = 0x1055;// 4181
            public const short msgQueryHeightLimit = 0x1056;// 4182
            public const short msgQueryLowBattThresh = 0x1057;// 4183
            public const short msgSetAttitude = 0x1058;// 4184
            public const short msgQueryAttitude = 0x1059; // 4185
        }
        // FlipType represents a flip direction.


        // Flip types...
        public enum FlipType
        {
            FlipForward,// = iota,
            FlipLeft,
            FlipBackward,
            FlipRight,
            FlipForwardLeft,
            FlipBackwardLeft,
            FlipBackwardRight,
            FlipForwardRight
        }

        // SvCmd is Smart Video flight command.
        private readonly byte SvCmd;

        // Smart Video flight commands...
        private const byte Sv360 = 1 << 2; // Slowly rotate around 360 degrees.
        private const byte SvCircle = 2 << 2; // Circle around a point in front of the drone.
        private const byte SvUpOut = 3 << 2; // Perform the 'Up and Out' manouvre.


        // VBR is a Video Bit Rate, the int value is meaningless.
        private readonly byte VBR;

        // VBR settings...
        private readonly byte VbrAuto;// = iota // let the Tello choose the best for the current connection
        private readonly byte Vbr1M;              // Set the VBR to 1Mbps
        private readonly byte Vbr1M5;             // Set the VBR to 1.5Mbps
        private readonly byte Vbr2M;              // Set the VBR to 2Mbps
        private readonly byte Vbr3M;              // Set the VBR to 3Mbps
        private readonly byte Vbr4M;              // Set the VBR to 4mbps


        private const byte vmNormal = 0;
        private const byte vmWide = 1;

        // FileType is the type of file being sent to/from the drone
        private readonly byte FileType;
        private byte[] payloadSize;

        // Known File Types...
        private const byte FtJPEG = 1;

        private struct FileData
        {
            private readonly byte FileType; // 1 = JPEG
            private readonly int FileSize;
            private readonly byte[] FileBytes;
        }

        private struct FileInternal
        {
            private readonly short fID;
            private readonly byte filetype;
            private readonly int expectedSize;
            private readonly int accumSize;
            private readonly byte[] filePiece;
        }

        private struct FilePiece
        {
            //fID       uint16
            private readonly int numChunks;
            private readonly byte[] fileChunk;
        }

        private struct FileChunk
        {
            private readonly short fID;
            private readonly uint pieceNum;
            private readonly uint chunkNum;
            private readonly short chunkLen;
            private readonly byte[] chunkData;
        }

        // FlightData holds our current knowledge of the drone's state.
        // This data is not all sent at once from the drone, different fields may be updated
        // at varying rates.
        internal struct FlightData
        {
            public bool BatteryCritical;
            public bool BatteryLow;
            public short BatteryMilliVolts;
            public short BatteryPercentage;
            public bool BatteryState;
            public byte CameraState;
            public bool DownVisualState;
            public short DroneFlyTimeLeft;
            public bool DroneHover;
            public short EastSpeed;
            public byte ElectricalMachineryState;
            public bool EmOpen;
            public bool ErrorState;
            public bool FactoryMode;
            public bool Flying;
            public byte FlyMode;
            public short FlyTime;
            public bool FrontIn;
            public bool FrontLSC;
            public bool FrontOut;
            public bool GravityState;
            public short GroundSpeed;
            public short Height; // seems to be in decimetres
            public IMUData IMU;
            public short ImuCalibrationState;
            public bool ImuState;
            public ushort LightStrength;
            public DateTime LightStrengthUpdated;
            public ushort LowBatteryThreshold;
            public ushort MaxHeight;
            public MVOData MVO;
            public short NorthSpeed;
            public bool OnGround;
            public bool OutageRecording;
            public bool PowerState;
            public bool PressureState;
            public short SmartVideoExitMode;
            public string SSID;
            public byte ThrowFlyTimer;
            public string Version;
            public short VerticalSpeed;
            public object VideoBitrate;
            public short WifiInterference;
            public short WifiStrength;
            public bool WindState;
        }

        // MVOData comes from the flight log messages
        internal struct MVOData
        {
            public float PositionX, PositionY, PositionZ;
            public short VelocityX, VelocityY, VelocityZ;
        }

        // IMUData comes from the flight log messages
        internal struct IMUData
        {
            public float QuaternionW, QuaternionX, QuaternionY, QuaternionZ;
            public short Temperature;
            public short Yaw;  // derived from Quat fields, -180 > degrees > +180
        }

        // StickMessage holds the signed 16-bit values of a joystick update.
        // Each value can range from -32768 to 32767
        internal struct StickMessage
        {
            private readonly short Rx, Ry, Lx, Ly;
        }

        private const char logRecordSeparator = 'U';

        // flight log message IDs
        private const byte logRecNewMVO = 0x001d;
        private const short logRecIMU = 0x0800;

        // TODO - there are many more 
        private const byte logValidVelX = 0x01;
        private const byte logValidVelY = 0x02;
        private const byte logValidVelZ = 0x04;
        private const byte logValidPosY = 0x10;
        private const byte logValidPosX = 0x20;
        private const byte logValidPosZ = 0x40;


        // utility funcs for message handling

        // bufferToPacket takes a raw buffer of bytes and populates our packet struct
        internal Packet BufferToPacket(byte[] buff)
        {
            Packet pkt = new Packet
            {
                header = buff[0],
                size13 = (short)((buff[1] + (buff[2]) << 8) >> 3),
                crc8 = buff[3],
                fromDrone = (buff[4] & 0x80) == 1,
                toDrone = (buff[4] & 0x40) == 1,
                packetType = ((short)((buff[4] >> 3) & 0x07)),
                packetSubtype = ((short)(buff[4] & 0x07)),
                messageID = (short)(((buff[6]) << 8) | (buff[5])),
                sequence = (short)(((buff[8]) << 8) | (buff[7]))
            };
            payloadSize = new byte[pkt.size13 - 11];
            if (payloadSize.Length > 0)
            {
                pkt.payload = payloadSize;
                Array.Copy(pkt.payload, buff, 9 + payloadSize.Length);
            }
            pkt.crc16 = (short)((buff[pkt.size13 - 1]) << (8 + (buff[pkt.size13 - 2])));
            return pkt;
        }

        // newPacket returns a packet with some fields populated
        internal Packet NewPacket(short pt, short cmd, short seq, int payloadSize)
        {
            Packet pkt = new Packet
            {
                header = msgHdr,
                toDrone = true,
                packetType = pt,
                messageID = cmd,
                sequence = seq
            };
            if (payloadSize > 0)
            {
                pkt.payload = new byte[payloadSize];
            }
            return pkt;
        }

        internal byte[] NewPacketAsBytes(short pt, short cmd, short seq, int payloadSize)
        {
            Packet pkt = new Packet
            {
                header = msgHdr,
                toDrone = true,
                packetType = pt,
                messageID = cmd,
                sequence = seq,
                payload = new byte[payloadSize]
            };
            if (payloadSize > 0)
            {
                pkt.payload = new byte[payloadSize];
            }
            return PacketToBuffer(pkt);
        }

        // pack the packet into raw buffer format and calculate CRCs etc.
        internal byte[] PacketToBuffer(Packet pkt)
        {
            byte[] buff = new byte[1];

            var payloadSize = pkt.payload.Length;
            var packetSize = minPktSize + payloadSize;
            buff = new byte[packetSize];

            buff[0] = pkt.header;
            buff[1] = (byte)(packetSize << 3);
            buff[2] = (byte)((byte)packetSize >> 5);
            buff[3] = Crc.CalculateCRC8(buff.Take(3).ToArray());
            buff[4] = (byte)(pkt.packetSubtype + (pkt.packetType << 3));

            if (pkt.toDrone)
            {
                buff[4] |= 0x40;
            }
            if (pkt.fromDrone)
            {
                buff[4] |= 0x80;
            }

            buff[5] = (byte)pkt.messageID;
            buff[6] = (byte)(pkt.messageID >> 8);
            buff[7] = (byte)pkt.sequence;
            buff[8] = (byte)(pkt.sequence >> 8);

            for (short p = 0; p < payloadSize; p++)
            {
                buff[9 + p] = pkt.payload[p];
            }

            ushort crc16 = Crc.CalculateCRC16(buff.Take(9 + payloadSize).ToArray());
            buff[9 + payloadSize] = (byte)crc16;
            buff[10 + payloadSize] = (byte)(crc16 >> 8);

            return buff;
        }

        internal FlightData PayloadToFlightData(byte[] pl)
        { 
            FlightData fd = new FlightData();

            fd.Height = (short)(pl[0] + pl[1] << 8);
            fd.NorthSpeed = (short)(pl[2] | (pl[3]) << 8);
            fd.EastSpeed = (short)(pl[4] | (pl[5] << 8));
            fd.VerticalSpeed = (short)(pl[6] | (pl[7]) << 8);
            fd.FlyTime = (short)(pl[8] | pl[9] << 8);

            fd.ImuState = (pl[10] & 1) == 1;
            fd.PressureState = (pl[10] >> 1 & 1) == 1;
            fd.DownVisualState = (pl[10] >> 2 & 1) == 1;
            fd.PowerState = (pl[10] >> 3 & 1) == 1;
            fd.BatteryState = (pl[10] >> 4 & 1) == 1;
            fd.GravityState = (pl[10] >> 5 & 1) == 1;
            //	// what is bit 6?
            fd.WindState = (pl[10] >> 7 & 1) == 1;

            fd.ImuCalibrationState = pl[11];
            fd.BatteryPercentage = pl[12];
            fd.DroneFlyTimeLeft = (short)(pl[13] + pl[14] << 8);
            fd.BatteryMilliVolts = (short)(pl[15] + pl[16] << 8);

            fd.Flying = (pl[17] & 1) == 1;
            fd.OnGround = (pl[17] >> 1 & 1) == 1;
            fd.EmOpen = (pl[17] >> 2 & 1) == 1;
            fd.DroneHover = (pl[17] >> 3 & 1) == 1;
            fd.OutageRecording = (pl[17] >> 4 & 1) == 1;
            fd.BatteryLow = (pl[17] >> 5 & 1) == 1;
            fd.BatteryCritical = (pl[17] >> 6 & 1) == 1;
            fd.FactoryMode = (pl[17] >> 7 & 1) == 1;

            fd.FlyMode = pl[18];
            fd.ThrowFlyTimer = pl[19];
            fd.CameraState = pl[20];
            fd.ElectricalMachineryState = pl[21];

            fd.FrontIn = (pl[22] & 1) == 1;
            fd.FrontOut = (pl[22] >> 1 & 1) == 1;
            fd.FrontLSC = (pl[22] >> 2 & 1) == 1;
            fd.ErrorState = (pl[23] & 1) == 1;

            return fd;
        }

        //func payloadToFileInfo(pl[]byte) (fType FileType, fSize uint32, fID uint16) {
        //	fType = FileType(pl[0])
        //	fSize = uint32(pl[1]) + uint32(pl[2]) << 8 + uint32(pl[3]) << 16 + uint32(pl[4]) << 24
        //	fID = uint16(pl[5]) + uint16(pl[6]) << 8
        //	return fType, fSize, fID
        //}

        //func payloadToFileChunk(pl[]byte) (fc fileChunk) {
        //	fc.fID = uint16(pl[0]) + uint16(pl[1]) << 8
        //	fc.pieceNum = uint32(pl[2]) + uint32(pl[3]) << 8 + uint32(pl[4]) << 16 + uint32(pl[5]) << 24
        //	fc.chunkNum = uint32(pl[6]) + uint32(pl[7]) << 8 + uint32(pl[8]) << 16 + uint32(pl[9]) << 24
        //	fc.chunkLen = uint16(pl[10]) + uint16(pl[11]) << 8
        //	fc.chunkData = pl[12:]
        //	return fc
        //}

        //private float bytesToFloat32(b[]byte) 
        //{
        //	return MathF.Float32frombits(binary.LittleEndian.Uint32(b))
    }
}
