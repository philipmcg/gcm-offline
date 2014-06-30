using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Utilities
{
    public partial class UpdateStatusForm : Form
    {
        public UpdateStatusForm()
        {
            InitializeComponent();
        }

        public void SetStatus(string title, string message, bool marquee)
        {
            this.Text = title;
            this.label1.Text = message;
            this.m_progressBar.Style = marquee ? ProgressBarStyle.Marquee : ProgressBarStyle.Continuous;
        }

        public void ShowWithBackgroundProcess(Action action)
        {
            action();
            ShowDialog();
        }
    }
}
