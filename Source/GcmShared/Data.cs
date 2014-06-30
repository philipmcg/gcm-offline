
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Utilities;
using Utilities.GCSV;

using System.IO;

namespace GcmShared
{
    public class GcmDataManager
    {
        DelimReader Reader;

        public Dictionary<string, WeightedList<string>> ListsOld;
        public DataFileManager<WeightedList<string>> Lists;
        public Dictionary<string, GradientTable<object>> GradientTables;
        public VariableBin VariableBin;
        public Dictionary<string, IGCSVHeader> GCSVHeaders;
        public GCSVManager GCSVs;
        public string[] FirstNames;
        public string[] LastNames;
        public Lazy<Dictionary<string, int>> FirstNameIDs;
        public Lazy<Dictionary<string, int>> LastNameIDs;

        public Func<string, string> GetPath { get; private set; }

        public GcmDataManager(Func<string, string> pathProvider)
        {
            this.GetPath = pathProvider;

            Initialize();
            LoadData();
        }

        public string FindInList(string list, string defalt)
        {
            if (Lists.ContainsKey(list))
                return Lists[list].GetRandom();
            else
                return defalt;
        }


        void Initialize()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            Reader = new DelimReader();
            Reader.Delimiter = new char[] { ',', '\t' };

            LoadVariableBin();
        }

        void LoadData()
        {
            LoadWeightedLists();
            LoadCSVs();
            LoadGradientTables();
            LoadNames();
        }

        void LoadVariableBin()
        {
            VariableBin = new VariableBin(path => Reader.ReadToString(path));
            VariableBin.LoadFromFile(this.GetPath("definitions.ini"));
            VariableBin.LoadFromFile("variables.ini");
        }

        void LoadNames()
        {
            FirstNames = File.ReadAllLines(FirstNamesFile());
            LastNames = File.ReadAllLines(LastNamesFile());
            FirstNameIDs = new Lazy<Dictionary<string, int>>(() =>
            {
                var dict = new Dictionary<string, int>(FirstNames.Length + 1000);

                for (int i = 0; i < FirstNames.Length; i++)
                {
                    dict[FirstNames[i]] = i;
                }
                return dict;
            });
            LastNameIDs = new Lazy<Dictionary<string, int>>(() => {
                var dict = new Dictionary<string, int>(LastNames.Length + 1000);

                for (int i = 0; i < LastNames.Length; i++)
                {
                    dict[LastNames[i]] = i;
                }
                return dict;
            });

        }
        
            
        public string GcmKeyboardGuideFile()
        {
            return GetPath("Other\\gcm_keyboard_guide.txt");
        }
        public string GcmKeyboardFile()
        {
            return GetPath("Other\\keyboard.csv");
        }
        public string FirstNamesFile()
        {
            return GetPath("Other\\first_names.txt");
        }
        public string LastNamesFile()
        {
            return GetPath("Other\\last_names.txt");
        }
        public string MessagesFile()
        {
            return GetPath("Other\\messages.txt");
        }

        void LoadWeightedLists()
        {
            string dir = GetPath("WeightedLists\\");

            Func<string, string, WeightedList<string>> loadFile = (directory, key) =>
            {
                string path = key + ".csv";
                path = System.IO.Path.Combine(directory, path);
                var l = Reader.ReadToWeightedList(path);
                l.Optimize();
                return l;
            };

            Lists = new DataFileManager<WeightedList<string>>(dir);
            Lists.LoadFile = loadFile;
        }
        void LoadCSVs()
        {

            GCSVs = new GCSVManager(Reader, GetPath("GCSVs\\"));
            GCSVs.LoadMasterFile("master");
            GCSVHeaders = GCSVMain.ReadMultipleFromFile(Reader, GetPath("GCSVs\\military_headers.csv")).Headers;
            Military.IO.MilitaryIO.Headers = GCSVHeaders;
        }
        void LoadGradientTables()
        {
            Dictionary<string, object[]> Elements = new Dictionary<string, object[]>()
            {
                {"regiment_commanders",new object[]{5,6,7,8,9}},
                {"gun_commanders",new object[]{9,10,15,16}},
                {"section_commanders",new object[]{9,10,15,16}},
                {"battery_commanders",new object[]{7,8,9,10}},
                {"brigade_commanders",new object[]{3,4,5,6,7}},
                {"division_commanders",new object[]{3,4,5,6,7}},
            };

            GradientTables = new Dictionary<string, GradientTable<object>>();

            var files = Directory.GetFiles(GetPath("GradientTables\\"), "*.csv", SearchOption.TopDirectoryOnly);

            foreach (var file in files)
            {
                string s = Path.GetDirectoryName(file);
                string key = (Path.GetFileNameWithoutExtension(file)).ToLower();
                var l = Reader.ReadToStringArray(file);
                GradientTables.Add(key, GradientTable<object>.CreateFromCSV(Elements[key], l));
            }
        }

        public string FactionAbbr(int factionID) {
          return GCSVs["factions"][factionID]["abbr"];
        }
        public string FactionName(int factionID) {
          return GCSVs["factions"][factionID]["name"];
        }
        public string FactionPfx(int factionID)
        {
            return GCSVs["factions"][factionID]["pfx"];
        }
        public string FactionPrefix(int factionID)
        {
            return GCSVs["factions"][factionID]["prefix"];
        }
    }
}

