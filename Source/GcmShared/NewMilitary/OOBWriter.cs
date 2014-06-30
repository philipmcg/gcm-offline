

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Utilities.GCSV;
using Military;
using GcmShared;

namespace GcmShared.NewMilitary
{
    public struct NewChain
    {
        public int[] Positions { get; private set; }

        public NewChain(int side) : this()
        {
            Positions = new int[7];
            Positions[1] = side;
        }

        public void Next(int level)
        {
            Positions[level]++;
            for (int i = level + 1; i < Positions.Length; i++)
            {
                Positions[i] = 0;
            }
        }

        public int Get(int level)
        {
            return Positions[level];
        }

        public int Side { get { return Positions[1]; } }
        public int Army { get { return Positions[2]; } }
        public int Corps { get { return Positions[3]; } }
        public int Division { get { return Positions[4]; } }
        public int Brigade { get { return Positions[5]; } }
        public int Regiment { get { return Positions[6]; } }
    }

    public class OOBWriter
    {
        IGCSVHeader Header;
        RandomCreator Creator;
        Dictionary<int, Location> Locations;

        public event Func<Organization, NewChain, Location, IEnumerable<string>> OnOrganizationFinished;

        public OOBWriter()
        {
            Creator = RandomCreator.Instance;
            Header = GCSVMain.CreateHeader(HeaderString.Split(','));
        }

        public IEnumerable<string> WriteToLines(IEnumerable<Organization> armies, Dictionary<int, Location> locations, int battleId, Action<NewChain, object> action)
        {
            Locations = locations;

            List<string> lines = new List<string>();
            lines.Add(HeaderString);

            const string csc = "2,1,{1},0,0,0,OOB_C_Courier,Confederate Courier [{0}],,C_Courier,,(1-0),,0,1.0000,0.0000,0,0,GFX_CS_National_Art,,DRIL_SupplyWagon,10,,,,,,5,6,8,4,4,1,1,1,0,2,7";
            const string usc = "1,1,{1},0,0,0,OOB_U_Courier,Union Courier [{0}],,U_Courier,,(1-0),,0,1.0000,0.0000,0,0,GFX_US_National_Cav,,DRIL_SupplyWagon,10,,,,,,5,6,8,4,4,1,1,1,0,2,7";

            foreach (var army in armies)
            {
                WriteOrganization(lines, army, new NewChain(army.Data.Side), action);

                // Add courier
                if (army.Data.Side == 1)
                {
                    lines.Add(string.Format(usc, battleId, army.NumOrganizations + 2));
                }
                else if (army.Data.Side == 2)
                {
                    lines.Add(string.Format(csc, battleId, army.NumOrganizations + 2));
                }
            }

            return lines;
        }


        void WriteOrganization(List<string> lines, Organization organization, NewChain chain, Action<NewChain, object> action)
        {
            var line = new GCSVLine(Header);
            WriteCommander(line, organization.Commander);
            chain.Next(organization.Data.Level);
            AddLocation(line, organization.Commander.Tag);
            AddChain(line, chain);
            action(chain, organization.Commander);
            lines.Add(line.ToString());

            Action<Unit> writeUnit = (unit) =>
            {
                line = new GCSVLine(Header);
                WriteUnit(line, unit);
                chain.Next(Levels.Unit);
                AddLocation(line, unit.Tag);
                AddChain(line, chain);
                action(chain, unit);
                lines.Add(line.ToString());
            };

            foreach (var unit in organization.UnitsInAlternatingOrder)
            {
                writeUnit(unit);
            }

            foreach (var org in organization.Organizations)
            {
                WriteOrganization(lines, org, chain, action);
            }

            if (OnOrganizationFinished != null)
                lines.AddRange(OnOrganizationFinished(organization, chain, Locations[(int)organization.Commander.Tag]));
        }



