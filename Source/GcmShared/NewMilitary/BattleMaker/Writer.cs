using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Military;
using System.IO;

using Utilities;
using Utilities.GCSV;


namespace GcmShared.NewMilitary
{
    

    public class ChainTagMap
    {
        List<IGCSVLine> Lines;
        public ChainTagMap()
        {
            Lines = new List<IGCSVLine>();
        }
        public void Add(IGCSVLine line)
        {
            Lines.Add(line);
        }
        public IEnumerable<string> GetLines { get { return Lines.Select(l => l.ToString(',')); } }
    }


    class ScnWriter : IScenarioWriter
    {
        OOBWriter Writer;
        ChainTagMap ChainTagMap;

        IGCSVHeader ChainTagMapHeader = GCSVMain.CreateHeader(new[]{"chain", "tag", "exp", "men"});

        string TempDirectory;

        Battle Battle;

        public ScnWriter(string tempDir)
        {
            TempDirectory = tempDir;
        }

        public void WriteScenario(IEnumerable<Organization> armies, Dictionary<int, Location> unitLocations, Battle battle)
        {
            Writer = new OOBWriter();
            ChainTagMap = new ChainTagMap();
            Battle = battle;
            Writer.OnOrganizationFinished += AddSupplyWagons;

            CreateScenarioFiles(battle, TempDirectory, armies);

            var lines = Writer.WriteToLines(armies, unitLocations, battle.BattleID, AddToChainTagMap);
            File.WriteAllLines(GcmShared.Files.Scenario(battle.BattleID, false), lines.ToArray());
            File.WriteAllLines(Paths.Local.ChainTagMap(battle.BattleID), ChainTagMap.GetLines.ToArray());
        }

        static Dictionary<string, string> SupplyFormation = new Dictionary<string, string>()
        {
            {"fast", "DRIL_SupplyWagon"},
            {"normal", "DRIL_SupplyWagon_Normal"},
            {"slow", "DRIL_SupplyWagon_Slow"},
            {"slowest", "DRIL_SupplyWagon_Slowest"},
        };

        static Dictionary<string, string> SupplyUnit = new Dictionary<string, string>()
        {
            {"fast", "Wagon_Fast"},
            {"medium", "Wagon_Medium"},
            {"slow", "Wagon_Slow"},
            {"slowest", "Wagon_Slowest"},
            {"immobile", "Wagon_Immobile"},
        };

        static readonly string[] wagons = 
        {
            "",  
            "1,1,{6},{7},{8},{9},OOB_U_Supply_{5},{11},,{10},,(1-0),,{0},{1},{2},{3},{4},GFX_US_National_Cav,,DRIL_SupplyWagon,10,,,,,,3,5,8,1,1,3,3,5,0,2,7",
            "2,1,{6},{7},{8},{9},OOB_C_Supply_{5},{11},,{10},,(1-0),,{0},{1},{2},{3},{4},GFX_CS_National_Art,,DRIL_SupplyWagon,10,,,,,,3,5,8,1,1,3,3,5,0,2,7",
        };

        static readonly string[] objective_wagons = 
        {
            "",  
            "1,1,{6},{7},{8},{9},OOB_U_Supply_{5},{11},,UGLB_USA_Objective_Wagon,,(1-0),,{0},{1},{2},{3},{4},GFX_US_National_Cav,,{10},10,,,,,,3,5,8,1,1,3,3,5,0,2,7",
            "2,1,{6},{7},{8},{9},OOB_C_Supply_{5},{11},,UGLB_CSA_Objective_Wagon,,(1-0),,{0},{1},{2},{3},{4},GFX_CS_National_Art,,{10},10,,,,,,3,5,8,1,1,3,3,5,0,2,7",
        };
        static readonly string[] objective_officers = {
                                                          "",
            "1,1,{6},{7},{8},{9},{5},{11},,UGLB_USA_Objective_Holder,,(8-7),,{0},{1},{2},{3},{4},GFX_US_National_Cav,,{10},1,4,7,7,0,7,0,,,,,,,,,,",
            "2,1,{6},{7},{8},{9},{5},{11},,UGLB_CSA_Objective_Holder,,(8-7),,{0},{1},{2},{3},{4},GFX_CS_National_Art,,{10},1,4,7,7,0,7,0,,,,,,,,,,",
          };
        static readonly string[] objective_guns = {
                                                          "",
            "1,1,{6},{7},{8},{9},{5},{11},,UGLB_USA_Art_Limbered_3,,(1-0),IDS_ARSN_Objective_Gun,{0},{1},{2},{3},{4},GFX_US_National_Cav,,{10},10,,,,,,3,6,19,3,3,3,3,3,3,3,7",
            "2,1,{6},{7},{8},{9},{5},{11},,UGLB_CSA_Art_Limbered_6,,(1-0),IDS_ARSN_Objective_Gun,{0},{1},{2},{3},{4},GFX_CS_National_Art,,{10},10,,,,,,3,6,19,3,3,3,3,3,3,3,7",
          };

