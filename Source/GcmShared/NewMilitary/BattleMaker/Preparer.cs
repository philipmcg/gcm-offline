using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GcmShared;
using Military;
using Utilities;


namespace GcmShared.NewMilitary {

  class ScnPreparer : IScenarioPreparer {
    Battle Battle;
    Dictionary<string, string> SpecialUnits;
    RandomCreator Creator;

    public ScnPreparer(Battle battle) {
      Battle = battle;
      SpecialUnits = Gcm.Data.GCSVs["specials"].ToDictionary(l => l["number"] + l["state"], l => l["type"]);
      Creator = RandomCreator.Instance;
    }

    public void Prepare(IEnumerable<Organization> armies) {
      Mil.SetTagsOnNonpersistentUnitsAndCommanders(armies);

      foreach (var army in armies) {
        var allUnits = army.AllUnits.ToArray();

        string pfx = Gcm.Data.GCSVs["factions"][army.Data.Side]["pfx"];

        foreach (var unit in allUnits) {
          SMil.ExportUnitData(unit);
        }

        foreach (var unit in allUnits.Where(u => u.Data.Type == UnitTypes.Infantry)) {
          // unit.ExportData.WeaponId = 46; // Springfield 1863s
          unit.ExportData.WeaponId = 100 + unit.ExportData.Marksmanship; // IDS_ARSN_GCM_Musket_ firearm

          if (unit.Data.Type == UnitTypes.Cavalry) {
            unit.ExportData.Flag1 = Gcm.Var.Int[pfx + "c_flag1"];
          }

          // Not all of the men get to fight.
          unit.ExportData.Men = Mil.GetValidMenForRegiment(unit);

          // Special units
          if (unit.Data.Type == UnitTypes.Infantry) {
            unit.ExportData.ClassId = Creator.Class(PickUniform(unit, army.Data.Side));

            string key = unit.Data.RegimentNumber + unit.Data.State;
            if (SpecialUnits.ContainsKey(key)) {
              switch (SpecialUnits[key]) {
                case "zoa": // Zouaves (blue pants)
                  unit.ExportData.ClassId = Creator.Class("UGLB_USA_Inf_6");
                  unit.ExportData.Edged += 1;
                  break;
                case "zo": // Zouaves
                  if (unit.Data.Edged > 0) {
                    unit.ExportData.ClassId = Creator.Class("UGLB_USA_Inf_2");
                    unit.ExportData.Edged += 1;
                  }
                  break;
                case "er": // Irish Brigade
                  unit.ExportData.Flag2 = Creator.Flag("GFX_US_Irish_Regt");
                  unit.ExportData.Edged += 2;
                  unit.ExportData.Close += 2;
                  unit.ExportData.Firearm -= 1;
                  break;
                case "ir": // Iron Brigade
                  unit.ExportData.ClassId = Creator.Class("UGLB_USA_Inf_3");
                  unit.ExportData.Open += 1;
                  break;
                case "1mn": // 1st Minnesota
                  unit.ExportData.Close += 1;
                  unit.ExportData.Edged += 2;
                  break;
                case "20me": // 20th Maine
                  unit.ExportData.Firearm += 1;
                  break;
              }
            } else {
              if (unit.Data.Experience >= 5 && unit.Data.Marksmanship >= 1.5 && army.Data.Side == Factions.USA)
                unit.ExportData.ClassId = Creator.Class("UGLB_USA_Inf_5"); // sharpshooters
            }
          }

        }

        foreach (var unit in allUnits.Where(u => u.Data.Type == UnitTypes.Artillery)) {
          unit.ExportData.Flag1 = Gcm.Var.Int[pfx + "a_flag1"];
          unit.ExportData.Men = 10;
        }

        /* foreach (var commander in army.AllCommanders)
         {
             commander.Data.SetSkills(v => Math.Floor(v));
         }*/
      }
    }

    static string[] westUSA = new[]{
                           "u_ar",
"u_al",
"u_ca",
"u_co",
"u_ga",
"u_il",
"u_in",
"u_ia",
"u_ks",
"u_ksm",
"u_ky",
"u_la",
"u_mn",
"u_mo",
"u_ne",
"u_nv",
"u_nc",
"u_oh",
"u_or",
"u_sc",
"u_tn",
"u_wi",
"u_mi",
};

