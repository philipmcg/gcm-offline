using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GcmShared;
using Military;
using Utilities;
using GcmShared.NewMilitary;

namespace GcmShared {

  public static class Mil {
    public const double CommanderSkillRange = 6d;
    public const double MaxPossibleCommanderSkill = 6.8;
    public const double ExperienceRange = 9d;
    public const double MaxPossibleExperience = 9.8;

    /// <summary>
    /// Navigates to the first organization of the given level in this tree.
    /// </summary>
    public static Organization MoveToLevel(this Organization me, int level) {
      Organization current = me;

      while (current.Data.Level > level)
        current = current.Parent;

      while (current.Data.Level < level) {
        if (!current.Organizations.Any())
          current.AddOrganization(CreateOrganization(me.Data.Side, current.Data.Level + 1));
        current = current.Organizations.First();
      }

      return current;
    }

    /// <summary>
    /// Creates an empty organization for this side at this level
    /// </summary>
    public static Organization CreateOrganization(int side, int level) {
      Organization organization = new Organization();
      organization.Data.Side = side;
      organization.Data.Level = level;
      return organization;
    }

    /// <summary>
    /// Gets the appropriate rank table for this command.
    /// </summary>
    private static string GetRankTable(object command) {
      string table = null;
      if (command is Unit) {
        var unit = (Unit)command;
        if (unit.Data.Type == UnitTypes.Artillery) {
          if (unit.Parent.IndexOfUnit(unit) % 2 == 0)
            table = "section_commanders";
          else
            table = "gun_commanders";

        } else if (unit.Data.Type == UnitTypes.Infantry)
          table = "regiment_commanders";
        else if (unit.Data.Type == UnitTypes.Cavalry)
          table = "regiment_commanders";
      } else if (command is Organization) {
        var org = (Organization)command;
        if (org.Data.Level == Levels.Brigade) {
          if (org.GetUnitType() == UnitTypes.Infantry)
            table = "brigade_commanders";
          else if (org.GetUnitType() == UnitTypes.Cavalry)
            table = "brigade_commanders";
          else if (org.GetUnitType() == UnitTypes.Artillery)
            table = "battery_commanders";
          else
            table = "brigade_commanders";
        } else if (org.Data.Level == Levels.Division)
          table = "division_commanders";

        if (table == null)
          throw new ArgumentException("Unable to find a rank table for this organization, level {0}".With(org.Data.Level));
      }

      if (table == null)
        throw new ArgumentException("Unable to find a rank table for this command");
      return table;
    }

    /// <summary>
    /// Returns the commander's rank after trying for promotion.  Does NOT set the commander's rank.  This function has no side effects.
    /// </summary>
    public static Rank GetPossiblePromotionRank(Rank rank, IForce command) {
      var table = Gcm.Data.GradientTables[GetRankTable(command)];
      if ((int)table.GradientDistribution(command.GetHeadCount()) < (int)rank) {
        do {
          rank--; // Ranks go backwards, lower number is higher rank
          if (rank < Rank.Invalid.AsInt)
            throw new InvalidOperationException("Negative rank");
        }
        while (!table.Objects.Contains((int)rank));
      }
      rank = ValidateRank(rank, command);
      return rank;
    }


    /// <summary>
    /// Number of sections in an artillery battery for this faction
    /// </summary>
    public static int GetRandomBatterySize(int factionID) {
      return Gcm.Data.Lists["randomdivision\\" + Gcm.Data.FactionPfx(factionID) + "battery_sizes"].GetRandom().ToInt();
    }
    public static int GetRandomBrigadeSize(int factionID) {
      return Gcm.Data.Lists["randomdivision\\" + Gcm.Data.FactionPfx(factionID) + "brigade_sizes"].GetRandom().ToInt();
    }

    /// <summary>
    /// Assigns an appropriate rank to the commander based on his command.  
    /// Ignores the commander's current rank.
    /// </summary>
    static void SetRank(this Commander me, IForce command) {
      if (me.CommandIsOrganization && me.Organization.Data.Level <= Levels.Corps) {
        me.Data.Rank = (int)Rank.MajGen;
      } else {
        var table = Gcm.Data.GradientTables[GetRankTable(command)];
        int headcount = command.GetHeadCount();
        int properRank = (int)Gcm.Data.GradientTables[GetRankTable(command)].GradientDistribution(headcount);
        properRank = ValidateRank(properRank, command);
        me.Data.Rank = properRank;
      }
    }

