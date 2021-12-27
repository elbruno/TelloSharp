namespace TelloSharp.WinformsExample
{
    partial class frmMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnTakeOff = new System.Windows.Forms.Button();
            this.btnLand = new System.Windows.Forms.Button();
            this.btnThrowTakeOff = new System.Windows.Forms.Button();
            this.btnStopLand = new System.Windows.Forms.Button();
            this.btnPalmLand = new System.Windows.Forms.Button();
            this.btnBounce = new System.Windows.Forms.Button();
            this.btnFlip = new System.Windows.Forms.Button();
            this.btnStartSmartVideo = new System.Windows.Forms.Button();
            this.btnStopSmartVideo = new System.Windows.Forms.Button();
            this.btnHover = new System.Windows.Forms.Button();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnRight = new System.Windows.Forms.Button();
            this.btnForward = new System.Windows.Forms.Button();
            this.btnBackward = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnCw = new System.Windows.Forms.Button();
            this.btnCcw = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.lblConState = new System.Windows.Forms.Label();
            this.btnStartCam = new System.Windows.Forms.Button();
            this.pictureBoxIpl1 = new OpenCvSharp.UserInterface.PictureBoxIpl();
            this.lblBattery = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIpl1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(18, 17);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnTakeOff
            // 
            this.btnTakeOff.Location = new System.Drawing.Point(18, 55);
            this.btnTakeOff.Name = "btnTakeOff";
            this.btnTakeOff.Size = new System.Drawing.Size(111, 23);
            this.btnTakeOff.TabIndex = 1;
            this.btnTakeOff.Text = "Take Off";
            this.btnTakeOff.UseVisualStyleBackColor = true;
            this.btnTakeOff.Click += new System.EventHandler(this.btnTakeOff_Click);
            // 
            // btnLand
            // 
            this.btnLand.Location = new System.Drawing.Point(342, 17);
            this.btnLand.Name = "btnLand";
            this.btnLand.Size = new System.Drawing.Size(75, 23);
            this.btnLand.TabIndex = 2;
            this.btnLand.Text = "Land";
            this.btnLand.UseVisualStyleBackColor = true;
            this.btnLand.Click += new System.EventHandler(this.btnLand_Click);
            // 
            // btnThrowTakeOff
            // 
            this.btnThrowTakeOff.Location = new System.Drawing.Point(18, 85);
            this.btnThrowTakeOff.Name = "btnThrowTakeOff";
            this.btnThrowTakeOff.Size = new System.Drawing.Size(111, 23);
            this.btnThrowTakeOff.TabIndex = 3;
            this.btnThrowTakeOff.Text = "Throw Take Off";
            this.btnThrowTakeOff.UseVisualStyleBackColor = true;
            this.btnThrowTakeOff.Click += new System.EventHandler(this.btnThrowTakeOff_Click);
            // 
            // btnStopLand
            // 
            this.btnStopLand.Location = new System.Drawing.Point(423, 17);
            this.btnStopLand.Name = "btnStopLand";
            this.btnStopLand.Size = new System.Drawing.Size(75, 23);
            this.btnStopLand.TabIndex = 4;
            this.btnStopLand.Text = "Stop Land";
            this.btnStopLand.UseVisualStyleBackColor = true;
            this.btnStopLand.Click += new System.EventHandler(this.btnStopLand_Click);
            // 
            // btnPalmLand
            // 
            this.btnPalmLand.Location = new System.Drawing.Point(504, 17);
            this.btnPalmLand.Name = "btnPalmLand";
            this.btnPalmLand.Size = new System.Drawing.Size(75, 23);
            this.btnPalmLand.TabIndex = 5;
            this.btnPalmLand.Text = "Palm Land";
            this.btnPalmLand.UseVisualStyleBackColor = true;
            this.btnPalmLand.Click += new System.EventHandler(this.btnPalmLand_Click);
            // 
            // btnBounce
            // 
            this.btnBounce.Location = new System.Drawing.Point(18, 143);
            this.btnBounce.Name = "btnBounce";
            this.btnBounce.Size = new System.Drawing.Size(75, 23);
            this.btnBounce.TabIndex = 6;
            this.btnBounce.Text = "Bounce";
            this.btnBounce.UseVisualStyleBackColor = true;
            this.btnBounce.Click += new System.EventHandler(this.btnBounce_Click);
            // 
            // btnFlip
            // 
            this.btnFlip.Location = new System.Drawing.Point(18, 172);
            this.btnFlip.Name = "btnFlip";
            this.btnFlip.Size = new System.Drawing.Size(75, 23);
            this.btnFlip.TabIndex = 7;
            this.btnFlip.Text = "Flip";
            this.btnFlip.UseVisualStyleBackColor = true;
            this.btnFlip.Click += new System.EventHandler(this.btnFlip_Click);
            // 
            // btnStartSmartVideo
            // 
            this.btnStartSmartVideo.Location = new System.Drawing.Point(18, 339);
            this.btnStartSmartVideo.Name = "btnStartSmartVideo";
            this.btnStartSmartVideo.Size = new System.Drawing.Size(111, 23);
            this.btnStartSmartVideo.TabIndex = 8;
            this.btnStartSmartVideo.Text = "Start SmartVid";
            this.btnStartSmartVideo.UseVisualStyleBackColor = true;
            this.btnStartSmartVideo.Click += new System.EventHandler(this.btnStartSmartVideo_Click);
            // 
            // btnStopSmartVideo
            // 
            this.btnStopSmartVideo.Location = new System.Drawing.Point(18, 368);
            this.btnStopSmartVideo.Name = "btnStopSmartVideo";
            this.btnStopSmartVideo.Size = new System.Drawing.Size(111, 23);
            this.btnStopSmartVideo.TabIndex = 9;
            this.btnStopSmartVideo.Text = "Stop SmartVid";
            this.btnStopSmartVideo.UseVisualStyleBackColor = true;
            this.btnStopSmartVideo.Click += new System.EventHandler(this.btnStopSmartVideo_Click);
            // 
            // btnHover
            // 
            this.btnHover.Location = new System.Drawing.Point(342, 143);
            this.btnHover.Name = "btnHover";
            this.btnHover.Size = new System.Drawing.Size(237, 23);
            this.btnHover.TabIndex = 10;
            this.btnHover.Text = "Hover";
            this.btnHover.UseVisualStyleBackColor = true;
            this.btnHover.Click += new System.EventHandler(this.btnHover_Click);
            // 
            // btnLeft
            // 
            this.btnLeft.Location = new System.Drawing.Point(469, 84);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(75, 23);
            this.btnLeft.TabIndex = 11;
            this.btnLeft.Text = "Left";
            this.btnLeft.UseVisualStyleBackColor = true;
            this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
            // 
            // btnRight
            // 
            this.btnRight.Location = new System.Drawing.Point(550, 84);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(75, 23);
            this.btnRight.TabIndex = 12;
            this.btnRight.Text = "Right";
            this.btnRight.UseVisualStyleBackColor = true;
            this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
            // 
            // btnForward
            // 
            this.btnForward.Location = new System.Drawing.Point(504, 55);
            this.btnForward.Name = "btnForward";
            this.btnForward.Size = new System.Drawing.Size(75, 23);
            this.btnForward.TabIndex = 13;
            this.btnForward.Text = "Forward";
            this.btnForward.UseVisualStyleBackColor = true;
            this.btnForward.Click += new System.EventHandler(this.btnForward_Click);
            // 
            // btnBackward
            // 
            this.btnBackward.Location = new System.Drawing.Point(504, 113);
            this.btnBackward.Name = "btnBackward";
            this.btnBackward.Size = new System.Drawing.Size(75, 23);
            this.btnBackward.TabIndex = 14;
            this.btnBackward.Text = "Backward";
            this.btnBackward.UseVisualStyleBackColor = true;
            this.btnBackward.Click += new System.EventHandler(this.btnBackward_Click);
            // 
            // btnUp
            // 
            this.btnUp.Location = new System.Drawing.Point(342, 55);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(75, 23);
            this.btnUp.TabIndex = 15;
            this.btnUp.Text = "Up";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnCw
            // 
            this.btnCw.Location = new System.Drawing.Point(285, 84);
            this.btnCw.Name = "btnCw";
            this.btnCw.Size = new System.Drawing.Size(81, 23);
            this.btnCw.TabIndex = 16;
            this.btnCw.Text = "Clockwise";
            this.btnCw.UseVisualStyleBackColor = true;
            this.btnCw.Click += new System.EventHandler(this.btnCw_Click);
            // 
            // btnCcw
            // 
            this.btnCcw.Location = new System.Drawing.Point(372, 84);
            this.btnCcw.Name = "btnCcw";
            this.btnCcw.Size = new System.Drawing.Size(91, 23);
            this.btnCcw.TabIndex = 17;
            this.btnCcw.Text = "C-Clockwise";
            this.btnCcw.UseVisualStyleBackColor = true;
            this.btnCcw.Click += new System.EventHandler(this.btnCcw_Click);
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(342, 114);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(75, 23);
            this.btnDown.TabIndex = 18;
            this.btnDown.Text = "Down";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // lblConState
            // 
            this.lblConState.AutoSize = true;
            this.lblConState.Location = new System.Drawing.Point(100, 20);
            this.lblConState.Name = "lblConState";
            this.lblConState.Size = new System.Drawing.Size(79, 15);
            this.lblConState.TabIndex = 19;
            this.lblConState.Text = "Disconnected";
            // 
            // btnStartCam
            // 
            this.btnStartCam.Location = new System.Drawing.Point(597, 354);
            this.btnStartCam.Name = "btnStartCam";
            this.btnStartCam.Size = new System.Drawing.Size(75, 23);
            this.btnStartCam.TabIndex = 20;
            this.btnStartCam.Text = "Start Cam";
            this.btnStartCam.UseVisualStyleBackColor = true;
            this.btnStartCam.Click += new System.EventHandler(this.button1_Click);
            // 
            // pictureBoxIpl1
            // 
            this.pictureBoxIpl1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBoxIpl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxIpl1.Location = new System.Drawing.Point(678, 17);
            this.pictureBoxIpl1.Name = "pictureBoxIpl1";
            this.pictureBoxIpl1.Size = new System.Drawing.Size(480, 360);
            this.pictureBoxIpl1.TabIndex = 21;
            this.pictureBoxIpl1.TabStop = false;
            // 
            // lblBattery
            // 
            this.lblBattery.AutoSize = true;
            this.lblBattery.Location = new System.Drawing.Point(185, 20);
            this.lblBattery.Name = "lblBattery";
            this.lblBattery.Size = new System.Drawing.Size(47, 15);
            this.lblBattery.TabIndex = 22;
            this.lblBattery.Text = "Battery:";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1174, 402);
            this.Controls.Add(this.lblBattery);
            this.Controls.Add(this.pictureBoxIpl1);
            this.Controls.Add(this.btnStartCam);
            this.Controls.Add(this.lblConState);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.btnCcw);
            this.Controls.Add(this.btnCw);
            this.Controls.Add(this.btnUp);
            this.Controls.Add(this.btnBackward);
            this.Controls.Add(this.btnForward);
            this.Controls.Add(this.btnRight);
            this.Controls.Add(this.btnLeft);
            this.Controls.Add(this.btnHover);
            this.Controls.Add(this.btnStopSmartVideo);
            this.Controls.Add(this.btnStartSmartVideo);
            this.Controls.Add(this.btnFlip);
            this.Controls.Add(this.btnBounce);
            this.Controls.Add(this.btnPalmLand);
            this.Controls.Add(this.btnStopLand);
            this.Controls.Add(this.btnThrowTakeOff);
            this.Controls.Add(this.btnLand);
            this.Controls.Add(this.btnTakeOff);
            this.Controls.Add(this.btnConnect);
            this.Name = "frmMain";
            this.Text = "TelloSharp";
            this.Load += new System.EventHandler(this.frmMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIpl1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button btnConnect;
        private Button btnTakeOff;
        private Button btnLand;
        private Button btnThrowTakeOff;
        private Button btnStopLand;
        private Button btnPalmLand;
        private Button btnBounce;
        private Button btnFlip;
        private Button btnStartSmartVideo;
        private Button btnStopSmartVideo;
        private Button btnHover;
        private Button btnLeft;
        private Button btnRight;
        private Button btnForward;
        private Button btnBackward;
        private Button btnUp;
        private Button btnCw;
        private Button btnCcw;
        private Button btnDown;
        private Label lblConState;
        private Button btnStartCam;
        private OpenCvSharp.UserInterface.PictureBoxIpl pictureBoxIpl1;
        private Label lblBattery;
    }
}