        const int Ammo = 100000;
        // this seems to be a hardcoded thing in SOWGB -- wagons will take ammo from other wagons up to 550 if they are below it.
        const int MinimumAmmo = 550;
        const int WagonOffset = 5120;
        string Wagon(Organization org, Location loc, int corps, int div, int brig, int regt, string supplyFormation, int ammo = Ammo, string name = "Ordnance Supply Wagon")
        {
            return wagons[org.Data.Side].With(ammo, loc.Dir.X, loc.Dir.Y, loc.Loc.X + Rand.Sign() * WagonOffset, loc.Loc.Y + Rand.Sign() * WagonOffset, brig + "_" + org.Commander.Data.Id, corps, div, brig, regt, supplyFormation, name);
        }
        string ObjectiveWagon(Organization org, Location loc, int corps, int div, int brig, int regt)
        {
            return objective_wagons[org.Data.Side].With(-10000000, loc.Dir.X, loc.Dir.Y, loc.Loc.X , loc.Loc.Y, brig + "_" + org.Commander.Data.Id, corps, div, brig, regt, "DRIL_SupplyWagon", "Objective Holder Wagon");
        }
        string ObjectiveOfficer(Organization org, Location loc, int corps, int div, int brig, int regt)
        {
            return objective_officers[org.Data.Side].With(0, loc.Dir.X, loc.Dir.Y, loc.Loc.X , loc.Loc.Y , "obj_officer_" + org.Commander.Data.Id + "_" + brig, corps, div, brig, regt, "DRIL_Lvl5_Art_Line", " Objective Holder");
        }
        string ObjectiveGun(Organization org, Location loc, int corps, int div, int brig, int regt)
        {
            return objective_guns[org.Data.Side].With(
                0, 
                loc.Dir.X, loc.Dir.Y, loc.Loc.X + Rand.Sign() * WagonOffset, loc.Loc.Y + Rand.Sign() * WagonOffset, 
                "obj_gun_" + org.Commander.Data.Id + "_" + brig, 
                corps, div, brig, regt,
                "DRIL_Lvl6_Art_Line", 
                " Objective Holder");
        }

        IEnumerable<string> AddSupplyWagons(Organization org, NewChain chain, Location loc)
        {
            int extrasInOrg = 0;

            string supplyUnit = "Wagon";
            if (SupplyUnit.ContainsKey(Gcm.Var.Str["opt_s_supply_unit"]))
            {
                string prefix = org.Data.Side == Factions.USA ? "UGLB_USA_" : "UGLB_CSA_";
                supplyUnit = prefix + SupplyUnit[Gcm.Var.Str["opt_s_supply_unit"]];
            }
            bool createObjectiveWagons = Gcm.Var.Str["opt_s_objective_holders"].Contains("wagons");
            bool createObjectiveHolders = Gcm.Var.Str["opt_s_objective_holders"].Contains("holders");
            bool createObjectiveGuns = Gcm.Var.Str["opt_s_objective_holders"].Contains("guns");

            // Only make objective holders in real MP battles.
            if (Battle.BattleType == BattleTypes.Normal || Battle.BattleType == BattleTypes.Competitive1v1)
            {
                if (org.IsAtLevel(Levels.Division) && createObjectiveHolders)
                {
                    int friendlyDivs = Battle.Divisions.Count(d => d.Side == org.Data.Side);
                    int numHoldersNeeded = Math.Max(friendlyDivs, (int)(Battle.NumObjectives * 1));

                    int numObjectiveHolders = numHoldersNeeded / friendlyDivs;
                    numObjectiveHolders += (int)Math.Max(0, (numHoldersNeeded % friendlyDivs - chain.Division) > 0 ? 1 : 0);
                    for (int i = 0; i < numObjectiveHolders; i++)
                    {
                        extrasInOrg++;
                        yield return ObjectiveOfficer(org, loc, chain.Corps, chain.Division, org.NumOrganizations + extrasInOrg, 0);
                        //if (createObjectiveWagons)
                        //    yield return ObjectiveWagon(org, loc, chain.Corps, chain.Division, org.NumOrganizations + extrasInOrg, 1);
                        if (createObjectiveGuns) 
                            yield return ObjectiveGun(org, loc, chain.Corps, chain.Division, org.NumOrganizations + extrasInOrg, 1);
                    }
                    if (createObjectiveWagons)
                    {
                        for (int i = 0; i < numObjectiveHolders; i++)
                        {
                            extrasInOrg++;
                            yield return Wagon(org, loc, chain.Get(Levels.Corps), chain.Get(Levels.Division), org.NumOrganizations + extrasInOrg, 0, supplyUnit, 20, "Objective Holder Wagon");
                        }
                    }
                }
            }

            if (Battle.SupplyType == SupplyType.NoSupply)
            {
                yield break;
            }
            else if (Battle.SupplyType == SupplyType.ByArmy)
            {
                if (org.IsAtLevel(Levels.Army))
                {
                    extrasInOrg++;
                    yield return Wagon(org, loc, org.NumOrganizations + extrasInOrg, 0, 0, 0, supplyUnit);
                }
            }
            else if (Battle.SupplyType == SupplyType.ByDivision)
            {
                if (org.IsAtLevel(Levels.Division))
                {
                    extrasInOrg++;
                    yield return Wagon(org, loc, chain.Get(Levels.Corps), chain.Get(Levels.Division), org.NumOrganizations + extrasInOrg, 0, supplyUnit);
                }
            }
        }
        

