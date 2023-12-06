using TelloSharp;

namespace TelloSharp.DynamicCode01
{
    public static class Program
    {
        private static bool Connected;
        static string logPath = "logs/";
        static string logFilePath = string.Empty;
        static DateTime logStartTime = DateTime.Now;
        static Tello? tello = null;

        public static void Main()
        {
            ClearConsole();

            Directory.CreateDirectory(Path.Combine("../", logPath));
            logStartTime = DateTime.Now;
            logFilePath = Path.Combine("../", logPath + logStartTime.ToString("yyyy-dd-M--HH-mm-ss") + ".csv");

            tello = new();
            tello.OnConnection += Tello_OnConnection;
            tello.OnUpdate += Tello_OnUpdate;
            tello.Connect();

            // sleep of 2 seconds
            Thread.Sleep(2000);

            var d = new DynamicTelloActions();
            d.run(tello);

            //while (!tello.CancelTokens.Token.IsCancellationRequested)
            //{
            //    var line = Console.ReadLine();
            //    if (line != null)
            //    {
            //        if (line.Contains("exit")) break;
            //        if (line.Contains("cls")) Console.Clear();
            //        if (line.Contains("takeoff")) tello.TakeOff();
            //        if (line.Contains("land")) tello.Land();
            //        if (line.Contains("flip f")) tello.Flip(Messages.FlipType.FlipForward);
            //        if (line.Contains("flip b")) tello.Flip(Messages.FlipType.FlipBackward);
            //        if (line.Contains("flip l")) tello.Flip(Messages.FlipType.FlipBackwardLeft);
            //        if (line.Contains("flip r")) tello.Flip(Messages.FlipType.FlipBackwardRight);

            //    }
            //}


        }

        private static void Tello_OnConnection(object? sender, Tello.ConnectionState e)
        {
            if (e == Tello.ConnectionState.Connected)
            {
                Connected = true;
                tello.GetSSID();
                tello.GetVersion();
            }
            if (e == Tello.ConnectionState.Disconnected)
                Connected = false;
        }

        private static void Tello_OnUpdate(object? sender, TelloStateEventArgs e)
        {
            if (e.LastCmdId == Messages.MessageTypes.msgFlightStatus)
            {
                //write update to log.
                var elapsed = DateTime.Now - logStartTime;
                File.AppendAllText(logFilePath, elapsed.ToString(@"mm\:ss\:ff\,") + tello?.State.GetLogLine());

                //display state in console.
                //var outStr = tello?.State.ToString();
                //PrintAt(0, 2, outStr);
            }
        }


        static void PrintAt(int x, int y, string? str)
        {
            var saveLeft = Console.CursorLeft;
            var saveTop = Console.CursorTop;
            Console.SetCursorPosition(x, y);
            Console.WriteLine(str + "     ");
            Console.SetCursorPosition(saveLeft, saveTop);

        }
        static void ClearConsole()
        {
            Console.Clear();
            Console.SetCursorPosition(0, 23);
            Console.WriteLine("Commands:takeoff,land,exit,cls");
        }
    }

    class DynamicTelloActions()
    {
        public void run(dynamic telloDrone)
        {
            // take off the drone
            telloDrone.TakeOff();

            // sleep of 2 seconds
            Thread.Sleep(2000);

            // flip forward the drone
            telloDrone.Flip(Messages.FlipType.FlipForward);

            // sleep of 2 seconds
            Thread.Sleep(2000);

            // flip backward the drone
            telloDrone.Flip(Messages.FlipType.FlipBackward);

            // sleep of 2 seconds
            Thread.Sleep(2000);

            // land the drone
            telloDrone.Land();
        }
    }

    class StaticTelloActions()
    {
        public void run(Tello telloDrone)
        {
            // take off the drone
            telloDrone.TakeOff();

            // sleep of 2 seconds
            Thread.Sleep(2000);

            // flip forward the drone
            telloDrone.Flip(Messages.FlipType.FlipForward);

            // sleep of 2 seconds
            Thread.Sleep(2000);

            // flip backward the drone
            telloDrone.Flip(Messages.FlipType.FlipBackward);

            // sleep of 2 seconds
            Thread.Sleep(2000);

            // land the drone
            telloDrone.Land();
        }
    }
}