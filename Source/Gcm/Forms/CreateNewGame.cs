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
    public partial class CreateNewGame : Dialog
    {
        public List<int> Sides { get; set; }

        public CreateNewGame()
        {
            InitializeComponent();
            m_OkButton.Enabled = false;
        }

        protected override void OnShown(EventArgs e)
        {
            m_OkButton.Enabled = false;
        }

        private void CheckedListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            m_OkButton.Enabled = false;

            bool[] b = new bool[3];

            foreach (var item in CheckedListBox.CheckedIndices)
            {
                b[Sides[(int)item]] = true;
                if (b[1] == true && b[2] == true)
                    m_OkButton.Enabled = true;
            }
        }
    }
}
