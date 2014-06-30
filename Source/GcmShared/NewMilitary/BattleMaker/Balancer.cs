using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Utilities;


using Military;

namespace GcmShared.NewMilitary {


  class ScnBalancer : IScenarioBalancer {
    public class DivisionUnits {
      public Queue<Unit> UnitsInOrderOfRemoval;
      public double Strength;
     // public double RatioOfStrengthToLimit { get { return Strength / Limits.MenLimit
      // Balancer should remove evenly in two tiers -- removing from the player with the highest strength / limit ratio first as long as they have more than 5 regiments.  Only when everyone has 5 regiments will it start removing from these players.
      const int minRegimentsToContinueBalancing = 5;
      public double InverseStrength { 
        get { 
          double inverseStrength = 1d / (Strength / Limits.RegimentsLimit);
          // Balancer should remove
          if (UnitsInOrderOfRemoval.Count <= 5)
            return inverseStrength * 10;
          else
            return inverseStrength;
        } 
      }
      DivisionLimits Limits;

      public DivisionUnits(Organization org, DivisionLimits div) {
        Limits = div;
        Strength = org.AllFightingUnits().Sum(u => u.ExportData.Men);
       // RatioOfStrengthToLimit = org.AllFightingUnits().Sum(u => u.ExportData.Men);
        UnitsInOrderOfRemoval = Mil.GetFightingUnitsInReverseOrderOfPriority(org);
      }
    }

    /// <summary>
    /// Sorts regiments in order of removal, grouped by division.  Then takes regiments from the division with the highest men/limit ratio, until all are taken.
    /// </summary>
    Queue<Unit> GetRegimentsInOrderToRemove(Organization army, Func<Organization, DivisionLimits> getDivision) {
      var units = new Heap<double, DivisionUnits>();
      var divisions = army.AllOrganizations.OfLevel(Levels.Division).Select(d => new DivisionUnits(d, getDivision(d))).ToList();
      foreach (var division in divisions) {
        units.Enqueue(division.InverseStrength, division);
      }

      Queue<Unit> toRemove = new Queue<Unit>();

      while (!units.IsEmpty) {
        var topDivision = units.DequeueValue();
        if (topDivision.UnitsInOrderOfRemoval.Any()) {
          var unit = topDivision.UnitsInOrderOfRemoval.Dequeue();
          toRemove.Enqueue(unit);
          topDivision.Strength -= unit.ExportData.Men;

          units.Enqueue(topDivision.InverseStrength, topDivision);
        }
      }

      return toRemove;
    }

    void RemoveUnitsFromArmyByWeight(Organization army, double weightToRemove, Func<Organization, DivisionLimits> getLimitsForDivision) {

      var unitsInOrderOfRemoval = GetRegimentsInOrderToRemove(army, getLimitsForDivision);

      // While there is a difference, find a random regiment to remove from the stronger side
      while (weightToRemove > 0) {
        var unitToRemove = unitsInOrderOfRemoval.Dequeue();

        double weight = SMil.GetUnitWeightOrExport(unitToRemove);
        if (weight <= weightToRemove) {
          unitToRemove.Parent.RemoveUnit(unitToRemove);
          weightToRemove -= weight;
        } else {
          double ratio = weightToRemove / weight;
          int menToRemove = (int)(ratio * unitToRemove.ExportData.Men);
          unitToRemove.ExportData.Men -= menToRemove;
          if (unitToRemove.ExportData.Men < 40)
            unitToRemove.Parent.RemoveUnit(unitToRemove);
          break;
        }
      }
    }

    void RemoveArtilleryFromArmy(Organization army, double weightToRemove) {
      // Remove random batteries, and finally random guns, from the side with more artillery
      while (weightToRemove > 0) {
        var batts = army.AllArtilleryBatteries().ToArray();
        var batteryToRemove = army.AllArtilleryBatteries().OrderBy(u => Rand.Next()).First();
        double weight = batteryToRemove.Units.Sum(u => SMil.GetUnitWeightOrExport(u));
        if (weight <= weightToRemove) {
          batteryToRemove.Parent.RemoveOrganization(batteryToRemove);
          weightToRemove -= weight;
        } else {
          double ratio = weightToRemove / weight;
          int gunsToRemove = (int)(ratio * batteryToRemove.Units.Count());
          for (int i = 0; i < gunsToRemove; i++) {
            batteryToRemove.RemoveUnit(batteryToRemove.Units.Last());
          }
          break;
        }
      }
    }

