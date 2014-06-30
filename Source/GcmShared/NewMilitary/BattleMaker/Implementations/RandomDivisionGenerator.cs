using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GcmShared;
using Military;

using Utilities;

namespace GcmShared.NewMilitary
{
    class RandomDivisionGenerator
    {
        GcmDataManager Data { get { return Gcm.Data; } }
        RandomCreator Creator { get { return RandomCreator.Instance; } }

        UnitsList RandDiv;

        public RandomDivisionGenerator()
        {
            RandDiv = new UnitsList();
            RandDiv.Reset();
        }

        class UnitsList
        {
            public int UnitNumber;
            public double UnitQuality;
            public HashSet<string> Units;


            public void Reset()
            {
                UnitNumber = 0;
                Units = new HashSet<string>();
                UnitQuality = Rand.CurvedDouble(4d, 10d);
            }

            public int GetUnitNumber(int num)
            {
                int n = UnitNumber;
                UnitNumber += num;
                return n;
            }

            public int GetRegtNumber(string state, int num)
            {
                while (Units.Contains(state + num))
                    num++;

                Units.Add(state + num);
                return num;
            }
        }


        List<Unit> RD_CreateRegiments(int side, int max_men)
        {
            List<Unit> units = new List<Unit>();

            int sum_men = 0;

            while (sum_men < max_men)
            {
                int num_in_group = Data.Lists["randomdivision\\" + Data.FactionPfx(side) + "regt_groups"].GetRandom().ToInt();
                string state = Data.Lists["regiment\\" + Data.FactionPfx(side) + "regt"].GetRandom();
                int freq = Data.GCSVs["regiments"][state]["freq"].ToInt();

                int r_num = Rand.Int(freq / 2) + 1;

                for (int k = 0; k < num_in_group; k++)
                {
                    r_num += Rand.Int(freq / 5);
                    r_num = RandDiv.GetRegtNumber(state, r_num);

                    double lateness = ((double)r_num / (double)freq);

                    int men = Rand.Curved(300, 30);
                    men += (int)(lateness * 200);
                    double qual = Rand.CurvedDouble(5, 20);
                    qual -= (lateness * 3.0);

                    men = Math.Min(men, Rand.Curved(500, 10));
                    men = Math.Max(men, Rand.Curved(100, 10));

                    var unit = Creator.CreateRegiment(state, side, r_num, men, qual);
                    unit.Commander.ReassignRank();

                    units.Add(unit);

                    sum_men += Mil.GetValidMenForRegiment(unit);
                    if (sum_men > max_men)
                    {
                        int closeness = 50;
                        int over = sum_men - max_men;
                        var bestToRemove = units.Select(u => new { Strength = Mil.GetValidMenForRegiment(u), Unit = u }).Where(u => u.Strength <= over + closeness).OrderBy(u => u.Strength - over).FirstOrDefault();
                        if (bestToRemove != null)
                        {
                            units.Remove(bestToRemove.Unit);
                            sum_men -= bestToRemove.Strength;
                        }

                        int counter = 0;
                        while (sum_men > max_men + closeness && counter < 20)
                        {
                            over = sum_men - max_men;
                            var randomUnit = units.GetRandom();
                            int regtMen = Mil.GetValidMenForRegiment(randomUnit);
                            if (randomUnit.Data.Men - over > 200)
                            {
                                randomUnit.Data.Men -= over;
                                int newRegtMen = Mil.GetValidMenForRegiment(randomUnit);
                                sum_men = sum_men - regtMen + newRegtMen;
                            }
                            counter++;
                        }

                        break;
                    }
                }
            }

            double avg_exp = units.Sum(u => u.Data.Men * u.Data.Experience) / units.Sum(u => u.Data.Men);
            double ratio = RandDiv.UnitQuality / avg_exp;

            if (ratio > 0)
                units.ForEach(u => u.Data.Experience *= ratio);

            avg_exp = units.Sum(u => u.Data.Men * u.Data.Experience) / units.Sum(u => u.Data.Men);

            double ratio_men = (max_men * RandDiv.UnitQuality) / (sum_men * avg_exp);

            units.RemoveAll(u => u.Data.Men < 100);

            units.ForEach(u => u.Data.Men = (int)(u.Data.Men * ratio_men));

            units.RemoveAll(u => u.Data.Men < 60);


            return units;
        }


