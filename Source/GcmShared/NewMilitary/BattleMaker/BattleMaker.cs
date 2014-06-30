using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GcmShared;
using Military;
using Utilities.GCSV;

using System.IO;

using Utilities;

namespace GcmShared.NewMilitary {

  public interface IBattleMaker {
    void MakeBattle(Battle battle);
  }

  /// <summary>
  /// Creates or finds organizations for each player and saves them to xml files.
  /// </summary>
  public interface IScenarioOrganizer {
    void Organize(IEnumerable<Division> divisions, int menGunRatio);
  }

  /// <summary>
  /// Loads the division xml files into Organizations
  /// </summary>
  public interface IScenarioLoader {
    Dictionary<int, Organization> LoadArmies(IEnumerable<Division> divisions);
  }

  public interface IScenarioPreparer {
    void Prepare(IEnumerable<Organization> armies);
  }

  /// <summary>
  /// Writes the OOB file and all other scenario files
  /// </summary>
  public interface IScenarioWriter {
    void WriteScenario(IEnumerable<Organization> armies, Dictionary<int, Location> unitLocations, Battle battle);
  }

  /// <summary>
  /// Gets the map locations for armies and objectives, and saves them in the Battle object
  /// </summary>
  public interface IScenarioLocationMaker {
    void GetArmyLocations(Battle battle);
    void SaveObjectiveLocations(Battle battle);
  }

  /// <summary>
  /// Balances the Organizations by removing and shrinking units.
  /// </summary>
  public interface IScenarioBalancer {
    void Balance(IEnumerable<Organization> armies, IEnumerable<Division> divisions, BalanceInfo info, Func<Organization, DivisionLimits> getLimitsForDivision);
  }

  /// <summary>
  /// Unifies the divisions into two Army organizations
  /// </summary>
  public interface IScenarioArmyUnifier {
    IEnumerable<Organization> Unify(IEnumerable<Organization> divisions);
  }


  public class DivisionLimits {
    public int GunLimit;
    public int MenLimit;
    public int RankGunLimit;
    public int RankMenLimit;
    public double RegimentsLimit;
  }

  class PostProcessor {
    public void ChooseCavalry2Regiments(IEnumerable<Organization> armies, Battle battle) {
      var army1 = armies.First();
      var army2 = armies.Last();
      var div1 = GetBestCavalryDivision(army1, battle);
      var div2 = GetBestCavalryDivision(army2, battle);
      if (div1 != null && div2 != null) {
        var brigade = div1.AllFightingBrigades().ToArray().GetRandom();
        var cavBrigade = SplitOffNRegiments(brigade, 2) ?? brigade;
        double weight = cavBrigade.AllUnits.Sum(u => FMil.GetUnitWeight(u));
        MakeBrigadeCavalry(cavBrigade);

        MakeDivisionHaveCavalryOfWeight(div2, weight);
      }
    }

    public void ChooseCavalryMultiple(IEnumerable<Organization> armies, Battle battle) {
      int totalCavalryRegimentsPerSide = battle.GetIntegerOption("cavalry_regts_per_side");
      int totalCavalryDivsPerSide = battle.GetIntegerOption("cavalry_divs_per_side");
      var army1 = armies.First();
      var army2 = armies.Last();
      var divs1 = GetCavalryDivisions(army1, battle, totalCavalryDivsPerSide);
      var divs2 = GetCavalryDivisions(army2, battle, divs1.Count);
      if (divs1.Count > 0 && divs2.Count > 0) {
        int cavOnSide1 = MakeDivisionsHaveCavalry(divs1, totalCavalryRegimentsPerSide);
        int cavOnSide2 = MakeDivisionsHaveCavalry(divs2, cavOnSide1);
      }
      Mil.SetTagsOnNonpersistentUnitsAndCommanders(armies);
    }

    int MakeDivisionsHaveCavalry(List<Organization> divisions, int maxNumberOfRegiments) {
      int regimentsRemaining = maxNumberOfRegiments;
      int divsRemaining = divisions.Count;
      foreach (var div in divisions) {
        int regts = Math.Min(regimentsRemaining, regimentsRemaining / divsRemaining);
        int numCavalry = MakeDivisionHaveCavalry(div, regts);
        regimentsRemaining -= numCavalry;
        divsRemaining--;
      }
      return maxNumberOfRegiments - regimentsRemaining;
    }

    // Returns the actual number of new cavalry regiments
    int MakeDivisionHaveCavalry(Organization division, int maxNumberOfRegiments) {
      var cavalryBrigades = SplitDivisionIntoGroupsAndReturnBrigadesFromNewGroup(division, maxNumberOfRegiments);
      foreach (var brigade in cavalryBrigades) {
        MakeBrigadeCavalry(brigade);
      }
      return cavalryBrigades.Sum(b => b.NumUnits);
    }

