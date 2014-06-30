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
    public partial class CropMap : Dialog
    {
        public CropMap()
        {
            InitializeComponent();
            this.Size = new Size(650, 700);
        }

        public Point TopLeft { get { return panel1.TopLeft; } set { panel1.TopLeft = value; } }
        public Point BottomRight { get { return panel1.BottomRight; } set { panel1.BottomRight = value; } }

        public void SetBounds(Point tl, Point br)
        {
            panel1.SetBounds(tl, br);
        }

        public void ShowImage(Image img)
        {
            panel1.ShowImage(img);
        }
    }
}