    static string[] westCSA = new[]{"c_ms",
                                        "c_tn",
                                        "c_ar",
                                        "c_la",
                                        "c_nc",
                                        "c_ky",
                                        "c_tx",
                                                };

    string PickUniform(Unit unit, int factionID) {
      if (factionID == Factions.USA) {
        int uid = (unit.Data.Id + unit.Data.RegimentNumber) % 20;
        if (unit.Data.State == "u_us") {
          return "UGLB_USA_Inf_3";
        } else {
          string theatre = westUSA.Contains(unit.Data.State) ? "West_" : "East_";
          string vet = unit.Data.Open > 0 ? "Vet_" : "";
          return "UGLB_USA_Inf_GCM_{0}{1}{2}".With(theatre, vet, uid);
        }
      } else {
        int uid = (unit.Data.Id + unit.Data.RegimentNumber) % 10;
        string theatre = westCSA.Contains(unit.Data.State) ? "West_" : "East_";
        string vet = unit.Data.Open > 0 ? "Vet_" : "";
        return "UGLB_CSA_Inf_GCM_{0}{1}{2}".With(theatre, vet, uid);
      }
    }

    void AssignPlayersToLocations() {
      foreach (var side in Battle.Sides) {
        int k = 0;
        foreach (var division in side.Divisions) {
          // TODO check
          side.Locations.Players[k] = new Dictionary<string, string>();
          side.Locations.Players[k]["id"] = division.DivisionID.ToString();
          side.Locations.Players[k]["name"] = division.UserName.ToString();
          side.Locations.Players[k]["side"] = division.Side.ToString();
          k++;
        }
      }
    }

    public Dictionary<int, Location> SetLocationsOnUnits(Dictionary<int, Organization> divisions, IEnumerable<Organization> armies, bool changeDirectionToFaceMapCenter) {
      AssignPlayersToLocations();

      Dictionary<int, Location> locations = new Dictionary<int, Location>();

      foreach (var side in Battle.Sides) {
        var divisionLocations = side.Locations.DivPoints;
        var armyDirection = side.Locations.Direction;

        var sideArmies = armies.Where(o => o.Data.Side == side.ID).ToList();

        // Assign army location to all units, and assign tag numbers to every unit or commander
        foreach (var army in sideArmies) {
          Location location = new Location(side.Locations.Main, armyDirection);

          var off = army.ThisAndAllOrganizations
              .OfAtLeastLevel(Levels.Corps).ToList();
          off.Select(o => o.Commander)
              .ForEach(c => { locations.Add((int)c.Tag, location.Fuzzy()); });
        }

        int k = 0;

          if (changeDirectionToFaceMapCenter) {
            var location = divisionLocations.AveragePoint();
            var offset = new System.Drawing.Point(Rand.Int(1, 300) * Rand.Sign(), Rand.Int(1, 300) * Rand.Sign());
            var offsetLocation = location.Add(offset);
            armyDirection = MapLocations.DirectionFromPointToPoint(offsetLocation, side.Locations.CenterOfMapArea);
            //divisionDirection = MapLocations.DirectionFromPointToPoint(offsetLocation, side.Locations.CenterOfMapArea);
          }
        // Then assign division locations to the divisions
        foreach (var div in side.Locations.Players) {
          var divisionDirection = armyDirection;
          Location location = new Location(divisionLocations[k], divisionDirection);
          int id = div["id"].ToInt();
          Organization org = divisions[id];
          org.AllUnits.ForEach(u => { locations[(int)u.Tag] = location; });
          org.AllCommanders.ForEach(c => { locations[(int)c.Tag] = location; });

          k++;
        }
      }

      return locations;
    }

    Division FindEligibleDivisionThatIsntAlreadyAssignedCommand(int side) {
      return CorpsCommandersPerSide[side].OrderBy(d => Guid.NewGuid()).First(d => !Battle.DivisionIDsAlreadyAssignedToCorpsCommand.Contains(d.DivisionID));
    }

    List<Division>[] CorpsCommandersPerSide;

