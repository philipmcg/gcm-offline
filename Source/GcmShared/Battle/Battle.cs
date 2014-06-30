using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using Utilities;
using Military;

using Ninject;


namespace GcmShared {


  public class RandomDivisionsOutput {
    public BalanceInfo BalanceInfo = new BalanceInfo();
    public int MenGunRatio = 350;
    public Division[] ArmyCommanders = new Division[3];
  }

  public class BalanceInfo {
    public int Team2InfantryPercentage;
    public int Team2ArtilleryPercentage;

    public double Team2InfantryMultiplier { get { return Team2InfantryPercentage / 100d; } }
    public double Team2ArtilleryMultiplier { get { return Team2ArtilleryPercentage / 100d; } }

    public bool ApplyBattleSizeHardCaps = false;

    public BalanceInfo() {
      Team2InfantryPercentage = 100;
      Team2ArtilleryPercentage = 100;
    }
  }

  class A {
    public int B { get; private set; }
  }

  public class Division2 {
    public string UserName;
    public int UserID;
    public int DivisionID;
    public int Side;

    public Dictionary<string, string> Query;

    public Name CharacterName;
    public int RD_Men;
    public int RD_LimitedMen;

    public string DivisionXmlPath;
  }
  public struct Location {
    public System.Drawing.PointF Loc;
    public System.Drawing.PointF Dir;
    public Location(System.Drawing.PointF loc, System.Drawing.PointF dir) {
      Loc = loc;
      Dir = dir;
    }
    public Location(float locx, float locz, float dirx, float dirz) {
      Loc = new System.Drawing.PointF(locx, locz);
      Dir = new System.Drawing.PointF(dirx, dirz);
    }

    const int Offset = 300;

    public Location Fuzzy(int offset = Offset) {
      Location loc = this;
      loc.Loc = new PointF(loc.Loc.X + Rand.Curved(offset) * Rand.Sign(), loc.Loc.Y + Rand.Curved(offset) * Rand.Sign());
      return loc;
    }
  }
  public interface IProvider {
    T Get<T>();
  }
  class Provider : IProvider {
    Ninject.StandardKernel k;
    public Provider(Ninject.StandardKernel kernel) {
      k = kernel;
    }

    public T Get<T>() {
      return k.Get<T>();
    }
  }

  public class Gcm {
    public static GcmDataManager Data { get; set; }
    public static IVariableBin Var { get { return Data.VariableBin; } }
    public static IProvider Provider { get; set; }

    public static void Initialize(Ninject.Modules.NinjectModule module) {
      Provider = new Provider(new StandardKernel(module));
    }
  }

  public class Factions {
    public const int Neutral = 0;
    public const int USA = 1;
    public const int CSA = 2;
    public const int Britain = 3;

    public static readonly int[] WabashFactions = { USA, CSA };
  }

  public class Winners {
    public const int Incomplete = 3;
    public const int Draw = 0;
  }

  public class BattleResult {
    public const int Unfinished = 0;
    public const int Terminated = 1;
    public const int Finished = 2;
  }

  public enum BattleTypes {
    Normal = 0,
    SingleplayerCampaign = 1,
    SingleplayerCustomCampaign = 2,
    Competitive1v1 = 3,
  }
  public enum BattleTypeChoices {
    None = 0,
    Standard = 1,
    Custom = 2,
    Competitive1v1 = 3,
  }

  public enum GameTypes {
    Normal = 0,
    ScenarioObjs = 1,
  }

  public enum OOBType {
    PersistentDivisions = 0,
    RandomDivisions = 1,
    Historical = 2,
  }

  public enum SupplyType {
    ByArmy = 0,
    ByDivision = 1,
    NoSupply = 2,
  }

  public static partial class Extensions {

    /// <summary>
    /// Returns true if first rank is higher than (superior of) second
    /// </summary>
    public static bool IsHigherThan(this Rank first, Rank second) {
      return (int)first < (int)second;
    }

    /// <summary>
    /// Returns true if first rank is higher than (inferior of) second
    /// </summary>
    public static bool IsLowerThan(this Rank first, Rank second) {
      return (int)first > (int)second;
    }

  }


  public class MapSize {
    public const int MapRegions = 8;


    public MapSize(string map, Size rect, Point topLeft, Point bottomRight) {

      double map_max_multiplier = double.Parse(Gcm.Data.GCSVs["maps"][map]["multiplier"]);
      double totalWidth = map_max_multiplier * MapRegions;

      double hoff = ((double)topLeft.X / (double)rect.Width) * totalWidth;
      double voff = ((double)topLeft.Y / (double)rect.Height) * totalWidth;

      double width = ((double)(bottomRight.X - topLeft.X) / (double)rect.Width) * totalWidth;
      double height = ((double)(bottomRight.Y - topLeft.Y) / (double)rect.Width) * totalWidth;

      HOffset = hoff;
      VOffset = voff;

      HMultiplier = width / MapRegions;
      VMultiplier = height / MapRegions;
    }

