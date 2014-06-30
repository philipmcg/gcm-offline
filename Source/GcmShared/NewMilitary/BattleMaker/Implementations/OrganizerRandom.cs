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

    class ScnOrganizerRandom : IScenarioOrganizer
    {
        RandomDivisionGenerator generator;
        Dictionary<int, Organization> PlayerDivisions;

        public ScnOrganizerRandom(bool allowCavalry, int battleID)
        {
            generator = new RandomDivisionGenerator();
        }

        public void Organize(IEnumerable<Division> divisions, int menGunRatio)
        {
            RandomCreator creator = RandomCreator.Instance;

            PlayerDivisions = new Dictionary<int, Organization>();

            // Get infantry
            foreach (var division in divisions)
            {
                Organization org = generator.CreateRandomDivision(division.RD_Men, 0, division.CharacterName, division);

                // Create the commander using the player character's name
                var cdr = creator.CreateNamedCommander(4, division.Side, UnitTypes.Infantry, 1, division.CharacterName, () => 3);
                cdr.Data.Rank = (int)Rank.MajGen;
                cdr.AssignCommand(org);
                org.Data.Name = division.CharacterName.Last + "'s Division";

                PlayerDivisions.Add(division.DivisionID, org);
            }

            // Make artillery for each side
            var numGuns = GetNumberOfGunsPerSide(divisions, menGunRatio);
            var sides = divisions.Select(d => d.Side).Distinct().ToArray();
            foreach (var side in sides)
            {
                var batteries = GetBatteriesForSide(side, numGuns);
                AssignArtilleryToSide(side, divisions.Where(d => d.Side == side), new Stack<Organization>(batteries));
            }

            Mil.SetTagsOnNonpersistentUnitsAndCommanders(PlayerDivisions.Values);
        }

        /// <summary>
        /// Gets the ideal number of artillery per side, proportional to the players' desired infantry strengths.
        /// </summary>
        int GetNumberOfGunsPerSide(IEnumerable<Division> divisions, int menGunRatio)
        {
            return (divisions.Sum(d => d.RD_Men) / menGunRatio) / 2;
        }

        /// <summary>
        /// Saves this player's division in xml format to the given path.
        /// </summary>
        public void SaveDivision(Division player, string path)
        {
            Military.IO.MilitaryIO.Writer.WriteToFile(path, PlayerDivisions[player.DivisionID].AsGroup());
        }



        IEnumerable<Organization> GetBatteriesForSide(int side, int numGuns)
        {
            return generator.RD_CreateBatteries(side, numGuns + 2);
        }

        void AssignArtilleryToSide(int side, IEnumerable<Division> divisions, Stack<Organization> batteries)
        {
            divisions = divisions.ToArray();

            while (batteries.Count > 0)
            {
                bool stillLacking = false;

                foreach (var div in divisions.OrderBy(d => Rand.Int(100)))
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

                // If all players think they have enough guns, add the remaining batteries to the divisions that want guns -- so the ratios of
                // desired artillery to real artillery are about equal.
                // If any divisions don't want guns at all, don't give them any.
                if (!stillLacking)
                {
                    while (batteries.Count > 0)
                    {
                        int divID = divisions.OrderBy(d => d.RD_Guns == 0 ? 1000 : PlayerDivisions[d.DivisionID].AllArtilleryUnits().Count() / (double)d.RD_Guns).First().DivisionID;
                        PlayerDivisions[divID].AddOrganization(batteries.Pop());
                    }
                }
            }
        }
    }

}
