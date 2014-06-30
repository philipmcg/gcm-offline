using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Utilities;

using Utilities.GCSV;

namespace Utilities
{

    using Options = IEnumerable<KeyValuePair<string, string>>;

    public class OptionList
    {
        Utilities.VariableBin Var;
        List<Option> Options;
        public Action<string> OnOptionChanged;
        public readonly bool Lockable;

        class Option
        {
            public OptionList Parent;
            public string ID;
            public string LockableID { get { return ID + "_locked"; } }
            public string Caption;
            public ComboBox ComboBox;
            public Label Label;
            public CheckBox Checkbox;
            public List<KeyValuePair<string, string>> Values;
            public bool Active;
            public string Default;

            public Option(bool lockable = false)
            {
                ComboBox = new ComboBox();
                Label = new Label();
                if (lockable) {
                  Checkbox = new CheckBox();
                }
            }

            public void Set()
            {
                Parent.Var.Str[ID] = Values[ComboBox.SelectedIndex].Key;
            }
            public void SetDefault()
            {
                SetToKey(Default);
            }
            public void SetToKey(string key)
            {
                Parent.Var.Str[ID] = key;
                ComboBox.SelectedIndex = Values.FindIndex(p => p.Key == key);
            }
            public void Initialize()
            {
                var cb = ComboBox;
                cb.Items.Clear();
                foreach (var item in Values)
                    cb.Items.Add(item.Value);
                cb.SelectedIndex = 0;

                cb.SelectionChangeCommitted += new EventHandler(cb_SelectionChangeCommitted);
                if (Parent.Lockable) {
                  Checkbox.CheckedChanged += new EventHandler(Checkbox_CheckedChanged);
                }

                Label.Text = Caption;
            }

            void Checkbox_CheckedChanged(object sender, EventArgs e) {
              bool locked = !Checkbox.Checked;
              RefreshLocked(locked);
              //Label.Text = locked ? Caption + " (locked)" : Caption;
            }
            public void RefreshLocked(bool locked) {
              Checkbox.Checked = !locked;
              Parent.Var.Bool[LockableID] = locked;
              ComboBox.Enabled = !locked;
              Label.Enabled = !locked;
            }

            void cb_SelectionChangeCommitted(object sender, EventArgs e)
            {
              if (ComboBox.SelectedIndex >= 0) {
                Set();
                if(Parent.OnOptionChanged != null)
                  Parent.OnOptionChanged(ID);
              }
            }
        }

        public OptionList(GCSVTable csv, IGCSVCollection csvs, Utilities.VariableBin var, Func<IData,bool> selector, Func<IData, string> getDefault = null, bool lockable = false)
        {
            Var = var;
            Options = new List<Option>();
            Lockable = lockable;

            foreach (var line in csv)
            {
                if (!selector(line))
                    continue;

                var o = new Option(lockable);
                o.Parent = this;
                o.Caption = line["caption"];
                o.ID = line["id"];
                o.Active = line["active"].ToBool();

                if (getDefault != null)
                  o.Default = getDefault(line);
                else
                  o.Default = line["default"];
                var l = csvs[line["list"]];
                o.Values = l.Where(m => !m.ContainsKey("active") || m["active"].ToBool()).Select(m => new KeyValuePair<string, string>(m[line["value"]], m[line["text"]])).ToList();
                o.Initialize();

                if (!var.Str.ContainsKey(o.ID) || !o.Active) // set defaults if variable is not set or option is not active
                    var.Str[o.ID] = o.Default;

                if (var.Str.ContainsKey(o.ID))
                    o.ComboBox.SelectedIndex = o.Values.FindIndex(p => p.Key == var.Str[o.ID]);

                Options.Add(o);
            }
        }

        public OptionList(GCSVTable csv, Func<IData, Options, string> getDefault, Func<string, Options> getList, Utilities.VariableBin var, Func<IData, bool> selector, bool lockable = false)
        {
            Var = var;
            Options = new List<Option>();
            Lockable = lockable;

            foreach (var line in csv)
            {
                if (!selector(line))
                    continue;

                var o = new Option(lockable);
                o.Parent = this;
                o.Caption = line["caption"];
                o.ID = line["id"];
                o.Active = line["active"].ToBool();
                var l = getList(line["list"]);
                o.Values = l.ToList();
                o.Default = getDefault(line, o.Values);
                o.Initialize();

                if (!var.Str.ContainsKey(o.ID))
                    var.Str[o.ID] = o.Default;

                if (var.Str.ContainsKey(o.ID))
                    o.ComboBox.SelectedIndex = o.Values.FindIndex(p => p.Key == var.Str[o.ID]);

                Options.Add(o);
            }
        }


        public void SetOptions()
        {
            foreach (var o in Options)
            {
                if (o.Active)
                    o.Set();
                else
                    o.SetDefault();
            }
        }
        
        public void SetDefaultVariablesIfEmpty()
        {
            foreach (var o in Options)
            {
                if (!Var.Str.ContainsKey(o.ID))
                    Var.Str[o.ID] = o.Default;
            }
        }
        public void SetDefaults()
        {
            foreach (var o in Options)
            {
                o.SetDefault();
            }
        }

      /// <summary>
      /// Changes the comboboxes to reflect what's int he variablebin now
      /// </summary>
        public void SetToVariables(IVariableBin var)
        {
            foreach (var o in Options)
            {
                if (var.Str.ContainsKey(o.ID))
                {
                    string optionID = o.ID;
                    string valueKey = var.Str[optionID];
                    if( o.Values.Any(v => v.Key == valueKey)) {
                        o.SetToKey(var.Str[o.ID]);
                    }
                }
            }
        }

        public void SetToPanel(Panel panel)
        {
            int n = 0;
            foreach (var o in Options)
            {
                var lbl = o.Label;
                lbl.AutoSize = true;
                lbl.Location = new System.Drawing.Point(3, 6);
                lbl.Size = new System.Drawing.Size(35, 13);
                lbl.TabIndex = 1;


                var cb = o.ComboBox;
                cb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                            | System.Windows.Forms.AnchorStyles.Right)));
                cb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
                cb.FormattingEnabled = true;
                cb.Location = new System.Drawing.Point(180, 3);
                cb.Size = new System.Drawing.Size(panel.Width - cb.Left - 10, 21);
                cb.TabIndex = 0;

                int offset = (cb.Height + 2);
                cb.Top += n * offset;
                lbl.Top += n * offset;
                panel.Parent.Height += offset;
                //panel.Height += offset;

                panel.Controls.Add(cb);
                panel.Controls.Add(lbl);

                if (Lockable) {
                  var checkbox = o.Checkbox;
                  checkbox.Location = new Point(160, 3);
                  checkbox.TabIndex = 0;
                  checkbox.Text = "";
                  checkbox.Width = 20;
                  checkbox.Top += n * offset;
                  panel.Controls.Add(checkbox);
                  checkbox.BringToFront();
                  o.RefreshLocked(Var.Bool[o.LockableID, false]);
                  ToolTip tooltip = new ToolTip();
                  tooltip.SetToolTip(checkbox, "Lock this option by unchecking the box");
                }

                n++;
            }
        }

    }
}