    Organization AddRegimentsToBrigadesToLimitAndThenSplitIntoNewBrigade(List<Organization> destinationBrigades, List<Unit> regiments, int brigadeLimit) {
      int space = destinationBrigades.Sum(b => Math.Max(brigadeLimit - b.NumUnits, 0));
      if (space >= regiments.Count) {
        foreach (var brigade in destinationBrigades) {
          int numToAdd = Math.Max(brigadeLimit - brigade.NumUnits, 0);
          for (int i = 0; i < numToAdd && regiments.Any(); i++) {
            var regiment = regiments.Last();
            regiments.RemoveAt(regiments.Count - 1);
            TransferRegiment(regiment, brigade);
          }
        }
        return null;
      } else {
        //int numBrigadesToCreate = regiments.Count / brigadeLimit + (regiments.Count % brigadeLimit > 0 ? 1 : 0);
        int side = regiments.First().Parent.Data.Side;
        foreach (var regiment in regiments) {
          regiment.Parent.RemoveUnit(regiment);
        }
        var brigade = RandomCreator.Instance.CreateBrigadeWithRegiments(side, regiments);
        return brigade;
      }
    }

    List<Organization> SplitDivisionIntoGroupsAndReturnBrigadesFromNewGroup(Organization division, int regimentsInMainGroup) {
      // If the total units in the division isn't more than the regiments we want to split, return all of them.
      if (division.AllFightingUnits().Count() <= regimentsInMainGroup) {
        return division.AllFightingBrigades().ToList();
      }
      
      var brigades = division.AllFightingBrigades().ToList();
      brigades.Shuffle();

      var newGroup = new List<Organization>();
      int numRegimentsRemaining = regimentsInMainGroup;
      foreach (var brigade in brigades) {
        // If this brigade fits in its entirety, add it to the new group.
        if (brigade.NumUnits <= numRegimentsRemaining) {
          newGroup.Add(brigade);
          numRegimentsRemaining -= brigade.NumUnits;
        } else {
          // else if we only take some regiments from this brigade, 
          var newBrigade = AddRegimentsToBrigadesToLimitAndThenSplitIntoNewBrigade(newGroup, brigade.Units.Take(numRegimentsRemaining).ToList(), 5);
          numRegimentsRemaining = 0;
          if (newBrigade != null) {
            newGroup.Add(newBrigade);
            division.AddOrganization(newBrigade);
          }
        }
        if (numRegimentsRemaining == 0) {
          break;
        }
      }
      return newGroup;
    }

    void TransferRegimentToSmallestOtherBrigadeInDivision(Unit regiment) {
      var brigade = regiment.Parent;
      var division = brigade.Parent;
      var otherBrigades = brigade.Parent.AllFightingBrigades().Except(brigade.AsEnumerable()).ToList();
      if (!otherBrigades.Any()) {
        var destinationBrigade = otherBrigades.OrderBy(b => b.NumUnits).First();
        TransferRegiment(regiment, destinationBrigade);
      } else {
        var newBrigade = SplitOffRegiments(new List<Unit>() { regiment });
        division.AddOrganization(newBrigade);
      }
    }

    void TransferRegiment(Unit regiment, Organization destinationBrigade) {
      regiment.Parent.RemoveUnit(regiment);
      destinationBrigade.AddUnit(regiment);
    }

    /// <summary>
    /// Splits N regiments from the original brigade into a new brigade with a random commander and returns it.
    /// </summary>
    Organization SplitOffNRegiments(Organization originalBrigade, int numRegiments) {
      if (numRegiments >= originalBrigade.NumUnits) {
        return null;
      }
      var regiments = originalBrigade.Units.ToList();
      regiments.Shuffle();
      
      var regimentsToSplit = regiments.Take(numRegiments).ToList();
      var newBrigade = SplitOffRegiments(regimentsToSplit);
      return newBrigade;
    }

    Organization SplitOffRegiments(List<Unit> regimentsToSplit) {
      int side = regimentsToSplit.First().Parent.Data.Side;
      var creator = RandomCreator.Instance;
      foreach (var regt in regimentsToSplit) {
        regt.Parent.RemoveUnit(regt);
      }

      var brigade = creator.CreateBrigadeWithRegiments(side, regimentsToSplit);
      return brigade;
    }

    Organization GetBestCavalryDivision(Organization army, Battle battle) {
      var division = battle.Divisions.Where(d => d.Side == army.Data.Side).GroupBy(d => d.PrefCavalry).OrderByDescending(g => g.Key).First().ToList().GetRandom();
      if (division.PrefCavalry > 0) {
        return battle.DivisionIndexById[division.DivisionID];
      } else {
        return null;
      }
    }
    const bool onlyHaveCavIfPreferIt = true;
    List<Organization> GetCavalryDivisions(Organization army, Battle battle, int numDivisions) {
      var divs = battle.Divisions.Where(d => d.Side == army.Data.Side).Where(d => d.PrefCavalry > 0 || !onlyHaveCavIfPreferIt).OrderByDescending(d => d.PrefCavalry).ThenByDescending(d => d.PlayerRank + Rand.Int(1000)).Take(numDivisions).ToList();
      return divs.Select(d => battle.DivisionIndexById[d.DivisionID]).ToList();
    }