    /// <summary>
    /// Assigns this commander to the command, and sets the commanders rank appropriately based on number of men in organization.
    /// This ideally should be done after all units have been added to the organization.  Ignores commander's current rank.
    /// </summary>
    public static void AssignCommand(this Commander cdr, IForce command) {
      if (command.HasCommander)
        command.SetNoCommander();
      command.SetCommander(cdr);
      cdr.SetRank(command);

      if (command is Organization)
        SetOrganizationName((Organization)command, cdr);
    }

    /// <summary>
    /// Assigns this commander to the command, and sets the commanders rank appropriately based on number of men in organization.
    /// This ideally should be done after all units have been added to the organization.  If commander's current rank is higher than 
    /// the new rank he is assigned, the old one will be kept.
    /// </summary>
    public static void AssignCommandWithoutDemoting(this Commander cdr, IForce command) {
      Rank currentRank = cdr.Rank;
      cdr.AssignCommand(command);
      if (cdr.Rank.IsLowerThan(currentRank))
        cdr.Rank = currentRank;
    }

    /// <summary>
    /// Ensures that the commander has equal or greater rank than all subordinates' -- if his rank is lower than a suboridinate's,
    /// his rank will be upgraded.
    /// </summary>
    public static int ValidateRank(Rank currentRank, IForce command) {
      // Prevent generals commanding regiments.
      if (currentRank.IsHigherThan(Rank.Col) && command is Unit)
        currentRank = Rank.Col;

      if (command is Organization)
        if (((Organization)command).AllUnits.Any())
          currentRank = Math.Min(currentRank, ((Organization)command).AllUnits.Min(u => u.Commander.Data.Rank));
      return currentRank;
    }

    public static void SetRank(this Commander me, Rank rank) {
      me.Data.Rank = (int)rank;
    }

    public static int GetFactionID(this Commander me) {
      return me.CommandIsOrganization ? me.Organization.Data.Side : me.Unit.Parent.Data.Side;
    }

    public static Rank GetRank(this Commander me) {
      return (Rank)me.Data.Rank;
    }

    public static bool WasInvolved(this UnitBattleResultData me) {
      return me.InvolvedMen > 0;
    }

    /// <summary>
    /// Reassigns a new rank for this commander, if it should be higher than it currently is based on the size of the command.
    /// </summary>
    public static void ReassignRank(this Commander me, IForce command = null) {
      if (command == null)
        command = me.Command as IForce;

      if (command == null)
        throw new Exception("Invalid command for commander");

      int previousRank = me.Data.Rank;
      me.SetRank(command);

      // If commander's rank was lowered, bring it back to previous rank.
      if (previousRank < me.Data.Rank)
        me.Data.Rank = previousRank;
    }

    static void SetOrganizationName(Organization org, Commander cdr) {
      string name = "Organization";

      if (org.Data.Level == Levels.Corps) {
        name = cdr.Data.LastName + "'s Corps";
      } else if (org.Data.Level == Levels.Division) {
        name = cdr.Data.LastName + "'s Division";
      } else if (org.Data.Level == Levels.Brigade) {
        int type = org.GetUnitType();
        if (type == UnitTypes.Infantry) {
          name = cdr.Data.LastName + "'s Brigade";
        } else if (type == UnitTypes.Artillery) {
          int batteryNumber = org.Data.OrganizationNumber;
          string batt = "ABCDEFGHIK"[batteryNumber % 10].ToString();
          string battno = Util.Ordinal(batteryNumber / 10 + 1);
          string state = org.Data.State;

          if (org.Data.Side == Factions.USA)
            name = string.Format("Battery {0} - {1} {2} Artillery", batt, battno, Gcm.Data.GCSVs["batteries"][state]["name"]);
          else
            name = string.Format("{0}'s ({1}) Artillery", cdr.Data.LastName, Gcm.Data.GCSVs["batteries"][state]["name"]);
        }

      }

      org.Data.Name = name;
    }

    public static string GetRegimentName(int num, string state) {
      return Util.Ordinal(num) + " " + Gcm.Data.GCSVs["regiments"][state]["name"];
    }

    /// <summary>
    /// Formats name as "Capt John W Smith"
    /// </summary>
    public static string GetAbbreviatedName(this Name me, int rank) {
      return string.Format("{0} {1} {2}{3}", Gcm.Var.Str["rank_abbr_" + rank], me.First, string.IsNullOrEmpty(me.Middle) ? "" : me.Middle + " ", me.Last);
    }

    /// <summary>
    /// Formats name as "Capt Smith"
    /// </summary>
    public static string GetVeryVeryShortName(this Name me, int rank) {
      return string.Format("{0} {1}", Gcm.Var.Str["rank_abbr_" + rank], me.Last);
    }

