using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using System.Threading;

using System.Web;
using System.Net;
using System.IO;
using Utilities;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using GcmShared;

namespace Launcher
{
    static class Program
    {
        static StatusWindow s_StatusWindow;
        [STAThread]
        static void Main()
        {
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            Utilities.Windows.Processes.KillOtherProcessesOfSameApplication();

            var starter = new Utilities.Windows.ProgramStarter();
            starter.Prepare(true);
            starter.StartWindow<MainForm>(() =>
            {
            },
            () =>
            {
                s_StatusWindow = new StatusWindow(() =>
                {
                    ((MainForm)Application.OpenForms[0]).OnShown();
                },() => {
                    ((MainForm)Application.OpenForms[0]).Close();
                    Application.Exit();
                }
                );
                s_StatusWindow.Begin();
            });
        }

        public class StatusWindow
        {
            UpdateStatusForm form;

            Action callback;
            Action quitEntirely;
            public StatusWindow(Action callback, Action quitEntirely)
            {
                form = new UpdateStatusForm();
                this.callback = callback;
                this.quitEntirely = quitEntirely;
            }

            void Quit()
            {
                form.Close();
                quitEntirely();
            }

            public volatile bool quit;

            public void Begin() {
              form.SetStatus("Loading", "Loading...", true);
              RunBackground(() => { }, LoadData);
              form.ShowDialog();
            }

            void RunBackground(Action run, Action finished) {
              BackgroundWorker worker = new BackgroundWorker();
              worker = new BackgroundWorker();
              worker.DoWork += new DoWorkEventHandler((s, a) => run());
              worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler((s, a) => finished());
              worker.RunWorkerAsync();
            }

            void LoadData() {
              if (quit) {
                Quit();
                return;
              } else {
                form.SetStatus("Loading", "Loading...", true);
                RunBackground(InitializeData, BeginLogin);
              }
            }
            void InitializeData() // async
            {
                GcmLauncher.InitializeData(); 
            }

            void BeginLogin() {
              if (quit) {
                Quit();
                return;
              } else {
                form.SetStatus("Connecting", "Connecting to GCM...", true);
                RunBackground(ActuallyLogin, HandleLogin);
              }
            }

            void ActuallyLogin() // async
            {
            }

            void HandleLogin() {
              if (quit) {
                Quit();
                return;
              } else {
                GcmLauncher.Auth = new AuthResult() {
                  PlayerID = 0,
                  UserID = Guid.NewGuid(),
                  Username = "Offline",
                };
                form.Close();
                callback();
              }
            }
        }
    }
}
