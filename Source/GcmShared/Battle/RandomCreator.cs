using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Utilities;
using Utilities.GCSV;
using Military;
using GcmShared.NewMilitary;

namespace GcmShared
{
    public class RandomCreator
    {
         private static readonly RandomCreator instance = new RandomCreator(Gcm.Data);
         public static RandomCreator Instance { get { return instance; } }

         static RandomCreator() {}

        GcmDataManager data;

        private RandomCreator(GcmDataManager data)
        {
            this.data = data;

            if (this.data != null) // We don't need the data for certain testing scenarios
            {
                Weapons = data.GCSVs["weapons"];
                Classes = data.GCSVs["classes"];
                Flags = data.GCSVs["flags"];

                WeaponsMap = Weapons.ToDictionary(l => l["id"].ToInt(), l => l["ids"]);
                ClassesMap = Classes.ToDictionary(l => l["id"].ToInt(), l => l["ids"]);
                FlagsMap = Flags.ToDictionary(l => l["id"].ToInt(), l => l["ids"]);
            }
        }

        readonly GCSVTable Weapons;
        readonly GCSVTable Classes;
        readonly GCSVTable Flags;

        readonly Dictionary<int, string> WeaponsMap;
        readonly Dictionary<int, string> ClassesMap;
        readonly Dictionary<int, string> FlagsMap;


        /// <summary>
        /// Gets a Weapon ID by the Weapon IDString
        /// </summary>
        public int Weapon(string ids)
        {
            return Weapons[ids]["id"].ToInt();
        }
        /// <summary>
        /// Gets a Class ID by the Class IDString
        /// </summary>
        public int Class(string ids)
        {
            return Classes[ids]["id"].ToInt();
        }
        /// <summary>
        /// Gets a Flag ID by the Flag IDString
        /// </summary>
        public int Flag(string ids)
        {
            if (string.IsNullOrWhiteSpace(ids))
                return 0;
            else if (Flags.ContainsKey(ids))
                return Flags[ids]["id"].ToInt();
            else
                return 0;
        }

        /// <summary>
        /// Gets a Weapon IDString by the Weapon ID
        /// </summary>
        public string Weapon(int id)
        {
            return id == 0 ? "" : WeaponsMap[id];
        }
        /// <summary>
        /// Gets a Class IDString by the Class ID
        /// </summary>
        public string Class(int id)
        {
            return id == 0 ? "" : ClassesMap[id];
        }
        /// <summary>
        /// Gets a Flag IDString by the Flag ID
        /// </summary>
        public string Flag(int id)
        {
            return id == 0 ? "" : FlagsMap[id];
        }


        public string GetRandomSectionType(int factionID)
        {
            return Gcm.Data.Lists["regiment\\" + data.FactionPfx(factionID) + "art"].GetRandom();
        }

        public string GetRandomState(int factionID)
        {
            var regt = Gcm.Data.Lists["regiment\\" + data.FactionPfx(factionID) + "regt"].GetRandom();
            return Gcm.Data.GCSVs["regiments"][regt]["origin"];
        }

        // Sets commander skills to numbers from the given function, which should go from 0 to 6.
        public void SetSkills(CommanderData data, Func<double, double> amount)
        {
            data.Experience = (amount(data.Experience) * Mil.ExperienceRange) / Mil.CommanderSkillRange; // because commander skills are in the 0-6 range, but experience is 0-9
            data.Leadership = amount(data.Leadership);
            data.Ability = amount(data.Ability);
            data.Command = amount(data.Command);
            data.Control = amount(data.Control);
            data.Style = amount(data.Style);
            data.Politics = amount(data.Politics);
        }

        // Sets commander skills to numbers from the given function, which should go from 0 to 6.
        public void SetSkills(CommanderData data, Func<double> amount)
        {
            SetSkills(data, s => amount());
        }

        public void SetSkills(UnitData data, double experience, Func<double> offsets)
        {
            data.Experience = experience;
            data.Marksmanship = offsets();
            data.Close = offsets();
            data.Open = offsets();
            data.Edged = offsets();
            data.Firearm = offsets();
            data.Horsemanship = offsets();
        }