    Organization GetRandomDivision(Organization army) {
      var divisions = army.Organizations.SelectMany(o => o.Organizations).Where(o => o.Data.Level == Levels.Division);
      return divisions.ToList().GetRandom();
    }

    double MakeRandomBrigadeCavalryAndReturnWeight(Organization division) {
      var brigade = division.AllFightingBrigades().ToArray().GetRandom();
      double weight = brigade.AllUnits.Sum(u => FMil.GetUnitWeight(u));
      MakeBrigadeCavalry(brigade);
      return weight;
    }

    void MakeDivisionHaveCavalryOfWeight(Organization division, double weight) {
      // Get the brigade closest in weight to the given weight amount.
      var brigade = division.AllFightingBrigades().OrderBy(b => Math.Abs(b.AllUnits.Sum(u => FMil.GetUnitWeight(u)) - weight)).ToArray().GetRandom();
      MakeBrigadeCavalry(brigade);
    }

    void MakeBrigadeCavalry(Organization brigade) {
      foreach (var unit in brigade.AllUnits) {
        unit.Data.Type = UnitTypes.Cavalry;
        unit.ExportData.Men = unit.ExportData.Men * 7 / 10;
        string pfx = Gcm.Data.GCSVs["factions"][brigade.Data.Side]["pfx"];
        unit.ExportData.Flag1 = Gcm.Var.Int[pfx + "c_flag1"];
        unit.ExportData.WeaponId = 200 + unit.ExportData.Marksmanship; // IDS_ARSN_GCM_Carbine_ firearm
        string[] stringsToReplace = new[] { "Colored Troops", "Volunteers", "Militia", "Infantry" };
        foreach (var str in stringsToReplace) {
          if (unit.ExportData.Name.Contains(str)) {
            unit.ExportData.Name = unit.ExportData.Name.Replace(" "+str, "");
            break;
          }
        }
        unit.ExportData.Name += " Cavalry";
        unit.ExportData.ClassId = RandomCreator.Instance.Class("UGLB_" + (brigade.Data.Side == Factions.USA ? "USA" : "CSA") + "_Cav_" + Rand.Int(1, 4));
      }
    }
  }
  public class BattleMaker {
    protected IScenarioOrganizer Organizer;
    protected IScenarioLoader Loader;
    protected IScenarioPreparer Preparer;
    protected IScenarioLocationMaker LocationMaker;
    protected IScenarioBalancer Balancer;
    protected IScenarioArmyUnifier Unifier;

    protected IScenarioWriter Writer;

    protected Battle Battle;

    protected Dictionary<int, Organization> Divisions;
    protected ArraySet<Organization> Armies;

    // Assigned by ScenarioPreparer, used by ScenarioWriter
    protected Dictionary<int, Location> UnitLocations;

    protected RandomCreator Creator;

    protected Action<string> PrintMessage;

    [Ninject.Inject]
    public ILog Log { get; set; }

    public BattleMaker() {
      Creator = RandomCreator.Instance;
      Armies = new ArraySet<Organization>();
    }

    void Print(string str) {
      if (PrintMessage != null)
        PrintMessage(str);
      Log.Write(str);
    }


    public void MakeBattle(Battle battle, Action<string> printMessage) {
      PrintMessage = printMessage;

      Battle = battle;

      Print("Creating armies");
      Organize();

      Print("Loading armies");
      LoadArmies();

      Print("Preparing armies");
      PrepareArmies();

      if (Battle.UseBalancer) {
        Print("Balancing armies");
        BalanceArmies();

        var finalArmyWeights = Armies.Select(a => new {
          FightingWeight = a.AllFightingUnits().Sum(u => SMil.GetUnitWeightOrExport(u)),
          ArtilleryWeight = a.AllArtilleryUnits().Sum(u => SMil.GetUnitWeightOrExport(u)),
          Side = a.Data.Side,
          Organization = a,
        }).ToDictionary(a => a.Side, a => a);

        File.WriteAllLines(Paths.Local.TempBattleCreationLog(Battle.BattleID), finalArmyWeights.Values.Select(w => w.Side + " f:" + w.FightingWeight + " a:" + w.ArtilleryWeight));
      }
      if (Battle.CavalryAllowed) {
        new PostProcessor().ChooseCavalryMultiple(this.Armies, Battle);
      }

      /*if (!battle.IsSingleplayer && battle.Divisions.Count > 2) {
        int totalMen = Armies.Sum(a => a.AllFightingUnits().Sum(u => u.ExportData.Men));

        int sr = 4;
        if (totalMen > 30000)
          sr = 4;
        if (totalMen > 40000)
          sr = 5;
        if (totalMen > 50000)
          sr = 6;
        if (totalMen > 60000)
          sr = 7;
        if (totalMen > 70000)
          sr = 8;
        if (totalMen > 85000)
          sr = 9;
        if (totalMen > 100000)
          sr = 10;
        battle.SpriteRatio = sr;

       // battle.SpriteRatio = Math.Min(10, Math.Max(4, totalMen / 9000));
      }*/

      GetLocations();

      Print("Writing scenario");
      WriteScenario();

    }

