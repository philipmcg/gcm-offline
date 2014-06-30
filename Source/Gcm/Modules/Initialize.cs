using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using Utilities.Windows.SimpleForms;
using Utilities.Windows;
using Utilities;
using Launcher.GCM;
using GcmShared;
using System.Management;
using System.Security.Principal;

namespace Launcher.Modules {
  class Initialize : GcmFormsModule {
    public override void DoYourJob() {
      SetStatus("Initializing");
      GcmLauncher.MainForm.Invoke(new Action(GcmLauncher.Helpers.SOW.EnsureSOWDirectory));



      if (!GcmLauncher.Helpers.SOW.SOWPathIsCorrect()) {
        MessageBox.Show("Failed to set SOW directory correctly");
        Utilities.Windows.Processes.KillThisProcess();
        return;
      }
      SetStatus("Initializing.");

      GcmLauncher.Helpers.SOW.DeleteOldMods();

      GcmLauncher.Helpers.SOW.InstallMod();
      GcmLauncher.Helpers.SOW.FixIniFile();
      SetStatus("Initializing..");
      GcmLauncher.Helpers.SOW.AutoDisableBugles();
      GcmLauncher.Helpers.SOW.UninstallFowMod(GcmLauncher.Helpers.SOW.TemporaryFowHolderMod);
      // GcmLauncher.Helpers.SOW.UpdateFilesVersion();

      SetStatus("Initializing...");
      if (GcmLauncher.Auth.Div1 != 0 && GcmLauncher.Auth.Div2 != 0) {
        GcmLauncher.MainForm.BeginInvoke(new Action(GcmLauncher.MainForm.SwitchToLaunchGame));
      }

      Gcm.Var.Int["next_battle_id"] = 0;

      DirectoryEx.EnsureDirectory(Paths.Local.GcmDir());
      SetStatus("Ready");
      if (Gcm.Var.Str["opt_pcid", null] == null) {
        Gcm.Var.Str["opt_pcid"] = Guid.NewGuid().ToString();
      }

    }

  }
}