    /// <summary>
    /// Formats name as "Capt J Smith"
    /// </summary>
    public static string GetVeryShortName(this Name me, int rank) {
      return string.Format("{0} {1} {2}", Gcm.Var.Str["rank_abbr_" + rank], me.First[0], me.Last);
    }

    /// <summary>
    /// Formats name as "Capt Smith" from "5-10"
    /// </summary>
    public static string GetNameFromIdentifier(string nameIdentifier) {
      string[] list = nameIdentifier.Split('-');
      if (list.Length == 2)
        return string.Format("{0} {1}", Gcm.Var.Str["rank_abbr_" + list[0]], Gcm.Data.LastNames[int.Parse(list[1])]);
      else if (list.Length == 3)
        return string.Format("{0} {1} {2}", Gcm.Var.Str["rank_abbr_" + list[0]], Gcm.Data.FirstNames[int.Parse(list[1])], Gcm.Data.LastNames[int.Parse(list[2])]);
      else
        return "Unknown";
    }

    /// <summary>
    /// Formats name as "Capt John W Smith"
    /// </summary>
    public static string GetAbbreviatedName(this Commander me) {
      return GetAbbreviatedName(me.GetName(), me.Data.Rank);
    }

    /// <summary>
    /// Formats name as "Capt J Smith"
    /// </summary>
    public static string GetVeryShortName(this Commander me) {
      return GetVeryShortName(me.GetName(), me.Data.Rank);
    }

    /// <summary>
    /// Formats name as "Capt Smith"
    /// </summary>
    public static string GetVeryVeryShortName(this Commander me) {
      return GetVeryVeryShortName(me.GetName(), me.Data.Rank);
    }


    /// <summary>
    /// Takes any organizations and assembles them into a single Army level organization, 
    /// creating random commanders for the new "filler" organizations.
    /// </summary>
    public static IEnumerable<Organization> Unify(RandomCreator creator, Military.IO.MilitaryGroup group) {
      var sides = group.Organizations.GroupBy(o => o.Data.Side);

      foreach (var grouping in sides) {
        Organization army = null;
        int side = grouping.Key;
        var organizations = grouping.OrderByDescending(o => o.Data.Level);

        foreach (var org in organizations) {
          if (army == null) {
            if (org.Data.Level == Levels.Army) {
              army = org;
              continue;
            } else
              army = Mil.CreateOrganization(side, Levels.Army);
          }

          Organization parent = army;

          parent = parent.MoveToLevel(org.Data.Level - 1);
          parent.AddOrganization(org);
        }

        creator.CreateCommandersForEmptySpots(army);

        yield return army;
      }
    }

    public static double GetCombinedExperience(int men1, double exp1, int men2, double exp2) {
      if (men1 <= 0 && men2 <= 0)
        return 0;
      else if (men1 <= 0)
        return exp2;
      else if (men2 <= 0)
        return exp1;

      if (exp1 < 0.01 || exp2 < 0.01 || exp1 > 10 || exp2 > 10) {
        throw new ArgumentException("Invalid Arguments");
      }
      return ((men1 * exp1) + (men2 * exp2)) / (men1 + men2);
    }

    public static HashSet<int> GetAllElementIDs(IEnumerable<Organization> armies) {
      HashSet<int> ids = new HashSet<int>();
      foreach (var army in armies) {
        foreach (var unit in army.AllUnits) {
          if (unit.Data.Id != 0)
            ids.Add(unit.Data.Id);
        }

        foreach (var cdr in army.AllCommanders) {
          if (cdr.Data.Id != 0)
            ids.Add(cdr.Data.Id);
        }
      }
      return ids;
      /*
      HashSet<int> ids = armies.Aggregate<Organization, IEnumerable<Unit>>(new Unit[] { }, (u, o) => u.Concat(o.AllUnits)).Where(u => u.Data.Id != 0).Select(u => u.Data.Id)
          .Concat(armies.Aggregate<Organization, IEnumerable<Commander>>(new Commander[] { }, (c, o) => c.Concat(o.AllCommanders)).Where(c => c.Data.Id != 0).Select(c => c.Data.Id))
          .ToHashSet();*/
    }

    /// <summary>
    /// Sets tags on single-use non-persistent units and commanders.
    /// </summary>
    public static void SetTagsOnNonpersistentUnitsAndCommanders(IEnumerable<Organization> armies) {
      HashSet<int> ids = GetAllElementIDs(armies);

      int elementID = 1; // Just hope we never have more than 1000000 units, because then these elementIDs will overlap the campaign entity ids.

      // assign tag numbers to every unit or commander
      foreach (var army in armies) {
        foreach (var unit in army.AllUnits) {
          if (unit.Data.Id != 0)
            unit.Tag = unit.Data.Id;
          else {
            do {
              elementID++;
            } while (ids.Contains(elementID));
            unit.Data.Id = elementID;
            unit.Tag = elementID;
          }
        }

        foreach (var cdr in army.AllCommanders) {
          if (cdr.Data.Id != 0)
            cdr.Tag = cdr.Data.Id;
          else {
            do {
              elementID++;
            } while (ids.Contains(elementID));
            cdr.Data.Id = elementID;
            cdr.Tag = elementID;
          }
        }
      }
    }

