using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Utilities;
using Utilities.Windows;
using System.IO;
using System.Windows.Forms;

using GcmShared;

namespace Launcher.GCM {
  public class SOWDirectory {
    public string ScourgeOfWarDirectory { get { return GcmLauncher.SOWDirectory; } }

    string GetPath(string afterSOW) {
      return Path.Combine(GcmLauncher.SOWDirectory, afterSOW);
    }

    string GetModDirectory(string modName) {
      return Path.Combine(GcmLauncher.SOWDirectory, "Mods", modName);
    }

    public string GetBattleDirectory(DateTime date) {
      return GetPath("Work\\Scenarios\\Battle {0}".With(date.ToGcmFileDateString()));
    }
    public string GetLatestBattleDirectory(DateTime date) {
      return GetPath("Work\\Scenarios\\-- Latest Battle {0}".With(date.ToGcmFileDateString()));
    }
    public string GetSeparatorBattleDirectory() {
      return GetPath("Work\\Scenarios\\- SELECT A SCENARIO BELOW");
    }
    public string[] GetOldLatestBattles() {
      string dir = GetPath("Work\\Scenarios\\");
      var dirs = Directory.GetDirectories(dir, "*-- Latest*");
      return dirs;
    }

    public string WorkDirectory { get { return GetPath("Work"); } }
    public string NetServDirectory { get { return GetPath("Work\\Scenarios\\NetServ"); } }
    public string IniFile { get { return GetPath("Work\\sowgb.ini"); } }
    public string HostListFile { get { return GetPath("Work\\servers.ini"); } }
    public string LogFile { get { return GetPath("Work\\sowgb.log"); } }
    public string SOWGB { get { return GetPath("SOWGB.exe"); } }
    public string SOWCV { get { return GetPath("SOWCV.exe"); } }
    public string SowgbLogFile { get { return GetPath("Work\\sowgb.log"); } }

    public bool SOWPathIsCorrect() {
      return SOWPathCorrect(ScourgeOfWarDirectory);
    }

    bool SOWPathCorrect(string path) {
      var ini = (File.Exists(Path.Combine(path + "\\Work\\", "sowgb.ini")));
      return ini && EitherFileExists(Path.Combine(path, "SOWGB.exe"), Path.Combine(path, "SOWCV.exe")); // need to make it accomodate both GB and CV
    }

    bool EitherFileExists(params string[] filenames) {
      foreach (var filename in filenames) {
        if (File.Exists(filename))
          return true;
      }
      return false;
    }

    static readonly string[] SOWProcesses = new string[] { "SOWGB", "SOWCV" };
    public bool OtherSOWProcessIsRunning() {
      return SOWProcesses.Any(name => Utilities.Windows.Processes.OtherProcessOfNameIsRunning(name));
    }
    public void KillSOWProcesses() {
      foreach (var name in SOWProcesses) {
        System.Diagnostics.Process.GetProcessesByName(name).ForEach(p => p.Kill());
      }
    }

    void StartWhicheverExists(params string[] paths) {
      foreach (var path in paths) {
        if (File.Exists(path)) {
          System.Diagnostics.Process.Start(path);
          return;
        }
      }
    }

    bool ShowSetSOWPath() {
      string path = GcmLauncher.SOWDirectory;
      try {
        if (!Directory.Exists(path))
          path = Util.ReadRegistryKey("SOFTWARE\\NorbSoftDev\\SowGB", "InstallDir");
      } catch { }

      if (!Directory.Exists(path))
        path = @"C:\Matrix Games";

      FolderBrowserDialog dlg = new FolderBrowserDialog();
      dlg.Description = "Select the main Scourge of War directory.";
      dlg.SelectedPath = path;
      var result = dlg.ShowDialog();

      if (result == DialogResult.OK) {
        if (SOWPathCorrect(dlg.SelectedPath))
          Gcm.Var.Str["opt_sow_directory"] = dlg.SelectedPath;
        else {
          MessageBox.Show("You must select the directory where SOWGB is installed.  This is typically the 'Scourge of War - Gettysburg' folder.  Try again or cancel.");
          ShowSetSOWPath();
        }
      } else {
        Application.Exit();
        return false;
      }
      return true;
    }

