using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GcmShared;
using Military;

namespace GcmShared.NewMilitary
{

    class ScnLoader : IScenarioLoader
    {
        Battle Battle;

        Military.IO.MilitaryReader Reader;

        public ScnLoader(Battle battle)
        {
            Battle = battle;
            Reader = new Military.IO.MilitaryReader();
        }

        public Dictionary<int, Organization> LoadArmies(IEnumerable<Division> divisions)
        {
            Battle.DivisionIndexById = new Dictionary<int, Organization>();
            Battle.OrganizationToDivisionID = new Dictionary<Organization, int>();

            foreach (var division in divisions)
            {
                var organization = Reader.ReadFromFile(division.DivisionXmlPath).Organizations.First();
                Battle.DivisionIndexById.Add(division.DivisionID, organization);
                Battle.OrganizationToDivisionID.Add(organization, division.DivisionID);
            }
            return Battle.DivisionIndexById;
        }
    }
}
