using TelloSharp;

namespace TelloSharp.Example
{
    public static class Program
    {
        static bool Connected = false;
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
            tello.StartConnecting();

            while (!tello.CancelTokens.Token.IsCancellationRequested)
            {
                string? line = Console.ReadLine();
                if (line != null)
                {
                    if (line.Contains("exit")) break;
                    if(line.Contains("cls")) Console.Clear();
                    if (line.Contains("takeoff")) tello.TakeOff();
                    if (line.Contains("land")) tello.Land(); 
                }
            }
        }

        private static void Tello_OnUpdate(int cmdId)
        {
            if (cmdId == 86) 
            {
                //write update to log.
                var elapsed = DateTime.Now - logStartTime;
                File.AppendAllText(logFilePath, elapsed.ToString(@"mm\:ss\:ff\,") + tello.State.GetLogLine());

                //display state in console.
                var outStr = tello.State.ToString();//ToString() = Formated state
                PrintAt(0, 2, outStr);
            }
        }

        private static void Tello_OnConnection(Tello.ConnectionState newState)
        {
            if(newState == Tello.ConnectionState.Connected) 
                Connected=true;
            if (newState == Tello.ConnectionState.Disconnected)
                Connected = false;
        }

        static void PrintAt(int x, int y, string str)
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
}