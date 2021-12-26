using static TelloSharp.Messages;

namespace TelloSharp.WinformsExample
{
    public partial class frmMain : Form
    {
        Tello tello;

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

        }

        private void Tello_OnConnection(object? sender, Tello.ConnectionState e)
        {
            
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
    }
}