    public void EnsureSOWDirectory() {
      if (GcmLauncher.HasSOWDirectory) {
        if (SOWPathCorrect(GcmLauncher.SOWDirectory))
          return;
      }

      ShowSetSOWPath();
    }

    public void ClearOldFiles() {
      try {
        ClearFilesMoreThanTwoDaysOld();
      } catch {
        MessageBox.Show("The launcher was unable to clear old files, perhaps you do not have permissions of some kind.");
      }
    }

    List<string[]> GetMd5sOfFilesInDirectory(string dir) {
      var files = Directory.GetFiles(dir);
      return files.Select(f => new[]{new FileInfo(f).Name, Utilities.Hashing.GetMD5(File.ReadAllText(f)), new FileInfo(f).Length.ToString()}).ToList();
    }

    public void WriteScenarioMd5sToFile(string file) {
      File.WriteAllLines(file, GetMd5sOfFilesInDirectory(NetServDirectory).Select(s => string.Join(",", s)));
    }

    public string FindLastMpLog() {
      string folder = Path.Combine(GcmLauncher.SOWDirectory, "Work");
      string initialFile = FileEx.FindLastModifiedFile(folder, "sowmp_log_*.log");
      return initialFile;
    }

    void ClearFilesMoreThanTwoDaysOld() {
      string workdir = Path.Combine(GcmLauncher.SOWDirectory, "Work");

      long bytes = 0;

      bytes += DeleteFiles("sowmp_log_*.log", workdir);
      bytes += DeleteFiles("sowgb_gamedb_*.csv", workdir);
      bytes += DeleteSubdirectories("Battle *", Path.Combine(workdir, "Scenarios"), false);

      bytes += DeleteFiles("*", Paths.Local.WabashDir());
      bytes += DeleteSubdirectories("*", Paths.Local.WabashDir(), true);

      System.Windows.Forms.MessageBox.Show(string.Format("Cleared {0} MB of old files", bytes / 1000 / 1000));
    }

    long DeleteFiles(string pattern, string directory) {
      long count = 0;
      long now = DateTime.Now.ToSeconds();
      int threeDays = (60 * 60 * 24 * 2);
      DirectoryInfo dir = new DirectoryInfo(directory);
      var files = dir.GetFiles(pattern);
      var delete = files.Where(f => f.LastWriteTime.ToSeconds() < now - threeDays);
      foreach (var file in delete) {
        try {
          count += file.Length;
          file.Delete();
        } catch { }
      }
      return count;
    }

    /// <summary>
    /// If clean is true, then recreate the directory (empty) after deleting it.
    /// </summary>
    long DeleteSubdirectories(string pattern, string directory, bool clean) {
      long count = 0;
      long now = DateTime.Now.ToSeconds();
      int threeDays = (60 * 60 * 24 * 2);

      DirectoryInfo dir = new DirectoryInfo(directory);
      var dirs = dir.GetDirectories(pattern);
      var delete = dirs.Where(f => f.CreationTime.ToSeconds() < now - threeDays);
      foreach (var d in delete) {
        try {
          count += DirectoryEx.GetSizeOfDirectory(d.FullName);
          d.Delete(true);
          if (clean)
            Directory.CreateDirectory(d.FullName);
        } catch { }
      }
      return count;
    }


    public void DisableMusic(Action onFailure, Action onSuccess) {
      try {
        string music = Path.Combine(GcmLauncher.SOWDirectory, "Base\\Music");
        if (Directory.Exists(music))
          Directory.Move(music, music + "_Backup");
      } catch {
        onFailure();
      }
    }