        const string HeaderString = "SANDBOXOOB,ARMY,CORPS,DIV,BGDE,REG,ID,NAME1,NAME2,CLASS,OOBMOD,PICT,WEAP,AMMO,dir x,dir z,loc x,loc z,FLAGS,FLAG2,Formation,Head Count,Ability,Command,Control,Leadership,Style,Experience,Fatigue,Morale,Close,Open,Edged,Firearm,Marksmanship,Horsemanship,Surgeon,Calisthenics";

        static string[] UnitFormations = new[] { "DRIL_Lvl6_Inf_Column", "DRIL_Lvl6_Art_Line", "DRIL_Lvl6_Cav_Column" };

        string Clamp(double number)
        {
            return Clamp((int)number);
        }

        string Clamp(int number)
        {
            return Math.Max(0, Math.Min(9, number)).ToString();
        }

        string Clamp6(double number)
        {
            return Clamp6((int)number);
        }

        string Clamp6(int number)
        {
            return Math.Max(0, Math.Min(6, number)).ToString();
        }

        void WriteUnit(IGCSVLine line, Unit unit)
        {
            UnitData data = unit.Data;
            UnitExportData ed = unit.ExportData;

            string tag = Utilities.Number.IntToBase52(data.Id);
            line["ID"] = "OOB_" + tag;

            line["NAME1"] = ed.Name + "      " + unit.Commander.GetVeryShortName();
            line["WEAP"] = Creator.Weapon(ed.WeaponId);
            line["CLASS"] = Creator.Class(ed.ClassId);
            line["FLAGS"] = Creator.Flag(ed.Flag1);
            line["FLAG2"] = Creator.Flag(ed.Flag2);

            // Sometimes swap flags on regiments that have two, so the regt flag is in front.
            // ---> cancelled this idea because we're used to the flags being in one order -- makes it look like the regiment is facing the wrong way.
            /* if (unit.Data.Id % 5 == 0 && !string.IsNullOrEmpty(line["FLAG2"]))
            {
                string temp = line["FLAG2"];
                line["FLAG2"] = line["FLAGS"];
                line["FLAGS"] = temp;
            } */

            line["Fatigue"] = "6";
            line["Morale"] = "19";
            line["Formation"] = UnitFormations[data.Type];
            line["Experience"] = Clamp(ed.Experience);
            line["Close"] = Clamp(ed.Close);
            line["Open"] = Clamp(ed.Open);
            line["Edged"] = Clamp(ed.Edged);
            line["Firearm"] = Clamp(ed.Firearm);
            line["Marksmanship"] = Clamp(ed.Marksmanship);
            line["Horsemanship"] = Clamp(ed.Horsemanship);
            line["Surgeon"] = "3";
            line["Calisthenics"] = Math.Max(0, Math.Min(9, ((ed.Men - (ed.Men % 50)) / 50) - 1)).ToString() ;
            line["Head Count"] = ed.Men.ToString();

            switch (data.Type)
            {
                case UnitTypes.Infantry:
                    line["AMMO"] = "60";
                    break;
                case UnitTypes.Cavalry:
                    //line["OOBMOD"] = "1";
                    line["AMMO"] = "60";
                    break;
                case UnitTypes.Artillery:
                    line["AMMO"] = (120 / ed.Men).ToString();
                    break;
            }

            line["PICT"] = "(1-0)";
        }


        void AddChain(IGCSVLine line, NewChain chain)
        {
            line["SANDBOXOOB"] = chain.Positions[1].ToString();
            line["ARMY"] = chain.Positions[2].ToString();
            line["CORPS"] = chain.Positions[3].ToString();
            line["DIV"] = chain.Positions[4].ToString();
            line["BGDE"] = chain.Positions[5].ToString();
            line["REG"] = chain.Positions[6].ToString();

            line["ID"] = "OOB_" + string.Join("_", chain.Positions.Skip(1).Select(i => i.ToString()).ToArray());
        }

        void AddLocation(IGCSVLine line, object tag)
        {
            Location location;

            if (tag == null || !Locations.ContainsKey((int)tag))
                location = new Location(0, 0, 0, 0);
            else
                location = Locations[(int)tag];

            line["loc x"] = location.Loc.X.ToString();
            line["loc z"] = location.Loc.Y.ToString();
            line["dir x"] = location.Dir.X.ToString();
            line["dir z"] = location.Dir.Y.ToString();
        }
        
