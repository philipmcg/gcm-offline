using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Launcher.Forms
{
    public partial class Dialog : Form
    {
        public string OkButtonText
        {
            get { return this.m_OkButton.Text; }
            set { this.m_OkButton.Text = value; }
        }

        public string CancelButtonText
        {
            get { return this.m_CancelButton.Text; }
            set { this.m_CancelButton.Text = value; }
        }

        public string Message
        {
            get { return this.m_label.Text; }
            set { this.m_label.Text = value; this.m_label.Visible = true; }
        }

        public Dialog()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
        }
    }
}
