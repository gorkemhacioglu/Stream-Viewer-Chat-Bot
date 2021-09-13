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
            this.startStopButton.Location = new System.Drawing.Point(889, 506);
            this.startStopButton.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.startStopButton.Name = "startStopButton";
            this.startStopButton.Size = new System.Drawing.Size(180, 90);
            this.startStopButton.TabIndex = 0;
            this.startStopButton.TabStop = false;
            this.startStopButton.Click += new System.EventHandler(this.startStopButton_Click);
            // 
            // txtStreamUrl
            // 
            this.txtStreamUrl.Location = new System.Drawing.Point(187, 533);
            this.txtStreamUrl.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.txtStreamUrl.Name = "txtStreamUrl";
            this.txtStreamUrl.Size = new System.Drawing.Size(684, 43);
            this.txtStreamUrl.TabIndex = 1;
            // 
            // lblStreamUrl
            // 
            this.lblStreamUrl.AutoSize = true;
            this.lblStreamUrl.Location = new System.Drawing.Point(27, 540);
            this.lblStreamUrl.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.lblStreamUrl.Name = "lblStreamUrl";
            this.lblStreamUrl.Size = new System.Drawing.Size(147, 37);
            this.lblStreamUrl.TabIndex = 2;
            this.lblStreamUrl.Text = "Stream Url:";
            // 
            // lblLog
            // 
            this.lblLog.AutoSize = true;
            this.lblLog.BackColor = System.Drawing.Color.White;
            this.lblLog.Font = new System.Drawing.Font("Segoe UI", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point);
            this.lblLog.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblLog.Location = new System.Drawing.Point(922, 34);
            this.lblLog.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.lblLog.Name = "lblLog";
            this.lblLog.Size = new System.Drawing.Size(104, 37);
            this.lblLog.TabIndex = 4;
            this.lblLog.Text = "Logger";
            // 
            // checkHeadless
            // 
            this.checkHeadless.AutoSize = true;
            this.checkHeadless.Location = new System.Drawing.Point(225, 407);
            this.checkHeadless.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.checkHeadless.Name = "checkHeadless";
            this.checkHeadless.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkHeadless.Size = new System.Drawing.Size(28, 27);
            this.checkHeadless.TabIndex = 5;
            this.checkHeadless.UseVisualStyleBackColor = true;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // txtProxyList
            // 
            this.txtProxyList.Location = new System.Drawing.Point(277, 459);
            this.txtProxyList.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.txtProxyList.Name = "txtProxyList";
            this.txtProxyList.Size = new System.Drawing.Size(594, 43);
            this.txtProxyList.TabIndex = 7;
            // 
            // browseProxyList
            // 
            this.browseProxyList.Location = new System.Drawing.Point(187, 459);
            this.browseProxyList.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.browseProxyList.Name = "browseProxyList";
            this.browseProxyList.Size = new System.Drawing.Size(76, 52);
            this.browseProxyList.TabIndex = 8;
            this.browseProxyList.Text = "...";
            this.browseProxyList.UseVisualStyleBackColor = true;
            this.browseProxyList.Click += new System.EventHandler(this.browseProxyList_Click);
            // 
            // logScreen
            // 
            this.logScreen.Location = new System.Drawing.Point(20, 27);
            this.logScreen.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.logScreen.Multiline = true;
            this.logScreen.Name = "logScreen";
            this.logScreen.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logScreen.Size = new System.Drawing.Size(1046, 281);
            this.logScreen.TabIndex = 10;
            // 
            // lblBrowserLimit
            // 
            this.lblBrowserLimit.AutoSize = true;
            this.lblBrowserLimit.Location = new System.Drawing.Point(27, 340);
            this.lblBrowserLimit.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.lblBrowserLimit.Name = "lblBrowserLimit";
            this.lblBrowserLimit.Size = new System.Drawing.Size(190, 37);
            this.lblBrowserLimit.TabIndex = 11;
            this.lblBrowserLimit.Text = "Browser Limit :";
            // 
            // txtBrowserLimit
            // 
            this.txtBrowserLimit.Location = new System.Drawing.Point(225, 331);
            this.txtBrowserLimit.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.txtBrowserLimit.Name = "txtBrowserLimit";
            this.txtBrowserLimit.PlaceholderText = "0";
            this.txtBrowserLimit.Size = new System.Drawing.Size(85, 43);
            this.txtBrowserLimit.TabIndex = 12;
            this.txtBrowserLimit.TextChanged += new System.EventHandler(this.txtBrowserLimit_TextChanged);
            // 
            // picVulture
            // 
            this.picVulture.BackgroundImage = global::TwitchBotUI.Properties.Resources.icon_onwhite1;
            this.picVulture.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.picVulture.Location = new System.Drawing.Point(18, 598);
            this.picVulture.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.picVulture.Name = "picVulture";
            this.picVulture.Size = new System.Drawing.Size(1053, 135);
            this.picVulture.TabIndex = 13;
            this.picVulture.TabStop = false;
            this.picVulture.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // numRefreshMinutes
            // 
            this.numRefreshMinutes.Location = new System.Drawing.Point(596, 331);
            this.numRefreshMinutes.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.numRefreshMinutes.Maximum = new decimal(new int[] {
            1440,
            0,
            0,
            0});
            this.numRefreshMinutes.Name = "numRefreshMinutes";
            this.numRefreshMinutes.Size = new System.Drawing.Size(144, 43);
            this.numRefreshMinutes.TabIndex = 14;
            this.numRefreshMinutes.Tag = "Refresh Browsers (Min.)";
            // 
            // lblRefreshMin
            // 
            this.lblRefreshMin.AutoSize = true;
            this.lblRefreshMin.Location = new System.Drawing.Point(412, 356);
            this.lblRefreshMin.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.lblRefreshMin.Name = "lblRefreshMin";
            this.lblRefreshMin.Size = new System.Drawing.Size(117, 37);
            this.lblRefreshMin.TabIndex = 15;
            this.lblRefreshMin.Text = "(Minute)";
            // 
            // tipLimitInfo
            // 
            this.tipLimitInfo.BackgroundImage = global::TwitchBotUI.Properties.Resources.info;
            this.tipLimitInfo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.tipLimitInfo.Location = new System.Drawing.Point(320, 326);
            this.tipLimitInfo.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.tipLimitInfo.Name = "tipLimitInfo";
            this.tipLimitInfo.Size = new System.Drawing.Size(36, 36);
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
            this.tipRefreshBrowser.BackgroundImage = global::TwitchBotUI.Properties.Resources.info;
            this.tipRefreshBrowser.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.tipRefreshBrowser.Location = new System.Drawing.Point(754, 326);
            this.tipRefreshBrowser.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.tipRefreshBrowser.Name = "tipRefreshBrowser";
            this.tipRefreshBrowser.Size = new System.Drawing.Size(36, 36);
            this.tipRefreshBrowser.TabIndex = 16;
            this.tipRefreshBrowser.TabStop = false;
            this.tipRefreshBrowser.MouseHover += new System.EventHandler(this.refreshInterval_MouseHover);
            // 
            // lblProxyList
            // 
            this.lblProxyList.AutoSize = true;
            this.lblProxyList.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.lblProxyList.Location = new System.Drawing.Point(27, 468);
            this.lblProxyList.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.lblProxyList.Name = "lblProxyList";
            this.lblProxyList.Size = new System.Drawing.Size(135, 37);
            this.lblProxyList.TabIndex = 17;
            this.lblProxyList.TabStop = true;
            this.lblProxyList.Text = "Proxy List:";
            this.lblProxyList.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblProxyList_LinkClicked);
            // 
            // lblRefreshMin2
            // 
            this.lblRefreshMin2.AutoSize = true;
            this.lblRefreshMin2.Location = new System.Drawing.Point(369, 324);
            this.lblRefreshMin2.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.lblRefreshMin2.Name = "lblRefreshMin2";
            this.lblRefreshMin2.Size = new System.Drawing.Size(198, 37);
            this.lblRefreshMin2.TabIndex = 15;
            this.lblRefreshMin2.Text = "Refresh Interval";
            // 
            // lblRefreshMin3
            // 
            this.lblRefreshMin3.AutoSize = true;
            this.lblRefreshMin3.Location = new System.Drawing.Point(565, 335);
            this.lblRefreshMin3.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.lblRefreshMin3.Name = "lblRefreshMin3";
            this.lblRefreshMin3.Size = new System.Drawing.Size(23, 37);
            this.lblRefreshMin3.TabIndex = 15;
            this.lblRefreshMin3.Text = ":";
            // 
            // lblQuality
            // 
            this.lblQuality.AutoSize = true;
            this.lblQuality.Location = new System.Drawing.Point(423, 398);
            this.lblQuality.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.lblQuality.Name = "lblQuality";
            this.lblQuality.Size = new System.Drawing.Size(164, 37);
            this.lblQuality.TabIndex = 19;
            this.lblQuality.Text = "Quality        :";
            // 
            // lstQuality
            // 
            this.lstQuality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstQuality.Location = new System.Drawing.Point(596, 392);
            this.lstQuality.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.lstQuality.Name = "lstQuality";
            this.lstQuality.Size = new System.Drawing.Size(139, 45);
            this.lstQuality.TabIndex = 20;
            this.lstQuality.SelectedIndexChanged += new System.EventHandler(this.lstQuality_SelectedIndexChanged);
            // 
            // tipQuality
            // 
            this.tipQuality.BackgroundImage = global::TwitchBotUI.Properties.Resources.info;
            this.tipQuality.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.tipQuality.Location = new System.Drawing.Point(754, 376);
            this.tipQuality.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.tipQuality.Name = "tipQuality";
            this.tipQuality.Size = new System.Drawing.Size(36, 36);
            this.tipQuality.TabIndex = 16;
            this.tipQuality.TabStop = false;
            this.tipQuality.MouseHover += new System.EventHandler(this.streamQuality_MouseHover);
            // 
            // txtLoginInfos
            // 
            this.txtLoginInfos.Location = new System.Drawing.Point(1170, 58);
            this.txtLoginInfos.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.txtLoginInfos.Multiline = true;
            this.txtLoginInfos.Name = "txtLoginInfos";
            this.txtLoginInfos.PlaceholderText = "Format is =>Username{Blank}Password{Enter}";
            this.txtLoginInfos.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLoginInfos.Size = new System.Drawing.Size(643, 670);
            this.txtLoginInfos.TabIndex = 21;
            this.txtLoginInfos.WordWrap = false;
            // 
            // btnWithLoggedIn
            // 
            this.btnWithLoggedIn.BackgroundImage = global::TwitchBotUI.Properties.Resources.withLoggedInUsers1;
            this.btnWithLoggedIn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnWithLoggedIn.Location = new System.Drawing.Point(1091, 245);
            this.btnWithLoggedIn.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.btnWithLoggedIn.Name = "btnWithLoggedIn";
            this.btnWithLoggedIn.Size = new System.Drawing.Size(65, 324);
            this.btnWithLoggedIn.TabIndex = 22;
            this.btnWithLoggedIn.TabStop = false;
            this.btnWithLoggedIn.Click += new System.EventHandler(this.btnWithLoggedIn_Click);
            // 
            // lblLoginInfoTitle
            // 
            this.lblLoginInfoTitle.AutoSize = true;
            this.lblLoginInfoTitle.Location = new System.Drawing.Point(1370, 18);
            this.lblLoginInfoTitle.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.lblLoginInfoTitle.Name = "lblLoginInfoTitle";
            this.lblLoginInfoTitle.Size = new System.Drawing.Size(224, 37);
            this.lblLoginInfoTitle.TabIndex = 23;
            this.lblLoginInfoTitle.Text = "Login Credentials";
            // 
            // lblHeadless
            // 
            this.lblHeadless.AutoSize = true;
            this.lblHeadless.Location = new System.Drawing.Point(27, 405);
            this.lblHeadless.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.lblHeadless.Name = "lblHeadless";
            this.lblHeadless.Size = new System.Drawing.Size(192, 37);
            this.lblHeadless.TabIndex = 24;
            this.lblHeadless.Text = "Headless         :";
            // 
            // picLiveViewer
            // 
            this.picLiveViewer.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("picLiveViewer.BackgroundImage")));
            this.picLiveViewer.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.picLiveViewer.Location = new System.Drawing.Point(956, 331);
            this.picLiveViewer.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.picLiveViewer.Name = "picLiveViewer";
            this.picLiveViewer.Size = new System.Drawing.Size(72, 72);
            this.picLiveViewer.TabIndex = 25;
            this.picLiveViewer.TabStop = false;
            // 
            // lblViewer
            // 
            this.lblViewer.Location = new System.Drawing.Point(846, 414);
            this.lblViewer.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.lblViewer.MaximumSize = new System.Drawing.Size(112, 29);
            this.lblViewer.Name = "lblViewer";
            this.lblViewer.Size = new System.Drawing.Size(72, 29);
            this.lblViewer.TabIndex = 26;
            this.lblViewer.Text = "0";
            this.lblViewer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // picBotViewer
            // 
            this.picBotViewer.BackgroundImage = global::TwitchBotUI.Properties.Resources.bot;
            this.picBotViewer.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.picBotViewer.Location = new System.Drawing.Point(846, 331);
            this.picBotViewer.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.picBotViewer.Name = "picBotViewer";
            this.picBotViewer.Size = new System.Drawing.Size(72, 72);
            this.picBotViewer.TabIndex = 27;
            this.picBotViewer.TabStop = false;
            // 
            // lblLiveViewer
            // 
            this.lblLiveViewer.Location = new System.Drawing.Point(956, 414);
            this.lblLiveViewer.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.lblLiveViewer.MaximumSize = new System.Drawing.Size(112, 29);
            this.lblLiveViewer.Name = "lblLiveViewer";
            this.lblLiveViewer.Size = new System.Drawing.Size(72, 29);
            this.lblLiveViewer.TabIndex = 28;
            this.lblLiveViewer.Text = "0";
            this.lblLiveViewer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tipLiveViewer
            // 
            this.tipLiveViewer.BackgroundImage = global::TwitchBotUI.Properties.Resources.info;
            this.tipLiveViewer.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.tipLiveViewer.Location = new System.Drawing.Point(1033, 326);
            this.tipLiveViewer.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.tipLiveViewer.Name = "tipLiveViewer";
            this.tipLiveViewer.Size = new System.Drawing.Size(36, 36);
            this.tipLiveViewer.TabIndex = 29;
            this.tipLiveViewer.TabStop = false;
            this.tipLiveViewer.MouseHover += new System.EventHandler(this.tipLiveViewer_MouseHover);
            // 
            // MainScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(216F, 216F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1834, 754);
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
            this.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.MaximumSize = new System.Drawing.Size(1862, 833);
            this.Name = "MainScreen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "My Twitch Viewer Bot";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainScreen_FormClosing);
            this.Shown += new System.EventHandler(this.MainScreen_Shown);
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

