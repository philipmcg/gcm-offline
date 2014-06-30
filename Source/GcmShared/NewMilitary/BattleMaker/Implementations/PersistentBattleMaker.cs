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

    public class PersistentBattleMaker : BattleMaker
    {
        public PersistentBattleMaker() : base() { }

        protected override void Organize()
        {
            Organizer = new ScnOrganizerPersistent(Gcm.Var.Str["opt_s_cavalry"] == "1", Paths.Local.DivisionFile, OOBType.PersistentDivisions, Battle.UseBalancer, Battle.UseGunPenalties);
            
            Organizer.Organize(Battle.Divisions, Battle.MenGunRatio);
        }

    }
}
