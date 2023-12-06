TelloSharp.Tello tello = new();
tello.Connect();

// take off the drone
tello.TakeOff();

// sleep of 3 seconds
System.Threading.Thread.Sleep(3000);

// land the drone
tello.Land();