    protected virtual void Organize() {
      throw new NotImplementedException();
    }

    protected virtual void LoadArmies() {
      Log.Write("Loading army files");
      Loader = new ScnLoader(Battle);
      Divisions = Loader.LoadArmies(Battle.Divisions);

      Log.Write("Unifying armies");
      Unifier = new ScnOrganizerHistorical(Gcm.Data.GCSVHeaders, Directory.GetFiles(Gcm.Data.GetPath("HistoricalOOBs\\gettysburg1")), Gcm.Var.Str["opt_s_cavalry"] == "1");
      var armies = Unifier.Unify(Divisions.Values);

      armies.ForEach(a => Armies.Insert(a.Data.Side, a));
    }

    protected virtual void GetLocations() {
      Battle.TotalMen = Armies.Sum(a => a.AllUnits.Where(u => u.Data.Active).Sum(u => u.ExportData.Men));

      Print("Placing armies");
      LocationMaker = new ScnLocationMaker();
      LocationMaker.GetArmyLocations(Battle);
      Print("Placing objectives");
      LocationMaker.SaveObjectiveLocations(Battle);
      Log.Write("Setting locations on units");
      UnitLocations = (Preparer as ScnPreparer).SetLocationsOnUnits(Divisions, Armies, Battle.SpawnByCorps);
      Log.Write("Forming corps");
      if (Battle.BattleType == BattleTypes.Normal || Battle.BattleType == BattleTypes.Competitive1v1)
        (Preparer as ScnPreparer).ReorganizeForMultipleCorps(Divisions, Armies, UnitLocations, Battle.ArmyCommanders);

      foreach (var div in Battle.Divisions) {
        var org = Battle.DivisionIndexById[div.DivisionID];
        org.Data.Name += " [{0}|{1}]".With(Utilities.Number.IntToBase52(div.PlayerID), Utilities.Number.IntToBase52(div.DivisionID));
      }
    }

    protected virtual void PrepareArmies() {
      Preparer = new ScnPreparer(Battle);
      Preparer.Prepare(Armies);
    }

    const double MenPerRegimentForLimits = 400;
    const int MenLimitForUnrankedCampaign = 4000;
    protected virtual void BalanceArmies() {
      Func<Organization, DivisionLimits> getLimit = o => {
        var pair = Battle.DivisionIndexById.First(p => p.Value == o);
        var div = Battle.Divisions.First(d => d.DivisionID == pair.Key);
        var limit = new DivisionLimits() {
          GunLimit = Battle.OOBTypeIsRandom ? div.RD_Guns : div.CD_Guns,
          MenLimit = Battle.OOBTypeIsRandom ? div.RD_Men : div.CD_Men,
          RankMenLimit = Battle.Ranked ? div.CD_Men_Rank : MenLimitForUnrankedCampaign, // in unranked campaign battles, give everyone this standard army size.  (This still won't work perfectly, because people who have smaller divisions to start with will not have enough men to fill the 4000 limit.
          RegimentsLimit = Battle.OOBTypeIsRandom ? (div.RD_Men / MenPerRegimentForLimits) : (Battle.Ranked ? div.CD_Regts_Rank_Modified : MenLimitForUnrankedCampaign / MenPerRegimentForLimits), // in unranked campaign battles, give everyone this standard army size.  (This still won't work perfectly, because people who have smaller divisions to start with will not have enough men to fill the 4000 limit.
          RankGunLimit = Battle.Ranked ? div.CD_Guns_Rank : 12,
        };
        return limit;
      };
      Balancer = new ScnBalancer();
      Battle.BalanceInfo.ApplyBattleSizeHardCaps = Battle.IsMultiplayer && Battle.OOBTypeIsFromCampaign;
      Balancer.Balance(Armies, Battle.Divisions, Battle.BalanceInfo, getLimit);
    }

    protected virtual void WriteScenario() {
      Writer = new ScnWriter(System.Environment.CurrentDirectory + "//temp"); // The crash was because units don't have location set.
      Writer.WriteScenario(Armies, UnitLocations, Battle);
    }
  }


}
