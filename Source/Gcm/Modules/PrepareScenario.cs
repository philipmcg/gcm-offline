using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using Utilities.Windows.SimpleForms;
using Launcher.Forms;
using GcmShared;
using Utilities;
using System.IO;
using System.Drawing;


namespace Launcher.Modules {
  class PrepareScenario : GcmFormsModule {

    // Sets the options in the main variable bin for a 1v1 competitive game
    void SetStandardOptions(int players) {
      Dictionary<string, object> options = new Dictionary<string, object>();
      options["supply_unit"] = "medium";
      options["supply"] = "by_division";
      options["objective_holders"] = "holders_wagons";
      options["objective_men_to_hold"] = 0;
      options["objective_point_percentages"] = 40;
      options["objective_radius"] = 100;
      options["objective_distance_from_edge"] = "20";
      options["objective_area"] = 80;
      options["objective_length"] = 0;
      options["start_location_spread"] = Rand.Int(1, 5) * 10;

      foreach (var item in options)
        Gcm.Var.Str["opt_s_" + item.Key] = item.Value.ToString();
    }

    void ChooseGameType(Battle battle, BattleTypeChoices defaultChoice = BattleTypeChoices.None) {
      BattleTypeChoices choice = BattleTypeChoices.Custom;
      Gcm.Var.Int["battle_type_choice"] = (int)choice;
    }

    void SetDefaultOptions() {
      var OptionList = new OptionList(Gcm.Data.GCSVs["game_options"], Gcm.Data.GCSVs.AsCollection(), Gcm.Data.VariableBin, s => true);
      OptionList.SetDefaultVariablesIfEmpty();
    }

    public override void DoYourJob() {
      DoYourJob(1);
    }
    public void DoYourJob(int battleID, BattleTypes type = BattleTypes.Normal, int playerSide = Factions.USA, bool autoYes = false) {
      SetStatus("Creating Battle");
      string scenario = ChooseScenario();
      if (Quit) return;



      int numPlayers;
      while (true) {
        TextForm form = new TextForm();
        form.ActionOnShown = f => { f.Text = "Choose number of players"; f.Label.Text = "Number of Players:"; f.TextBox.Text = Gcm.Var.Str["opt_num_players_offline", "8"]; f.TextBox.Focus();  f.TextBox.SelectAll(); };
        if (form.ShowDialog() == DialogResult.OK ) {
          if (int.TryParse(form.TextBox.Text, out numPlayers) && numPlayers >= 2) {
            Gcm.Var.Str["opt_num_players_offline"] = numPlayers.ToString();
            GcmLauncher.Data.SaveVariables();
            break;
          }
        } else {
          QuitOperation("Cancelled");
          return;
        }
      }

      Battle battle = CreateBattle(battleID, numPlayers);
      battle.BattleType = type;
      battle.PlayerSide = playerSide;
      battle.BalanceInfo.Team2ArtilleryPercentage = 100;
      battle.BalanceInfo.Team2InfantryPercentage = 100;
      ApplyScenario(battle, scenario);

      SetStatus("Choose game type");
      ChooseGameType(battle, autoYes ? (BattleTypeChoices)Gcm.Var.Int["battle_type_choice", 0] : BattleTypeChoices.None);
      if (Quit) return;
      if (!autoYes && battle.Flags.Contains("custom_settings")) {
        SetStatus("Choose game settings");
        ShowChooseGameOptionsDialog(battle);
        if (Quit) return;
      }

      if (type == BattleTypes.SingleplayerCustomCampaign) // If user chose random divisions for a campaign battle, ignore them.
        Gcm.Var.Str["opt_s_gametype"] = "campaign";

      SetDefaultOptions();

      ApplyGameOptions(battle, autoYes);

      battle.Divisions.ForEach(d => d.DivisionXmlPath = Paths.Local.DivisionFile(d.DivisionID));
      if (Quit) return;

      DirectoryEx.EnsureDirectory(Paths.Local.BattleDir(battle.BattleID));

      Gcm.Data.VariableBin.SaveToFileByPrefix(Paths.Local.BattleOptions(battle.BattleID), "opt_s_");

      SetStatus("Preparing armies");

      var output = new RandomDivisionsOutput();
      SetArmyBalance(battle.Divisions, output, battle);
      battle.BalanceInfo = output.BalanceInfo;
      battle.ArmyCommanders = output.ArmyCommanders;
      battle.MenGunRatio = output.MenGunRatio;

      if (Quit) return;

      GcmShared.NewMilitary.BattleMaker battleMaker;

      if (battle.OOBType == OOBType.PersistentDivisions)
        battleMaker = Gcm.Provider.Get<GcmShared.NewMilitary.PersistentBattleMaker>();
      else if (battle.OOBType == OOBType.RandomDivisions)
        battleMaker = Gcm.Provider.Get<GcmShared.NewMilitary.RandomBattleMaker>();
      else if (battle.OOBType == OOBType.Historical)
        battleMaker = Gcm.Provider.Get<GcmShared.NewMilitary.HistoricalBattleMaker>();
      else
        throw new NotImplementedException("Invalid OOB Type");

      SetStatus("Creating scenario");
      battleMaker.MakeBattle(battle, s => SetStatusWithoutLog(s));

      SetStatus("Installing scenario");
      DateTime date = DateTime.UtcNow;
      string scenarioDirectory = GcmLauncher.Helpers.SOW.GetBattleDirectory(date);
      DirectoryEx.EnsureDirectory(scenarioDirectory);
      CopyScenarioToFolder(Files.TempDir, scenarioDirectory);
      CopyScenarioToLatestScenarioFolder(date, scenarioDirectory);

      Gcm.Var.Int["opt_last_battle_players_hash"] = GcmShared.Hashes.HashPlayerList(battle.Divisions.Select(d => d.PlayerID));
      GcmLauncher.LogFile.Write("Players: {0} -> {1}".With(string.Join(",", battle.Divisions.Select(d => d.PlayerID).OrderBy(p => p)), Gcm.Var.Int["opt_last_battle_players_hash"]));
      SetStatus("Installed scenario");
    }

