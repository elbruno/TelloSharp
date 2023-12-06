using TelloSharp;

bool Connected = false;
Tello tello = new();
tello.Connect();

Console.WriteLine("Start Connection");
// for loop that check if the drone is connected and wait 1 second until check again
for (int i = 0; i < 10; i++)
{
    if (Connected)
        break;
    System.Threading.Thread.Sleep(1000);
}

Console.WriteLine("Connected. Battery Level: " + tello.State.BatteryPercentage);

// take off the drone
tello.TakeOff();
Console.WriteLine("Take Off");

// sleep of 3 seconds
System.Threading.Thread.Sleep(5000);

// tello drone flip right
tello.Flip(Messages.FlipType.FlipRight);
Console.WriteLine("Flip right");

// sleep of 3 seconds
System.Threading.Thread.Sleep(5000);

// land the drone
tello.Land();
Console.WriteLine("Land");