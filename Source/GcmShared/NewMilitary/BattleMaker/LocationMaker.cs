using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;

using GcmShared;

using Utilities;

namespace GcmShared.NewMilitary
{
    class ScnLocationMaker : IScenarioLocationMaker
    {
        public void GetArmyLocations(Battle battle)
        {
            MapLocations MapLocations;

            int spread = Gcm.Var.Str["opt_s_start_location_spread"].ToInt();
            battle.SpawnByCorps = spread % 10 == 1;
            MapLocations = new MapLocations(battle.Divisions.Count, spread);
            Point[] armyPoints = MapLocations.GetTwoEdgePoints();
            foreach (int side in new int[] { 1, 2 }.ShuffleNew())
            {
                battle.Sides[side].Locations = MapLocations.GetLocationsForSide(armyPoints[side-1], battle.Divisions.Count(d => d.Side == side), side);
                battle.Sides[side].Locations = MapLocations.GetLocationsForSide(armyPoints[side - 1], battle.Divisions.Count(d => d.Side == side), side);
            }
            battle.Sides[1].Locations.Prepare(battle);
            battle.Sides[2].Locations.Prepare(battle);
        }


        public void SaveObjectiveLocations(Battle battle)
        {
            battle.Objectives =  Objectives.CreateMapLocationsFile(GcmShared.Files.MapLocations(battle.BattleID, false), battle);
        }
    }
}
