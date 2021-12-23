using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelloSharp
{
	internal class Messages
	{
		const byte msgHdr = 0xcc; // 204

		// packet is our internal representation of the messages passed to/from the Tello
		struct Packet
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

		const short minPktSize = 11; // smallest possible raw packet

		// tello packet types, 3 and 7 currently unknown
		const short ptExtended = 0;
		const short ptGet = 1;
		const short ptData1 = 2;
		const short ptData2 = 4;
		const short ptSet = 5;
		const short ptFlip = 6;


		// Tello message IDs

		const byte msgDoConnect = 0x0001; // 1
		const byte msgConnected = 0x0002; // 2
		const byte msgQuerySSID = 0x0011; // 17
		const byte msgSetSSID = 0x0012; // 18
		const byte msgQuerySSIDPass = 0x0013; // 19
		const byte msgSetSSIDPass = 0x0014; // 20
		const byte msgQueryWifiRegion = 0x0015; // 21
		const byte msgSetWifiRegion = 0x0016; // 22
		const byte msgWifiStrength = 0x001a; // 26
		const byte msgSetVideoBitrate = 0x0020; // 32
		const byte msgSetDynAdjRate = 0x0021; // 33
		const byte msgEisSetting = 0x0024; // 36
		const byte msgQueryVideoSPSPPS = 0x0025; // 37
		const byte msgQueryVideoBitrate = 0x0028; // 40
		const byte msgDoTakePic = 0x0030; // 48
		const byte msgSwitchPicVideo = 0x0031; // 49
		const byte msgDoStartRec = 0x0032; // 50
		const byte msgExposureVals = 0x0034; // 52 (Get or set?)
		const byte msgLightStrength = 0x0035; // 53
		const byte msgQueryJPEGQuality = 0x0037; // 55
		const byte msgError1 = 0x0043; // 67
		const byte msgError2 = 0x0044; // 68
		const byte msgQueryVersion = 0x0045; // 69
		const byte msgSetDateTime = 0x0046;// 70
		const byte msgQueryActivationTime = 0x0047; // 71
		const byte msgQueryLoaderVersion = 0x0049; // 73
		const byte msgSetStick = 0x0050;// 80
		const byte msgDoTakeoff = 0x0054; // 84
		const byte msgDoLand = 0x0055;// 85
		const byte msgFlightStatus = 0x0056;// 86
		const byte msgSetHeightLimit = 0x0058;// 88
		const byte msgDoFlip = 0x005c;// 92
		const byte msgDoThrowTakeoff = 0x005d; // 93
		const byte msgDoPalmLand = 0x005e; // 94
		const byte msgFileSize = 0x0062; // 98
		const byte msgFileData = 0x0063; // 99
		const byte msgFileDone = 0x0064; // 100
		const byte msgDoSmartVideo = 0x0080; // 128
		const byte msgSmartVideoStatus = 0x0081; // 129
		const ushort msgLogHeader = 0x1050;// 4176
		const ushort msgLogData = 0x1051;// 4177
		const ushort msgLogConfig = 0x1052;// 4178
		const ushort msgDoBounce = 0x1053;// 4179
		const ushort msgDoCalibration = 0x1054; // 4180
		const ushort msgSetLowBattThresh = 0x1055;// 4181
		const ushort msgQueryHeightLimit = 0x1056;// 4182
		const ushort msgQueryLowBattThresh = 0x1057;// 4183
		const ushort msgSetAttitude = 0x1058;// 4184
		const ushort msgQueryAttitude = 0x1059; // 4185

		// FlipType represents a flip direction.


		// Flip types...
		enum FlipType
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
		byte SvCmd;
		// Smart Video flight commands...
		const byte Sv360 = 1 << 2; // Slowly rotate around 360 degrees.
		const byte SvCircle = 2 << 2; // Circle around a point in front of the drone.
		const byte SvUpOut = 3 << 2; // Perform the 'Up and Out' manouvre.


		// VBR is a Video Bit Rate, the int value is meaningless.
		byte VBR;
		// VBR settings...
		byte VbrAuto;// = iota // let the Tello choose the best for the current connection
		byte Vbr1M;              // Set the VBR to 1Mbps
		byte Vbr1M5;             // Set the VBR to 1.5Mbps
		byte Vbr2M;              // Set the VBR to 2Mbps
		byte Vbr3M;              // Set the VBR to 3Mbps
		byte Vbr4M;              // Set the VBR to 4mbps


		const byte vmNormal = 0;
		const byte vmWide = 1;

		// FileType is the type of file being sent to/from the drone
		byte FileType;
		private byte[] payloadSize;

		// Known File Types...
		const byte FtJPEG = 1;


		struct FileData
		{
			byte FileType; // 1 = JPEG
			int FileSize;
			byte[] FileBytes;
		}

		struct FileInternal
		{
			short fID;
			byte filetype;
			int expectedSize;
			int accumSize;
			byte[] filePiece;
		}

		struct FilePiece
		{
			//fID       uint16
			int numChunks;
			byte[] fileChunk;
		}

		struct FileChunk
		{
			short fID;
			uint pieceNum;
			uint chunkNum;
			short chunkLen;
			byte[] chunkData;
		}

		// FlightData holds our current knowledge of the drone's state.
		// This data is not all sent at once from the drone, different fields may be updated
		// at varying rates.
		struct FlightData
		{
			bool BatteryCritical;
			bool BatteryLow;
			short BatteryMilliVolts;
			short BatteryPercentage;
			bool BatteryState;
			ushort CameraState;
			bool DownVisualState;
			short DroneFlyTimeLeft;
			bool DroneHover;
			short EastSpeed;
			ushort ElectricalMachineryState;
			bool EmOpen;
			bool ErrorState;
			bool FactoryMode;
			bool Flying;
			ushort FlyMode;
			short FlyTime;
			bool FrontIn;
			bool FrontLSC;
			bool FrontOut;
			bool GravityState;
			short GroundSpeed;
			short Height; // seems to be in decimetres
			IMUData IMU;
			short ImuCalibrationState;
			bool ImuState;
			ushort LightStrength;
			DateTime LightStrengthUpdated;
			ushort LowBatteryThreshold;
			ushort MaxHeight;
			MVOData MVO;
			short NorthSpeed;
			bool OnGround;
			bool OutageRecording;
			bool PowerState;
			bool PressureState;
			short SmartVideoExitMode;
			string SSID;
			short ThrowFlyTimer;
			string Version;
			short VerticalSpeed;
			object VideoBitrate;
			short WifiInterference;
			short WifiStrength;
			bool WindState;
		}

		// MVOData comes from the flight log messages
		struct MVOData
		{
			float PositionX, PositionY, PositionZ;
			short VelocityX, VelocityY, VelocityZ;
		}

		// IMUData comes from the flight log messages
		struct IMUData
		{
			float QuaternionW, QuaternionX, QuaternionY, QuaternionZ;
			short Temperature;
			short Yaw;  // derived from Quat fields, -180 > degrees > +180
		}

		// StickMessage holds the signed 16-bit values of a joystick update.
		// Each value can range from -32768 to 32767
		struct StickMessage
		{
			short Rx, Ry, Lx, Ly;
		}

		const char logRecordSeparator = 'U';

		// flight log message IDs
		const byte logRecNewMVO = 0x001d;
		const short logRecIMU = 0x0800;
		// TODO - there are many more


		const byte logValidVelX = 0x01;
		const byte logValidVelY = 0x02;
		const byte logValidVelZ = 0x04;
		const byte logValidPosY = 0x10;
		const byte logValidPosX = 0x20;
		const byte logValidPosZ = 0x40;


		// utility funcs for message handling

		// bufferToPacket takes a raw buffer of bytes and populates our packet struct
		private Packet BufferToPacket(byte[] buff)
		{
			Packet pkt = new Packet();
			pkt.header = buff[0];
			pkt.size13 = (short)((buff[1] + (buff[2]) << 8) >> 3);
			pkt.crc8 = buff[3];
			pkt.fromDrone = (buff[4] & 0x80) == 1;
			pkt.toDrone = (buff[4] & 0x40) == 1;
			pkt.packetType = ((short)((buff[4] >> 3) & 0x07));
			pkt.packetSubtype = ((short)(buff[4] & 0x07));
			pkt.messageID = (short)(((buff[6]) << 8) | (buff[5]));
			pkt.sequence = (short)(((buff[8]) << 8) | (buff[7]));
			payloadSize = new byte[pkt.size13 - 11];
			if (payloadSize.Length > 0)
			{
				pkt.payload = (byte[])payloadSize;
				Array.Copy(pkt.payload, buff, 9 + payloadSize.Length);
			}
			pkt.crc16 = (short)((buff[pkt.size13 - 1]) << (8 + (buff[pkt.size13 - 2])));
			return pkt;
		}

		// newPacket returns a packet with some fields populated
		private Packet NewPacket(short pt, short cmd, short seq, int payloadSize)
		{
			Packet pkt = new Packet();
			pkt.header = msgHdr;
			pkt.toDrone = true;
			pkt.packetType = pt;
			pkt.messageID = cmd;
			pkt.sequence = seq;
			if (payloadSize > 0)
			{
				pkt.payload = new byte[payloadSize];
			}
			return pkt;
		}

		//// pack the packet into raw buffer format and calculate CRCs etc.
		//		private byte[] packetToBuffer(Packet pkt) 
		//		{
		//			byte[] buff = new byte[1];
		//		// create a buffer of the right size
		//		payloadSize:= pkt.payload.Length;
		//			packetSiz= minPktSize + payloadSize
		//			buff = make([]byte, packetSize)

		//			// copy each field, manipulating if necessary
		//			buff[0] = pkt.header
		//			buff[1] = byte(packetSize << 3)
		//			buff[2] = byte(packetSize >> 5)
		//			buff[3] = calculateCRC8(buff[0:3])
		//			buff[4] = pkt.packetSubtype + (pkt.packetType << 3)
		//			if pkt.toDrone {
		//				buff[4] |= 0x40
		//			}
		//			if pkt.fromDrone {
		//				buff[4] |= 0x80
		//			}
		//			buff[5] = byte(pkt.messageID)
		//			buff[6] = byte(pkt.messageID >> 8)
		//			buff[7] = byte(pkt.sequence)
		//			buff[8] = byte(pkt.sequence >> 8)

		//			for p := 0; p < payloadSize; p++ {
		//				buff[9 + p] = pkt.payload[p]
		//			}
		//		crc16:= calculateCRC16(buff[0 : 9 + payloadSize])
		//			buff[9 + payloadSize] = byte(crc16)
		//			buff[10 + payloadSize] = byte(crc16 >> 8)

		//			return buff
		//}

		//func payloadToFlightData(pl[]byte) (fd FlightData) {
		//	fd.Height = int16(pl[0]) + int16(pl[1]) << 8
		//	fd.NorthSpeed = int16(uint16(pl[2]) | uint16(pl[3]) << 8)
		//	fd.EastSpeed = int16(pl[4]) | int16(pl[5]) << 8
		//	fd.VerticalSpeed = int16(pl[6]) | int16(pl[7]) << 8
		//	fd.FlyTime = int16(pl[8]) | int16(pl[9]) << 8

		//	fd.ImuState = (pl[10] & 1) == 1
		//	fd.PressureState = (pl[10] >> 1 & 1) == 1
		//	fd.DownVisualState = (pl[10] >> 2 & 1) == 1
		//	fd.PowerState = (pl[10] >> 3 & 1) == 1
		//	fd.BatteryState = (pl[10] >> 4 & 1) == 1
		//	fd.GravityState = (pl[10] >> 5 & 1) == 1
		//	// what is bit 6?
		//	fd.WindState = (pl[10] >> 7 & 1) == 1

		//	fd.ImuCalibrationState = int8(pl[11])
		//	fd.BatteryPercentage = int8(pl[12])
		//	fd.DroneFlyTimeLeft = int16(pl[13]) + int16(pl[14]) << 8
		//	fd.BatteryMilliVolts = int16(pl[15]) + int16(pl[16]) << 8

		//	fd.Flying = (pl[17] & 1) == 1
		//	fd.OnGround = (pl[17] >> 1 & 1) == 1
		//	fd.EmOpen = (pl[17] >> 2 & 1) == 1
		//	fd.DroneHover = (pl[17] >> 3 & 1) == 1
		//	fd.OutageRecording = (pl[17] >> 4 & 1) == 1
		//	fd.BatteryLow = (pl[17] >> 5 & 1) == 1
		//	fd.BatteryCritical = (pl[17] >> 6 & 1) == 1
		//	fd.FactoryMode = (pl[17] >> 7 & 1) == 1

		//	fd.FlyMode = uint8(pl[18])
		//	fd.ThrowFlyTimer = int8(pl[19])
		//	fd.CameraState = uint8(pl[20])
		//	fd.ElectricalMachineryState = uint8(pl[21])

		//	fd.FrontIn = (pl[22] & 1) == 1
		//	fd.FrontOut = (pl[22] >> 1 & 1) == 1
		//	fd.FrontLSC = (pl[22] >> 2 & 1) == 1
		//	fd.ErrorState = (pl[23] & 1) == 1

		//	return fd
		//}

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