    /// <summary>
    /// Splits the divisions into multiple corps for each side.
    /// </summary>
    public void ReorganizeForMultipleCorps(Dictionary<int, Organization> divisions, IEnumerable<Organization> armies, Dictionary<int, Location> unitLocations, Division[] armyCommanders) {
      CorpsCommandersPerSide = new List<Division>[armies.Max(o => o.Data.Side) + 1];
      Organization[] divs = divisions.Values.ToArray();

      double divsPerCorps = 2.2;
      Dictionary<int, Queue<Organization>> corpsForside = new Dictionary<int, Queue<Organization>>();
      foreach (var army in armies) {
        int side = army.Data.Side;
        var oldCorps = army.Organizations.First();
        army.RemoveOrganization(oldCorps);
        unitLocations.Remove(oldCorps.Commander.Data.Id);
        var divsForSide = new Stack<Organization>(divs.OrderBy(d => unitLocations[d.Commander.Data.Id].Loc.X).Where(d => d.Data.Side == side).ToArray());

        int numCorpsForSide = Math.Max(1, (int)(divsForSide.Count / (divsPerCorps)));
        if (divsForSide.Count >= 4 && divsForSide.Count <= 5)
          numCorpsForSide = 2;
        CorpsCommandersPerSide[side] = Battle.Sides[side].DivisionsOrderedBySuitabilityForHighCommand.Take(numCorpsForSide).ToList();

        corpsForside[side] = new Queue<Organization>();
        for (int i = 0; i < numCorpsForSide; i++) {
          var corpsCdr = RandomCreator.Instance.CreateRandomCommander(side);
          var corps = RandomCreator.Instance.CreateEmptyCorps(side);
          Mil.AssignCommand(corpsCdr, corps);
          corpsForside[side].Enqueue(corps);
          army.AddOrganization(corps);
        }

        var corpsSizes = AllocateItemsIntoEvenGroupsRandomly(divsForSide.Count, numCorpsForSide);

        foreach (var corpsSize in corpsSizes) {
          var corps = corpsForside[side].Dequeue();
          for (int i = 0; i < corpsSize; i++) {
            corps.AddOrganization(divsForSide.Pop());
          }
        }
      }

      Mil.SetTagsOnNonpersistentUnitsAndCommanders(armies);

      foreach (var army in armies) {
        // For each corps, find the division nearest the center or the corps, and move it to the front of the list so it gets the corps commander.
        foreach (var corps in army.Organizations) {
          var corpsCenter = corps.Organizations.AveragePt(d => unitLocations[d.Commander.Data.Id].Loc);
          var centerDivision = corps.Organizations.OrderBy(d => unitLocations[d.Commander.Data.Id].Loc.Distance(corpsCenter)).First();

          int currentCenterDivisionId = Battle.OrganizationToDivisionID[centerDivision];

          // If this division is not eligible for corps command, swap it with one that is.
          if (!CorpsCommandersPerSide[army.Data.Side].Any(d => d.DivisionID == currentCenterDivisionId)) {
            Division thisDivision = Battle.Divisions.First(d => d.DivisionID == currentCenterDivisionId);
            Division otherDivision = FindEligibleDivisionThatIsntAlreadyAssignedCommand(army.Data.Side);
            SwapDivisionsAndTheirUnitsLocations(unitLocations, Battle.DivisionIndexById[thisDivision.DivisionID], Battle.DivisionIndexById[otherDivision.DivisionID]);
            centerDivision = Battle.DivisionIndexById[otherDivision.DivisionID];
          }

          Battle.DivisionIDsAlreadyAssignedToCorpsCommand.Add(Battle.OrganizationToDivisionID[centerDivision]);

          // Move center division to first spot in corps, so it gets corps commander.
          corps.RemoveOrganization(centerDivision);
          corps.AddOrganizationToFirstSlot(centerDivision);

          var corpsLocation = unitLocations[centerDivision.Commander.Data.Id].Fuzzy();
          unitLocations.Add((int)corps.Commander.Tag, corpsLocation);

          // Locate all the other divisions in the corps near this center division
          if (Battle.SpawnByCorps) {
            var angleAway = Rand.NextDouble() * (2 * Math.PI);
            foreach (var division in corps.Organizations.Skip(1)) {
              angleAway += (2 * Math.PI) / (Math.Max(1, corps.NumOrganizations - 1));
              double angleOffset = Rand.Double() * (2 * Math.PI) * 0.2;
              var radius = Rand.CurvedDouble(1000) + 6000 + Rand.Int(500);
              var x = Math.Sin(angleAway + angleOffset) * radius;
              var y = Math.Cos(angleAway + angleOffset) * radius;
              var location = corpsLocation;
              location.Loc.X += (float)x;
              location.Loc.Y += (float)y;
              MoveOrganizationToLocation(division, unitLocations, location);
            }
          }
        }

        var armyLocation = unitLocations[(int)army.Organizations.First().Commander.Tag];
        // If a special army commander has been set, assign army location to the location of the preferred army commander's division's commander.
        if (armyCommanders != null && armyCommanders[army.Data.Side] != null)
          armyLocation = unitLocations[(int)divisions[armyCommanders[army.Data.Side].DivisionID].Commander.Tag];
        unitLocations[(int)army.Commander.Tag] = armyLocation;
        army.Data.Name += " [{0}]".With(Utilities.Number.IntToBase52(Battle.BattleID));
      }
    }