        public double RandomStatOffset()
        {
            int choice = Rand.Int(100);

            if (choice < 4)
                return Rand.Double() * 6;
            else if (choice < 35)
                return Rand.Double() * 3.6;
            else if (choice < 70)
                return Rand.Double() * 2;
            else // if (choice < 100)
                return Rand.Double();
        }

        // returns a double from 0 to 6.
        public double RandomCommanderStat()
        {
            int choice = Rand.Int(100);

            if (choice < 50)
                return Rand.CurvedDouble(3.5, 50);
            else if (choice < 84)
                return Rand.CurvedDouble(3.3, 90);
            else if (choice < 90)
                return Rand.Int(1, 6) + Rand.Double();
            else // if (choice < 100)
                return Rand.CurvedDouble(3, 50) + (Rand.Int(1, 2) * Rand.Sign());
        }

        void SetSkills(UnitData data, double amount)
        {
            SetSkills(data, amount, RandomStatOffset);
        }

        public void ApplyGunType(UnitData d, string guntype)
        {
            d.ClassId = Class(data.Lists["class\\" + data.GCSVs["artillery"][guntype]["class"]].GetRandom());
            d.Name = data.GCSVs["artillery"][guntype]["name"];
            d.WeaponId = Weapon(data.GCSVs["artillery"][guntype]["weapon"]);
        }

        public Unit CreateGun(int side, string guntype, double exp)
        {
            Unit unit = new Unit();
            var d = unit.Data;
            d.Type = UnitTypes.Artillery;

            d.ClassId = Class(data.Lists["class\\" + data.GCSVs["artillery"][guntype]["class"]].GetRandom());
            d.Name = data.GCSVs["artillery"][guntype]["name"];
            d.WeaponId = Weapon(data.GCSVs["artillery"][guntype]["weapon"]);
            d.Active = true;
            SetSkills(d, exp);
            d.Experience = exp;
            d.Men = 15;
            
            return unit;
        }


        public Commander CreateRandomCommander(int factionID, Func<double> stats = null)
        {
            Name name = CreateName(factionID);

            return CreateNamedCommander(name, stats ?? RandomCommanderStat);
        }

        public Commander CreateNamedCommander(Name name, Func<double> stats)
        {
            Commander cdr = new Commander();
            var data = cdr.Data;
            data.FirstName = name.First;
            data.MiddleInitial = name.Middle;
            data.LastName = name.Last;
            data.Portrait = "";
            data.Rank = 0;
            
            SetSkills(data, stats);

            return cdr;
        }

        public Commander CreateNamedCommander(int level, int side, int type, int strength, Name name, Func<double> stats)
        {
            Commander cdr = new Commander();
            var data = cdr.Data;
            data.FirstName = name.First;
            data.MiddleInitial = name.Middle;
            data.LastName = name.Last;
            data.Portrait = "";
            data.Rank = 0;
            SetSkills(data, stats);

            return cdr;
        }

        public Unit CreateRegiment(string state, int side, int regtNumber, int men, double quality)
        {
            Unit unit = CreateRegiment(side, quality);
            var d = unit.Data;

            d.Men = men;
            d.InitialExperience = d.Experience;
            d.InitialMen = d.Men;
            d.Name = Util.Ordinal(regtNumber) + " " + data.GCSVs["regiments"][state]["name"];
            d.Type = UnitTypes.Infantry;
            d.RegimentNumber = regtNumber;
            d.State = state;

            string unitClass = data.Lists["class\\" + data.GCSVs["regiments"][state]["class"]].GetRandom();
            string unitWeapon = data.Lists["weapon\\" + data.GCSVs["regiments"][state]["weapon"]].GetRandom();
            string unitFlag = data.Lists["flag\\" + data.GCSVs["regiments"][state]["flag1"]].GetRandom();
            string unitFlag2 = data.GCSVs["regiments"][state]["flag2"];

            if (!unitFlag2.StartsWith("GFX"))
                unitFlag2 = Gcm.Data.Lists["flag\\" + unitFlag2].GetRandom();

            d.ClassId = Class(unitClass);
            d.WeaponId = Weapon(unitWeapon);
            d.Flag1 = Flag(unitFlag);
            d.Flag2 = Flag(unitFlag2);

            d.Active = true;
            d.Engagements = 0;
            return unit;
        }

