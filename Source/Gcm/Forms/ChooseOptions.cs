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


namespace Launcher.Forms
{

    using Pair = KeyValuePair<string, string>;
    using Options = IEnumerable<KeyValuePair<string,string>>;

    public partial class ChooseOptions : Dialog
    {
        OptionList OptionList;
        public ChooseOptions()
        {
            InitializeComponent();
        }


        public void Reset()
        {
            OptionList.SetDefaults();
        }

        public void Initialize(GCSVTable options, Func<IData, bool> selector, Func<IData,Options,string> getDefault, Func<string,Options> getList, VariableBin bin)
        {
            OptionList = new OptionList(options, getDefault, getList, bin, selector);
            OptionList.SetToPanel(m_optionsPanel);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Reset();
        }
    }
}
