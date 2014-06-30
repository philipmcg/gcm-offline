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
    public partial class CropScreenshot : Dialog
    {
        public int JpegQuality { get { return JpegQualities[this.comboBox1.SelectedIndex]; } }

        int[] JpegQualities = new int[] { 100, 97, 92, 85, 60 };
        string[] QualityStrings = new string[] { "Maximum", "High", "Medium", "Low", "Lowest" };

        public CropScreenshot()
        {
            InitializeComponent();
            this.comboBox1.Items.AddRange(QualityStrings);
            this.comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        public string UserCaption { get { return this.textBox1.Text; } }


        private void CropScreenshot_Load(object sender, EventArgs e)
        {

        }

        public Point TopLeft { get { return panel.TopLeft; } }
        public Point BottomRight { get { return panel.BottomRight; } }

        public void SetBounds(Point tl, Point br)
        {
            panel.SetBounds(tl, br);
        }
        public void ShowImage(Image img)
        {
            panel.ShowImage(img);
        }
    }


    public class CropImagePanel : Panel
    {
        private Point topLeft;

        public Point TopLeft
        {
            get { return topLeft; }
            set
            {
                int x = Math.Max(0, Math.Min(pictureBox.Image.Width, value.X));
                int y = Math.Max(0, Math.Min(pictureBox.Image.Height, value.Y));
                topLeft = new Point(x, y);
            }
        }
        private Point bottomRight;

        public Point BottomRight
        {
            get { return bottomRight; }
            set 
            {
                int x = Math.Max(0, Math.Min(pictureBox.Image.Width, value.X));
                int y = Math.Max(0, Math.Min(pictureBox.Image.Height, value.Y));
                bottomRight = new Point(x, y);
            }
        }

        private PictureBox pictureBox;

        public CropImagePanel()
        {
            pictureBox = new PictureBox();
            pictureBox.Paint += new PaintEventHandler(pictureBox1_Paint);
            pictureBox.MouseDown += new MouseEventHandler(pictureBox1_MouseDown);
            pictureBox.MouseMove += new MouseEventHandler(pictureBox1_MouseMove);


            this.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.AutoScroll = true;
            this.Controls.Add(this.pictureBox);
            this.Location = new System.Drawing.Point(13, 13);
            this.Name = "panel1";
            this.Size = new System.Drawing.Size(728, 422);

            pictureBox.Location = new System.Drawing.Point(4, 4);
            pictureBox.Name = "pictureBox1";
            pictureBox.Size = new System.Drawing.Size(464, 288);
            brush = new SolidBrush(Color.FromArgb(128, 0, 0, 0));
        }
        void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            Mouse(e);
        }

        public void SetBounds(Point tl, Point br)
        {
            TopLeft = tl;
            BottomRight = br;

            pictureBox.Refresh();
        }

        void Mouse(MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                TopLeft = e.Location;
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                BottomRight = e.Location;
            }

            if (TopLeft.X > BottomRight.X)
                TopLeft = new Point(BottomRight.X, TopLeft.Y);
            if (TopLeft.Y > BottomRight.Y)
                TopLeft = new Point(TopLeft.X, BottomRight.Y);

            pictureBox.Refresh();
        }

        void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            Mouse(e);
        }

        void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Fill(e.Graphics, 0, 0, TopLeft.X, pictureBox.Height);
            Fill(e.Graphics, BottomRight.X, 0, pictureBox.Width, BottomRight.Y);
            Fill(e.Graphics, TopLeft.X, 0, BottomRight.X, TopLeft.Y);
            Fill(e.Graphics, TopLeft.X, BottomRight.Y, pictureBox.Width, pictureBox.Height);
        }
        Brush brush;

        void Fill(Graphics g, int left, int top, int right, int bottom)
        {
            g.FillRectangle(brush, new Rectangle(left, top, right - left, bottom - top));
        }
        public void ShowImage(Image img)
        {
            this.pictureBox.Size = img.Size;
            this.pictureBox.Image = img;
            brush = new SolidBrush(Color.FromArgb(128, 0, 0, 0));
            BottomRight = new Point(img.Width, img.Height);
        }
    }
}
