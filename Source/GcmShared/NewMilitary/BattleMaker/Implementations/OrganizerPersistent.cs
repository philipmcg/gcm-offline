using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GcmShared;
using Military;
using Utilities.GCSV;
using Utilities;

namespace GcmShared.NewMilitary {

  class ScnOrganizerPersistent : IScenarioOrganizer {
    bool allowCavalry;
    bool useTroopLimits;
    bool useRealBattlePenalties;
    OOBType type;
    Func<int, string> divisionFilePath;

    public ScnOrganizerPersistent(bool allowCavalry, Func<int, string> divisionFilePath, OOBType type, bool useTroopLimits, bool useRealBattlePenalties) {
      this.allowCavalry = allowCavalry;
      this.type = type;
      this.divisionFilePath = divisionFilePath;
      this.useTroopLimits = useTroopLimits;
      this.useRealBattlePenalties = useRealBattlePenalties;
    }
    public void Organize(IEnumerable<Division> divisions, int menGunRatio) {
      // For each division, reduce it to its limits set in the Division object
      foreach (var division in divisions) {
        var org = Military.IO.MilitaryIO.Reader.ReadFromFile(divisionFilePath(division.DivisionID)).FirstOrganization;

        if (!allowCavalry) {
          var cavalryBrigades = org.Organizations.Where(o => o.Data.Level == Levels.Brigade && o.GetUnitType() == UnitTypes.Cavalry).ToArray();
          foreach (var cb in cavalryBrigades) {
            org.RemoveOrganization(cb);
          }
        }

        RemoveInactiveUnits(org);

        if (useTroopLimits) {
          if (type == OOBType.PersistentDivisions) { // for campaign divisions, we limit by number of regiments, not number of men.
            int rankLimit = division.CD_Regts_Rank;
              var rand = division.GetRealBattleRandom();
            if (useRealBattlePenalties) {
              if (rand.Percent(25)) {
                if (rand.Percent(90)) {
                  rankLimit++;
                } else {
                  rankLimit += 2;
                }
              } 
            }
            int regtLimit = Math.Min(division.CD_Regts_Preference, rankLimit);
            if(useRealBattlePenalties) {
              if (rand.Percent(20)) {
                const int baseline = 7;
                int regimentCap = rand.InRange(baseline, Math.Max(baseline, rankLimit));
                regtLimit = Math.Min(regimentCap, regtLimit);
              }
            }
            division.CD_Regts_Rank_Modified = regtLimit;

            RemoveRegtsFromDivisionToLimit(org, division.CD_Regts_Rank_Modified);
          } else {
            RemoveTroopsFromDivisionToLimit(org, type == OOBType.PersistentDivisions ? division.CD_Men : division.RD_Men);
          }

          int numGuns = (type == OOBType.PersistentDivisions ? (useRealBattlePenalties ? division.CD_Guns_Penalty : division.CD_Guns) : division.RD_Guns);
          RemoveGunsFromDivisionToLimit(org, numGuns);
        }

        /*if (useGunPenalty) {
          RemovePercentageOfBatteries(org, division.CD_Guns_Penalty);
        }*/

        org.WriteToFile(divisionFilePath(division.DivisionID));
      }
    }

    void RemoveInactiveUnits(Organization org) {
      var deactivatedUnits = org.AllUnits.Where(u => !u.Data.Active).ToList();
      foreach (var unit in deactivatedUnits) {
        unit.Parent.RemoveUnit(unit);
      }
    }

    void RemoveRegtsFromDivisionToLimit(Organization div, int regtsLimit) {
      int totalRegts = div.AllFightingUnits().Count();

      // First remove brigades from the bottom of the division
      var brigadesToRemove = div.AllFightingBrigades().Reverse().ToArray();

      foreach (var brigade in brigadesToRemove) {
        if (totalRegts - brigade.NumUnits >= regtsLimit) {
          div.RemoveOrganization(brigade);
          totalRegts -= brigade.NumUnits;
        } else
          break;
      }

      // Then remove regiments from the bottom of the division
      var regimentsToRemove = Mil.GetFightingUnitsInReverseOrderOfPriority(div);

      foreach (var regt in regimentsToRemove) {
        if (totalRegts - 1 >= regtsLimit) {
          regt.Parent.RemoveUnit(regt);
          totalRegts -= 1;
        } else
          break;
      }
    }

    void RemoveTroopsFromDivisionToLimit(Organization div, int menLimit) {

      int totalTroops = div.AllFightingUnits().Sum(u => Mil.GetValidMenForRegiment(u));

      // First remove brigades from the bottom of the division
      var brigadesToRemove = div.AllFightingBrigades().Reverse().ToArray();

      foreach (var brigade in brigadesToRemove) {
        int brigadeMen = brigade.AllFightingUnits().Sum(u => Mil.GetValidMenForRegiment(u));
        if (totalTroops - brigadeMen >= menLimit) {
          div.RemoveOrganization(brigade);
          totalTroops -= brigadeMen;
        } else
          break;
      }

      // Then remove regiments from the bottom of the division
      var regimentsToRemove = Mil.GetFightingUnitsInReverseOrderOfPriority(div);

      foreach (var regt in regimentsToRemove) {
        int men = Mil.GetValidMenForRegiment(regt);

        if (totalTroops - men >= menLimit - (men / 2d)) {
          regt.Parent.RemoveUnit(regt);
          totalTroops -= men;
        } else
          break;
      }
    }

    void RemoveGunsFromDivisionToLimit(Organization div, int gunLimit) {
      int totalGuns = div.AllArtilleryUnits().Count();

      // First remove brigades from the bottom of the division
      var brigadesToRemove = div.AllArtilleryBatteries().Reverse().ToArray();

      // 50% of the time, we randomize the order that batteries are removed, so you can't choose to only bring the guns you want.
      if (Rand.Percent(50)) {
        brigadesToRemove.Shuffle();
      }

      foreach (var brigade in brigadesToRemove) {
        int brigadeGuns = brigade.AllArtilleryUnits().Count();
        if (totalGuns - brigadeGuns >= gunLimit) {
          div.RemoveOrganization(brigade);
          totalGuns -= brigadeGuns;
        } else
          break;
      }

      // Then remove regiments from the bottom of the division
      var regimentsToRemove = div.AllArtilleryUnits().Reverse().ToArray();

      foreach (var regt in regimentsToRemove) {
        if (totalGuns - 1 >= gunLimit) {
          regt.Parent.RemoveUnit(regt);
          totalGuns -= 1;
        } else
          break;
      }
    }
  }
}
