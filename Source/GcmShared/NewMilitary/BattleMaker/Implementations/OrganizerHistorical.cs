using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GcmShared;
using Military;
using Utilities.GCSV;
using Utilities;

namespace GcmShared.NewMilitary
{

    class ScnOrganizerHistorical : IScenarioOrganizer, IScenarioArmyUnifier
    {
        class Battlegroup
        {
            public readonly Organization Organization;
            public readonly int Type;
            public readonly int Strength;
            public readonly int Side;
            public bool Taken;

            public Battlegroup(Organization org, int type)
            {
                Organization = org;
                Type = type;
                Taken = false;
                Strength = type == UnitTypes.Infantry ? org.AllUnits.Sum(u => u.Data.Men) : org.AllUnits.Count();
                Side = org.Data.Side;
            }
        }

        Dictionary<int, Battlegroup> Battlegroups;
        Military.IO.MilitaryWriter Writer;


        List<Organization> Corps;
        List<Organization> Armies;

        Dictionary<int, Organization> PlayerDivisions;
        int[] Sides = { 1, 2 };

        public ScnOrganizerHistorical(Dictionary<string, IGCSVHeader> headers, IEnumerable<string> battlegroupPaths, bool allowCavalry)
        {
            Writer = new Military.IO.MilitaryWriter(headers);
            var organizations = LoadOrganizationsFromFiles(battlegroupPaths).Where(o => allowCavalry || o.GetUnitType() != UnitTypes.Cavalry);
            LoadOrganizations(organizations);
        }

        IEnumerable<Organization> LoadOrganizationsFromFiles(IEnumerable<string> paths)
        {
            Military.IO.MilitaryReader reader = new Military.IO.MilitaryReader();

            foreach (var path in paths)
            {
                var group = reader.ReadFromFile(path);
                var org = group.Organizations.First();
                yield return org;
            }
        }

        void LoadOrganizations(IEnumerable<Organization> organizations)
        {
            Battlegroups = new Dictionary<int, Battlegroup>();
            Corps = new List<Organization>();
            Armies = new List<Organization>();

            int id = 1;

            foreach (var org in organizations)
            {
                if (org.Data.Level == 5)
                {
                    Battlegroups.Add(id, new Battlegroup(org, org.GetUnitType()));
                    id++;
                }
                else if (org.Data.Level == 3)
                {
                    Corps.Add(org);
                }
                else if (org.Data.Level == 2)
                {
                    Armies.Add(org);
                }
            }
        }

        public void Organize(IEnumerable<Division> divisions, int menGunRatio)
        {
            RandomCreator creator = RandomCreator.Instance;

            PlayerDivisions = new Dictionary<int, Organization>();

            // Get infantry
            foreach (var division in divisions)
            {
                Organization org = Mil.CreateOrganization(division.Side, Levels.Division);
                
                // Create the commander using the player character's name
                var cdr = creator.CreateNamedCommander(4, division.Side, UnitTypes.Infantry, 1, division.CharacterName, () => 3);
                cdr.Data.Rank = (int)Rank.MajGen;
                cdr.AssignCommand(org);
                org.Data.Name = division.CharacterName.Last + "'s Division";

                PlayerDivisions.Add(division.DivisionID, org);

                GetInfantryForDivisionByNumbers(org, division.RD_Men);
            }

            // Get artillery
            int gunsPerSide = GetNumberOfGunsPerSide(divisions, menGunRatio);

            foreach (var side in Sides)
            {
                AssignArtilleryToSide(side, gunsPerSide, divisions.Where(d => d.Side == side));
            }

            Mil.SetTagsOnNonpersistentUnitsAndCommanders(PlayerDivisions.Values);
        }

        /// <summary>
        /// Saves this player's division in xml format to the given path.
        /// </summary>
        public void SaveDivision(Division player, string path)
        {
            Writer.WriteToFile(path, PlayerDivisions[player.DivisionID].AsGroup());
        }

        /// <summary>
        /// Gets the ideal number of artillery per side, proportional to the players' desired infantry strengths.
        /// </summary>
        int GetNumberOfGunsPerSide(IEnumerable<Division> divisions, int menGunRatio)
        {
            return (divisions.Sum(d => d.RD_Men) / menGunRatio) / 2;
        }


