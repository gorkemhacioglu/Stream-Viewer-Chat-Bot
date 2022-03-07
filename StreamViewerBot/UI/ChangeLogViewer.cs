using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace StreamViewerBot.UI
{
    public partial class Changelog : Form
    {
        public Changelog()
        {
            InitializeComponent();
        }

        public void SetChangelog(string changelog) 
        {
            txtChangelog.Text = changelog;
        }
    }
}
