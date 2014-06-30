using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Utilities;
using Utilities.GCSV;

namespace GcmShared
{

    public class BattleReport
    {
        /// <summary>
        /// Number of enemy casualties inflicted by unit
        /// </summary>
        public SoftDictionary<int, int> Inflicted;

        /// <summary>
        /// Average experience of the casualties inflicted by unit
        /// </summary>
        public SoftDictionary<int, double> InflictedExperience;
        public SoftDictionary<int, int> Lost;
        public SoftDictionary<int, int> Ammo;
        public SoftDictionary<int, int> HeadCount;
        public SoftDictionary<int, string> Status;
        public HashSet<int> Involved;
        public SoftDictionary<string, int> Players;

        public Dictionary<string, GCSVLine> ChainTagMap;

        public int BattleID;
        public string PlayersList;

        public static GCSVHeader Header = new GCSVHeader("Side,Army,Corp,Div,Brig,Reg,Our Name,Ammo,Status,Deserted,Killed,Wounded,R1,R2,R3,R4,R5,R6,Killer Name,Casualties".Split(','));


        public BattleReport()
        {
            ChainTagMap = new Dictionary<string, GCSVLine>();
        }

        public void LoadFromFile(int battleID, string chainTagMapFile, string gameDbFile)
        {
            DelimReader reader = new DelimReader();
            var chainTagMap = reader.ReadToStringArray(chainTagMapFile);
            LoadChainTagMap(chainTagMap);

            var gameDb = reader.ReadToStringArray(gameDbFile);
            Load(gameDb);
        }

        private void LoadChainTagMap(IEnumerable<string[]> lines)
        {
            var header = new GCSVHeader(new[] { "chain", "tag", "exp", "men" });
            foreach (var line in lines)
            {
                ChainTagMap.Add(line[0], new GCSVLine(header, line));
            }
        }

        public bool HasPlayersList
        {
            get
            {
                return Players.Any(p => p.Key != "Unknown");
            }
        }

        public string GetPlayersList()
        {
            var lplayers = Players.OrderBy(p => p.Value).Select(p => p.Key);

            StringBuilder sb = new StringBuilder();
            foreach (var player in lplayers)
            {
                sb.Append(player);
                sb.Append('\n');
            }
            return sb.ToString();
        }

        static string[] recv_parts = new string[] { "Side", "Army", "Corp", "Div", "Brig", "Reg" };
        static string[] give_parts = new string[] { "R1", "R2", "R3", "R4", "R5", "R6" };

        private string ChainPacked(GCSVLine line, bool receive)
        {
            StringBuilder sb = new StringBuilder();
            for (int k = 0; k < 6; k++)
            {
                if (receive)
                    sb.Append(line[recv_parts[k]]);
                else
                    sb.Append(line[give_parts[k]]);
                sb.Append('_');
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
        private int ChainNumber(GCSVLine line, bool receive)
        {
            int n = 0;
            int mult = (int)Math.Pow(10, 5);
            for (int k = 0; k < 6; k++)
            {
                if (receive)
                    n += int.Parse(line[recv_parts[k]]) * mult;
                else
                    n += int.Parse(line[give_parts[k]]) * mult;

                mult /= 10;
            }

            return n;
        }

        public void Load(IEnumerable<string[]> lines)
        {
            Inflicted = new SoftDictionary<int, int>();
            Lost = new SoftDictionary<int, int>();
            Status = new SoftDictionary<int, string>();
            Ammo = new SoftDictionary<int, int>();
            Involved = new HashSet<int>();
            Players = new SoftDictionary<string, int>();
            InflictedExperience = new SoftDictionary<int, double>();
            HeadCount = new SoftDictionary<int, int>();

            Func<string, string> GetPlayer = (s) =>
            {
                int index = s.IndexOf('(');
                int indexF = s.IndexOf(')');
                if (index == -1)
                    return "Unknown";
                string name = s.Substring(index + 1, s.Length - (index + 2));
                return name;
            };

            Func<GCSVLine, int> GetLoss = (l) =>
            {
                return int.Parse(l["Deserted"]) + int.Parse(l["Killed"]) + int.Parse(l["Wounded"]);
            };

            lines = lines.Skip(3);

            foreach (var str in lines)
            {
                int recv;
                int give;
                bool hasGiver = false;
                GCSVLine line = new GCSVLine(Header, str);
                string name = line["Our Name"];

                Func<char, char, string, string> GetBetween = (a, b, value) =>
                {
                    int al = value.IndexOf(a);
                    int bl = value.IndexOf(b);
                    var inside = value.Substring(al + 1, bl - al - 1);
                    return inside;
                };

                if (name.Contains("Courier"))
                    if (name.Contains("]"))
                        BattleID = int.Parse(GetBetween('[', ']', name));

                int side = int.Parse(line["Side"]);
                string player = GetPlayer(name);

                Players[player] = side;

                if (line["Reg"] == "0")
                    continue;

                string recv_chain = ChainPacked(line, true);
                if (!ChainTagMap.ContainsKey(recv_chain))
                    continue;

                // Unit that took casualties
                recv = ChainTagMap[recv_chain]["tag"].ToInt();
                var recv_exp = double.Parse(ChainTagMap[recv_chain]["exp"]);

                if (line.FieldArray.Length > 13)
                    hasGiver = true;

                if (!Involved.Contains(recv))
                    Involved.Add(recv);
                if (!Status.ContainsKey(recv))
                    Status.Add(recv, line["Status"]);
                if (!Ammo.ContainsKey(recv))
                    Ammo.Add(recv, int.Parse(line["Ammo"] == "" ? "0" : line["Ammo"]));

                HeadCount[recv] = ChainTagMap[recv_chain]["men"].ToInt();

                if (hasGiver)
                {
                    int cas = line["Casualties"].ToInt();
                    string give_chain = ChainPacked(line, false);

                    if (ChainTagMap.ContainsKey(give_chain))
                    {
                        give = ChainTagMap[give_chain]["tag"].ToInt();

                        if (!Inflicted.ContainsKey(give))
                        {
                            Inflicted[give] = 0;
                            InflictedExperience[give] = 0;
                        }

                        Inflicted[give] += cas;
                        InflictedExperience[give] += cas * recv_exp;
                    }
                    if (!Lost.ContainsKey(recv))
                        Lost.Add(recv, GetLoss(line));
                }
            }

            // Make the KilledExp to be an average
            var keys = InflictedExperience.Keys.ToArray();
            foreach (var ke in keys)
            {
                if (Inflicted[ke] > 0)
                {
                    InflictedExperience[ke] = InflictedExperience[ke] / Inflicted[ke];
                }
            }

        }
    }
}