        Battlegroup[] GetBatteries(int side, int numGuns)
        {
            int strength = 0;

            return Battlegroups.Values
                .Where(g => !g.Taken && g.Type == UnitTypes.Artillery && g.Side == side)
                .OrderBy(g => Rand.Next())
                .TakeWhile(g => { strength += g.Strength; return strength < numGuns; })
                .ToArray();
        }

        Organization[] GetBatteriesForSide(int side, int numGuns)
        {
            var battlegroups = Enumerable.Range(0, 4).Select(i => GetBatteries(side, numGuns))
                .OrderBy(gs => gs.Sum(g => g.Strength) - numGuns).First();

            battlegroups.ForEach(g => g.Taken = true);

            return battlegroups
                .Select(g => g.Organization)
                .ToArray();
        }

        void AssignArtilleryToSide(int side, int numGuns, IEnumerable<Division> divisions)
        {
            Stack<Organization> batteries = new Stack<Organization>(GetBatteriesForSide(side, numGuns));
            divisions = divisions.ToArray();


            while (batteries.Count > 0)
            {
                bool stillLacking = false;

                foreach (var div in divisions.OrderBy(d => Rand.Next()))
                {
                    int guns = div.RD_Guns;

                    var org = PlayerDivisions[div.DivisionID];
                    int currentGuns = org.AllUnits.Where(u => u.Data.Type == UnitTypes.Artillery).Count();

                    if (currentGuns < guns)
                    {
                        stillLacking = true;
                        org.AddOrganization(batteries.Pop());
                    }

                    if (batteries.Count == 0)
                        return;
                }

                // If all players think they have enough guns, add the remaining batteries to randomly selected divisions.
                if (!stillLacking)
                {
                    while (batteries.Count > 0)
                    {
                        int divID = divisions.OrderBy(d => Rand.Next()).First().DivisionID;
                        PlayerDivisions[divID].AddOrganization(batteries.Pop());
                    }
                }
            }
        }

        void GetInfantryForDivisionByNumbers(Organization org, int numMen)
        {
            // Get four sets of battlegroups
            var tries = new List<IEnumerable<Battlegroup>> {
                GetBrigades(org.Data.Side, numMen),
                GetBrigades(org.Data.Side, numMen),
                GetBrigades(org.Data.Side, numMen),
                GetBrigades(org.Data.Side, numMen),
            };

            // take the set with the closest number of troops to desired
            var set = tries.OrderBy(gs => gs.Sum(g => g.Strength) - numMen).First();

            foreach (var battlegroup in set)
            {
                battlegroup.Taken = true;
                org.AddOrganization(battlegroup.Organization);
            }

            // remove random regiments to bring the strength down to desired.
            int totalStrength = org.AllUnits.Sum(u => u.Data.Men);

            while (totalStrength > numMen + 300)
            {
                var unitToRemove = org.AllUnits.OrderBy(u => Rand.Int(100)).First();
                unitToRemove.Parent.RemoveUnit(unitToRemove);
                totalStrength -= unitToRemove.Data.Men;
            }
        }

        /// <summary>
        /// Gets random brigades until it has more men than requested.  The numbers can be reduced by the balancer later.
        /// </summary>
        IEnumerable<Battlegroup> GetBrigades(int side, int numMen)
        {
            int strength = 0;

            return Battlegroups.Values
                .Where(g => !g.Taken && g.Type != UnitTypes.Artillery && g.Side == side)
                .OrderBy(g => Rand.Int(1000))
                .TakeWhile(g => { bool done = strength > numMen; strength += g.Strength; return !done; }).ToArray();
        }



        public IEnumerable<Organization> Unify(IEnumerable<Organization> divisions)
        {
            foreach (var side in Sides)
            {
                var army = Armies.Where(a => a.Data.Side == side).OrderBy(o => Rand.Next()).First();
                var corps = Corps.Where(a => a.Data.Side == side).OrderBy(o => Rand.Next()).First();

                army.AddOrganization(corps);
                foreach (var div in divisions.Where(d => d.Data.Side == side))
                {
                    corps.AddOrganization(div);
                }
                yield return army;
            }
        }
    }
}