        List<int> RD_GetBrigadeSizes(int num_regiments, int side)
        {
            if (num_regiments <= 7)
                return new List<int>() { num_regiments };

            List<int> list;
            int total = 0;

            while (true)
            {
                total = 0;
                list = new List<int>();

                // Make a list of brigade sizes
                while (true)
                {
                    int brigade_size = Data.Lists["randomdivision\\" + Data.FactionPfx(side) + "brigade_sizes"].GetRandom().ToInt();
                    list.Add(brigade_size);
                    total += brigade_size;
                    if (total >= num_regiments)
                        break;
                }

                // If the list fits the number of regiments we have, use it, otherwise repeat
                if (total == num_regiments)
                    return list;
                else
                    continue;
            }
        }


        public List<Organization> RD_AssignRegimentsToBrigades(int side, List<Unit> regts, List<int> brigadeSizes)
        {
            List<Organization> brigades = new List<Organization>();

            foreach (var b in brigadeSizes)
            {
                var lists = regts.GroupBy(u => u.Data.State).OrderByDescending(g => g.Count()).Select(g => new object[] { g.Count(), g.First().Data.State });

                List<Unit> regiments;

                if (Rand.Percent(side == 1 ? 60 : 100) && (int)lists.First()[0] >= b)
                {
                    var state = (string)lists.First()[1];
                    int k = 0;
                    regiments = regts.Where(u => u.Data.State == state ? k++ < b : false).ToList();
                    regts.RemoveAll(u => regiments.Contains(u));
                }
                else
                {
                    if (Rand.Percent(side == 1 ? 50 : 90))
                        regts = regts.OrderByDescending(u => u.Data.State).ToList();
                    else
                        regts = regts.GetShuffled();

                    regiments = regts.GetRange(0, b);
                    regts.RemoveRange(0, b);
                }


                var brigade = Creator.CreateBrigadeWithRegiments(side, regiments);

                brigades.Add(brigade);
            }

            return brigades;
        }

        public List<Organization> RD_CreateBatteries(int side, int maxguns)
        {
            List<Organization> batteries = new List<Organization>();
            int guns_in_battery = (side == 1 ? 6 : 4);
            int sum_guns = 0;

            while (sum_guns + guns_in_battery <= maxguns)
            {
                string state = Data.Lists["batteries\\" + Data.FactionPfx(side) + "art"].GetRandom();
                int numguns = guns_in_battery;
                var brigade = Creator.CreateNewBattery(side, numguns / 2, state, Rand.Int(20));
                batteries.Add(brigade);
                sum_guns += numguns;
            }

            return batteries;
        }
        public Organization CreateRandomDivision(int maxInf, int maxCav, Name name, Division player)
        {
            int side = player.Side;

            Organization div = Creator.CreateEmptyDivision(side);

            // No infantry for divisions smaller than 1000 men.  This is to make it easier to have artillery-only organizations
            if (maxInf >= 1000)
            {
                var regts = RD_CreateRegiments(side, maxInf);
                var brigadeSizes = RD_GetBrigadeSizes(regts.Count, side);
                var brigades = RD_AssignRegimentsToBrigades(side, regts, brigadeSizes);
                brigades = brigades.GetShuffled();
                foreach (var brigade in brigades)
                    div.AddOrganization(brigade);
            }

            // Artillery handled outside this class.
           /* var batteries = RD_CreateBatteries(side, maxGuns);
            batteries = batteries.GetShuffled();
            foreach (var battery in batteries)
                div.AddOrganization(battery);*/ 

            Commander cdr = Creator.CreateNamedCommander(name, () => 3);
            cdr.AssignCommand(div);

            int numunits = 0;

            int unit_num = RandDiv.GetUnitNumber(numunits);

            return div;
        }
    }
}
