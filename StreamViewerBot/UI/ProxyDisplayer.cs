using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace StreamViewerBot.UI
{
    public partial class ProxyDisplayer : Form
    {
        public ProxyDisplayer(List<string> proxyList)
        {
            InitializeComponent();

            txtProxyList.Text = string.Empty;

            foreach (var proxy in proxyList)
            {
                txtProxyList.AppendText(proxy);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                string strCmdLine = "/C explorer \"https://www.webshare.io/?referral_code=ceuygyx4sir2";
                var browserProcess = Process.Start("CMD.exe", strCmdLine);
                browserProcess?.Close();
            }
            catch (Exception)
            {
                //ignored
            }
        }
    }
}
