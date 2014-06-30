using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;
using Utilities.GCSV;
using System.Drawing;

namespace GcmShared {
  public class ReplayProcessor {
    string Version;
    string Scenario;
    string Map;
    string Minimap;
    long StartTime;
    string Line5;
    string HostName;
    int BattleID; // not always present!
    List<Point> ObjectiveLocations;
    bool IsSingleplayer = false;
    GCSVTable Units;
    GCSVTable ScenarioInfo;
    GCSVTable players;
    List<GCSVTable> frames;
    List<GCSVTable> frameInfoes;
    Dictionary<int, int> ID_Mapping;
    UnitState[] UnitStates;
    
    // Replay reading state
    string[] lines;
    int kk;
    GcmShared.GcmDataManager data;

    int MapSize = 32768;
    const int MapSizeScalar = 8;
    const int ReplayScale = 1000;

    List<Player> Players;
    Dictionary<int, Player> PlayersDict;
    Dictionary<string, Player> CommandsToPlayers;

    CommandNode Root;

    class CommandNode {
      public Player Player;
      public ArraySet<CommandNode> Children;
      public CommandNode Parent;

      public CommandNode() {
        Children = new ArraySet<CommandNode>();
      }

      public CommandNode MakeChild(int key) {
        if (!Children.ContainsKey(key)) {
          Children.Insert(key, new CommandNode());
          Children[key].Parent = this;
        }
        return Children[key];
      }
      public CommandNode this[int key] {
        get {
          return MakeChild(key);
        }
      }
      public Player GetPlayer() {
        CommandNode node = this;
        while (node.Player == null && node.Parent != null) {
          node = node.Parent;
        }
        return node.Player;
      }
    }

    CommandNode GetCommandNodeAt(int[] command) {
      var node = Root;
      int k = 0;
      while (k < command.Length && command[k] != 0) {
        node = node[command[k]];
        k++;
      }
      return node;
    }

    void InsertPlayer(Player player) {
      var node = GetCommandNodeAt(player.Command);
      node.Player = player;
    }
    void CreateCommandStructure() {
      Root = new CommandNode();

      foreach (var player in Players) {
        InsertPlayer(player);
      }
    }

    class Player {
      public readonly int ID;
      public readonly int[] Command;
      public readonly string CommandString;
      public int UnitID;
      public readonly string Name;
      public Player(int id, string commandstring, string name) {
        ID = id;
        CommandString = commandstring;
        Command = CommandString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.ToInt()).ToArray();
        Name = name;
      }
    }

    struct UnitState {
      public int M; // Men
      public int C; // Casualties
      public int I; // Inflicted
      public int S; // Score
      public int X; // X
      public int Z; // Z
      public int F; // Z
      public int MO; // Z
    }

