using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using GcmShared;
using Utilities;

namespace Launcher.Forms
{
    using PS = KeyValuePair<string, string>;
    using P = Tuple3<string,string, List<KeyValuePair<string, string>>>;

    public partial class SetupDivision : Dialog
    {
        ComboBox[] DropDowns;
        Label[] Labels;
        ToolTip ToolTip = new ToolTip();

        public SetupDivision()
        {
            InitializeComponent();
        }

        public void LoadDropDowns(int faction, Name initialName, string initialState)
        {
            //var states = GcmLauncher.Web.GetRequestStruct<string[]>("divisions", "GetAvailableStates", faction);

            var pfx = GcmLauncher.Data.GCSVs["factions"][faction]["pfx"];

            // Don't allow players to choose these states
           // string[] no_choice = new string[] {"u_us","c_md","u_ky","c_ky","u_ks","u_md"};
            //var states = XML.GetFirstElement(xml, "state_choices").Split('|').OrderBy(s => s);
            //var state_choices = Data.CSVs["regiments"].Where(c => states.Contains(c["abbr"]) && !no_choice.Contains(c["abbr"])).Select((c, r) => new PS(c["name"], c["abbr"]));

           // var state_choices = Gcm.Data.GCSVs["regiments"].Join(states,(c,k) => true,(c,k) => true,(c,s,k) => c["abbr"] == s).Select((c,s) => new KeyValuePair<string,string>(c["name"],s));
            //.Where(l => l["side"] == side.ToString()).Select(l => new PS(l["name"],l["abbr"])).OrderBy(p => p.Value)
            //states.Select(i => new PS(i,i)).OrderBy(p => p.Value)),
            textBox1.Lines = new string[] {(faction == 1 ? "UNION" : "CONFEDERATE") + " CHARACTER PAGE", " ", "Here you pick your character's name and the state he is from.  Your first brigade will have troops from this state.  Some states may have already sent their quota of troops and you cannot pick them."};
            var dropDowns = new List<P>()
            {
                new P("Choose the state your character is from.","State",
                    GcmLauncher.Data.GCSVs["states"]
                        .Where(l => l["side"].ToInt() == faction)
                        .OrderByDescending(l => l["freq"].ToInt())
                        .Select(l => new PS(l["name"], l["abbr"])).ToList()),
                
                new P("Choose your character's first name.","First Name",GcmLauncher.Data.Lists["names\\" + pfx+ "first"].List.Select(i => new PS(i.Value,i.Value)).OrderBy(p => p.Value).ToList()),
                new P("Choose your character's middle initial.","Middle Initial",GcmLauncher.Data.Lists["names\\" + pfx + "middle"].List.Select(i => new PS(i.Value,i.Value)).OrderBy(p => p.Value).ToList()),
                new P("Choose your character's last name.","Surname",GcmLauncher.Data.Lists["names\\" + pfx + "last"].List.Select(i => new PS(i.Value,i.Value)).OrderBy(p => p.Value).ToList()),
            };

            DropDowns = new ComboBox[dropDowns.Count];
            Labels = new Label[dropDowns.Count];
            int k = 0;
            foreach (var dropdown in dropDowns)
            {
                Label lb = new Label();
                lb.Text = dropdown.Item2;
                ComboBox cb = new ComboBox();
                cb.Tag = new List<string>();
                var list = dropdown.Item3;

                foreach (var item in list)
                {
                    cb.Items.Add(item.Key);
                    ((List<string>)cb.Tag).Add(item.Value);
                }

                Labels[k] = lb;
                DropDowns[k] = cb;

                lb.Parent = panel1;
                cb.Parent = panel1;

                int h = k * 22;
                lb.Top = h;
                lb.AutoSize = true;
                cb.Top = h;
                cb.Left = 80;
                cb.DropDownStyle = ComboBoxStyle.DropDownList;
                cb.MaxDropDownItems = 12;

                ToolTip.SetToolTip(cb, dropdown.Item1);
                ToolTip.SetToolTip(lb, dropdown.Item1);


                cb.SelectedIndexChanged += new EventHandler(cb_SelectedIndexChanged);

                k++;
            }

            if ((DropDowns[0].Tag as List<string>).Any(i => i == initialState))
                DropDowns[0].SelectedIndex = (DropDowns[0].Tag as List<string>).IndexOf(initialState);

            if (DropDowns[1].Items.Cast<string>().Any(i => i == initialName.First))
                DropDowns[1].SelectedIndex = DropDowns[1].Items.IndexOf(initialName.First);

            if (DropDowns[2].Items.Cast<string>().Any(i => i == initialName.Middle))
                DropDowns[2].SelectedIndex = DropDowns[2].Items.IndexOf(initialName.Middle);

            if (DropDowns[3].Items.Cast<string>().Any(i => i == initialName.Last))
                DropDowns[3].SelectedIndex = DropDowns[3].Items.IndexOf(initialName.Last);

            base.m_OkButton.Enabled = false;
            ToolTip.SetToolTip(base.m_OkButton, "This will be enabled when you have made a selection in each of the categories in this form.");
            //this.AutoSize = true;


            Validate();
        }

        void cb_SelectedIndexChanged(object sender, EventArgs e)
        {

            Validate();
        }

        void Validate()
        {
            if (ReadyToSubmit())
            {
                ToolTip.SetToolTip(base.m_OkButton, "");
                m_OkButton.Enabled = true;
            }
        }

        bool ReadyToSubmit()
        {
            foreach (var cb in DropDowns)
            {
                if (cb.SelectedIndex == -1)
                {
                    return false;
                }
            }
            return true;
        }



        public new Dictionary<string,string> ShowDialog()
        {
            var dict = new Dictionary<string,string>();

            var r = base.ShowDialog();

            if (r == DialogResult.OK)
            {
                for (int k = 0; k < DropDowns.Length; k++)
                {
                    var cb = DropDowns[k];
                    int i = cb.SelectedIndex;
                    dict.Add(Labels[k].Text, ((List<string>)cb.Tag)[i]);

                }
                return dict;

            }
            else
                return null;
        }

        private void SetupDivision_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int k = 0; k < DropDowns.Length; k++)
            {
                var cb = DropDowns[k];
                cb.SelectedIndex = Rand.Int(cb.Items.Count);
            }

        }
    }
}
