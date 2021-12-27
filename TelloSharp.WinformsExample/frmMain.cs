using static TelloSharp.Messages;

namespace TelloSharp.WinformsExample
{
    public partial class frmMain : Form
    {
        readonly Tello tello;
        private CameraModule cameraModule = new();
        private int _pError;
        private bool _processingCommand = false;

        public frmMain()
        {
            InitializeComponent();
            
            tello = new();

            tello.OnConnection += Tello_OnConnection; ;
            tello.OnUpdate += Tello_OnUpdate; ;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            tello.Connect();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            
        }

        private void Tello_OnUpdate(object? sender, TelloStateEventArgs e)
        {
            Invoke(new Action(() => lblBattery.Text = $"Battery: {e.State.BatteryPercentage}"));            
        }

        private void Tello_OnConnection(object? sender, Tello.ConnectionState e)
        {
            lblConState.Invoke(new Action(()=> lblConState.Text = e.ToString()));
        }

        private void btnTakeOff_Click(object sender, EventArgs e)
        {
            tello.TakeOff();
        }

        private void btnLand_Click(object sender, EventArgs e)
        {
            tello.Land();
        }

        private void btnStopLand_Click(object sender, EventArgs e)
        {
            tello.StopLanding();
        }

        private void btnThrowTakeOff_Click(object sender, EventArgs e)
        {
            tello.ThrowTakeOff();
        }

        private void btnPalmLand_Click(object sender, EventArgs e)
        {
            tello.PalmLanding();
        }

        private void btnBounce_Click(object sender, EventArgs e)
        {
            tello.Bounce();
        }

        private void btnFlip_Click(object sender, EventArgs e)
        {
            tello.Flip(FlipType.FlipForward);
        }

        private void btnStartSmartVideo_Click(object sender, EventArgs e)
        {
            tello.StartSmartVideo(SmartVideoCmd.Sv360);
        }

        private void btnStopSmartVideo_Click(object sender, EventArgs e)
        {
            tello.StopSmartVideo(SmartVideoCmd.Sv360);
        }

        private void btnHover_Click(object sender, EventArgs e)
        {
            tello.Hover();
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            tello.Left(20);
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            tello.Right(20);
        }

        private void btnForward_Click(object sender, EventArgs e)
        {
            tello.Forward(20);
        }

        private void btnBackward_Click(object sender, EventArgs e)
        {
            tello.Backward(20);
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            tello.Up(20);
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            tello.Down(20);
        }

        private void btnCw_Click(object sender, EventArgs e)
        {
            tello.ClockWise(20);
        }

        private void btnCcw_Click(object sender, EventArgs e)
        {
            tello.AntiClockWise(20);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tello.SendCommand("command");
            tello.SendCommand("streamon");

            cameraModule.NewCoordInfo += CameraModule_NewCoordInfo;
            cameraModule.Init();

            Task.Run(() => cameraModule.FindPerson(pictureBoxIpl1));

        }

        private void CameraModule_NewCoordInfo(object? sender, CoordinatesEventArgs e)
        {
            if (!_processingCommand)
            {
                _processingCommand = true;

                var areaTop = 6200;
                var areaBottom = 6800;

                Pid pid = new();

                short forwardBack = 0;
                var error = e.Location.X - pictureBoxIpl1.Width/2;
                short speed = (short)(pid.P * error + pid.I * (error - _pError));

                if (speed > 100) speed = 100;
                if (speed < -100) speed = -100;

                if (e.Location.X == 0)
                {
                    speed = 0;
                    error = 0;
                    tello.Hover();
                    _processingCommand = false;
                    return;
                }

                _pError = error; 

                if (e.Area > areaTop && e.Area < areaBottom)
                {
                    forwardBack = 0;
                    tello.Hover();
                }
                else if (e.Area > areaBottom)
                {
                    forwardBack = -20;
                }
                else if (e.Area < areaTop && e.Area != 0)
                {
                    forwardBack = 20;
                }

                Task.Run(() => Console.WriteLine($"Speed {speed} | ForwardBackward {forwardBack}"));

                tello.SendRCAxis(0, forwardBack, 0, speed);
                _processingCommand = false;
            }
        }
        
        public struct Pid
        {
            /// <summary>
            /// Proportional
            /// </summary>
            public double P = 0.4;
            /// <summary>
            /// Integral
            /// </summary>
            public double I = 0.4;
            /// <summary>
            /// Derivative
            /// </summary>
            public double D = 0;
        }
    }
}