    public MapSize(string map) {
      double map_max_multiplier = double.Parse(Gcm.Data.GCSVs["maps"][map]["multiplier"]);
      double multiplier = double.Parse(Gcm.Data.GCSVs["map_sizes"][Gcm.Var.Str["opt_s_map_width"]]["multiplier"]);

      HMultiplier = Math.Min(multiplier, map_max_multiplier);
      VMultiplier = Math.Min(multiplier, map_max_multiplier);

      HOffset = Rand.Int((int)(map_max_multiplier - HMultiplier)) * MapRegions;
      VOffset = Rand.Int((int)(map_max_multiplier - VMultiplier)) * MapRegions;
    }

    /// <summary>
    /// Offset from top and left of map
    /// </summary>
    public double HOffset;
    public double VOffset;

    public double HMultiplier;
    public double VMultiplier;
  }

  public class Battle {

    public BattleSetup Setup;
    public string HostUsername = "";
    public int HostPlayerID = 0;
    public string GcmVersion = "";
    public bool SpawnByCorps = false;

    public BalanceInfo BalanceInfo = new BalanceInfo();
    public Division[] ArmyCommanders;
    public int MenGunRatio = 350;

    public int SpriteRatio = 4;
    public int NumObjectives;
    public int LengthOfBattleInMinutes;
    public string Map;
    public string MapDbName;
    public GameTypes GameType;
    public BattleTypes BattleType;
    public int BattleID;
    public OOBType OOBType;
    public SupplyType SupplyType;
    public bool Ranked;
    public bool OOBTypeIsRandom { get { return OOBType == OOBType.RandomDivisions || OOBType == OOBType.Historical; } }
    public bool OOBTypeIsFromCampaign { get { return !OOBTypeIsRandom; } }

    public int GetIntegerOption(string optionName) {
      return int.Parse(Gcm.Var.Str["opt_s_"+optionName]);
    }

    public HashSet<string> Flags;

    public List<Division> Divisions;
    public Dictionary<int, Organization> DivisionIndexById;
    public Dictionary<Organization, int> OrganizationToDivisionID;
    public List<PointF> Objectives;
    public MapSize MapSize;
    public int TotalMen;
    public int TotalGuns;
    public HashSet<int> DivisionIDsAlreadyAssignedToCorpsCommand = new HashSet<int>();

    public bool CavalryAllowed;

    public bool UseBalancer { get { return BattleType == BattleTypes.Normal || BattleType == BattleTypes.Competitive1v1; } }
    public bool UseGunPenalties {
      get {
        return
          BattleType == BattleTypes.Normal
          && Ranked
          && Divisions.Count >= 6
          && OOBType == GcmShared.OOBType.PersistentDivisions;
      }
    }

    public bool IsSingleplayer {
      get {
        return BattleType == BattleTypes.SingleplayerCampaign || BattleType == BattleTypes.SingleplayerCustomCampaign;
      }
    }
    public bool IsMultiplayer { get { return !IsSingleplayer; } }

    public int PlayerSide; // used if this is an SP campaign battle

    public ArraySet<Side> Sides;

    static string[] OOBTypeNames = new string[] { "Campaign", "Random", "Historical" };

    public string OOBTypeName { get { return OOBTypeNames[(int)OOBType]; } }

    public Battle() {
      Flags = new HashSet<string>();
      Flags.Add("all");
      Flags.Add("custom_settings");

      Sides = new ArraySet<Side>();
      Sides.Insert(1, new Side(this, 1));
      Sides.Insert(2, new Side(this, 2));

      DivisionIndexById = new Dictionary<int, Organization>();
    }


    public static OOBType GetOOBTypeByID(string name) {
      if (name == "random")
        return OOBType.RandomDivisions;
      else if (name == "historical")
        return OOBType.Historical;
      else
        return OOBType.PersistentDivisions;
    }

    static string[] OOBTypeIDs = new string[] { "normal", "by_division", "no_wagons" };

    public static SupplyType GetSupplyTypeByID(string name) {
      return (SupplyType)OOBTypeIDs.IndexOf(name);
    }
  }

  public class Side {
    public Side(Battle battle, int id) {
      Battle = battle;
      ID = id;
    }
    public Battle Battle;
    public SidePoints Locations;
    public int ID;
    public IEnumerable<Division> Divisions { get { return Battle.Divisions.Where(d => d.Side == ID); } }
    public Division[] DivisionsOrderedBySuitabilityForHighCommand;
    public void SetDivisions() {
      DivisionsOrderedBySuitabilityForHighCommand = Divisions.OrderByDescending(d => d.PrefHighCommand).ThenByDescending(d => d.PlayerRank + Rand.Int(1000)).ToArray();
    }
  }
}