    void SetArmyBalance(List<Division> divisions, RandomDivisionsOutput output, Battle battle) {
      BalanceSides bs = new BalanceSides(divisions, divisions.GroupBy(d => d.FactionID).First().Sum(d => d.RD_Men), output, battle.BattleType != BattleTypes.Competitive1v1 ? BalanceSidesOptions.AllowImbalance : BalanceSidesOptions.Default);

      if (bs.ShowDialog() == DialogResult.OK) {
        // apply side multipliers to the random division numbers
        divisions.Where(d => d.FactionID == 2).ForEach(d => { d.RD_Men = (int)(d.RD_Men * output.BalanceInfo.Team2InfantryMultiplier); d.RD_Guns = (int)(d.RD_Guns * output.BalanceInfo.Team2ArtilleryMultiplier); });
        return;
      } else {
        QuitOperation("Cancelled");
        return;
      }
    }

    new int ChooseGame(List<int> games) {
      games.Sort();
      games.Reverse();

      GameListForm form = new GameListForm("Select Battle", "Launch");
      form.SetGameList(games.Select(i => i.ToString()));
      form.ShowDialog();
      if (form.DialogResult != DialogResult.OK) {
        QuitOperation("Cancelled");
        return -1;
      }

      int gameID = int.Parse(form.SelectedGame);
      return gameID;
    }

    Battle CreateBattle(int battleID, int numPlayers) {
      Log.Write("GetListOfPlayersForBattle");
      // var divisions = GcmLauncher.Web.RequestStruct<List<Division>, GetListOfPlayersModel>(Controllers.Battles, "GetListOfPlayersForBattle", new GetListOfPlayersModel() { BattleID = battleID, IsRanked = false });
      int divisionID = 0;
      int playerID = 0;
      List<Division> divisions = new List<Division>();
      var factions = new[]{Factions.USA, Factions.CSA};
      int[] playersOnTeam = new[] { numPlayers / 2, numPlayers / 2 + (numPlayers % 2) };
      playersOnTeam.Shuffle();
      for (int i = 0; i < 2; i++)
			{
        int factionID = factions[i];
      for (int d = 0; d < playersOnTeam[i]; d++) {
        var characterName = RandomCreator.Instance.CreateCharacterName(factionID);
        var div = new Division() {
          CharacterName = characterName,
          DivisionID = divisionID++,
          PlayerID = playerID++,
          FactionID = Factions.USA,
          RD_Men = 4000,
          RD_Men_Preference = 4000,
          RD_Guns = 12,
          RD_Guns_Preference = 12,
          PrefHighCommand = 0,
          PrefCavalry = 0,
          LastRealBattleKey = Guid.NewGuid(),
          Side = factionID,
          PlayerRank = 10,
          PlayerLevel = 10,
          FilesVersion = Guid.Empty,
          UserID = Guid.NewGuid(),
          UserName = characterName.Last,
        };
        divisions.Add(div);
      }
			}

      BattleSetup setup = new BattleSetup() {
        AvgMinutesOnMap = null,
        BattleID = 100,
        Divisions = divisions,
      };

      Log.Write("Setup Battle");

      Battle battle = new Battle();
      battle.Setup = setup;
      battle.HostUsername = GcmLauncher.Auth.Username;
      battle.HostPlayerID = GcmLauncher.Auth.PlayerID;
      battle.GcmVersion = "offline";
      battle.Divisions = divisions;
      battle.Sides.ForEach(s => s.SetDivisions());
      battle.GameType = GameTypes.Normal;
      battle.BattleID = battleID;

      int count = divisions.Count;
      if (count < 2)
        count = 2;
      else if (count > 32)
        count = 32;
      string countstr = count.ToString();
      Gcm.Data.GCSVs["game_options"]["opt_s_map_width"]["default"] = Gcm.Data.GCSVs["defaults"][countstr]["map_size"];
      Gcm.Data.GCSVs["game_options"]["opt_s_time_limit"]["default"] = Gcm.Data.GCSVs["defaults"][countstr]["time"];
      Gcm.Data.GCSVs["game_options"]["opt_s_num_objectives"]["default"] = Gcm.Data.GCSVs["defaults"][countstr]["objectives"];

      return battle;
    }

