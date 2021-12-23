using static TelloSharp.Tello;

namespace TelloSharp
{
    public class TelloStateEventArgs : EventArgs
    {
        public FlyData State { get; set; }
        public ushort LastCmdId { get; set; }

        public TelloStateEventArgs(FlyData state, ushort lastCmdId)
        {
            State = state;
            LastCmdId = lastCmdId;
        }
    }
}