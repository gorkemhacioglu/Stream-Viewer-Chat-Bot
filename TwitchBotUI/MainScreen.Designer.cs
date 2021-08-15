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
            ((System.ComponentModel.ISupportInitialize)(this.startStopButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picVulture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRefreshMinutes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tipLimitInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tipRefreshBrowser)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tipQuality)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnWithLoggedIn)).BeginInit();
            this.SuspendLayout();
            // 
            // startStopButton
            // 
            this.startStopButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("startStopButton.BackgroundImage")));
            this.startStopButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.startStopButton.Location = new System.Drawing.Point(395, 216);
            this.startStopButton.Name = "startStopButton";
            this.startStopButton.Size = new System.Drawing.Size(80, 40);
            this.startStopButton.TabIndex = 0;
            this.startStopButton.TabStop = false;
            this.startStopButton.Click += new System.EventHandler(this.startStopButton_Click);
            // 
            // txtStreamUrl
            // 
            this.txtStreamUrl.Location = new System.Drawing.Point(83, 238);
            this.txtStreamUrl.Name = "txtStreamUrl";
            this.txtStreamUrl.Size = new System.Drawing.Size(306, 23);
            this.txtStreamUrl.TabIndex = 1;
            // 
            // lblStreamUrl
            // 
            this.lblStreamUrl.AutoSize = true;
            this.lblStreamUrl.Location = new System.Drawing.Point(12, 241);
            this.lblStreamUrl.Name = "lblStreamUrl";
            this.lblStreamUrl.Size = new System.Drawing.Size(65, 15);
            this.lblStreamUrl.TabIndex = 2;
            this.lblStreamUrl.Text = "Stream Url:";
            // 
            // lblLog
            // 
            this.lblLog.AutoSize = true;
            this.lblLog.Location = new System.Drawing.Point(211, 8);
            this.lblLog.Name = "lblLog";
            this.lblLog.Size = new System.Drawing.Size(44, 15);
            this.lblLog.TabIndex = 4;
            this.lblLog.Text = "Logger";
            // 
            // checkHeadless
            // 
            this.checkHeadless.AutoSize = true;
            this.checkHeadless.Location = new System.Drawing.Point(457, 6);
            this.checkHeadless.Name = "checkHeadless";
            this.checkHeadless.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkHeadless.Size = new System.Drawing.Size(15, 14);
            this.checkHeadless.TabIndex = 5;
            this.checkHeadless.UseVisualStyleBackColor = true;
            this.checkHeadless.Visible = false;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // txtProxyList
            // 
            this.txtProxyList.Location = new System.Drawing.Point(123, 209);
            this.txtProxyList.Name = "txtProxyList";
            this.txtProxyList.Size = new System.Drawing.Size(266, 23);
            this.txtProxyList.TabIndex = 7;
            // 
            // browseProxyList
            // 
            this.browseProxyList.Location = new System.Drawing.Point(83, 209);
            this.browseProxyList.Name = "browseProxyList";
            this.browseProxyList.Size = new System.Drawing.Size(34, 23);
            this.browseProxyList.TabIndex = 8;
            this.browseProxyList.Text = "...";
            this.browseProxyList.UseVisualStyleBackColor = true;
            this.browseProxyList.Click += new System.EventHandler(this.browseProxyList_Click);
            // 
            // logScreen
            // 
            this.logScreen.Location = new System.Drawing.Point(8, 26);
            this.logScreen.Multiline = true;
            this.logScreen.Name = "logScreen";
            this.logScreen.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logScreen.Size = new System.Drawing.Size(467, 137);
            this.logScreen.TabIndex = 10;
            // 
            // lblBrowserLimit
            // 
            this.lblBrowserLimit.AutoSize = true;
            this.lblBrowserLimit.Location = new System.Drawing.Point(12, 184);
            this.lblBrowserLimit.Name = "lblBrowserLimit";
            this.lblBrowserLimit.Size = new System.Drawing.Size(82, 15);
            this.lblBrowserLimit.TabIndex = 11;
            this.lblBrowserLimit.Text = "Browser Limit:";
            // 
            // txtBrowserLimit
            // 
            this.txtBrowserLimit.Location = new System.Drawing.Point(100, 179);
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
            this.numRefreshMinutes.Location = new System.Drawing.Point(267, 180);
            this.numRefreshMinutes.Maximum = new decimal(new int[] {
            1440,
            0,
            0,
            0});
            this.numRefreshMinutes.Name = "numRefreshMinutes";
            this.numRefreshMinutes.Size = new System.Drawing.Size(48, 23);
            this.numRefreshMinutes.TabIndex = 14;
            this.numRefreshMinutes.Tag = "Refresh Browsers (Min.)";
            // 
            // lblRefreshMin
            // 
            this.lblRefreshMin.AutoSize = true;
            this.lblRefreshMin.Location = new System.Drawing.Point(182, 187);
            this.lblRefreshMin.Name = "lblRefreshMin";
            this.lblRefreshMin.Size = new System.Drawing.Size(53, 15);
            this.lblRefreshMin.TabIndex = 15;
            this.lblRefreshMin.Text = "(Minute)";
            // 
            // tipLimitInfo
            // 
            this.tipLimitInfo.Image = ((System.Drawing.Image)(resources.GetObject("tipLimitInfo.Image")));
            this.tipLimitInfo.Location = new System.Drawing.Point(144, 175);
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
            this.tipRefreshBrowser.Location = new System.Drawing.Point(321, 175);
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
            this.lblProxyList.Location = new System.Drawing.Point(12, 213);
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
            this.lblRefreshMin2.Location = new System.Drawing.Point(166, 172);
            this.lblRefreshMin2.Name = "lblRefreshMin2";
            this.lblRefreshMin2.Size = new System.Drawing.Size(88, 15);
            this.lblRefreshMin2.TabIndex = 15;
            this.lblRefreshMin2.Text = "Refresh Interval";
            // 
            // lblRefreshMin3
            // 
            this.lblRefreshMin3.AutoSize = true;
            this.lblRefreshMin3.Location = new System.Drawing.Point(251, 182);
            this.lblRefreshMin3.Name = "lblRefreshMin3";
            this.lblRefreshMin3.Size = new System.Drawing.Size(10, 15);
            this.lblRefreshMin3.TabIndex = 15;
            this.lblRefreshMin3.Text = ":";
            // 
            // lblQuality
            // 
            this.lblQuality.AutoSize = true;
            this.lblQuality.Location = new System.Drawing.Point(342, 184);
            this.lblQuality.Name = "lblQuality";
            this.lblQuality.Size = new System.Drawing.Size(48, 15);
            this.lblQuality.TabIndex = 19;
            this.lblQuality.Text = "Quality:";
            // 
            // lstQuality
            // 
            this.lstQuality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstQuality.Location = new System.Drawing.Point(391, 179);
            this.lstQuality.Name = "lstQuality";
            this.lstQuality.Size = new System.Drawing.Size(64, 23);
            this.lstQuality.TabIndex = 20;
            this.lstQuality.SelectedIndexChanged += new System.EventHandler(this.lstQuality_SelectedIndexChanged);
            // 
            // tipQuality
            // 
            this.tipQuality.Image = ((System.Drawing.Image)(resources.GetObject("tipQuality.Image")));
            this.tipQuality.Location = new System.Drawing.Point(457, 175);
            this.tipQuality.Name = "tipQuality";
            this.tipQuality.Size = new System.Drawing.Size(16, 16);
            this.tipQuality.TabIndex = 16;
            this.tipQuality.TabStop = false;
            this.tipQuality.MouseHover += new System.EventHandler(this.streamQuality_MouseHover);
            // 
            // txtLoginInfos
            // 
            this.txtLoginInfos.Location = new System.Drawing.Point(520, 23);
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
            // MainScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(820, 335);
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
            this.Controls.Add(this.logScreen);
            this.Controls.Add(this.browseProxyList);
            this.Controls.Add(this.txtProxyList);
            this.Controls.Add(this.checkHeadless);
            this.Controls.Add(this.lblLog);
            this.Controls.Add(this.lblStreamUrl);
            this.Controls.Add(this.txtStreamUrl);
            this.Controls.Add(this.startStopButton);
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
    }
}