    void MoveOrganizationToLocation(Organization organization, Dictionary<int, Location> unitLocations, Location location) {
      ApplyLocationToAllUnitsInOrganization(organization, unitLocations, location);
      // This doesn't account for the SidePoints that appear in the flash map
      /*
      var pi1 = side.Locations.Players.First(p => p["id"] == Battle.OrganizationToDivisionID[div1].ToString());
      var pi2 = side.Locations.Players.First(p => p["id"] == Battle.OrganizationToDivisionID[div2].ToString());
      SwapDictionaries(pi1, pi2);
      */
    }

    void SwapDivisionsAndTheirUnitsLocations(Dictionary<int, Location> unitLocations, Organization div1, Organization div2) {
      Location loc1 = unitLocations[(int)div1.Commander.Tag];
      Location loc2 = unitLocations[(int)div2.Commander.Tag];
      ApplyLocationToAllUnitsInOrganization(div1, unitLocations, loc2);
      ApplyLocationToAllUnitsInOrganization(div2, unitLocations, loc1);
      var div1Parent = div1.Parent;
      var div2Parent = div2.Parent;
      div1.Parent.RemoveOrganization(div1);
      div2.Parent.RemoveOrganization(div2);
      div1Parent.AddOrganization(div2);
      div2Parent.AddOrganization(div1);
      var side = Battle.Sides[div1.Data.Side];
      var pi1 = side.Locations.Players.First(p => p["id"] == Battle.OrganizationToDivisionID[div1].ToString());
      var pi2 = side.Locations.Players.First(p => p["id"] == Battle.OrganizationToDivisionID[div2].ToString());
      SwapDictionaries(pi1, pi2);
    }

    void SwapDictionaries<K, V>(Dictionary<K, V> d1, Dictionary<K, V> d2) {
      var temp = d1.ToDictionary(p => p.Key, p => p.Value);
      d1.Clear();
      d2.ForEach(p => d1.Add(p.Key, p.Value));
      d2.Clear();
      temp.ForEach(p => d2.Add(p.Key, p.Value));
    }

    void ApplyLocationToAllUnitsInOrganization(Organization org, Dictionary<int, Location> unitLocations, Location location) {
      org.AllUnits.ForEach(u => { unitLocations[(int)u.Tag] = location; });
      org.AllCommanders.ForEach(c => { unitLocations[(int)c.Tag] = location; });
    }

    int[] AllocateItemsIntoEvenGroupsRandomly(int total, int numGroups) {
      int[] groups = new int[numGroups];
      double avgGroupSize = total / (double)numGroups;
      int remaining = total;
      int numExtras = total % numGroups;

      for (int i = 0; i < numGroups; i++) {
        groups[i] = (int)avgGroupSize + (numExtras > 0 ? 1 : 0);
        remaining -= groups[i];
        numExtras--;
      }

      groups.Shuffle();

      return groups;
    }
  }
}
