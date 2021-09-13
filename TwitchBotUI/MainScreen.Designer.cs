namespace TwitchBotUI
{
    partial class MainScreen
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainScreen));
            this.startStopButton = new System.Windows.Forms.PictureBox();
            this.txtStreamUrl = new System.Windows.Forms.TextBox();
            this.lblStreamUrl = new System.Windows.Forms.Label();
            this.lblLog = new System.Windows.Forms.Label();
            this.checkHeadless = new System.Windows.Forms.CheckBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.txtProxyList = new System.Windows.Forms.TextBox();
            this.browseProxyList = new System.Windows.Forms.Button();
            this.logScreen = new System.Windows.Forms.TextBox();
            this.lblBrowserLimit = new System.Windows.Forms.Label();
            this.txtBrowserLimit = new System.Windows.Forms.TextBox();
            this.picVulture = new System.Windows.Forms.PictureBox();
            this.numRefreshMinutes = new System.Windows.Forms.NumericUpDown();
            this.lblRefreshMin = new System.Windows.Forms.Label();
            this.tipLimitInfo = new System.Windows.Forms.PictureBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.tipRefreshBrowser = new System.Windows.Forms.PictureBox();
            this.lblProxyList = new System.Windows.Forms.LinkLabel();
            this.lblRefreshMin2 = new System.Windows.Forms.Label();
            this.lblRefreshMin3 = new System.Windows.Forms.Label();
            this.lblQuality = new System.Windows.Forms.Label();
            this.lstQuality = new System.Windows.Forms.ComboBox();
            this.tipQuality = new System.Windows.Forms.PictureBox();
            this.txtLoginInfos = new System.Windows.Forms.TextBox();
            this.btnWithLoggedIn = new System.Windows.Forms.PictureBox();
            this.lblLoginInfoTitle = new System.Windows.Forms.Label();
            this.lblHeadless = new System.Windows.Forms.Label();
            this.picLiveViewer = new System.Windows.Forms.PictureBox();
            this.lblViewer = new System.Windows.Forms.Label();
            this.picBotViewer = new System.Windows.Forms.PictureBox();
            this.lblLiveViewer = new System.Windows.Forms.Label();
            this.tipLiveViewer = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.startStopButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picVulture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRefreshMinutes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tipLimitInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tipRefreshBrowser)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tipQuality)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnWithLoggedIn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLiveViewer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBotViewer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tipLiveViewer)).BeginInit();
            this.SuspendLayout();
            // 
            // startStopButton
            // 
            this.startStopButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("startStopButton.BackgroundImage")));
            this.startStopButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.startStopButton.Location = new System.Drawing.Point(395, 225);
            this.startStopButton.Name = "startStopButton";
            this.startStopButton.Size = new System.Drawing.Size(80, 40);
            this.startStopButton.TabIndex = 0;
            this.startStopButton.TabStop = false;
            this.startStopButton.Click += new System.EventHandler(this.startStopButton_Click);
            // 
            // txtStreamUrl
            // 
            this.txtStreamUrl.Location = new System.Drawing.Point(83, 237);
            this.txtStreamUrl.Name = "txtStreamUrl";
            this.txtStreamUrl.Size = new System.Drawing.Size(306, 23);
            this.txtStreamUrl.TabIndex = 1;
            // 
            // lblStreamUrl
            // 
            this.lblStreamUrl.AutoSize = true;
            this.lblStreamUrl.Location = new System.Drawing.Point(12, 240);
            this.lblStreamUrl.Name = "lblStreamUrl";
            this.lblStreamUrl.Size = new System.Drawing.Size(65, 15);
            this.lblStreamUrl.TabIndex = 2;
            this.lblStreamUrl.Text = "Stream Url:";
            // 
            // lblLog
            // 
            this.lblLog.AutoSize = true;
            this.lblLog.BackColor = System.Drawing.Color.White;
            this.lblLog.Font = new System.Drawing.Font("Segoe UI", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point);
            this.lblLog.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblLog.Location = new System.Drawing.Point(410, 15);
            this.lblLog.Name = "lblLog";
            this.lblLog.Size = new System.Drawing.Size(45, 15);
            this.lblLog.TabIndex = 4;
            this.lblLog.Text = "Logger";
            // 
            // checkHeadless
            // 
            this.checkHeadless.AutoSize = true;
            this.checkHeadless.Location = new System.Drawing.Point(100, 181);
            this.checkHeadless.Name = "checkHeadless";
            this.checkHeadless.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkHeadless.Size = new System.Drawing.Size(15, 14);
            this.checkHeadless.TabIndex = 5;
            this.checkHeadless.UseVisualStyleBackColor = true;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // txtProxyList
            // 
            this.txtProxyList.Location = new System.Drawing.Point(123, 204);
            this.txtProxyList.Name = "txtProxyList";
            this.txtProxyList.Size = new System.Drawing.Size(266, 23);
            this.txtProxyList.TabIndex = 7;
            // 
            // browseProxyList
            // 
            this.browseProxyList.Location = new System.Drawing.Point(83, 204);
            this.browseProxyList.Name = "browseProxyList";
            this.browseProxyList.Size = new System.Drawing.Size(34, 23);
            this.browseProxyList.TabIndex = 8;
            this.browseProxyList.Text = "...";
            this.browseProxyList.UseVisualStyleBackColor = true;
            this.browseProxyList.Click += new System.EventHandler(this.browseProxyList_Click);
            // 
            // logScreen
            // 
            this.logScreen.Location = new System.Drawing.Point(9, 12);
            this.logScreen.Multiline = true;
            this.logScreen.Name = "logScreen";
            this.logScreen.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logScreen.Size = new System.Drawing.Size(467, 127);
            this.logScreen.TabIndex = 10;
            // 
            // lblBrowserLimit
            // 
            this.lblBrowserLimit.AutoSize = true;
            this.lblBrowserLimit.Location = new System.Drawing.Point(12, 151);
            this.lblBrowserLimit.Name = "lblBrowserLimit";
            this.lblBrowserLimit.Size = new System.Drawing.Size(85, 15);
            this.lblBrowserLimit.TabIndex = 11;
            this.lblBrowserLimit.Text = "Browser Limit :";
            // 
            // txtBrowserLimit
            // 
            this.txtBrowserLimit.Location = new System.Drawing.Point(100, 147);
            this.txtBrowserLimit.Name = "txtBrowserLimit";
            this.txtBrowserLimit.PlaceholderText = "0";
            this.txtBrowserLimit.Size = new System.Drawing.Size(40, 23);
            this.txtBrowserLimit.TabIndex = 12;
            this.txtBrowserLimit.TextChanged += new System.EventHandler(this.txtBrowserLimit_TextChanged);
            // 
            // picVulture
            // 
            this.picVulture.Image = ((System.Drawing.Image)(resources.GetObject("picVulture.Image")));
            this.picVulture.Location = new System.Drawing.Point(8, 266);
            this.picVulture.Name = "picVulture";
            this.picVulture.Size = new System.Drawing.Size(468, 60);
            this.picVulture.TabIndex = 13;
            this.picVulture.TabStop = false;
            this.picVulture.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // numRefreshMinutes
            // 
            this.numRefreshMinutes.Location = new System.Drawing.Point(265, 147);
            this.numRefreshMinutes.Maximum = new decimal(new int[] {
            1440,
            0,
            0,
            0});
            this.numRefreshMinutes.Name = "numRefreshMinutes";
            this.numRefreshMinutes.Size = new System.Drawing.Size(64, 23);
            this.numRefreshMinutes.TabIndex = 14;
            this.numRefreshMinutes.Tag = "Refresh Browsers (Min.)";
            // 
            // lblRefreshMin
            // 
            this.lblRefreshMin.AutoSize = true;
            this.lblRefreshMin.Location = new System.Drawing.Point(183, 158);
            this.lblRefreshMin.Name = "lblRefreshMin";
            this.lblRefreshMin.Size = new System.Drawing.Size(53, 15);
            this.lblRefreshMin.TabIndex = 15;
            this.lblRefreshMin.Text = "(Minute)";
            // 
            // tipLimitInfo
            // 
            this.tipLimitInfo.Image = ((System.Drawing.Image)(resources.GetObject("tipLimitInfo.Image")));
            this.tipLimitInfo.Location = new System.Drawing.Point(142, 145);
            this.tipLimitInfo.Name = "tipLimitInfo";
            this.tipLimitInfo.Size = new System.Drawing.Size(16, 16);
            this.tipLimitInfo.TabIndex = 16;
            this.tipLimitInfo.TabStop = false;
            this.tipLimitInfo.MouseHover += new System.EventHandler(this.picLimitInfo_MouseHover);
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 10000;
            this.toolTip.BackColor = System.Drawing.Color.Aqua;
            this.toolTip.InitialDelay = 300;
            this.toolTip.IsBalloon = true;
            this.toolTip.ReshowDelay = 100;
            // 
            // tipRefreshBrowser
            // 
            this.tipRefreshBrowser.Image = ((System.Drawing.Image)(resources.GetObject("tipRefreshBrowser.Image")));
            this.tipRefreshBrowser.Location = new System.Drawing.Point(335, 145);
            this.tipRefreshBrowser.Name = "tipRefreshBrowser";
            this.tipRefreshBrowser.Size = new System.Drawing.Size(16, 16);
            this.tipRefreshBrowser.TabIndex = 16;
            this.tipRefreshBrowser.TabStop = false;
            this.tipRefreshBrowser.MouseHover += new System.EventHandler(this.refreshInterval_MouseHover);
            // 
            // lblProxyList
            // 
            this.lblProxyList.AutoSize = true;
            this.lblProxyList.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.lblProxyList.Location = new System.Drawing.Point(12, 208);
            this.lblProxyList.Name = "lblProxyList";
            this.lblProxyList.Size = new System.Drawing.Size(61, 15);
            this.lblProxyList.TabIndex = 17;
            this.lblProxyList.TabStop = true;
            this.lblProxyList.Text = "Proxy List:";
            this.lblProxyList.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblProxyList_LinkClicked);
            // 
            // lblRefreshMin2
            // 
            this.lblRefreshMin2.AutoSize = true;
            this.lblRefreshMin2.Location = new System.Drawing.Point(164, 144);
            this.lblRefreshMin2.Name = "lblRefreshMin2";
            this.lblRefreshMin2.Size = new System.Drawing.Size(88, 15);
            this.lblRefreshMin2.TabIndex = 15;
            this.lblRefreshMin2.Text = "Refresh Interval";
            // 
            // lblRefreshMin3
            // 
            this.lblRefreshMin3.AutoSize = true;
            this.lblRefreshMin3.Location = new System.Drawing.Point(251, 149);
            this.lblRefreshMin3.Name = "lblRefreshMin3";
            this.lblRefreshMin3.Size = new System.Drawing.Size(10, 15);
            this.lblRefreshMin3.TabIndex = 15;
            this.lblRefreshMin3.Text = ":";
            // 
            // lblQuality
            // 
            this.lblQuality.AutoSize = true;
            this.lblQuality.Location = new System.Drawing.Point(188, 177);
            this.lblQuality.Name = "lblQuality";
            this.lblQuality.Size = new System.Drawing.Size(72, 15);
            this.lblQuality.TabIndex = 19;
            this.lblQuality.Text = "Quality        :";
            // 
            // lstQuality
            // 
            this.lstQuality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstQuality.Location = new System.Drawing.Point(265, 174);
            this.lstQuality.Name = "lstQuality";
            this.lstQuality.Size = new System.Drawing.Size(64, 23);
            this.lstQuality.TabIndex = 20;
            this.lstQuality.SelectedIndexChanged += new System.EventHandler(this.lstQuality_SelectedIndexChanged);
            // 
            // tipQuality
            // 
            this.tipQuality.Image = ((System.Drawing.Image)(resources.GetObject("tipQuality.Image")));
            this.tipQuality.Location = new System.Drawing.Point(335, 167);
            this.tipQuality.Name = "tipQuality";
            this.tipQuality.Size = new System.Drawing.Size(16, 16);
            this.tipQuality.TabIndex = 16;
            this.tipQuality.TabStop = false;
            this.tipQuality.MouseHover += new System.EventHandler(this.streamQuality_MouseHover);
            // 
            // txtLoginInfos
            // 
            this.txtLoginInfos.Location = new System.Drawing.Point(520, 26);
            this.txtLoginInfos.Multiline = true;
            this.txtLoginInfos.Name = "txtLoginInfos";
            this.txtLoginInfos.PlaceholderText = "Format is =>Username{Blank}Password{Enter}";
            this.txtLoginInfos.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLoginInfos.Size = new System.Drawing.Size(288, 300);
            this.txtLoginInfos.TabIndex = 21;
            this.txtLoginInfos.WordWrap = false;
            // 
            // btnWithLoggedIn
            // 
            this.btnWithLoggedIn.Image = ((System.Drawing.Image)(resources.GetObject("btnWithLoggedIn.Image")));
            this.btnWithLoggedIn.Location = new System.Drawing.Point(485, 109);
            this.btnWithLoggedIn.Name = "btnWithLoggedIn";
            this.btnWithLoggedIn.Size = new System.Drawing.Size(29, 144);
            this.btnWithLoggedIn.TabIndex = 22;
            this.btnWithLoggedIn.TabStop = false;
            this.btnWithLoggedIn.Click += new System.EventHandler(this.btnWithLoggedIn_Click);
            // 
            // lblLoginInfoTitle
            // 
            this.lblLoginInfoTitle.AutoSize = true;
            this.lblLoginInfoTitle.Location = new System.Drawing.Point(609, 8);
            this.lblLoginInfoTitle.Name = "lblLoginInfoTitle";
            this.lblLoginInfoTitle.Size = new System.Drawing.Size(99, 15);
            this.lblLoginInfoTitle.TabIndex = 23;
            this.lblLoginInfoTitle.Text = "Login Credentials";
            // 
            // lblHeadless
            // 
            this.lblHeadless.AutoSize = true;
            this.lblHeadless.Location = new System.Drawing.Point(12, 180);
            this.lblHeadless.Name = "lblHeadless";
            this.lblHeadless.Size = new System.Drawing.Size(84, 15);
            this.lblHeadless.TabIndex = 24;
            this.lblHeadless.Text = "Headless         :";
            // 
            // picLiveViewer
            // 
            this.picLiveViewer.Image = ((System.Drawing.Image)(resources.GetObject("picLiveViewer.Image")));
            this.picLiveViewer.Location = new System.Drawing.Point(425, 147);
            this.picLiveViewer.Name = "picLiveViewer";
            this.picLiveViewer.Size = new System.Drawing.Size(32, 32);
            this.picLiveViewer.TabIndex = 25;
            this.picLiveViewer.TabStop = false;
            // 
            // lblViewer
            // 
            this.lblViewer.Location = new System.Drawing.Point(376, 184);
            this.lblViewer.MaximumSize = new System.Drawing.Size(50, 13);
            this.lblViewer.Name = "lblViewer";
            this.lblViewer.Size = new System.Drawing.Size(32, 13);
            this.lblViewer.TabIndex = 26;
            this.lblViewer.Text = "0";
            this.lblViewer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // picBotViewer
            // 
            this.picBotViewer.Image = ((System.Drawing.Image)(resources.GetObject("picBotViewer.Image")));
            this.picBotViewer.Location = new System.Drawing.Point(376, 147);
            this.picBotViewer.Name = "picBotViewer";
            this.picBotViewer.Size = new System.Drawing.Size(32, 32);
            this.picBotViewer.TabIndex = 27;
            this.picBotViewer.TabStop = false;
            // 
            // lblLiveViewer
            // 
            this.lblLiveViewer.Location = new System.Drawing.Point(425, 184);
            this.lblLiveViewer.MaximumSize = new System.Drawing.Size(50, 13);
            this.lblLiveViewer.Name = "lblLiveViewer";
            this.lblLiveViewer.Size = new System.Drawing.Size(32, 13);
            this.lblLiveViewer.TabIndex = 28;
            this.lblLiveViewer.Text = "0";
            this.lblLiveViewer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tipLiveViewer
            // 
            this.tipLiveViewer.Image = ((System.Drawing.Image)(resources.GetObject("tipLiveViewer.Image")));
            this.tipLiveViewer.Location = new System.Drawing.Point(459, 145);
            this.tipLiveViewer.Name = "tipLiveViewer";
            this.tipLiveViewer.Size = new System.Drawing.Size(16, 16);
            this.tipLiveViewer.TabIndex = 29;
            this.tipLiveViewer.TabStop = false;
            this.tipLiveViewer.MouseHover += new System.EventHandler(this.tipLiveViewer_MouseHover);
            // 
            // MainScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(820, 335);
            this.Controls.Add(this.tipLiveViewer);
            this.Controls.Add(this.lblLiveViewer);
            this.Controls.Add(this.picBotViewer);
            this.Controls.Add(this.lblViewer);
            this.Controls.Add(this.picLiveViewer);
            this.Controls.Add(this.lblLog);
            this.Controls.Add(this.lblHeadless);
            this.Controls.Add(this.lblLoginInfoTitle);
            this.Controls.Add(this.btnWithLoggedIn);
            this.Controls.Add(this.txtLoginInfos);
            this.Controls.Add(this.tipQuality);
            this.Controls.Add(this.lstQuality);
            this.Controls.Add(this.lblQuality);
            this.Controls.Add(this.lblRefreshMin3);
            this.Controls.Add(this.lblRefreshMin2);
            this.Controls.Add(this.lblProxyList);
            this.Controls.Add(this.tipRefreshBrowser);
            this.Controls.Add(this.tipLimitInfo);
            this.Controls.Add(this.lblRefreshMin);
            this.Controls.Add(this.numRefreshMinutes);
            this.Controls.Add(this.picVulture);
            this.Controls.Add(this.txtBrowserLimit);
            this.Controls.Add(this.lblBrowserLimit);
            this.Controls.Add(this.browseProxyList);
            this.Controls.Add(this.txtProxyList);
            this.Controls.Add(this.checkHeadless);
            this.Controls.Add(this.lblStreamUrl);
            this.Controls.Add(this.txtStreamUrl);
            this.Controls.Add(this.startStopButton);
            this.Controls.Add(this.logScreen);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainScreen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "My Twitch Viewer Bot";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainScreen_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.startStopButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picVulture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRefreshMinutes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tipLimitInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tipRefreshBrowser)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tipQuality)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnWithLoggedIn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLiveViewer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBotViewer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tipLiveViewer)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox startStopButton;
        private System.Windows.Forms.TextBox txtStreamUrl;
        private System.Windows.Forms.Label lblStreamUrl;
        private System.Windows.Forms.Label lblLog;
        private System.Windows.Forms.CheckBox checkHeadless;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button browseProxyList;
        private System.Windows.Forms.TextBox txtProxyList;
        private System.Windows.Forms.TextBox logScreen;
        private System.Windows.Forms.Label lblBrowserLimit;
        private System.Windows.Forms.TextBox txtBrowserLimit;
        private System.Windows.Forms.PictureBox picVulture;
        private System.Windows.Forms.NumericUpDown numRefreshMinutes;
        private System.Windows.Forms.Label lblRefreshMin;
        private System.Windows.Forms.PictureBox tipLimitInfo;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.PictureBox tipRefreshBrowser;
        private System.Windows.Forms.LinkLabel lblProxyList;
        private System.Windows.Forms.Label lblRefreshMin2;
        private System.Windows.Forms.Label lblRefreshMin3;
        private System.Windows.Forms.Label lblQuality;
        private System.Windows.Forms.ComboBox lstQuality;
        private System.Windows.Forms.PictureBox tipQuality;
        private System.Windows.Forms.TextBox txtLoginInfos;
        private System.Windows.Forms.PictureBox btnWithLoggedIn;
        private System.Windows.Forms.Label lblLoginInfoTitle;
        private System.Windows.Forms.Label lblHeadless;
        private System.Windows.Forms.PictureBox picLiveViewer;
        private System.Windows.Forms.Label lblViewer;
        private System.Windows.Forms.PictureBox picBotViewer;
        private System.Windows.Forms.Label lblLiveViewer;
        private System.Windows.Forms.PictureBox tipLiveViewer;
    }
}

