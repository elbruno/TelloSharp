namespace TelloSharp.WinformsExample
{
    public partial class frmMain : Form
    {
        Tello tello;

        public frmMain()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {

        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            tello = new Tello();
            tello.OnConnection += Tello_OnConnection;
            tello.OnUpdate += Tello_OnUpdate;

        }

        private void Tello_OnUpdate(int cmdId)
        {
            
        }

        private void Tello_OnConnection(Tello.ConnectionState newState)
        {
            
        }
    }
}