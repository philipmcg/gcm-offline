using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Launcher.GCM;
using Utilities;
using GcmShared;

namespace Launcher.Forms
{
    public enum BalanceSidesOptions
    {
        Default,
        AllowImbalance,
    }

    public partial class BalanceSides : Dialog
    {
        public int TotalMenTeam1 { get; set; }
        public int TotalMenTeam2 { get; set; }
        public int TotalGuns { get; set; }
        public int Spacer = 8;
        public int Players;
        int DefaultTotalMen;
        public string[] SideIDs = new[] { "", "us_inf", "cs_inf" };

        RandomDivisionsOutput Output;

        class PlayerInfo
        {
            public Panel panel;
            public Label name;
            public Label strength;
            public Division player;

            public PlayerInfo()
            {
                panel = new Panel();
                name = new Label();
                strength = new Label();

                name.AutoSize = true;
                name.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                name.ForeColor = System.Drawing.Color.White;
                name.Location = new System.Drawing.Point(2, 2);
                name.Parent = panel;

                strength.AutoSize = true;
                strength.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                strength.ForeColor = System.Drawing.Color.White;
                strength.Location = new System.Drawing.Point(2, 16);
                strength.Parent = panel;
            }
        }

        public BalanceSides(List<Division> list, int totalMen, RandomDivisionsOutput output, BalanceSidesOptions options)
        {
            this.Output = output;
            InitializeComponent();
            players = new Dictionary<string, List<PlayerInfo>>();
            Panels = new Dictionary<string, Panel>();
            Panels.Add(SideIDs[1], panel1);
            Panels.Add(SideIDs[2], panel2);

            TotalMenTeam1 = totalMen;
            DefaultTotalMen = TotalMenTeam1;
            Players = list.Count;

            var l = list.Where(d => d.Side == 1).ToList();
            SplitSide(l, panel1, SideIDs[1]);

            l = list.Where(d => d.Side == 2).ToList();
            SplitSide(l, panel2, SideIDs[2]);

            totalMenSlider.Maximum = (int)(TotalMenTeam1 * 5 + (Math.Sqrt(TotalMenTeam1) * 10)) / 5 * 5 / 100;
            totalMenSlider.TickStyle = TickStyle.None;
            totalMenSlider.Minimum = (int)(TotalMenTeam1 * 0.2 - (Math.Sqrt(TotalMenTeam1) * 10)) / 5 * 5 / 100;
            totalMenSlider.ValueChanged += new EventHandler(totalMenSlider_ValueChanged);
            totalMenSlider.LargeChange = 5;
            totalMenSlider.SmallChange = 1;
            splitContainer1.SplitterDistance = splitContainer1.Width / 2;
            SetRatioSlider(infantryRatioSlider, sideStrengthMultipliers);
            SetRatioSlider(artilleryRatioSlider, sideStrengthMultipliers);
            SetRatioSlider(menGunRatioSlider, menGunRatios);
            Reset();

            if (options != BalanceSidesOptions.AllowImbalance)
                imbalancePanel.Visible = false;
        }

        void SetRatioSlider(TrackBar bar, Array values)
        {
            bar.Maximum = values.Length - 1;
            bar.Minimum = 0;
            bar.LargeChange = 1;
            bar.SmallChange = 1;
            bar.TickStyle = TickStyle.BottomRight;
            bar.Value = values.Length / 2;
        }

        Dictionary<string,List<PlayerInfo>> players;
        Dictionary<string, Panel> Panels;

        private void SplitSide(List<Division> list, Panel parent, string id)
        {
            int count = list.Count;
            int AvailableHeight = parent.Height - ((list.Count-1) * Spacer);
            double multiplier = ((double)AvailableHeight / (double)TotalMenTeam1);

            var plyrs = new List<PlayerInfo>();
            players.Add(id, plyrs);

            for (int k = 0; k < count; k++)
            {
                PlayerInfo pl = new PlayerInfo();
                Panel p = pl.panel;
                pl.player = list[k];
                pl.name.Text = pl.player.UserName;
                plyrs.Add(pl);
                parent.Controls.Add(p);
                p.Width = parent.Width;
                p.Left = 0;
                p.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                p.MouseDown += new MouseEventHandler((sp,e) => p_MouseDown(sp,e,pl,id));
            }

            ClampNumbers(id);

            FitPanels(id);

            for (int k = 0; k < count - 1; k++)
            {
                var s = AddSplitter(plyrs[k].panel, plyrs[k + 1].panel, false);
                s.SplitterMoved += new SplitterEventHandler((sp,e) => s_SplitterMoved(sp,e,id));
            }
        }

        PlayerInfo[] ArmyCommanders = new PlayerInfo[3];

