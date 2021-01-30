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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainScreen));
            this.startStopButton = new System.Windows.Forms.PictureBox();
            this.txtStreamUrl = new System.Windows.Forms.TextBox();
            this.lblStreamUrl = new System.Windows.Forms.Label();
            this.logList = new System.Windows.Forms.ListView();
            this.lblLog = new System.Windows.Forms.Label();
            this.checkHeadless = new System.Windows.Forms.CheckBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.lblProxyList = new System.Windows.Forms.Label();
            this.txtProxyList = new System.Windows.Forms.TextBox();
            this.browseProxyList = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.startStopButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // startStopButton
            // 
            this.startStopButton.Image = ((System.Drawing.Image)(resources.GetObject("startStopButton.Image")));
            this.startStopButton.Location = new System.Drawing.Point(391, 180);
            this.startStopButton.Name = "startStopButton";
            this.startStopButton.Size = new System.Drawing.Size(134, 50);
            this.startStopButton.TabIndex = 0;
            this.startStopButton.TabStop = false;
            this.startStopButton.Click += new System.EventHandler(this.startStopButton_Click);
            // 
            // txtStreamUrl
            // 
            this.txtStreamUrl.Location = new System.Drawing.Point(74, 207);
            this.txtStreamUrl.Name = "txtStreamUrl";
            this.txtStreamUrl.Size = new System.Drawing.Size(311, 23);
            this.txtStreamUrl.TabIndex = 1;
            // 
            // lblStreamUrl
            // 
            this.lblStreamUrl.AutoSize = true;
            this.lblStreamUrl.Location = new System.Drawing.Point(3, 210);
            this.lblStreamUrl.Name = "lblStreamUrl";
            this.lblStreamUrl.Size = new System.Drawing.Size(65, 15);
            this.lblStreamUrl.TabIndex = 2;
            this.lblStreamUrl.Text = "Stream Url:";
            // 
            // logList
            // 
            this.logList.HideSelection = false;
            this.logList.Location = new System.Drawing.Point(12, 26);
            this.logList.Name = "logList";
            this.logList.Size = new System.Drawing.Size(505, 115);
            this.logList.TabIndex = 3;
            this.logList.UseCompatibleStateImageBehavior = false;
            this.logList.View = System.Windows.Forms.View.List;
            // 
            // lblLog
            // 
            this.lblLog.AutoSize = true;
            this.lblLog.Location = new System.Drawing.Point(231, 8);
            this.lblLog.Name = "lblLog";
            this.lblLog.Size = new System.Drawing.Size(51, 15);
            this.lblLog.TabIndex = 4;
            this.lblLog.Text = "Logging";
            // 
            // checkHeadless
            // 
            this.checkHeadless.AutoSize = true;
            this.checkHeadless.Location = new System.Drawing.Point(10, 155);
            this.checkHeadless.Name = "checkHeadless";
            this.checkHeadless.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkHeadless.Size = new System.Drawing.Size(76, 19);
            this.checkHeadless.TabIndex = 5;
            this.checkHeadless.Text = ":Headless";
            this.checkHeadless.UseVisualStyleBackColor = true;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // lblProxyList
            // 
            this.lblProxyList.AutoSize = true;
            this.lblProxyList.Location = new System.Drawing.Point(10, 181);
            this.lblProxyList.Name = "lblProxyList";
            this.lblProxyList.Size = new System.Drawing.Size(58, 15);
            this.lblProxyList.TabIndex = 6;
            this.lblProxyList.Text = "ProxyList:";
            // 
            // txtProxyList
            // 
            this.txtProxyList.Location = new System.Drawing.Point(104, 178);
            this.txtProxyList.Name = "txtProxyList";
            this.txtProxyList.Size = new System.Drawing.Size(281, 23);
            this.txtProxyList.TabIndex = 7;
            // 
            // browseProxyList
            // 
            this.browseProxyList.Location = new System.Drawing.Point(74, 178);
            this.browseProxyList.Name = "browseProxyList";
            this.browseProxyList.Size = new System.Drawing.Size(28, 23);
            this.browseProxyList.TabIndex = 8;
            this.browseProxyList.Text = "...";
            this.browseProxyList.UseVisualStyleBackColor = true;
            this.browseProxyList.Click += new System.EventHandler(this.browseProxyList_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(414, 147);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(87, 30);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Visible = false;
            this.pictureBox1.Click += new System.EventHandler(this.emergency_Click);
            // 
            // MainScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(529, 233);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.browseProxyList);
            this.Controls.Add(this.txtProxyList);
            this.Controls.Add(this.lblProxyList);
            this.Controls.Add(this.checkHeadless);
            this.Controls.Add(this.lblLog);
            this.Controls.Add(this.logList);
            this.Controls.Add(this.lblStreamUrl);
            this.Controls.Add(this.txtStreamUrl);
            this.Controls.Add(this.startStopButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainScreen";
            this.Text = "My Twitch Bot";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainScreen_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.startStopButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox startStopButton;
        private System.Windows.Forms.TextBox txtStreamUrl;
        private System.Windows.Forms.Label lblStreamUrl;
        private System.Windows.Forms.ListView logList;
        private System.Windows.Forms.Label lblLog;
        private System.Windows.Forms.CheckBox chechHeadless;
        private System.Windows.Forms.CheckBox checkHeadless;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label lblProxyList;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button browseProxyList;
        private System.Windows.Forms.TextBox txtProxyList;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