    public void DisableBugles(bool silent) {
      try {
        string folder = Path.Combine(GcmLauncher.SOWDirectory, "Base\\Sounds");
        string dest = Path.Combine(folder, "Bugle_Calls");
        if (!Directory.Exists(dest))
          Directory.CreateDirectory(dest);

        foreach (var file in Gcm.Data.GCSVs["bugle_calls"]) {
          string filename = file["name"] + ".wav";
          string filepath = Path.Combine(folder, filename);
          if (File.Exists(filepath))
            File.Move(filepath, Path.Combine(dest, filename));
        }
        if (!silent)
          System.Windows.Forms.MessageBox.Show("Bugle call sounds disabled.");
      } catch {
        if (!silent)
          System.Windows.Forms.MessageBox.Show("Unable to disable your in-game bugle calls.  This may happen if you do not have administrator rights.");
      }
    }

    public void EnableBugles() {
      try {
        string soundsFolder = Path.Combine(GcmLauncher.SOWDirectory, "Base\\Sounds");
        string buglesFolder = Path.Combine(soundsFolder, "Bugle_Calls");
        if (Directory.Exists(buglesFolder)) {
          DirectoryEx.MoveContents(buglesFolder, soundsFolder);
          System.Windows.Forms.MessageBox.Show("Enabled in-game bugle calls");
        } else {
          System.Windows.Forms.MessageBox.Show("You do not appear to have bugle calls disabled already.");
        }
      } catch {
        System.Windows.Forms.MessageBox.Show("Unable to enable your in-game bugle calls.  This may happen if you do not have administrator rights.");
      }
    }

    public void InstallMod() {
      InstallGCMMods();
    }