    public static void RemoveEmptySubOrganizations(Organization org) {
      foreach (var suborg in org.AllOrganizations.ToArray()) {
        if (!suborg.AllUnits.Any())
          suborg.Parent.RemoveOrganization(suborg);
      }
    }


    public static void MergeSmallBrigades(Organization org, int type) {
      var brigades = org.Organizations.OfLevel(Levels.Brigade).Where(b => b.GetUnitType() == type).ToList();
      while (brigades.Any(b => b.NumUnits <= 2)) {
        // Get the first brigade with 2 or less regts
        var br = brigades.First(b => b.NumUnits <= 2);
        // And another brigade with 3 or less regts, but not the same one
        var other = brigades.FirstOrDefault(b => b.NumUnits <= 3 && b.Commander.Data.Id != br.Commander.Data.Id);

        // If another is found, 
        if (other != null) {
          foreach (var unit in other.Units.ToArray()) {
            br.AddUnit(unit);
          }
          other.Parent.RemoveOrganization(other);
          brigades = org.Organizations.OfLevel(Levels.Brigade).Where(b => b.GetUnitType() == type).ToList();
        } else break;
      }
    }

    struct GunType {
      public int Guntype;
      public int Quantity;
    }
    static void MoveGunsToThisOrder(Dictionary<Organization, int> guntypes, List<Organization> batteries) {
      var allGuns = batteries.SelectMany(b => b.AllArtilleryUnits()).ToArray();

      var batteriesToAddGunsTo = batteries.Select(b => new { Battery = b, Size = b.NumUnits }).OrderByDescending(b => guntypes.ContainsKey(b.Battery)).ToList();
      foreach (var gun in allGuns) {
        gun.Parent.RemoveUnit(gun);
      }
      foreach (var battery in batteriesToAddGunsTo) {
        int type = guntypes.ContainsKey(battery.Battery) ? guntypes[battery.Battery] : 0;
        // Do two guns at a time, we move them by SECTION
        for (int i = 0; i < allGuns.Length && battery.Battery.NumUnits < battery.Size; i+=2) {
          if (allGuns[i] != null && (type == 0 || allGuns[i].Data.WeaponId == type)) {
            battery.Battery.AddUnit(allGuns[i]);
            allGuns[i] = null;
            battery.Battery.AddUnit(allGuns[i+1]);
            allGuns[i+1] = null;
          }
        }
      }
    }
    static bool GunIsFirstInSection(Unit u) {
      return u.Parent.IndexOfUnit(u) % 2 == 0;
    }
    public static void SwapSectionsToMakeUniformBatteries(List<Organization> batteries) {
      //var batterySizes = batteries.Select(b => b.NumUnits).ToList();
      var numGunTypes = batteries.SelectMany(z => z.AllArtilleryUnits().Where(GunIsFirstInSection)).GroupBy(u => u.Data.WeaponId).ToList();

      var guntypes = numGunTypes.Select(g => new GunType { Guntype = g.Key, Quantity = g.Count() * 2 }).OrderByDescending(p => p.Quantity).ToList();
      //var batterySizes = batteries.Select(b => new { Battery = b, Size = b.NumUnits }).OrderByDescending(b => b.Size).ToList();
      var batts = batteries.OrderByDescending(z => z.NumUnits).ToList();

      int numBatteriesWithOnlyOneGunType = batteries.Count(z => z.AllUnits.Where(GunIsFirstInSection).Select(u => u.Data.WeaponId).Distinct().Count() == 1);
      Dictionary<Organization, int> batteryToGuntypeSelection = new Dictionary<Organization, int>();

      int b = 0; // current battery index
      int t = 0; // current guntype index
      int ts = 0; // Guntype start
      int bc = batts.Count;
      while(b < bc) {
        t = 0;
        int tc = guntypes.Count;
        while (t < tc) {
          if (batts[b].NumUnits < guntypes[t].Quantity && t + 1 < tc && batts[b].NumUnits <= guntypes[t+1].Quantity){
            t++;
          }
          if (batts[b].NumUnits <= guntypes[t].Quantity && ((t + 1 < tc && batts[b].NumUnits > guntypes[t+1].Quantity) || t + 1 == tc)){
            // Now b points to current battery and t points to the guntype we want.  So 
            guntypes[t] = new GunType() { Guntype = guntypes[t].Guntype, Quantity = guntypes[t].Quantity - batts[b].NumUnits };
            var guntype = guntypes[t];
            Comparison<GunType> mycomp = (x, y) => x.Quantity.CompareTo(y.Quantity);
            guntypes.Sort(mycomp);
            guntypes.Reverse();
            batteryToGuntypeSelection.Add(batts[b], guntype.Guntype);
            break; // t = guntype we want to use for b.
          }
          if (batts[b].NumUnits > guntypes[t].Quantity) {
            break; // can't fix b, so don't try.
          }
          t++;
        }
        b++;
      }

      if (numBatteriesWithOnlyOneGunType < batteryToGuntypeSelection.Count) {
        MoveGunsToThisOrder(batteryToGuntypeSelection, batteries);
      }




    }


