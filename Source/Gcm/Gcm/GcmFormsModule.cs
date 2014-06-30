using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.IO;
using GcmShared;
using Utilities;
using Launcher.GCM;
using Launcher.Forms;
using System.Windows.Forms;

namespace Launcher
{

    public class GcmFormsModule : Utilities.Windows.SimpleForms.SimpleFormsModule
    {
        protected ILog Log { get { return GcmLauncher.LogFile; } }

        public MainForm Form { get { return GcmLauncher.MainForm; } }
        public override void DoYourJob()
        {
            throw new NotImplementedException();
        }
        protected void SetStatusWithoutLog(string message)
        {
            Form.SetStatus(message);
        }
        protected void SetStatus(string message)
        {
            Form.SetStatus(message);
            GcmLauncher.LogFile.Write(message);
        }
        protected void ShowStatus(string message)
        {
            Form.SetStatus(message);
            System.Windows.Forms.MessageBox.Show(message);
        }

        public bool Quit { get; protected set; }

        protected void QuitOperation(string message = "Ready")
        {
            Quit = true;
            SetStatus(message);
        }

        protected void QuitOperationWithMessageBox(string message = "Ready")
        {
            Quit = true;
            ShowStatus(message);
        }

        public override void BeforeJob()
        {
            Quit = false;
        }


        // Functions that are shared by several modules

        protected int ChooseGame(List<int> games)
        {
            if (games.Count == 0)
            {
                ShowStatus("You have no battles to submit screenshots for.");
                QuitOperation();
                return -1;
            }

            games.Sort();
            games.Reverse();

            GameListForm form = new GameListForm("Select Battle", "Continue");
            form.SetGameList(games.Select(i => i.ToString()));
            form.ShowDialog();
            if (form.DialogResult != DialogResult.OK)
            {
                QuitOperation();
                return -1;
            }

            int gameID = int.Parse(form.SelectedGame);
            return gameID;
        }
    }
}
