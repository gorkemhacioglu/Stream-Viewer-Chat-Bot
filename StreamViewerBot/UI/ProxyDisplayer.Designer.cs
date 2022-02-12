namespace StreamViewerBot.UI
{
    partial class ProxyDisplayer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblHeader = new System.Windows.Forms.Label();
            this.txtProxyList = new System.Windows.Forms.RichTextBox();
            this.lblMessage = new System.Windows.Forms.Label();
            this.lblBuyProxy = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.lblHeader.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblHeader.Location = new System.Drawing.Point(23, 9);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(465, 25);
            this.lblHeader.TabIndex = 0;
            this.lblHeader.Text = "These proxies are not private or not working properly!";
            // 
            // txtProxyList
            // 
            this.txtProxyList.Location = new System.Drawing.Point(12, 37);
            this.txtProxyList.Name = "txtProxyList";
            this.txtProxyList.ReadOnly = true;
            this.txtProxyList.Size = new System.Drawing.Size(494, 470);
            this.txtProxyList.TabIndex = 1;
            this.txtProxyList.Text = "";
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblMessage.Location = new System.Drawing.Point(23, 508);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(427, 20);
            this.lblMessage.TabIndex = 2;
            this.lblMessage.Text = "You must have private and working proxies. You can buy it from";
            // 
            // lblBuyProxy
            // 
            this.lblBuyProxy.AutoSize = true;
            this.lblBuyProxy.Font = new System.Drawing.Font("Segoe UI", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point);
            this.lblBuyProxy.ForeColor = System.Drawing.Color.LimeGreen;
            this.lblBuyProxy.Location = new System.Drawing.Point(446, 507);
            this.lblBuyProxy.Name = "lblBuyProxy";
            this.lblBuyProxy.Size = new System.Drawing.Size(55, 21);
            this.lblBuyProxy.TabIndex = 3;
            this.lblBuyProxy.Text = "HERE!";
            this.lblBuyProxy.Click += new System.EventHandler(this.lblBuyProxy_Click);
            this.lblBuyProxy.MouseEnter += new System.EventHandler(this.lblBuyProxy_MouseEnter);
            this.lblBuyProxy.MouseLeave += new System.EventHandler(this.lblBuyProxy_MouseLeave);
            // 
            // ProxyDisplayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(518, 540);
            this.Controls.Add(this.lblBuyProxy);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.txtProxyList);
            this.Controls.Add(this.lblHeader);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProxyDisplayer";
            this.Text = "Proxy Check";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.RichTextBox txtProxyList;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Label lblBuyProxy;
    }
}