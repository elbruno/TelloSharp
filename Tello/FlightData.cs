using System.Text;

namespace TelloSharp
{
    public partial class Messages
    {
        // FlightData holds our current knowledge of the drone's state.
        // This data is not all sent at once from the drone, different fields may be updated
        // at varying rates.
        public class FlightData
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
            public VBR VideoBitrate;
            public short WifiInterference;
            public short WifiStrength;
            public bool WindState;


            public override string ToString()
            {
                StringBuilder sb = new();
                int count = 0;
                foreach (System.Reflection.FieldInfo property in GetType().GetFields())
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