        void p_MouseDown(object sender, MouseEventArgs e, PlayerInfo pl, string id)
        {
            bool menChanged = false;
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                {
                    pl.player.RD_Guns = 0;
                }
                else if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                {
                    pl.player.RD_Men = pl.player.RD_Men_Preference;
                    pl.player.RD_Guns = pl.player.RD_Guns_Preference;
                    menChanged = true;
                }
                else
                {
                    pl.player.RD_Guns = Math.Max(6, pl.player.RD_Guns + 6);
                }
            }
            else
            {
                var current = ResetArmyCommander(pl.player.Side);
                if (pl != current)
                {
                    SetPlayerStatus(pl, true);
                    ArmyCommanders[pl.player.Side] = pl;
                    Output.ArmyCommanders[pl.player.Side] = pl.player;
                }
            }

            if (menChanged)
            {
                FitPanels(id);
                RefreshDisplay(id);
                FitPanels(id);
            }
            else
            {
                RefreshDisplay(id);
            }
        }

        void SetPlayerStatus(PlayerInfo pl, bool isArmyCommander)
        {
            if (pl != null)
            {
                if (isArmyCommander)
                {
                    pl.name.ForeColor = System.Drawing.Color.Yellow;
                    pl.name.Text = pl.player.UserName + " (with army)";
                }
                else
                {
                    pl.name.ForeColor = System.Drawing.Color.White;
                    pl.name.Text = pl.player.UserName;
                }
            }
        }

        void s_SplitterMoved(object sender, SplitterEventArgs e, string id)
        {
            RefreshDisplay(id);
        }

        private void RefreshDisplays()
        {
            for (int i = 1; i <= 2; i++)
            {
                FitPanels(SideIDs[i]);
                RefreshDisplay(SideIDs[i]);
            }
        }

        private void FitPanels(string id)
        {
            var plyrs = players[id];
            int count = plyrs.Count;
            var panel = Panels[id];

            int AvailableHeight = panel.Height - ((count-1) * Spacer);
            double mult = ((double)AvailableHeight / (double)TotalMenTeam1);

            int top = 0;
            for (int k = 0; k < count; k++)
            {
                var p = plyrs[k].panel;
                var pl = plyrs[k];
                p.Height = Math.Min((int)(mult * pl.player.RD_Men), p.Parent.Height - top);
                p.Left = 0;
                p.Top = top;
                top += p.Height + Spacer;
            }
        }

        private void RefreshDisplay(string id)
        {
            var plyrs = players[id];
            int count = plyrs.Count;
            var panel = Panels[id];
            int AvailableHeight = panel.Height - ((count-1) * Spacer);

            int sum = 0;
            foreach (var p in plyrs)
            {
                double multiplier = ((double)p.panel.Height / AvailableHeight);
                int men = (int)(multiplier * TotalMenTeam1);
                p.player.RD_Men = men;
                sum += men;
            }

            ClampNumbers(id);

            int side;
            int batterySize;
            foreach (var p in plyrs)
            {
                side = p.player.Side;
                batterySize = side == 1 ? 6 : 4;
                int men = (int)(p.player.RD_Men * (side == 2 ? Output.BalanceInfo.Team2InfantryMultiplier : 1));
                int guns = p.player.RD_Guns / batterySize * batterySize;
                var guntext = guns != 0 ? ", " + guns + " guns" : "";
                p.strength.Text = string.Format("{0} Men (player prefers {1}){2}", men < 1000 ? 0 : men, p.player.RD_Men_Preference, guntext);
                int ratio = (int)(((double)men * plyrs.Count / (double)TotalMenTeam1) * 100);
                ratio = Math.Min(150, ratio);
                p.panel.BackColor = Color.FromArgb(255, (side == 2 ? 100 + ratio : 50), 50, (side == 1 ? 100 + ratio : 50));
            }
        }

        private void ClampNumbers(string id)
        {
            var plyrs = players[id];
            int sum = plyrs.Sum(p => p.player.RD_Men);
            double mult = (double)TotalMenTeam1 / (double)sum;
            plyrs.ForEach(p => { p.player.RD_Men = (int)(p.player.RD_Men * mult); });
        }

        void ResetSliders()
        {
            totalMenSlider.Value = DefaultTotalMen / 100;
            infantryRatioSlider.Value = sideStrengthMultipliers.Length /2;
            artilleryRatioSlider.Value = sideStrengthMultipliers.Length /2;
        }

        void ResetArmyCommanders()
        {
            for (int i = 1; i <= 2; i++)
                ResetArmyCommander(i);
        }

        PlayerInfo ResetArmyCommander(int side)
        {
            var current = ArmyCommanders[side];
            SetPlayerStatus(current, false);
            ArmyCommanders[side] = null;
            Output.ArmyCommanders[side] = null;
            return current;
        }

