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
    public partial class SelectVictor : Dialog
    {
        public int SelectedVictor
        {
            get
            {
                return comboBox1.SelectedIndex;
            }
        set {
          if (value > 0) {

          comboBox1.SelectedIndex = value;
          }
        }
        }

        public bool GameCrashed
        {
            get
            {
                return checkBox1.Checked;
            }
        }
        public int Objectives1
        {
            get
            {
                return cbObjectives1.SelectedIndex - 1;
            }
        }
        public int Objectives2
        {
            get
            {
                return cbObjectives2.SelectedIndex - 1;
            }
        }
        int NumObjectives;

        public SelectVictor()
        {
            InitializeComponent();
        }

        void PrepareComboBox(ComboBox cb, int num)
        {
            cb.Items.Clear();
            for (int k = -1; k <= num; k++)
            {
                if (k == -1)
                    cb.Items.Add("");
                else
                    cb.Items.Add(k);
            }
            cb.SelectedIndex = 0;
        }

        public void Prepare(int numObjectives)
        {
            NumObjectives = numObjectives;
            PrepareComboBox(cbObjectives1, NumObjectives);
            PrepareComboBox(cbObjectives2, NumObjectives);
        }

        private void SelectVictor_Load(object sender, EventArgs e)
        {
            panel1.Visible = false;
            this.Height -= panel1.Height;
        }

        bool Ready()
        {
            if (cbObjectives1.SelectedIndex == 0 || cbObjectives2.SelectedIndex == 0)
                return false;
            if (Objectives1 + Objectives2 > NumObjectives)
                return false;

            if (comboBox1.SelectedIndex == 1 && Objectives1 <= Objectives2)
                return false;
            if (comboBox1.SelectedIndex == 2 && Objectives2 <= Objectives1)
                return false;

            return true;
        }

        private void cbObjectives_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.m_OkButton.Enabled = true;
        } 
    }
}
