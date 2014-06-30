using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Launcher.Modules;

using Utilities.Windows.SimpleForms;
using Utilities.Windows;
using Utilities;
using Utilities.GCSV;

namespace Launcher {
  using Pair = KeyValuePair<string, string>;
  using Options = IEnumerable<KeyValuePair<string, string>>;
    
    public partial class MainForm : Form
    {
        public SimpleFormsController Controller { get; private set; }

        List<Control> actionStarters;

        public bool IsBusy { get; private set; }

        public MainForm()
        {
            this.Visible = false;
            InitializeComponent();
            this.Visible = false;

            this.progressBar.Visible = false;

            Controller = new SimpleFormsController(this);
            Controller.ActionStarted += StartedActionCallback;
            Controller.ActionFinished += FinishedActionCallback;
            
            actionStarters = new List<Control>()
            {
                panel1,
                menuStrip1,
            };
            launchBattleButton.Click += Run<PrepareScenario>;
            startSowButton.Click += (s, e) => Run(GcmLauncher.Helpers.SOW.StartSOW);
            startScourgeOfWarToolStripMenuItem.Click += (s, e) => Run(GcmLauncher.Helpers.SOW.StartSOW);
            prepareScenarioToolStripMenuItem.Click += Run<PrepareScenario>;

            openLogFileToolStripMenuItem.Click += (s, e) => Run(GcmLauncher.Helpers.SOW.OpenLogFile);
            deleteTemporaryFilesToolStripMenuItem.Click += (s, e) => Run(GcmLauncher.Helpers.SOW.ClearOldFiles);
            disableBugleCallsToolStripMenuItem.Click += (s, e) => Run(() => GcmLauncher.Helpers.SOW.DisableBugles(silent: false));
            enableBugleCallsToolStripMenuItem.Click += (s, e) => Run(GcmLauncher.Helpers.SOW.EnableBugles);
            enableGCMHotkeysToolStripMenuItem.Click += (s, e) => Run(GcmLauncher.Helpers.SOW.InstallGcmHotkeys);
            disableMusicToolStripMenuItem.Click += (s, e) => Run(() => GcmLauncher.Helpers.SOW.DisableMusic(() => System.Windows.Forms.MessageBox.Show("Unable to disable your menu music.  You can disable it manually by deleting the Base\\Music directory."), () =>  System.Windows.Forms.MessageBox.Show("Menu music disabled.")));

            disableAllNonGCMModsToolStripMenuItem.Click += (s, e) => Run(GcmLauncher.Helpers.SOW.DisableAllNonGcmMods);
        }

        void Run<T>(object sender, EventArgs e) where T : ISimpleFormsModule, new()
        {
            Controller.RunModule<T>();
        }

        public void Run<T>() where T : ISimpleFormsModule, new()
        {
            Controller.RunModule<T>();
        }
        
        void Run(Action action)
        {
            Controller.RunAction(action);
        }

        void StartedActionCallback(string actionName)
        {
            foreach (var item in actionStarters) item.Enabled = false;

            IsBusy = true;

            this.progressBar.Style = ProgressBarStyle.Marquee;
            //this.statusLabel.Text = "Executing " + actionName;
            this.progressBar.Visible = true;
        }

        void FinishedActionCallback(string actionName)
        {
            this.progressBar.Visible = false;

            if(this.statusLabel.Text == "Executing Action")
                this.statusLabel.Text = "Ready";

            IsBusy = false;

            foreach (var item in actionStarters) item.Enabled = true;
        }

        public void SetStatus(string message)
        {
            this.InvokeIfRequired(() => statusLabel.Text = message);
        }

        private void MapToolMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Utilities.Windows.WindowsExtensions.SaveFormLayout(this, GcmLauncher.Var, "opt_mainwindow_");
                GcmLauncher.MusicPlayer.Stop();
                GcmLauncher.Data.SaveVariables();
                Controller.Quit();
                Utilities.Windows.Processes.KillThisProcess();
            }
            catch (NullReferenceException)
            {
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Hide();
        }

        public void OnShown()
        {
            GcmLauncher.MainForm = this;
            if (GcmLauncher.Auth.IsDeveloper)
                devToolStripMenuItem.Visible = true;
            GcmLauncher.OnFormLoad();
            this.checkVersionTimer.Start();
            this.reportBattleTimer.Start();
            Utilities.Windows.WindowsExtensions.ApplySavedFormLayout(this, GcmLauncher.Var, "opt_mainwindow_");
            this.BringToFront();
            Run<Initialize>();
        }


        static bool shown = false;

        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (shown) return;
            shown = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Utilities.Windows.Processes.RestartThisApplication();
            this.Close();
        }

        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GcmLauncher.Functions.Restart();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GcmLauncher.Quit();
        }

        private void uploadScreenshotToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void uploadScreenshotToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        public void SwitchToLaunchGame()
        {
            startSowButton.Visible = true;
        }

        private void crashToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new InvalidOperationException("crash");
        }

        private void startSowButton_Click(object sender, EventArgs e)
        {

        }

        private void killSOWProcessesToolStripMenuItem_Click(object sender, EventArgs e) {
          GcmLauncher.Helpers.SOW.KillSOWProcesses();
        }

        private void startScourgeOfWarToolStripMenuItem_Click(object sender, EventArgs e) {

        }
    }
}