        void WriteCommander(IGCSVLine line, Commander commander)
        {
            CommanderData data = commander.Data;
            Organization org = commander.Organization;

            int side = org.Data.Side;


            string unitTypePrefix = "";

            if (org.Data.Level == Levels.Brigade)
            {
                if (org.GetUnitType() == UnitTypes.Artillery)
                    unitTypePrefix = "art_";
                else if (org.GetUnitType() == UnitTypes.Cavalry)
                    unitTypePrefix = "cav_";
            }

            line["PICT"] = (org.Data.Level == 5 && org.GetUnitType() == UnitTypes.Artillery ? "(10-7)" : "(8-7)");
            if(!string.IsNullOrEmpty(data.Portrait))
                line["PICT"] = data.Portrait;
            line["NAME1"] = string.Join("", Enumerable.Range(0, Math.Max(0, (5 - org.Data.Level))).Select(i => "--").ToArray())+  commander.GetAbbreviatedName();
            line["NAME2"] = org.Data.Name;
            line["CLASS"] = Gcm.Var.Str[Gcm.Data.FactionPrefix(side) + "cdrclass_" + unitTypePrefix + org.Data.Level];
            line["Head Count"] = "1";
            line["AMMO"] = "0";

            line["FLAGS"] = Gcm.Var.Str[Gcm.Data.FactionPrefix(side) + "cdrflag_" + unitTypePrefix + org.Data.Level];

            if (org.Data.Side == Factions.USA && org.Data.Level >= Levels.Division)
            {
                if (line["FLAGS"].Contains("{0}"))
                {
                    int cdrhash = Math.Abs(org.MoveToLevel(Levels.Division).Commander.Data.LastName.GetHashCode());
                    int cdrfirsthash = Math.Abs(org.MoveToLevel(Levels.Division).Commander.Data.FirstName.GetHashCode());
                    int corps = cdrhash % 2 + 1;
                    int brigade = cdrfirsthash % 2 + 1;
                    int division = cdrhash % 7 + 1;
                    if (division > 3)
                        division -= 3;
                    if (division > 3)
                        division = 2;
                    if (org.Data.Level == Levels.Brigade && line["FLAGS"].Contains("{2}"))
                    {
                        line["FLAGS"] = string.Format(line["FLAGS"], corps, division, brigade);
                    }
                    else
                    {
                        line["FLAGS"] = string.Format(line["FLAGS"], corps, division);
                    }
                }
            }

            line["CLASS"] = Gcm.Var.Str[Gcm.Data.FactionPrefix(side) + "cdrclass_" + unitTypePrefix + org.Data.Level];

            line["Ability"] = Clamp6(data.Ability);
            line["Command"] = Clamp6(data.Command);
            line["Leadership"] = Clamp6(data.Leadership);
            line["Experience"] = Clamp(Math.Max(2, (int)data.Experience));

            if(org.Data.Level == Levels.Brigade && org.GetUnitType() == UnitTypes.Artillery) {
                line["Ability"] = "7";
                line["Command"] = "7";
                line["Leadership"] = "7";
                line["Experience"] = "4";
            }

            int control = 0;
            if (org.Data.Level == Levels.Brigade)
            {
                if (org.GetUnitType() == UnitTypes.Artillery)
                    control = 1;
                else
                    control = 2;
            }
            else if (org.Data.Level == Levels.Division)
                control = 3;
            else if (org.Data.Level == Levels.Corps)
                control = 4;
            else if (org.Data.Level == Levels.Army)
                control = 5;

            line["Control"] = Clamp6(control);

            int style = (int)data.Style;
            style = Math.Max(2, style);
            line["Style"] =Clamp6( style);

            line["Formation"] = Gcm.Var.Str["form_" + unitTypePrefix + "lv_" + org.Data.Level];

        }


    }
}