    string ChooseScenario() {
      /*string scenario;
      var m_chooseScenario = new ChooseScenario();
      if (m_chooseScenario.ShowDialog() != DialogResult.OK)
      {
          QuitOperation("Cancelled");
          return null;
      }
      scenario = m_chooseScenario.ScenarioChoice;*/

      string scenario = "Custom Objectives";
      Gcm.Var.Str["opt_last_scenario"] = scenario;

      return scenario;
    }

    void ApplyScenario(Battle battle, string scenario) {
      if (scenario == "Random") {
        battle.Flags.Add("random");
        Gcm.Var.Str["opt_s_objective_distribution"] = "even";
        Gcm.Var.Str["opt_s_objective_time_buffer"] = "40";
      } else if (scenario == "Custom Objectives") {
        battle.Flags.Add("custom_obj");
        battle.Flags.Add("custom_objective_settings");
        battle.Flags.Add("random");
      }
    }

    bool AllHaveSameVersion(IEnumerable<Division> divisions) {
      Log.Write("Check Player Versions");
      var current = divisions.First().FilesVersion;
      return divisions.All(d => d.FilesVersion == current);
    }

    void ShowChooseGameOptionsDialog(Battle battle) {
      var dialog = new SetupGame();
      dialog.Initialize(battle.Flags, AllHaveSameVersion(battle.Divisions), battle.Setup);
      var result = dialog.ShowDialog();

      if (result != DialogResult.OK) {
        QuitOperation("Cancelled");
        return;
      }

      GcmLauncher.Data.SaveVariables();
    }

    bool MapExistsInMod(string modname, string mapname) {
      string modfolder = Path.Combine(GcmLauncher.SOWDirectory, "Mods", modname, "Maps");
      return GcmLauncher.HasSOWDirectory &&
          Directory.Exists(modfolder) &&
          File.Exists(Path.Combine(modfolder, mapname + ".lsl"));
    }

    void ApplyGameOptions(Battle battle, bool autoYes) {
      battle.CavalryAllowed = battle.GetIntegerOption("cavalry_regts_per_side") > 0;
      battle.LengthOfBattleInMinutes = Gcm.Var.Str["opt_s_time_limit"].ToInt();

      battle.NumObjectives = Gcm.Var.Str["opt_s_num_objectives"].ToInt();

      if (Gcm.Var.Str["opt_s_map"] == "Random")
        Gcm.Var.Str["opt_s_map"] = Gcm.Data.Lists["other\\random_maps"].GetRandom();

      if (battle.Divisions.Count < 4 && battle.BattleType != BattleTypes.Competitive1v1)
        Gcm.Var.Str["opt_s_ranked"] = "0";

      battle.Map = Gcm.Var.Str["opt_s_map"];
      battle.Ranked = Gcm.Var.Str["opt_s_ranked"] == "1";
      battle.MapDbName = Gcm.Data.GCSVs["maps"][battle.Map]["dbname"];
      battle.SupplyType = Battle.GetSupplyTypeByID(Gcm.Var.Str["opt_s_supply"]);
      battle.OOBType = Battle.GetOOBTypeByID(Gcm.Var.Str["opt_s_gametype"]);

      if (battle.GameType == GameTypes.Normal) {
        if (Gcm.Var.Str["opt_s_map_width"] == "custom") {
          if (!CropMapArea(battle, autoYes)) {
            QuitOperation("Cancelled");
            return;
          }
        } else {
          battle.MapSize = new MapSize(battle.Map);
        }
      }
    }