        private void Reset()
        {
            ResetSliders();
            ResetArmyCommanders();
            foreach (var plyrs in players.Values)
            {
                foreach (var p in plyrs)
                {
                    p.player.RD_Men = p.player.RD_Men_Preference;
                    p.player.RD_Guns = p.player.RD_Guns_Preference;
                }
            }
            for (int i = 1; i <= 2; i++)
                ClampNumbers(SideIDs[i]);
            RefreshDisplays();
        }
        private void BalanceInf()
        {
            for (int i = 1; i <= 2; i++)
                BalanceInf(SideIDs[i]);
            RefreshDisplays();
        }
        private void BalanceInf(string id)
        {
            ResetSliders();
            var plyrs = players[id];
            int count = plyrs.Count;
            int menPerPlayer = (int)(DefaultTotalMen / (double)count);

            foreach (var p in plyrs)
            {
                p.player.RD_Men = menPerPlayer;
            }

            ClampNumbers(id);
        }

        /// <summary>
        /// Add a splitter between the two given controls
        /// </summary>
        /// <param name="control1">Control 1</param>
        /// <param name="control2">Control 2</param>
        /// <param name="vertical">Vertical orientation</param>
        /// <returns>The created Splitter</returns>
        public Splitter AddSplitter(Control control1, Control control2, bool vertical)
        {
            Control parent = control1.Parent;

            // Validate
            if (parent != control2.Parent)
                throw new ArgumentException(
                        "Both controls must be placed on the same Containter");

            //if (parent.Controls.Count > 2)
            //    throw new ArgumentException(
            //            "There may only be 2 controls on the Container");

            parent.SuspendLayout();

            // Move control2 before control1
            if (parent.Controls.IndexOf(control2) > parent.Controls.IndexOf(control1))
                parent.Controls.SetChildIndex(control2, 0);

            // Create splitter
            Splitter splitter = new Splitter();
            splitter.Dock = System.Windows.Forms.DockStyle.Left;

            splitter.MinExtra = 25;

            // Set controls properties
            control2.Dock = DockStyle.Fill;
            if (vertical)
            {
                control1.Dock = DockStyle.Left;
                splitter.Dock = DockStyle.Left;
                splitter.Width = Spacer;
            }
            else
            {
                control1.Dock = DockStyle.Top;
                splitter.Dock = DockStyle.Top;
                splitter.Height = Spacer;
            }

            // Add splitter to the parent in the middle
            parent.Controls.Add(splitter);
            parent.Controls.SetChildIndex(splitter, 1);
            parent.ResumeLayout();
            splitter.Cursor = Cursors.SizeNS;
            splitter.BackColor = Color.Black;
            return splitter;
        }

        int lastHeight = 0;
        private void BalanceSides_ResizeEnd(object sender, EventArgs e)
        {
            if (this.Height != lastHeight && lastHeight != 0)
            {
                RefreshDisplays();
            }
            lastHeight = this.Height;
        }

        private void BalanceSides_SizeChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            BalanceInf();
        }

        void totalMenSlider_ValueChanged(object sender, EventArgs e)
        {
            TotalMenTeam1 = totalMenSlider.Value * 100;
            totalMenLabel.Text = TotalMenTeam1 + " Men for US side";
            players.Keys.ForEach(s => RefreshDisplay(s));
        }

        private void infantryRatioSlider_ValueChanged(object sender, EventArgs e)
        {
            RatioSliderChanged(infantryRatioSlider, infantryRatioLabel, pct => Output.BalanceInfo.Team2InfantryPercentage = pct);
        }

        private void artilleryRatioSlider_ValueChanged(object sender, EventArgs e)
        {
            RatioSliderChanged(artilleryRatioSlider, artilleryRatioLabel, pct => Output.BalanceInfo.Team2ArtilleryPercentage = pct);
        }

        private void RatioSliderChanged(TrackBar slider, Label label, Action<int> percentAction)
        {
            double ratio = sideStrengthMultipliers[slider.Value];
            label.Text = "{0} :: 100".With((int)(ratio * 100));
            int percent =  (int)((ratio) * 100);
            percentAction(percent);

            players.Keys.ForEach(s => RefreshDisplay(s));
        }

        private double[] sideStrengthMultipliers = new[]{
0.1,
0.125,
0.142857142857143,
0.166666666666667,
0.2,
0.25,
0.333333333333333,
0.5,
0.526315789473684,
0.555555555555556,
0.588235294117647,
0.625,
0.666666666666667,
0.714285714285714,
0.769230769230769,
0.8,
0.833333333333333,
0.869565217391304,
0.909090909090909,
0.952380952380952,
1,
1.05,
1.1,
1.15,
1.2,
1.25,
1.3,
1.4,
1.5,
1.6,
1.7,
1.8,
1.9,
2,
3,
4,
5,
6,
7,
8,
10,

        };
        private int[] menGunRatios = new[]{
10,
50,
100,
125,
150,
175,
200,
225,
250,
275,
300,
325,
350,
375,
400,
425,
450,
475,
500,
550,
600,
700,
800,
900,
1000,
        };

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            int ratio = menGunRatios[menGunRatioSlider.Value];
            menGunRatioLabel.Text = ratio + " men per gun";
            Output.MenGunRatio = ratio;
        }
    }
}
