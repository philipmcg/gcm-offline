using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Utilities;

namespace GcmShared
{
    public class BattleOptions : Utilities.VariableBin
    {
        private BattleOptions() { }

        public OOBType OOBType { get { return Battle.GetOOBTypeByID(base.Str["opt_s_gametype"]); } }

        public static BattleOptions ReadFromFile(string path)
        {
            BattleOptions bo = new BattleOptions();
            bo.LoadFromFile(path);
            return bo;
        }
    }
}