    void ProcessPlayers(string[] lines, ref int k) {
      string playerAssignments = null;
      List<string> players = new List<string>();
      while (true) {
        string line = lines[k];
        k++;
        if (line.StartsWith("MP")) {
          playerAssignments = line;
          players.Clear();
        } else if (line[0] == '"') {
          players.Add(GcmFormats.FormatName(line));
        } else if (Char.IsNumber(line[0])) {
          players.Insert(0, HostName);
          // At this point, we have the playerAssignments string and all player names in our list in order.
          break;
        }
      }
      players.Reverse();
      playerAssignments = playerAssignments.Replace("MP", "|");
      playerAssignments = playerAssignments.Substring(0, playerAssignments.IndexOf('"'));
      var commands = playerAssignments.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim(',')).ToArray();
      int p = 0;
      foreach (var command in commands) {
        AddPlayer(p, command, players[p]);
        p++;
      }
      AddComputerArmyCommanders(ref p);
    }

    const string Army2CommandString = "2,1,0,0,0,0";
    const string Army1CommandString = "1,1,0,0,0,0";

    void SetupSingleplayerPlayers(string spstring) {
      int p = 0;
      string playerCommand = spstring.Substring(2).Trim(',');
      AddPlayer(p++, playerCommand, "Human");
      AddComputerArmyCommanders(ref p);
    }

    void AddComputerArmyCommanders(ref int p) {
      if (!CommandsToPlayers.ContainsKey(Army1CommandString)) {
        AddPlayer(p++, Army1CommandString, "Computer");
      }
      if (!CommandsToPlayers.ContainsKey(Army2CommandString)) {
        AddPlayer(p++, Army2CommandString, "Computer");
      }
    }

    void AddPlayer(int id, string command, string name) {
        Player player = new Player(id, command, name);
        Players.Add(player);
        PlayersDict.Add(id, player);
        CommandsToPlayers.Add(command, player);
    }

    public static class FrameUnitHeader {
      public const int Time = 0;
      public const int Side = 1;
      public const int Army = 2;
      public const int Corp = 3;
      public const int Div = 4;
      public const int Brig = 5;
      public const int Reg = 6;
      public const int ID = 7;
      public const int Unused1 = 8;
      public const int Ammo = 9;
      public const int FacingX = 10;
      public const int FacingZ = 11;
      public const int X = 12;
      public const int Z = 13;
      public const int Unused2 = 14;
      public const int Men = 15;
      public const int Fatigue = 16;
      public const int Morale = 17;
      public const int Score = 18;
      public const int Casualties = 19;
      public const int Inflicted = 20;
      public const int Formation = 21;
    }

   private ReplayProcessor ()
   {
    }

    // Reads the part of the replay with map, scenario and players info.
    void ProcessHeader() {
      
      Version = lines[0];
      Scenario = lines[1];
      Map = lines[2];
      Minimap = Map;
      StartTime = long.Parse(lines[3]) / 360;
      Line5 = lines[4];
      if (lines[5].StartsWith("SP,")) {
        HostName = "Human";
        IsSingleplayer = true;
      } else {
        HostName = lines[5].Split(',').Last();
        HostName = GcmFormats.FormatName(HostName);
      }

      if (Map.Contains("_RandomVariant")) {
        Map = Map.Substring(0, Map.IndexOf("_RandomVariant"));
      }
      if (data.GCSVs["maps"].ContainsKey(Map)) {
        var mapinfo = data.GCSVs["maps"][Map];
        MapSize = int.Parse(mapinfo["multiplier"]);
        Minimap = mapinfo["minimap"];
      }

      ScenarioInfo = new GCSVTable("scenario", new GCSVHeader(new[] { "minimap", "map", "starttime", "host" }));
      ScenarioInfo.Add(new GCSVLine(ScenarioInfo.Header, new[] { Minimap, Map, StartTime.ToString(), HostName }));

      kk = 6;
      Players = new List<Player>();
      PlayersDict = new Dictionary<int, Player>();
      CommandsToPlayers = new Dictionary<string, Player>();
      if (IsSingleplayer) {
        SetupSingleplayerPlayers(lines[5]);
      } else {
        ProcessPlayers(lines, ref kk);
      }
      CreateCommandStructure();
      // Now k points to the next line after the players list.
      kk++;
      // Now k points to the first objective line, if there are any.
      while (lines[kk].Contains("0,0,0,0,0,0,0,OBJ")) {
        kk++;
      }

    }

    int ToIntSafe(string str) {
      int val;
      double vald;
      if (int.TryParse(str, out val)) {
        return val;
      } else if (double.TryParse(str, out vald)) {
        return (int)vald;
      } else return 0;
    }

    // Processes the part of the replay with unit definitions (none of the actual frames yet)
    void ProcessUnits() {
      string UnitDefsTime = lines[kk].Split(',').First();
      int passed;
      Units = GetUnits(lines.Skip(kk).Where(l => l.StartsWith(UnitDefsTime)), out passed);
      UnitStates = new UnitState[Units.Count];
      kk += passed;
      players = GetPlayers();
    }
      // id x z men casualties inflicted score fatigue morale
      string[] frameHeaderFields = { "id", "x", "z", "m", "c", "i", "s", "f", "mo" };
      string[] frameInfoHeaderFields = { "s1", "c1", "k1", "s2", "c2", "k2", "time" };
      GCSVHeader frameUnitHeader = new GCSVHeader("Time,Side,Army,Corp,Div,Brig,Reg,id,unused1,ammo,facingx,facingz,x,z,unused2,men,fatigue,morale,score,casualties,inflicted,formation".Split(','));
      GCSVHeader frameObjHeader = new GCSVHeader("Time,Side,Army,Corp,Div,Brig,Reg,Controller,name,unused1,unused2,x,z".Split(','));
    void ProcessFullFrames() {
      int k = kk;
      // Now k points to the first unit in the units list.

      int frameID = 0;
      frames = new List<GCSVTable>();
      frameInfoes = new List<GCSVTable>();
      while (k < lines.Length) {
        string frameTime = lines[k].Substring(0, lines[k].IndexOf(','));
        GCSVTable frame = GCSVMain.Create("frame" + frameID, frameHeaderFields);
        GCSVTable frameInfoTable = GCSVMain.Create("frameinfo" + frameID, frameInfoHeaderFields);
        GCSVLine frameInfo = new GCSVLine(frameInfoTable.Header);
        frameInfo["time"] = (long.Parse(frameTime) / 360 - StartTime).ToString();
        List<GCSVLine> objectives = new List<GCSVLine>();
        while (k < lines.Length && lines[k].StartsWith(frameTime)) {
          var s = lines[k].Split(',');
          if (s[8] == "OBJ") {
            var obj = new GCSVLine(frame.Header);
            obj["x"] = Scale(s[frameObjHeader["x"]]);
            obj["z"] = Scale(s[frameObjHeader["z"]]);
            obj["id"] = "obj" + s[frameObjHeader["Controller"]];
            objectives.Add(obj);
          } else if (ID_Mapping.ContainsKey(s[frameUnitHeader["id"]].ToInt())) {
            var unit = new GCSVLine(frame.Header);
            int id = ID_Mapping[s[FrameUnitHeader.ID].ToInt()];
            string x = Scale(s[FrameUnitHeader.X]);
            string z = Scale(s[FrameUnitHeader.Z]);
            int diff = 0;
            diff = ToIntSafe(x) - UnitStates[id].X; UnitStates[id].X += diff; unit["x"] = Relevant(diff);
            diff = ToIntSafe(z) - UnitStates[id].Z; UnitStates[id].Z += diff; unit["z"] = Relevant(diff);
            diff = ToIntSafe(s[FrameUnitHeader.Men]) - UnitStates[id].M; UnitStates[id].M += diff; unit["m"] = Relevant(diff);
            diff = ToIntSafe(s[FrameUnitHeader.Casualties]) - UnitStates[id].C; UnitStates[id].C += diff; unit["c"] = Relevant(diff);
            diff = ToIntSafe(s[FrameUnitHeader.Inflicted]) - UnitStates[id].I; UnitStates[id].I += diff; unit["i"] = Relevant(diff);
            diff = ToIntSafe(s[FrameUnitHeader.Score]) - UnitStates[id].S; UnitStates[id].S += diff; unit["s"] = Relevant(diff);
            diff = ToIntSafe(s[FrameUnitHeader.Fatigue]) - UnitStates[id].F; UnitStates[id].F += diff; unit["f"] = Relevant(diff);
            diff = ToIntSafe(s[FrameUnitHeader.Morale]) - UnitStates[id].MO; UnitStates[id].MO += diff; unit["mo"] = Relevant(diff);
            unit["id"] = id.ToString();
            frame.Add(unit);
          }

          if (s[3] == "0" && s[2] == "1") {
            long armyScore = (long)double.Parse(s[frameUnitHeader["score"]]);
            int kills = int.Parse(s[frameUnitHeader["inflicted"]]);
            int casualties = int.Parse(s[frameUnitHeader["casualties"]]);
            frameInfo["k" + s[frameUnitHeader["Side"]]] = kills.ToString();
            frameInfo["c" + s[frameUnitHeader["Side"]]] = casualties.ToString();
            frameInfo["s" + s[frameUnitHeader["Side"]]] = armyScore.ToString();
          }
          k++;
        }
        frame.AddRange(objectives.Take(objectives.Count - 4));
        frames.Add(frame);
        frameInfoTable.Add(frameInfo);
        frameInfoes.Add(frameInfoTable);
        frameID++;
      }
      kk = k;
    }

    void WriteCondensedReplay(string outputFile) {
      var gcsvs = new List<GCSVTable>(){
                ScenarioInfo,
                players,
                Units,
            }.Concat(frames).Concat(frameInfoes);
      GCSVMain.WriteToFile(outputFile, gcsvs);
    }

    public static BattleState GetBattleStateFromReplay(GcmShared.GcmDataManager data_, string inputFile) {
      BattleState state = new BattleState();
      state.Sides = new List<SideState>();
      ReplayProcessor proc = new ReplayProcessor();
      proc.lines = System.IO.File.ReadAllLines(inputFile);
      proc.data = data_;
      proc.ProcessHeader();
      proc.ProcessUnits();
      var armyCommanders = proc.ProcessFramesToGetArmyCommanders();
      state.Sides = new List<SideState>();
      foreach (var team in GcmShared.Factions.WabashFactions) {
        var side = new SideState();
        side.Side = team;
        var cdr = armyCommanders[team];
        side.Men = cdr[FrameUnitHeader.Men].ToInt();
        side.Score = cdr[FrameUnitHeader.Score].ToInt();
        side.Inflicted = cdr[FrameUnitHeader.Inflicted].ToInt();
        side.Casualties = cdr[FrameUnitHeader.Casualties].ToInt();
        side.Players = proc.players.Where(p => p["side"].ToInt() == team).Select(p => p["name"]).ToList();
        state.Sides.Add(side);
        state.Time = (int)(long.Parse(cdr[FrameUnitHeader.Time]) / 360 - proc.StartTime);
      };
      state.BattleID = proc.BattleID;
      state.Host = proc.HostName;
      state.Map = proc.Map;
      state.TimeRecorded = DateTime.UtcNow;
      state.IsSingleplayer = proc.IsSingleplayer;
      return state;
    }

    static int GetBattleIDFromArmyCommanderName(string name) {
      if (!name.Contains('[') || !name.Contains(']') || !name.Contains(" ["))
        return 0;
      try {

        int index = name.IndexOf('[');
        int stop = name.IndexOf(']');
        string bit = name.Substring(index + 1, stop - index - 1);
        int value = (int)Utilities.Number.Base52ToInt(bit);
        return value;
      }catch
      {
        return 0;
      }
    }

    public static void ProcessFullReplay(GcmShared.GcmDataManager data_, string inputFile, string outputFile) {
      ReplayProcessor proc = new ReplayProcessor();
      proc.lines = System.IO.File.ReadAllLines(inputFile);
      proc.data = data_;
      proc.ProcessHeader();
      proc.ProcessUnits();
      proc.ProcessFullFrames();
      proc.WriteCondensedReplay(outputFile);
    }


    Dictionary<int,string[]> ProcessFramesToGetArmyCommanders() {
      int k = kk;
      // Now k points to the first unit in the units list.
        //string frameTime = lines[k].Substring(0, lines[k].IndexOf(','));
        //frameInfo["time"] = (long.Parse(frameTime) / 360 - StartTime).ToString();
      string currentFrameTime = null;
      Queue<string> frameLines = new Queue<string>();
      int frameID = 0;

      string lastFrameTime = lines.Last().Substring(0, lines[k].IndexOf(','));
      long lastFrameSeconds = (long.Parse(lastFrameTime) / 360 - StartTime);
      var armyCommanders = new Dictionary<int,string[]>();
      foreach (var line in lines.Reverse()) {
        if(!line.StartsWith(lastFrameTime)) {
          break;
        }
        if (line.StartsWith(lastFrameTime + ",2,1,0,0,0,0,")) {
          armyCommanders.Add(2, line.Split(','));
        }
        if (line.StartsWith(lastFrameTime + ",1,1,0,0,0,0,")) {
          armyCommanders.Add(1, line.Split(','));
        }
      }
      return armyCommanders;
    }

    // Returns string value of integer, or empty string if it is 0
    string Relevant(int value) {
      return value == 0 ? "" : value.ToString();
    }

    string Scale(string coord) {
      int val;
      if (!int.TryParse(coord, out val))
        return "0";
      return (val * ReplayScale / (MapSize * MapSizeScalar)).ToString();
    }

    public GCSVTable GetPlayers() {
      var table = GCSVMain.Create("players", new[] { "id", "name", "side", "army", "corps", "div", "brig", "reg", "command", "unitid" });
      foreach (var player in Players) {
        GCSVLine p = new GCSVLine(table.Header);
        p["id"] = player.ID.ToString();
        p["name"] = player.Name;
        p["side"] = player.Command[0].ToString();
        p["army"] = player.Command[1].ToString();
        p["corps"] = player.Command[2].ToString();
        p["div"] = player.Command[3].ToString();
        p["brig"] = player.Command[4].ToString();
        p["reg"] = player.Command[5].ToString();
        p["unitid"] = player.UnitID.ToString();
        table.Add(p);
      }
      return table;
    }
    string GetCommandString(string unit) {
      int k = 0;
      int start = 0;
      int x = 0;
      while (x < unit.Length && k < 7) {
        if(unit[x] == ',') {
          k++;
          if (start == 0)
            start = x+1;
        }
        x++;
      }
      return unit.Substring(start, x - (start) - 1);
    }
    public GCSVTable GetUnits(IEnumerable<string> lines, out int passed) {
      passed = 0;
      ID_Mapping = new Dictionary<int, int>();
      var header = new GCSVHeader("Time,Side,Army,Corp,Div,Brig,Reg,id,unused1,class,name,name2,starting_men,weapon,flag,unused3,ammo,facingx,facingz,x,z,unused2,men,fatigue,morale,score,casualties,inflicted,formation".Split(','));
      var table = GCSVMain.Create("units", new[] { "men", "name", "id", "side", "corps", "div", "player", "type" });
      int id = 0;
      bool isPlayer = false;
      Player player = null;
      foreach (var line in lines) {
        passed++;
        var s = line.Split(',');
        string commandString = GetCommandString(line);
        int[] command = commandString.Split(',').Select(k => k.ToInt()).ToArray();
        var commandingPlayer = GetCommandNodeAt(command).GetPlayer();
        string type = null;
        if (s[6] != "0" && (s[header["formation"]].Contains("LVL6_INF") || s[header["formation"]].Contains("LVL6_CAV"))) {
          type = "regiment";
        } else if (s[6] != "0" && s[header["formation"]].Contains("LVL6_ART")) {
          type = "gun";
        } else {
          if (commandString == Army1CommandString) {
            BattleID = GetBattleIDFromArmyCommanderName(s[header["name2"]]);
          }
          if (CommandsToPlayers.ContainsKey(commandString)) {
            player = CommandsToPlayers[commandString];
            player.UnitID = id;
            isPlayer = true;
            type = "player";
          } else {
            continue;
          }
        }

        var unit = new GCSVLine(table.Header);
        unit["player"] = (commandingPlayer != null ? commandingPlayer.ID : -1).ToString();
        unit["men"] = s[header["men"]];
        unit["id"] = id.ToString();
        ID_Mapping.Add(s[header["id"]].ToInt(), id);
        unit["name"] = s[header["name"]];
        unit["side"] = s[header["Side"]];
        unit["corps"] = s[header["Corp"]];
        unit["div"] = s[header["Div"]];
        unit["type"] = type;
        table.Add(unit);
        id++;
      }
      return table;
    }
  }
}
