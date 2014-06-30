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
    public partial class ComplexDialog : Dialog
    {
        public Button MiddleButton { get { return this.button1; } }
        public ComplexDialog()
        {
            InitializeComponent();
        }
    }
}
