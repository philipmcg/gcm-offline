using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.IO;
using GcmShared;
using Utilities;
using Launcher.GCM;

using Utilities.Windows;

namespace Launcher
{
    public class GcmLauncherDependencies : Ninject.Modules.NinjectModule
    {
        public override void Load()
        {
            Bind<ILog>().ToMethod(c => GcmLauncher.LogFile);
        }
    }

    public class GcmLauncher
    {
        public const string AppName = "GCM.exe";

        public static GcmLauncherData Data { get; private set; }
        public static AuthResult Auth { get; set; }
        public static IVariableBin Var { get { return Data.VariableBin; } }

        public static readonly string Directory = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);

        public static bool HasSOWDirectory { get { return Var.Str.ContainsKey("opt_sow_directory") && !string.IsNullOrWhiteSpace(SOWDirectory); } }

        public static GcmHelpers Helpers { get; private set; }
        
        public static SimpleFunctions Functions = new SimpleFunctions();
        public static MainForm MainForm { get; set; }
        public static LogFile LogFile;
        public static Mp3Player MusicPlayer { get; private set; }

        public static string SOWDirectory
        {
            get
            {
                return Var.Str["opt_sow_directory"];
            }
        }

        public static string ReplayDirectory { get { return Path.Combine(GcmLauncher.SOWDirectory, "Work\\Saved Games\\Replay"); } }

        public static void InitializeData()
        {
            LogFile = new Utilities.LogFile("gcm.log", FileMode.Create);
            LogFile.TimestampFormat = "0000.000  ";
            LogFile.Start();
            Gcm.Initialize(new GcmLauncherDependencies());

            Data = new GcmLauncherData(s => Path.Combine(Directory, "Data", s));

            Gcm.Data = Data;
            Files.TempDir = Path.Combine(Environment.CurrentDirectory, "temp");
            DirectoryEx.EnsureDirectory(Files.TempDir);
            Helpers = new GcmHelpers();
            MusicPlayer = new Mp3Player();
        }

        public static void OnFormLoad()
        {
            if (!Var.Bool["opt_tried_to_disable_music", false])
            {
                bool success = true;
                GcmLauncher.Helpers.SOW.DisableMusic(() => success = false, () => { });
                Var.Bool["opt_tried_to_disable_music"] = true;
            }
            MainForm.Text = "Offline GCM Launcher";
        }


        public static void Quit()
        {
            if (MainForm != null)
                MainForm.InvokeIfRequired(MainForm.Close);
            else
                Utilities.Windows.Processes.KillThisProcess();
        }
    }
}
