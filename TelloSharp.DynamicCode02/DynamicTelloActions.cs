
// using System.Threading;
namespace TelloSharp.DynamicCode02
{
    public class DynamicTelloActions()
    {
        public void run(Tello telloDrone)
        {
            // take off the drone
            telloDrone.TakeOff();

            // sleep of 3 seconds
            System.Threading.Thread.Sleep(3000);

            // land the drone
            telloDrone.Land();
        }
    }
}