        void AddToChainTagMap(NewChain chain, object element)
        {
            var line = new GCSVLine(ChainTagMapHeader);
            line["chain"] = string.Join("_", chain.Positions.Skip(1).Select(i => i.ToString()).ToArray());
            line["tag"] = (element as AbstractMilitary.IMilitaryConstruct).Tag.ToString();
            
            if(element is Unit)
            {
                line["exp"] = (element as Unit).Data.Experience.Truncate4();
                line["men"] = (element as Unit).ExportData.Men.ToString();
            }
            else if(element is Commander)
            {
                line["exp"] = (element as Commander).Data.Experience.Truncate4();
                line["men"] = "1";
            }

            ChainTagMap.Add(line);
        }

        void CreateScenarioFiles(Battle battle, string destinationDirectory, IEnumerable<Organization> armies)
        {
            DestinationDir = destinationDirectory;

            // Backup and prepare scenario files
            ProcessFile("oob", false);
            ProcessFile("intro", true);
            ProcessFile("screen", true);
            ProcessFile("script", true);
            ProcessFile("ini", true);


            string mapinfo = MapInfo.Export(battle);
                
            FileEx.WriteAllLines(Paths.Local.MapInfo(battle.BattleID), new string[] { mapinfo });

            // Append map and weather info to ini file
            DelimReader r = new DelimReader();
            var iniLines = r.ReadToString(ScnFilePath("ini"));
            iniLines.Add("weather=" + 2);
            iniLines.Add("map=" + battle.Map.Replace("[RandomVariant]", Rand.Int(100).ToString()));
            iniLines.Add("starttime=" + Objectives.TimeToAbsolute(battle.LengthOfBattleInMinutes));

            if (!battle.IsSingleplayer) {
              if (battle.TotalMen > 40000)
                battle.SpriteRatio = 5;
              //if (battle.TotalMen > 60000)
              //  battle.SpriteRatio = 6;
            }

            iniLines.Add("[MenPerSprite]");
            iniLines.Add("Inf=" + battle.SpriteRatio);
            iniLines.Add("Cav=" + battle.SpriteRatio);
            iniLines.Add("Art=10");

            iniLines.Add("[rank]");
            iniLines.Add("cmdlvl1="+battle.PlayerSide);
            iniLines.Add("cmdlvl2=1");
            iniLines.Add("cmdlvl3=1");
            iniLines.Add("cmdlvl4=1");
            iniLines.Add("cmdlvl5=0");
            iniLines.Add("cmdlvl6=0");
            iniLines.Add("[GCM]");
            iniLines.Add("host_id="+battle.HostPlayerID);
            
            File.WriteAllLines(ScnFilePath("ini"), iniLines.ToArray());

            ScenarioFiles.WriteIntro(ScnFilePath("intro"), battle, armies);
        }
        void CopyScenarioToFolder(string dest)
        {
            CopyScnFile("oob", dest);
            CopyScnFile("maplocs", dest);
            CopyScnFile("intro", dest);
            CopyScnFile("screen", dest);
            CopyScnFile("script", dest);
            CopyScnFile("ini", dest);
        }
        /// <summary>
        /// The folder where all the scenario files will end up
        /// </summary>
        string DestinationDir;
        string ScnFilePath(string id)
        {
            return Path.Combine(DestinationDir, Gcm.Var.Str["scn_" + id]);
        }

        /// <summary>
        /// Copy this scenario file from the ScenarioFiles folder to 
        /// the DestinationDir
        /// </summary>
        void ProcessFile(string id, bool copy)
        {
            string file = Gcm.Var.Str["scn_" + id];
            Backup.BackupFile(Path.Combine(DestinationDir, file), 3);
            if (copy)
                File.Copy(Path.Combine(Gcm.Data.GetPath("ScenarioFiles"), file), Path.Combine(DestinationDir, file));
        }


        void CopyScnFile(string id, string dest)
        {
            string d = Path.Combine(dest, Gcm.Var.Str["scn_" + id]);
            if (File.Exists(d))
                File.Delete(d);

            File.Copy(ScnFilePath(id), d);
        }
    }
}
