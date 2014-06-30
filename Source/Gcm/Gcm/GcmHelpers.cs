using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.IO;
using GcmShared;
using Utilities;
using Launcher.GCM;

namespace Launcher
{
    public class GcmHelpers
    {
        LazyClassLoader Loader;
        public GcmHelpers()
        {
            Loader = new LazyClassLoader();
        }

        public SOWDirectory SOW { get { return Loader.Get<SOWDirectory>(); } }

        public IVariableBin LoadOptionsFromBattle(int battleID)
        {
            string localFile = Paths.Local.TempBattleOptions(battleID);
            VariableBin bin = new VariableBin();
            bin.LoadFromFile(localFile);
            return bin;
        }

        public int ChooseBattleID(string caption)
        {
            var form = new Launcher.Forms.TextForm();
            form.ActionOnShown = f => { f.Text = caption; f.Label.Text = "Enter Battle ID"; f.TextBox.Focus(); };

            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                int battleID = 0;
                var value = form.TextBox.Text;
                if (int.TryParse(value, out battleID) && battleID > 0)
                {
                    return battleID;
                }
            }
            return 0;
        }
    }
}
