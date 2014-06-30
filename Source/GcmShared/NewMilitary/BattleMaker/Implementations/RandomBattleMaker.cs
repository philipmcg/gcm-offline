using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GcmShared;
using Military;
using Utilities.GCSV;

using System.IO;

using Utilities;

namespace GcmShared.NewMilitary
{

    public class RandomBattleMaker : BattleMaker
    {
        public RandomBattleMaker() : base() { }

        protected override void Organize()
        {
            Organizer = new ScnOrganizerRandom(Gcm.Var.Str["opt_s_cavalry"] == "1", Battle.BattleID);

            Organizer.Organize(Battle.Divisions, Battle.MenGunRatio);

            foreach (var division in Battle.Divisions)
            {
                division.DivisionXmlPath = GcmShared.Paths.Local.RandomDivisionFile(Battle.BattleID, division.DivisionID);
                DirectoryEx.EnsureDirectory(division.DivisionXmlPath);
                (Organizer as ScnOrganizerRandom).SaveDivision(division, division.DivisionXmlPath);
            }
        }

    }
}
