using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;


using Military;

namespace GcmShared.NewMilitary
{



    public static class MilitaryExtensions
    {

        // Moves this number of men to "missing" status, representing desertions.
        public static void MakeDeserters(this Unit me, int amount)
        {
            if (amount >= me.Data.Men || amount < 0)
                throw new ArgumentException("Desertion amount invalid");

            double missingExp = Mil.GetCombinedExperience(me.Data.CurMissing, me.Data.MissingExp, amount, me.Data.Experience);

            me.Data.CurMissing = me.Data.CurMissing + amount;
            me.Data.MissingExp = missingExp;
            me.Data.Men -= amount;
        }


        // Format unit/commander identification for history:

        // 20-u_me
        public static string GetRegimentIdentification(this Unit u)
        {
            return u.Data.RegimentNumber + "-" + u.Data.State;
        }
        // 5-1337 (where 1337 is lastname lookup for "Chamberlain" and 5 is rank id for Colonel.)
        public static string GetCommanderIdentification(this Commander c)
        {
            return c.Data.Rank + "-" + Gcm.Data.LastNameIDs.Value[c.Data.LastName];
        }



        public static void SetSkills(this CommanderData data, Func<double, double> amount)
        {
            data.Experience = amount(data.Experience);
            data.Leadership = amount(data.Leadership);
            data.Ability = amount(data.Ability);
            data.Command = amount(data.Command);
            data.Control = amount(data.Control);
            data.Style = amount(data.Style);
        }

        /// <summary>
        /// Creates a MilitaryGroup with this Organization as the only organization in the group.
        /// </summary>
        public static Military.IO.MilitaryGroup AsGroup(this Organization me)
        {
            return new Military.IO.MilitaryGroup(me);
        }

        public static void WriteToFile(this Organization me, string path)
        {
            Military.IO.MilitaryIO.Writer.WriteToFile(path, me.AsGroup());
        }
        
        public static Name GetName(this PreviousCommanderData pc)
        {
            return new Name(pc.FirstName, pc.MiddleInitial, pc.LastName);
        }

        public static string GetName(this IForce me)
        {
            if (me is Unit)
                return (me as Unit).Data.Name;
            else if (me is Organization)
                return (me as Organization).Data.Name;
            else
                return null;
        }

        /// <summary>
        /// Reorders brigades and batteries so brigades come first.
        /// </summary>
        public static void ReorderSuborganizations(this Organization org)
        {
            int last = UnitTypes.None;
            bool outOfOrder = false;
            foreach (var item in org.Organizations)
            {
                var type = item.GetUnitType();
                if (last == UnitTypes.Artillery && UnitTypes.IsInfantryOrCavalry(type))
                {
                    outOfOrder = true;
                    break;
                }
                last = type;
            } 

            if (!outOfOrder)
                return;

            var brigades = org.AllFightingBrigades().ToList();
            var batteries = org.AllArtilleryBatteries().ToList();

            org.ReorderOrganizations(brigades.Concat(batteries));
        }

    }
}
