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

    public class HistoricalBattleMaker : BattleMaker
    {
        public HistoricalBattleMaker() : base() { }

        protected override void Organize()
        {
            Organizer = new ScnOrganizerHistorical(Gcm.Data.GCSVHeaders, Directory.GetFiles(Gcm.Data.GetPath("HistoricalOOBs\\gettysburg1")), Gcm.Var.Str["opt_s_cavalry"] == "1");

            Organizer.Organize(Battle.Divisions, Battle.MenGunRatio);


            foreach (var division in Battle.Divisions)
            {
                division.DivisionXmlPath = GcmShared.Paths.Local.RandomDivisionFile(Battle.BattleID, division.DivisionID);
                DirectoryEx.EnsureDirectory(division.DivisionXmlPath);
                (Organizer as ScnOrganizerHistorical).SaveDivision(division, division.DivisionXmlPath);
            }
        }

    }
}