    bool CropMapArea(Battle battle, bool autoYes) {
      DirectoryEx.EnsureDirectory(Paths.Local.MinimapsDir());
      string minimap = Gcm.Data.GCSVs["maps"][battle.Map]["minimap"];
      string minimapPath = Paths.Local.Minimap(minimap);
      if (!File.Exists(minimapPath)) {
        try {
          Web.DownloadRequestToFile("http://www.sow.philipmcg.com/images/mm/large/{0}_MM.png".With(minimap), minimapPath);
        } catch {
          minimapPath = Paths.Local.NoMinimap();
        }
      }

      using (Bitmap image = new Bitmap(minimapPath)) {
        if (autoYes) {
          battle.MapSize = new MapSize(battle.Map, image.Size, LoadPoint(minimap + "tl"), LoadPoint(minimap + "br"));
          return true;
        }

        CropMap dialog = new CropMap();
        dialog.ShowImage(image);
        dialog.Text = "Click and Right-Click to set the map boundaries";

        if (LoadPoint(minimap + "tl") != Point.Empty)
          dialog.TopLeft = LoadPoint(minimap + "tl");
        else
          dialog.TopLeft = new Point(10, 10);

        if (LoadPoint(minimap + "br") != Point.Empty)
          dialog.BottomRight = LoadPoint(minimap + "br");
        else
          dialog.BottomRight = new Point(image.Width - 10, image.Height - 10);

        DialogResult result;
        int x = 0;
        int y = 0;

        while (true) {
          result = dialog.ShowDialog();

          if (result != DialogResult.OK)
            return false;

          x = dialog.BottomRight.X - dialog.TopLeft.X;
          y = dialog.BottomRight.Y - dialog.TopLeft.Y;

          if (x < image.Width / 10 || y < image.Height / 10)
            MessageBox.Show("Your map area is too small, it needs to be bigger.");
          else if (x * 2 < y || y * 2 < x)
            MessageBox.Show("Your map area is too narrow, it must be more like a square.");
          else
            break;
        }

        SavePoint(minimap + "tl", dialog.TopLeft);
        SavePoint(minimap + "br", dialog.BottomRight);

        Point tl = new Point(dialog.TopLeft.Y, dialog.TopLeft.X);
        Point br = new Point(dialog.BottomRight.Y, dialog.BottomRight.X);
        battle.MapSize = new MapSize(battle.Map, image.Size, tl, br);
      }

      return true;
    }

    void SavePoint(string name, Point pt) {
      Gcm.Var.Int["opt_pt_{0}_x".With(name)] = pt.X;
      Gcm.Var.Int["opt_pt_{0}_y".With(name)] = pt.Y;
    }

    Point LoadPoint(string name) {
      if (Gcm.Var.Int.ContainsKey("opt_pt_{0}_x".With(name)) && Gcm.Var.Int.ContainsKey("opt_pt_{0}_y".With(name)))
        return new Point(Gcm.Var.Int["opt_pt_{0}_x".With(name)], Gcm.Var.Int["opt_pt_{0}_y".With(name)]);
      else
        return Point.Empty;
    }

    void CopyScenarioToFolder(string temp, string dest) {
      DestinationDir = temp;
      CopyScnFile("oob", dest);
      CopyScnFile("maplocs", dest);
      CopyScnFile("intro", dest);
      CopyScnFile("screen", dest);
      CopyScnFile("script", dest);
      CopyScnFile("ini", dest);
    }

    void CopyScenarioToLatestScenarioFolder(DateTime date, string scenarioDir) {
      foreach (var dir in GcmLauncher.Helpers.SOW.GetOldLatestBattles()) {
        try {
          DirectoryEx.DeleteContents(dir);
          Directory.Delete(dir);
        } catch { }
      }
      string latestScenarioDir = GcmLauncher.Helpers.SOW.GetLatestBattleDirectory(date);
      DirectoryEx.EnsureDirectory(latestScenarioDir);
      DirectoryEx.CopyContents(scenarioDir, latestScenarioDir);
    }
    /// <summary>
    /// The folder where all the scenario files will end up
    /// </summary>
    static string DestinationDir;
    string ScnFilePath(string id) {
      return Path.Combine(DestinationDir, Gcm.Var.Str["scn_" + id]);
    }
    void CopyScnFile(string id, string dest) {
      string d = Path.Combine(dest, Gcm.Var.Str["scn_" + id]);
      if (File.Exists(d))
        File.Delete(d);

      File.Copy(ScnFilePath(id), d);
    }
  }
}
