// --------------------------
// Section usings
// --------------------------
using TelloSharp;

// --------------------------
// Section connect to drone
// --------------------------
int i = 0;
Tello tello = new();
tello.Connect();

// --------------------------
// Section validate connection
// --------------------------
Console.WriteLine("Start Connection");
while (tello._connectionState != Tello.ConnectionState.Connected)
{    
    Console.WriteLine("Connecting ... " + i);
    System.Threading.Thread.Sleep(1000);
    i++;
    if (i == 100)
        break;
}

// --------------------------
// Section display battery level
// --------------------------
Console.WriteLine("Connected. Battery Level " + tello.State.BatteryPercentage);

// --------------------------
// Section actions
// --------------------------
// take off the drone
tello.TakeOff();
Console.WriteLine("Take Off");

// sleep of 5 seconds
System.Threading.Thread.Sleep(5000);

//// tello drone flip right
//tello.Flip(Messages.FlipType.FlipRight);
//Console.WriteLine("Flip right");

//// tello drone flip backward
//tello.Flip(Messages.FlipType.FlipBackward);
//Console.WriteLine("Flip backward");

//// tello drone flip left
//tello.Flip(Messages.FlipType.FlipLeft);
//Console.WriteLine("Flip left");

//// tello drone flip forward
//tello.Flip(Messages.FlipType.FlipForward);
//Console.WriteLine("Flip forward");

//// tello drone move up 50 centimeters
//tello.Up(50);
//Console.WriteLine("Up 50");

//// tello drone move down 50 centimeters
//tello.Down(50);
//Console.WriteLine("Down 50");

//// tello drone move left 50 centimeters
//tello.Left(50);
//Console.WriteLine("Left 50");

//// tello drone move right 50 centimeters
//tello.Right(50);
//Console.WriteLine("Right 50");

//// tello drone move forward 50 centimeters
//tello.Forward(50);
//Console.WriteLine("Forward 50");

//// tello drone move backward 50 centimeters
//tello.Backward(50);
//Console.WriteLine("Backward 50");

//// tello drone rotate clockwise 90 degrees
//tello.ClockWise(90);
//Console.WriteLine("Clockwise 90");

//// tello drone rotate counter clockwise 90 degrees
//tello.AntiClockWise(90);
//Console.WriteLine("Counter Clockwise 90");

// tello drone bounce
tello.Bounce();
Console.WriteLine("Bounce");
System.Threading.Thread.Sleep(5000);

// --------------------------
// Section land drone
// --------------------------
// land the drone
tello.Land();
Console.WriteLine("Land");