    public void Balance(IEnumerable<Organization> armies, IEnumerable<GcmShared.Division> divisions, BalanceInfo balanceInfo, Func<Organization, DivisionLimits> getLimitsForDivision) {
      // Compute army weights
      var armyWeights = armies.Select(a => new {
        FightingWeight = a.AllFightingUnits().Sum(u => SMil.GetUnitWeightOrExport(u)),
        ArtilleryWeight = a.AllArtilleryUnits().Sum(u => SMil.GetUnitWeightOrExport(u)),
        NumTroops = a.AllFightingUnits().Sum(u => u.ExportData.Men),
        NumRegiments = a.AllFightingUnits().Count(),
        NumGuns = a.AllArtilleryUnits().Count(),
        Side = a.Data.Side,
        Organization = a,
      }).ToDictionary(a => a.Side, a => a);

      var armyWithLessInfantry = armyWeights.OrderByDescending(a => a.Value.FightingWeight).Last().Value;

      // hard caps to prevent games from being too big.
      double hardCapTroops = Gcm.Var.Str["opt_s_battle_size_limit", "100000"].ToInt();
      double hardCapRegiments = hardCapTroops / 400;
      double hardCapTroopsPerSide = hardCapTroops / 2;
      double hardCapRegimentsPerSide = hardCapRegiments / 2;
      double hardCapMenPerGun = 440d;
      double hardCapNumGuns = hardCapTroops/hardCapMenPerGun;
      double hardCapNumGunsPerSide = hardCapNumGuns/ 2;

      if (balanceInfo.ApplyBattleSizeHardCaps) {
        // We only apply the caps if there are too many infantry on the smaller side.
        if (armyWithLessInfantry.NumTroops > hardCapTroopsPerSide || armyWithLessInfantry.NumRegiments > hardCapRegimentsPerSide) {
          // Cut the number of infantry down to the cap.
          double fractionOfTroopWeightToKeep = Math.Min(hardCapTroopsPerSide / armyWithLessInfantry.NumTroops, hardCapRegimentsPerSide / armyWithLessInfantry.NumRegiments);
          double troopWeightToRemove = (1d-fractionOfTroopWeightToKeep)*armyWithLessInfantry.FightingWeight;
          RemoveUnitsFromArmyByWeight(armyWithLessInfantry.Organization, troopWeightToRemove, getLimitsForDivision);

          // Cut the number of artillery down to the cap.
          int numTroopsRemaining = armyWithLessInfantry.Organization.AllFightingUnits().Sum(u => u.ExportData.Men);
          double idealNumGuns = Math.Min(hardCapNumGunsPerSide, numTroopsRemaining / hardCapMenPerGun);
          double fractionOfGunsWeightToKeep = idealNumGuns / armyWithLessInfantry.NumGuns;
          double gunWeightToRemove = (1d - fractionOfGunsWeightToKeep) * armyWithLessInfantry.ArtilleryWeight;
          RemoveArtilleryFromArmy(armyWithLessInfantry.Organization, gunWeightToRemove);

          // Print what changes were applied here.
          int numRegimentsRemaining = armyWithLessInfantry.Organization.AllFightingUnits().Count();
          int numGunsRemaining = armyWithLessInfantry.Organization.AllArtilleryUnits().Count();
          

          // Since this army has changed, recompute the total weights
          armyWeights = armies.Select(a => new {
            FightingWeight = a.AllFightingUnits().Sum(u => SMil.GetUnitWeightOrExport(u)),
            ArtilleryWeight = a.AllArtilleryUnits().Sum(u => SMil.GetUnitWeightOrExport(u)),
            NumTroops = a.AllFightingUnits().Sum(u => u.ExportData.Men),
            NumRegiments = a.AllFightingUnits().Count(),
            NumGuns = a.AllArtilleryUnits().Count(),
            Side = a.Data.Side,
            Organization = a,
          }).ToDictionary(a => a.Side, a => a);
        }
      }

      // Get the army with stronger infantry, and find the difference
      var armyWithMoreInfantry = armyWeights.OrderByDescending(a => a.Value.FightingWeight).First().Value;
      double infDifference = Math.Abs(armyWeights[1].FightingWeight - armyWeights[2].FightingWeight);

      // Now with differently weighted sides, we need to do this.
      // This applies the InfantryMultiplier that can be set for non-Campaign games.
      if (armyWeights[1].FightingWeight * balanceInfo.Team2InfantryMultiplier > armyWeights[2].FightingWeight) {
        armyWithMoreInfantry = armyWeights[1];
        infDifference = armyWeights[1].FightingWeight - armyWeights[2].FightingWeight / balanceInfo.Team2InfantryMultiplier;
      } else {
        armyWithMoreInfantry = armyWeights[2];
        infDifference = armyWeights[2].FightingWeight - armyWeights[1].FightingWeight * balanceInfo.Team2InfantryMultiplier;
      }

      // While there is a difference, find a random regiment to remove from the stronger side
      RemoveUnitsFromArmyByWeight(armyWithMoreInfantry.Organization, infDifference, getLimitsForDivision);

      // Merge small brigades
      foreach (var army in armies)
        foreach (var org in army.AllOrganizations.OfLevel(Levels.Division).ToList())
          MergeBrigades(org, UnitTypes.Infantry, 2, 5);

      // Drop any brigades that are now empty due to balancing
      foreach (var army in armies)
        Mil.RemoveEmptySubOrganizations(army);

      // Find the army with stronger artillery
      var armyWithArtillery = armyWeights.OrderByDescending(a => a.Value.ArtilleryWeight).First().Value;
      double artyDifference = Math.Abs(armyWeights[1].ArtilleryWeight - armyWeights[2].ArtilleryWeight);

      // Now with differently weighted sides, we need to do this.
      // This applies the ArtilleryMultiplier that can be set for non-Campaign games.
      if (armyWeights[1].ArtilleryWeight * balanceInfo.Team2ArtilleryMultiplier > armyWeights[2].ArtilleryWeight) {
        armyWithArtillery = armyWeights[1];
        artyDifference = armyWeights[1].ArtilleryWeight - armyWeights[2].ArtilleryWeight / balanceInfo.Team2ArtilleryMultiplier;
      } else {
        armyWithArtillery = armyWeights[2];
        artyDifference = armyWeights[2].ArtilleryWeight - armyWeights[1].ArtilleryWeight * balanceInfo.Team2ArtilleryMultiplier;
      }

      // Remove random batteries, and finally random guns, from the side with more artillery
      RemoveArtilleryFromArmy(armyWithArtillery.Organization, artyDifference);

      // Merge small batteries
      foreach (var army in armies)
        foreach (var org in army.AllOrganizations.OfLevel(Levels.Division).ToList())
          MergeBrigades(org, UnitTypes.Artillery, 2, 6);

      // Drop empty batteries
      foreach (var army in armies)
        Mil.RemoveEmptySubOrganizations(army);

      // Compute final army weights
      /*var finalArmyWeights = armies.Select(a => new
      {
          FightingWeight = a.AllFightingUnits().Sum(u =>  SMil.GetUnitWeightOrExport(u)),
          ArtilleryWeight = a.AllArtilleryUnits().Sum(u =>  SMil.GetUnitWeightOrExport(u)),
          Side = a.Data.Side,
          Organization = a,
      }).ToDictionary(a => a.Side, a => a);*/
    }


    void MoveUnitsFromOneToOther(Organization start, Organization dest) {
      foreach (var unit in start.Units.ToList()) {
        unit.Parent.RemoveUnit(unit);
        dest.AddUnit(unit);
      }
    }

    void MergeBrigades(Organization division, int unitType, int sizeToMerge, int maxSizeToForm) {
      var brigades = division.AllOrganizations
          .OfLevel(Levels.Brigade)
          .Where(b => b.GetUnitType() == unitType)
          .ToList();

      if (brigades.Count < 2 || brigades.Sum(b => b.NumUnits) <= sizeToMerge)
        return;

      var order = brigades.OrderBy(b => b.NumUnits).ToList();

      while (order.First().NumUnits <= sizeToMerge) {
        var start = order.First();
        var dest = order.Skip(1).First();

        // ALWAYS merge brigades of size 1
        if (start.NumUnits <= 1 || start.NumUnits + dest.NumUnits <= maxSizeToForm) {
          MoveUnitsFromOneToOther(start, dest);
          start.Parent.RemoveOrganization(start);
          brigades.Remove(start);
          order = brigades.OrderBy(b => b.NumUnits).ToList();
        } else break;
      }
    }
  }
}
