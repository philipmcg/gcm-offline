using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using Utilities;

namespace GcmShared {
  public class Paths {
    public static OnlinePaths Online { get; private set; }
    public static LocalPaths Local { get; private set; }

    static Paths() {
      Online = new OnlinePaths();
      Local = new LocalPaths();

      Online.GetPath = p => p;
      Local.GetPath = p => p;
    }
  }

  public class OnlinePaths {
    public Func<string, string> GetPath { get; set; }
    protected string Path(string path, params object[] args) {
      return GetPath(string.Format(path, args));
    }


    public string WabashBackupZip(DateTime date) {
      return Path("gcm\\backup\\wabash{0}.zip", date.ToFileNameString());
    }

    public string GcmVariables() {
      return Path("gcm\\wabash\\variables.ini");
    }

    public string WabashDir() {
      return Path("gcm\\wabash\\");
    }

    public string RegimentListFile(int systemID) {
      return Path("gcm\\cache\\{0}\\regimentlist.csv", systemID);
    }

    public string DataZipFile() {
      return Path("gcm\\data.zip");
    }

    public string GcmDir() {
      return Path("gcm\\");
    }

    public string DivisionDir(int divisionID) {
      return Path("gcm\\wabash\\divisions\\{0}\\", divisionID);
    }

    public string DivisionFile(int divisionID) {
      return Path("gcm\\wabash\\divisions\\{0}\\division_{0}.xml", divisionID);
    }

    public string TempDivisionFile(int battleID, int divisionID) {
      return Path("gcm\\wabash\\battles\\{0}\\battle_{0}_temp_division_{1}.xml", battleID, divisionID);
    }

    public string ChainTagMap(int battleID) {
      return Path("gcm\\wabash\\battles\\{0}\\chain_tag_map_{0}.xml", battleID);
    }

    public string MapInfo(int battleID) {
      return Path("gcm\\wabash\\battles\\{0}\\mapinfo_{0}.xml", battleID);
    }

    public string BattleDir(int battleID) {
      return Path("gcm\\wabash\\battles\\{0}\\", battleID);
    }

    public string BattleOptions(int battleID) {
      return Path("gcm\\wabash\\battles\\{0}\\options_{0}.ini", battleID);
    }

  }

  public class LocalPaths : OnlinePaths {
    public string TempTxtFile() {
      return Path("temp.txt");
    }
    public string TempBattleCreationLog(int battleID) {
      return Path("{0}\\{1}.log", Files.TempDir, battleID);
    }

    public string TempBattleOptions(int battleID) {
      return Path("{0}\\battle_{1}_options.ini", Files.TempDir, battleID);
    }

    public string TempZipFile() {
      return Path("temp.zip");
    }

    public string ScreenshotFile(string hash) {
      return Path("screenshots\\{0}.jpg", hash);
    }

    public string NoMinimap() {
      return Path("Data\\Other\\nominimap.png");
    }

    public string Minimap(string minimap) {
      return Path("minimaps\\{0}.png", minimap);
    }

    public string MinimapsDir() {
      return Path("minimaps");
    }

    public string RandomDivisionFile(int battleID, int divisionID) {
      return Path("gcm\\wabash\\random_divisions\\battle_{0}_random_division_{1}.xml", battleID, divisionID);
    }

    public string CampaignMusic() {
      return Path("music\\b.mp3");
    }
    public string TempLogs() {
      return Path("temp\\logs\\");
    }
    public string TempLogsZip(int battleID) {
      return Path("temp\\logs\\logs_{0}.zip", battleID);
    }
    public string TempLogsMpLog(int battleID) {
      return Path("temp\\logs\\sowmp_log_{0}.log", battleID);
    }
    public string TempLogsIni(int battleID) {
      return Path("temp\\logs\\sowgb_{0}.ini", battleID);
    }
    public string TempLogsSowgbLog(int battleID) {
      return Path("temp\\logs\\sowgb_{0}.log", battleID);
    }
    public string TempLogsSowgbGamedb(int battleID) {
      return Path("temp\\logs\\gamedb_{0}.csv", battleID);
    }
    public string TempLogsScenarioMd5s(int battleID) {
      return Path("temp\\logs\\scenario_md5s_{0}.csv", battleID);
    }
  }


  public class Files {
    public static string TempDir;

    public static string DivTurnReport(object player, object turn, bool online) {
      string file = string.Format("report_player_{0}_turn_{1}.xml", player, turn);
      if (online)
        return Path.Combine(PlayerFolder(player), file);
      else
        return Path.Combine(TempDir, "Reports\\" + file);
    }


    public static string ScenarioObjectives(string scenario, bool online) {
      string file = string.Format(scenario + ".csv");
      if (online)
        return Path.Combine(ScenarioFolder(), file);
      else
        return Path.Combine(TempDir, file);
    }
    public static string CampaignScenario(string campaign, string scenario, bool online) {
      string file = string.Format(scenario + ".csv");
      if (online)
        return Path.Combine(CampaignScenarioFolder(campaign), file);
      else
        return Path.Combine(TempDir, file);
    }

    public static string Scenario(int battle, bool online) {
      string file = string.Format("scenario.csv", battle);
      if (online)
        return Path.Combine(BattleFolder(battle), file);
      else
        return Path.Combine(TempDir, file);
    }
    public static string Campaign(int campaign, bool online) {
      string file = string.Format("saved_campaign_{0}.csv", campaign);
      if (online)
        return Path.Combine(CampainFolder(campaign), file);
      else
        return Path.Combine(TempDir, file);
    }
    public static string CampaignMoves(int campaign, int side, int turn, bool online) {
      string file = string.Format("moves_{0}_side_{1}_turn_{2}.csv", campaign, side, turn);
      if (online)
        return Path.Combine(CampainFolder(campaign), file);
      else
        return Path.Combine(TempDir, file);
    }
    public static string MapLocations(int battle, bool online) {
      string file = string.Format("maplocations.csv", battle);
      if (online)
        return Path.Combine(BattleFolder(battle), file);
      else
        return Path.Combine(TempDir, file);
    }

    public static string ScenarioFolder() {
      return string.Format("scenarios/");
    }
    public static string CampaignScenarioFolder(string campaign) {
      return string.Format("campaign_setups/" + campaign);
    }
    public static string BattleFolder(object battle) {
      return string.Format("battles/{0}/", battle);
    }
    public static string PlayerFolder(object player) {
      return string.Format("players/{0}/", player);
    }
    public static string CampainFolder(object campaign) {
      return string.Format("campaigns/{0}/", campaign);
    }
  }
}