    // returns false if any SOW processes are still running after this function is called.
    public bool PromptToKillSowProcesses(string message) {
      if (OtherSOWProcessIsRunning()) {
        var result = MessageBox.Show(message, "Scourge of War is still running!", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
        if (result == DialogResult.Yes) {
          KillSOWProcesses();
          return false;
        } else {
          return true;
        }
      }
      return false;
    }
    void InstallGCMMods() {
      string directoryToMove = Path.Combine(GcmLauncher.Directory, "Move");
      if (Directory.GetFiles(directoryToMove).Any()) {
        bool sowIsRunning = PromptToKillSowProcesses("Do you want the Launcher to close Scourge of War and proceed to install the update?  If not, close Scourge of War yourself and then restart the Launcher.");
        if (sowIsRunning) {
          GcmLauncher.MainForm.InvokeIfRequired(GcmLauncher.MainForm.Close);
          return;
        }
      }

      if (!System.IO.Directory.Exists(directoryToMove))
        System.IO.Directory.CreateDirectory(directoryToMove);

      try {
        DirectoryEx.CopyContents(directoryToMove, GcmLauncher.SOWDirectory);
      } catch (System.UnauthorizedAccessException) {
        System.Windows.Forms.MessageBox.Show("Unable to install GCM Mod.  You need to run the Launcher with Admin rights for this to work.\n\nYou may need to reinstall GCM.");
        GcmLauncher.MainForm.Close();
        return;
      } catch (System.IO.IOException) {
        System.Windows.Forms.MessageBox.Show("Unable to install GCM Mod.  If you have Scourge of War running, you should close it and then run the Launcher again.");
        GcmLauncher.MainForm.Close();
        return;
      } catch (System.ArgumentException) {
        System.Windows.Forms.MessageBox.Show("Unable to install GCM Mod.  Try restarting or reinstalling the Launcher and setting your Scourge of War path correctly.");
        GcmLauncher.MainForm.Close();
        return;
      }

      try {
        DirectoryEx.DeleteContents(directoryToMove);
      } catch { }

    }

    Guid GetFilesVersion() {
      return Guid.Parse(Util.GetMD5HashFromFile(Path.Combine(GcmLauncher.SOWDirectory, "Base\\Maps\\KS.bmp")));
    }

    public void StartSOW() {
      UninstallFowMod(TemporaryFowHolderMod);
      System.Diagnostics.Process.Start(SOWGB);
    }

    public int GetDayHash() {
      var date = DateTime.UtcNow.AddHours(12).Date.ToBinary();
      int hash = new Random((int)date).Next() % 89 + 10;
      return hash;
    }

    public readonly string TemporaryFowHolderMod = "GCM_Version";

    public void StartSOWWithFOW() {
      DeleteFile(Path.Combine(GcmLauncher.SOWDirectory, "Mods", "GCM_Toolbar", "Logistics", "gscreens.csv"));
      InstallFowMod("GCM_FogOfWar", TemporaryFowHolderMod);
      System.Diagnostics.Process.Start(SOWGB);
    }

    void DeleteFile(string path) {
      if (File.Exists(path)) {
        try {
          File.Delete(path);
        } catch { }
      }
    }

    string ModDirectory(string modName) {
      return Path.Combine(GcmLauncher.SOWDirectory, "Mods", modName);
    }

    public void UninstallFowMod(string temporaryHolderMod) {
      string dirToDelete = Path.Combine(ModDirectory(temporaryHolderMod), "Logistics");
      if (Directory.Exists(dirToDelete)) {
        try {
          DirectoryEx.DeleteContents(dirToDelete);
          Directory.Delete(dirToDelete);
        } catch { throw; }
      }
    }

    void InstallFowMod(string fowMod, string temporaryHolderMod) {
      string dirToCopy = Path.Combine(ModDirectory(fowMod), "Logistics");
      string destDir = Path.Combine(ModDirectory(temporaryHolderMod), "Logistics");
      DirectoryEx.EnsureDirectory(destDir);
      DirectoryEx.CopyContents(dirToCopy, destDir);
    }

    public void OpenLogFile() {
      System.Diagnostics.Process.Start(LogFile);
    }

    public void LoadLogFile() {
      if (File.Exists(LogFile)) {
        File.Copy(LogFile, SowgbLogFile);
        System.Diagnostics.Process.Start(SowgbLogFile);
      } else
        MessageBox.Show("No log file found");
    }

    public void DisableAllNonGcmMods() {
      try {
        if (File.Exists(IniFile)) {
          DisableAllMods();
          EnableGcmMods();
          MessageBox.Show("Successfully disabled non-GCM mods.");
        } else
          MessageBox.Show("Could not find your sowgb.ini file.");
      } catch {
        MessageBox.Show("There was a problem disabling mods, you may need to do it manually.");
      }
    }

    public void DeleteOldMods() {
      var oldMods = new List<string>()
            {
                "MP_GCM",
                "GCM_Gameplay",
                "GCM_Minimaps",
            };

      foreach (var mod in oldMods) {
        string dir = GetModDirectory(mod);

        try {
          if (Directory.Exists(dir)) {
            DirectoryEx.DeleteContents(dir);
            Directory.Delete(dir);
          }
        } catch (Exception) {
        }
      }
    }

    void DisableAllMods() {
      HashSet<string> ModsToLeave = new HashSet<string>()
                {
                    "GCM_Toolbar",
                    "GCM_StockToolbar",
                };

      var lines = File.ReadAllLines(IniFile).ToList();

      for (int i = 0; i < lines.Count; i++) {
        string line = lines[i];

        if (line.StartsWith("ModName")) {
          if (!ModsToLeave.Contains(GetModName(line))) {
            int modNumber = GetModNumber(line);

            if (lines[i + 1].StartsWith("ModActive{0}".With(modNumber)))
              lines.RemoveAt(i + 1);

            lines.RemoveAt(i);
          }
        }
      }

      File.WriteAllLines(IniFile, lines);
    }

    string GetModName(string modNameLine) {
      return modNameLine.Split('\\').Last();
    }
    int GetModNumber(string modNameLine) {
      return int.Parse(modNameLine.Substring("ModName".Length, modNameLine.IndexOf('=') - "ModName".Length));
    }

    void TrimHostList() {
      if (!File.Exists(HostListFile))
        return;

      var lines = File.ReadAllLines(HostListFile);
      var results = new List<string>();
      results.Add("[MPHosts]");

      SortedDictionary<string, string> hosts = new SortedDictionary<string, string>();
      SortedSet<string> existing = new SortedSet<string>();

      foreach (var line in lines) {
        if (line.Length == 0)
          continue;
        else if (line[0] >= '0' && line[0] <= '9') {
          var split = line.Split(' ');
          int unused = 0;
          if (int.TryParse(split[0], out unused) && split.Length >= 2) {
            var rest = line.Substring(split[0].Length + 1).Split('=');
            if (rest.Length == 2)
              hosts[rest[0]] = rest[1];
          }
        } else if (line[0] == '[' && line.StartsWith("[MPHosts]"))
          continue;
        else
          existing.Add(line);
      }

      if (hosts.Any()) {
        foreach (var host in hosts) {
          var line = host.Key + "=" + host.Value;
          if (existing.Contains(line))
            continue;
          else
            results.Add(line);
        }
        foreach (var line in existing) {
          results.Add(line);
        }
        results.Add("");
        File.WriteAllLines(HostListFile, results);
      }
    }

    const string IniSectionBeginner = "[";

    void SetValuesInIniFile(List<string> lines, string section, Dictionary<string, string> pairs) {
      // Find the index of this section in the ini file, or add it as a new section at the end.
      int index = lines.IndexOf(section);
      if (index < 0) {
        lines.Add(section);
        index = lines.Count - 1;
      }

      foreach (var pair in pairs) {
        // Find the index of this key in the ini file and replace it
        // or insert a new line if it's not already there.
        string line = pair.Key + "=" + pair.Value;
        int keyIndex = FindKeyInIniFileSection(lines, index, pair.Key);

        // if the value is the empty string, that means remove the line from the ini file.
        if (pair.Value == string.Empty) {
          if (keyIndex != -1)
            lines.RemoveAt(keyIndex);
        } else {
          if (keyIndex == -1)
            lines.Insert(index + 1, line);
          else
            lines[keyIndex] = line;
        }
      }
    }

    // Provide the startIndex of the section.
    // This will find the index of the given key in the list of lines, if such a key is found before a new section begins.
    // If this key is not in the given section, we will return -1.
    int FindKeyInIniFileSection(List<string> lines, int startIndex, string key) {
      int index = startIndex + 1;
      while (lines.Count > index) {
        if (lines[index].StartsWith(IniSectionBeginner))
          return -1;
        else if (lines[index].StartsWith(key + "="))
          return index;
        index++;
      }
      return -1;
    }

    void SetDebugSettings(List<string> iniLines) {
      SetValuesInIniFile(iniLines, "[Debug]", new Dictionary<string, string>()
            { 
                {"MPLog", "1"},
                {"AutoGameDB", "1"},
                {"aicount", string.Empty}, 
            });
    }

    void SetPlayerName(string name) {
      var lines = File.ReadAllLines(IniFile).ToList();
      for (int i = 0; i < lines.Count; i++) {
        string line = lines[i];
        if (line.StartsWith("$plyrname=")) // set their player name in game to their username.
          lines[i] = "$plyrname=" + name;
      }
      File.WriteAllLines(IniFile, lines);
    }

    void EnableGcmMods() {
      var ModsToActivate = Gcm.Data.GCSVs["mods"]
          .Where(l => l["enable"].ToBool() == true)
          .Select(l => l["name"])
          .ToHashSet();

      var ModsToDeactivate = Gcm.Data.GCSVs["mods"]
          .Where(l => l["enable"].ToBool() == false)
          .Select(l => l["name"])
          .ToHashSet();


      var modPairs = new List<string[]>() { 
                new[] { "GCM_StockToolbar", "GCM_Toolbar" },
            };

      var modsBeingUsed = modPairs.Select(p => p[0]).ToDictionary(s => s, s => false);

      var lines = File.ReadAllLines(IniFile).ToList();

      SetDebugSettings(lines);


      if (!lines.Contains("[Mods]")) {
        lines.Add("[Mods]");
        lines.Add("Count=0");
      }

      int modStart = lines.IndexOf("[Mods]");
      if (!lines[modStart + 1].StartsWith("Count"))
        lines.Insert(modStart + 1, "Count=0");

      for (int i = 0; i < lines.Count; i++) {

        string line = lines[i];
 foreach (var pair in modPairs) {
          if (line.EndsWith(pair[0]) && line.StartsWith("ModName"))
            modsBeingUsed[pair[0]] = lines[i + 1].EndsWith("1");
        }
      }

      foreach (var pair in modPairs) {
        if (modsBeingUsed[pair[0]])
          ModsToDeactivate.Add(pair[1]);
        else
          ModsToActivate.Add(pair[1]);
      }


      SortModsNew(lines, ModsToActivate, ModsToDeactivate);


      File.WriteAllLines(IniFile, lines);

    }

    void SortModsNew(List<string> lines, HashSet<string> ModsToActivate, HashSet<string> ModsToDeactivate) {
      var modsToSave = new[] { "GCM_Version" };
      var currentModsInList = new HashSet<string>();
      var currentMods = new Heap<int, Tuple<bool, string>>();
      int max = 0;
      for (int i = 0; i < lines.Count; i++) {
        string line = lines[i];
        if (line.StartsWith("ModName")) {
          int number = int.Parse(line.Substring("ModName".Length, line.IndexOf('=') - "ModName".Length));
          bool active = int.Parse(lines[i + 1].Substring(lines[i + 1].Length - 1)).ToBool();
          string modName = line.Split('\\').Last();
          if (!modsToSave.Contains(modName)) {
            max = Math.Max(max, number);
            currentMods.Enqueue(number, Tuple.Create(active, modName));
            currentModsInList.Add(modName);
          }
          // stay in place in list
          lines.RemoveAt(i);
          lines.RemoveAt(i);
          i--;
        }
      }

      var modsToAdd = ModsToActivate.Where(m => !currentModsInList.Contains(m)).ToArray();
      int j = max + 1;
      foreach (var mod in modsToAdd) {
        currentMods.Enqueue(j, Tuple.Create(true, mod));
        j++;
      }

      int indexOfMods = lines.IndexOf("[Mods]");
      int indexOfCount = indexOfMods + 1;
      int indexToInsert = indexOfCount + 1;

      int k = 0;
      while (!currentMods.IsEmpty) {
        var mod = currentMods.DequeueValue();
        string name = mod.Item2;
        bool currentlyActive = mod.Item1;
        bool shouldBeActive = (currentlyActive && !ModsToDeactivate.Contains(name)) || ModsToActivate.Contains(name);
        lines.Insert(indexToInsert++, "ModName{0}=Mods\\{1}".With(k, name));
        lines.Insert(indexToInsert++, "ModActive{0}={1}".With(k, shouldBeActive ? 1 : 0));
        k++;
      }

      lines[indexOfCount] = "Count={0}".With(k);
    }

    void SortModsOld(List<string> lines, HashSet<string> ModsToActivate, HashSet<string> ModsToDeactivate) {
      var activatedMods = new HashSet<string>();
      int max = 0;
      int insertNewMods = 0;
      int indexOfCount = 0;

      for (int i = 0; i < lines.Count; i++) {
        string line = lines[i];
        if (line.StartsWith("ModName")) {
          int number = int.Parse(line.Substring("ModName".Length, line.IndexOf('=') - "ModName".Length));
          max = Math.Max(max, number);

          string end = line.Split('\\').Last();
          if (ModsToActivate.Contains(end)) {
            lines[i + 1] = lines[i + 1].Substring(0, lines[i + 1].Length - 1) + "1";
            activatedMods.Add(end);
          } else if (ModsToDeactivate.Contains(end)) {
            lines[i + 1] = lines[i + 1].Substring(0, lines[i + 1].Length - 1) + "0";
          }
        } else if (line.StartsWith("ModActive")) {
          insertNewMods = i + 1;
        } else if (line.StartsWith("Count")) {
          indexOfCount = i;
        }
      }

      int modID = max + 1;
      var modsToAdd = ModsToActivate.Where(m => !activatedMods.Contains(m)).ToArray();
      if (insertNewMods == 0)
        insertNewMods = indexOfCount + 1;
      foreach (var mod in modsToAdd) {
        lines.Insert(insertNewMods++, "ModName{0}=Mods\\{1}".With(modID, mod));
        lines.Insert(insertNewMods++, "ModActive{0}=1".With(modID));
        modID++;
      }
      lines[indexOfCount] = "Count={0}".With(modID);
    }

    public void FixIniFile() {
      try {
        if (File.Exists(HostListFile)) {
          TrimHostList();
        }
      } catch (Exception e) {
        GcmLauncher.LogFile.Write(e.ToString());
      }

      try {
        if (File.Exists(IniFile)) {
          EnableGcmMods();
        }
      } catch (Exception e) {
        GcmLauncher.LogFile.Write(e.ToString());
        MessageBox.Show("There was a problem activating your mods, you may need to do it manually.");
      }
    }

    public void AutoDisableBugles() {
      if (!Gcm.Var.Bool["disabled_bugles", false]) {
        Gcm.Var.Bool["disabled_bugles"] = true;
        DisableBugles(silent: true);
      }
    }

    public void InstallGcmHotkeys() {
      string keyboardFile = "keyboard.csv";
      string src = Gcm.Data.GcmKeyboardFile();
      string dest = Path.Combine(this.WorkDirectory, keyboardFile);
      string backup = dest + ".before_gcm";
      try {
        if (File.Exists(backup))
          Utilities.Backup.BackupFile(backup, 10);
        File.Move(dest, backup);
        File.Copy(src, dest);
        MessageBox.Show("GCM Keyboard file successfully installed.");
        System.Diagnostics.Process.Start(Gcm.Data.GcmKeyboardGuideFile());
      } catch {
        MessageBox.Show("Unable to install GCM Keyboard file.  You may need administrator rights to do this.");
      }
    }

    public void CopyRelevantMpLogToNewFile(string mpLogFile, string newFile) {
      long fileSize = new FileInfo(mpLogFile).Length;
      string[] relevantLines;
      if (fileSize > 20000000) {
        relevantLines = GetRelevantMpLog_Wrapped(mpLogFile);
      } else if (fileSize < 20000) {
        relevantLines = File.ReadAllLines(mpLogFile);
      } else {
        relevantLines = GetRelevantMpLog_Medium(mpLogFile);
      }
      File.WriteAllLines(newFile, relevantLines);
    }
    public string[] GetRelevantMpLog_Medium(string mpLogFile) {
      var lines = File.ReadAllLines(mpLogFile);
      if (lines.Length < NumLinesToTake) {
        return lines;
      } else {
        return lines.Take(HalfLinesToTake).Concat("##########".AsEnumerable()).Concat(lines.Skip(lines.Length - HalfLinesToTake)).ToArray();
      }
    }
    const int StartTime = 13608000;
    const int NumLinesToTake = 300;
    const int HalfLinesToTake = NumLinesToTake / 2;
    public string[] GetRelevantMpLog_Wrapped(string mpLogFile) {
      var lines = File.ReadAllLines(mpLogFile);
      int k = 0;
      Func<string, int> GetTime = s => { 
        int val = 0;
        if(s.Length > 9 && int.TryParse(s.Substring(0, 8), out val))
          return val;
        return 0;
      };
      int time = 0;
      foreach (var line in lines.Skip(k)) {
        k++;
        int curTime = GetTime(line);
        if (curTime != 0 && curTime != StartTime) {
          time = curTime;
          break;
        }
      }

      foreach (var line in lines.Skip(k)) {
        k++;
        int curTime = GetTime(line);
        // If time went down, we found the divider
        if (curTime == 0 || curTime < time) {
          int begin = Math.Max(0, k - HalfLinesToTake);
          int end = Math.Min(lines.Length - 2, k + HalfLinesToTake);
          return lines.Skip(begin).Take(end - begin).ToArray();
        }
      }

      // Of if somehow the above logic failed, just return the beginning and end of the log.
      return GetRelevantMpLog_Medium(mpLogFile);
    }
  }

}
