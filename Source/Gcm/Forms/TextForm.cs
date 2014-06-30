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
    public partial class TextForm : Dialog
    {
        public Action<TextForm> ActionOnShown;

        public TextForm()
        {
            InitializeComponent();
            this.Shown += new EventHandler(TextForm_Shown);
        }

        void TextForm_Shown(object sender, EventArgs e)
        {
            if (ActionOnShown != null)
                ActionOnShown(this);
        }



        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        private void TextForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.m_OkButton.PerformClick();
            }
        }
    }
}