        /// <summary>
        /// Creates a new Unit with the given side, initializes the unit's experience and skills, and assigns a new random commander to the regiment
        /// </summary>
        public Unit CreateRegiment(int side, double exp)
        {
            Unit unit = new Unit();
            var data = unit.Data;

            SetSkills(data, exp);

            var cdr = CreateNamedCommander(Levels.Unit, side, UnitTypes.Infantry, 1, CreateCharacterName(side), () => Rand.CurvedDouble(4));
            cdr.AssignCommand(unit);

            return unit;
        }

        /// <summary>
        /// Assigns a new generated commander for all commands in the organization that lack a commander.
        /// </summary>
        public void CreateCommandersForEmptySpots(Organization organization)
        {
            foreach (var org in organization.AllOrganizations.Append(organization))
            {
                if (!org.HasCommander)
                {
                    var cdr = CreateNamedCommander(org.Data.Level, org.Data.Side, org.GetUnitType(), org.AllUnits.Sum(u => u.Data.Men), CreateCharacterName(org.Data.Side), () => Rand.CurvedDouble(4));
                    cdr.AssignCommand(org);
                }
            }
        }


        Name CreateName(int side)
        {
            string pfx = data.GCSVs["factions"][side]["pfx"];
            return new Name
                (
                data.Lists["names\\" + pfx + "first"].GetRandom(),
                data.Lists["names\\" + pfx + "middle"].GetRandom(),
                data.Lists["names\\" + pfx + "last"].GetRandom()
                );
        }

        public Name CreateCharacterName(int side)
        {
            Name name;
            do
            {
                name = CreateName(side);
            } while (name.First[0] == name.Last[0]);

            return name;
        }

        /// <summary>
        /// Creates an empty brigade with no commander.
        /// </summary>
        public Organization CreateEmptyBrigade(int side)
        {
            var org = new Organization();
            org.Data.Level = Levels.Brigade;
            org.Data.Side = side;
            return org;
        }
        /// <summary>
        /// Creates an empty division with no commander.
        /// </summary>
        public Organization CreateEmptyDivision(int side)
        {
            var org = new Organization();
            org.Data.Level = Levels.Division;
            org.Data.Side = side;
            return org;
        }

        /// <summary>
        /// Creates an empty corps with no commander.
        /// </summary>
        public Organization CreateEmptyCorps(int side)
        {
            var org = new Organization();
            org.Data.Level = Levels.Corps;
            org.Data.Side = side;
            return org;
        }

      /// <summary>
      /// Creates a new brigade with the given units in it, with a new random commander
      /// </summary>
        public Organization CreateBrigadeWithRegiments(int side, List<Unit> units)
        {
            Organization org = CreateEmptyBrigade(side);
            var cdr = CreateRandomCommander(side);

            foreach (var unit in units)
            {
                org.AddUnit(unit);
            }

            org.SortUnits();

            cdr.AssignCommand(org);

            return org;
        }


        public Organization CreateNewBattery(int side, int sections, string state, int batteryNumber)
        {
            var org = CreateEmptyBrigade(side);

            string batt = "ABCDEFGHIK"[batteryNumber % 10].ToString();
            string battno = Util.Ordinal(batteryNumber / 10 + 1);
            org.Data.State = state;
            org.Data.OrganizationNumber = batteryNumber;

            for (int s = 0; s < sections; s++)
            {
                AddSectionToBattery(org);
            }

            var cdr = CreateRandomCommander(side);
            cdr.AssignCommand(org);

            return org;
        }

        public void AddSectionToBattery(Organization battery)
        {
            int factionID = battery.Data.Side;
            string type = GetRandomSectionType(factionID);
            double sectionQuality = Rand.CurvedDouble(3.8, 20);
            for (int g = 0; g < 2; g++)
            {
                string gunType = type;
                if(Rand.OneIn(factionID == Factions.USA ? 600 : 200))
                    gunType = GetRandomSectionType(factionID);

                Unit unit = CreateGun(factionID, gunType, Rand.CurvedDouble(sectionQuality, 10));
                battery.AddUnit(unit);
                var gunCdr = CreateRandomCommander(factionID);
                gunCdr.AssignCommand(unit);
            }
        }

    }

}
