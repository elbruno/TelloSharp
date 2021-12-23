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
            this.btnTakeOff.Location = new System.Drawing.Point(175, 17);
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
            this.btnThrowTakeOff.Location = new System.Drawing.Point(175, 46);
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
            this.btnBounce.Location = new System.Drawing.Point(175, 85);
            this.btnBounce.Name = "btnBounce";
            this.btnBounce.Size = new System.Drawing.Size(75, 23);
            this.btnBounce.TabIndex = 6;
            this.btnBounce.Text = "Bounce";
            this.btnBounce.UseVisualStyleBackColor = true;
            this.btnBounce.Click += new System.EventHandler(this.btnBounce_Click);
            // 
            // btnFlip
            // 
            this.btnFlip.Location = new System.Drawing.Point(175, 114);
            this.btnFlip.Name = "btnFlip";
            this.btnFlip.Size = new System.Drawing.Size(75, 23);
            this.btnFlip.TabIndex = 7;
            this.btnFlip.Text = "Flip";
            this.btnFlip.UseVisualStyleBackColor = true;
            this.btnFlip.Click += new System.EventHandler(this.btnFlip_Click);
            // 
            // btnStartSmartVideo
            // 
            this.btnStartSmartVideo.Location = new System.Drawing.Point(175, 152);
            this.btnStartSmartVideo.Name = "btnStartSmartVideo";
            this.btnStartSmartVideo.Size = new System.Drawing.Size(111, 23);
            this.btnStartSmartVideo.TabIndex = 8;
            this.btnStartSmartVideo.Text = "Start SmartVid";
            this.btnStartSmartVideo.UseVisualStyleBackColor = true;
            this.btnStartSmartVideo.Click += new System.EventHandler(this.btnStartSmartVideo_Click);
            // 
            // btnStopSmartVideo
            // 
            this.btnStopSmartVideo.Location = new System.Drawing.Point(175, 181);
            this.btnStopSmartVideo.Name = "btnStopSmartVideo";
            this.btnStopSmartVideo.Size = new System.Drawing.Size(111, 23);
            this.btnStopSmartVideo.TabIndex = 9;
            this.btnStopSmartVideo.Text = "Stop SmartVid";
            this.btnStopSmartVideo.UseVisualStyleBackColor = true;
            this.btnStopSmartVideo.Click += new System.EventHandler(this.btnStopSmartVideo_Click);
            // 
            // btnHover
            // 
            this.btnHover.Location = new System.Drawing.Point(175, 210);
            this.btnHover.Name = "btnHover";
            this.btnHover.Size = new System.Drawing.Size(75, 23);
            this.btnHover.TabIndex = 10;
            this.btnHover.Text = "Hover";
            this.btnHover.UseVisualStyleBackColor = true;
            this.btnHover.Click += new System.EventHandler(this.btnHover_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
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
            this.ResumeLayout(false);

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
    }
}