    /// <summary>
    /// Creates a new commander and assigns him the command, ensuring his rank is less than the previous commander's.
    /// </summary>
    public static Commander GetReplacementCommander(int factionID, IForce force, Rank previousRank) {
      if (previousRank == Rank.Invalid)
        previousRank = force.Commander.GetRank();
      Commander cdr = RandomCreator.Instance.CreateRandomCommander(factionID);
      cdr.AssignCommand(force);
      if (cdr.Rank.IsHigherThan(previousRank))
        cdr.Rank = previousRank;

      // Most of the time, if the new commander is lower than the old one, the new commander's rank should
      // be one step below the old.  Example: this should move a new Capt up to Maj if he is replacing a Lt Col
      if (Rand.Percent(80) && cdr.Rank.IsLowerThan(previousRank))
        cdr.Rank = previousRank.MinusOne;

      // If the replacement is equal to previous and higher than a major, decrease his rank by one.
      if (cdr.Rank == previousRank && cdr.Rank.IsHigherThan(Rank.Maj))
        cdr.Rank = cdr.Rank.MinusOne;

      return cdr;
    }

    // Order of priority means the order they would be brought to battle.
    // Priority goes by brigade in order of brigades, and then by style.  (high style commanders will be brought first.)
    public static Queue<Unit> GetFightingUnitsInReverseOrderOfPriority(Organization division) {
      Queue<Unit> queue = new Queue<Unit>();
      var brigades = division.AllFightingBrigades().Reverse().ToList();
      foreach (var brigade in brigades) {
        var regiments = brigade.Units.ToList();
        foreach (var unit in regiments.OrderBy(r => r.Commander.Data.Style)) {
          queue.Enqueue(unit);
        }
      }
      return queue;
    }

    public static int GetValidMenForRegiment(Unit unit) {
      Random r = new Random((int)((unit.Commander.Data.Style * 128) * unit.Commander.Data.Experience));
      int variance = 90;
      int baseTroops = 525 - variance;

      // multiplier of troops who are stragglers.  if command = 6, stragglers = 0.0
      double stragglers = 0.12 - unit.Commander.Data.Command * 0.02;
      stragglers = Math.Max(0.0, stragglers);
      int rand = r.Next(0, variance / 2) + r.Next(0, variance / 2);

      // if cdr has low Control, this regiment will be limited by that.
      int recruitLimit = GetRecruitLimitForRegimentByAttribute(unit, c => c.Data.Leadership);
      int menAvailable = Math.Min(recruitLimit, unit.Data.Men);
      menAvailable = Rand.Int(menAvailable, unit.Data.Men);
      int validMen = Math.Min(rand + baseTroops, (int)(menAvailable * (1.0 - stragglers)));
      int validMen2 = Math.Min(rand + baseTroops, (int)(unit.Data.Men * (1.0 - stragglers)));
      //Console.WriteLine("men: {0}, normal: {1}, w/control: {2}".With(unit.Data.Men, validMen2, validMen));
      return validMen2;
    }

    public static int GetRecruitLimitForRegimentByAttribute(Unit unit, Func<Commander, double> commanderToAttribute) {
      int recruitLimit = Math.Min(unit.Data.RecruitLimit, 160 + (int)(commanderToAttribute(unit.Commander) * 130));
      recruitLimit += (20 - rand.Seed(unit.Commander.Data.Id).Int(40));
      return recruitLimit;
    }

    public static int GetRecruitLimitForRegiment(Unit unit) {
      return GetRecruitLimitForRegimentByAttribute(unit, c => c.Data.Control);
    }

    static ControlledRandom rand = new ControlledRandom(0);
  }


}
