using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using GcmShared;

namespace Launcher.Forms
{
    public partial class ChooseScenario : Dialog
    {
        public string ScenarioChoice
        {
            get
            {
                return this.comboBox1.SelectedItem.ToString();
            }
        }
        public ChooseScenario()
        {
            InitializeComponent();
            this.comboBox1.Items.Add("Random");
            this.comboBox1.Items.Add("Custom Objectives");

            /*string scns = Main.Server.GetRequestToString("scenarios/list_scenarios.php", "");
            var list = scns.Split(',');
            list = list.OrderBy(s => s).ToArray();
            foreach (var scn in list)
            {
                if(!string.IsNullOrEmpty(scn))
                    this.comboBox1.Items.Add(scn);
            }*/

            if (Gcm.Var.Str.ContainsKey("opt_last_scenario"))
                this.comboBox1.SelectedIndex = this.comboBox1.Items.IndexOf(Gcm.Var.Str["opt_last_scenario"]);
            else
                this.comboBox1.SelectedIndex = 0;
        }
    }
}
