
	
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Xml.Serialization;

namespace GcmShared
{
	
	

	public class BulletinModel
	{
		
[XmlElement("p1")] 
 public string Name { get; set; }
[XmlElement("p2")] 
 public string Key { get; set; }
[XmlElement("p3")] 
 public bool Authenticated { get; set; }
[XmlElement("p4")] 
 public int PlayerID { get; set; }

		
		 public BulletinModel () {} 
		
		public BulletinModel ( string name, string key, bool authenticated, int playerID ) 
		{
			
Name = name;  
Key = key;  
Authenticated = authenticated;  
PlayerID = playerID;  

		}
	}
	

	public struct CampaignListInfo
	{
		
[XmlElement("p1")] 
 public int CampaignID { get; set; }
[XmlElement("p2")] 
 public int PlayerID { get; set; }
[XmlElement("p3")] 
 public string PlayerName { get; set; }
[XmlElement("p4")] 
 public int NumBattles { get; set; }
[XmlElement("p5")] 
 public int Turn { get; set; }
[XmlElement("p6")] 
 public string CampaignName { get; set; }
[XmlElement("p7")] 
 public DateTime? LastBattleDate { get; set; }

		
		
		
		public CampaignListInfo ( int campaignID, int playerID, string playerName, int numBattles, int turn, string campaignName, DateTime? lastBattleDate )  : this()
		{
			
CampaignID = campaignID;  
PlayerID = playerID;  
PlayerName = playerName;  
NumBattles = numBattles;  
Turn = turn;  
CampaignName = campaignName;  
LastBattleDate = lastBattleDate;  

		}
	}
	

	public class BattleResults
	{
		
[XmlElement("p1")] 
 public DateTime DateFinished { get; set; }
[XmlElement("p2")] 
 public int WinningTeam { get; set; }
[XmlElement("p3")] 
 public int BattleID { get; set; }
[XmlElement("p4")] 
 public OOBType OOBType { get; set; }
[XmlElement("p5")] 
 public List<SideReport> SideReports { get; set; }

		
		 public BattleResults () {} 
		
		public BattleResults ( DateTime dateFinished, int winningTeam, int battleID, OOBType oOBType, List<SideReport> sideReports ) 
		{
			
DateFinished = dateFinished;  
WinningTeam = winningTeam;  
BattleID = battleID;  
OOBType = oOBType;  
SideReports = sideReports;  

		}
	}
	

	public struct MinutesOnMap
	{
		
[XmlElement("p1")] 
 public int Minutes { get; set; }
[XmlElement("p2")] 
 public string MapID { get; set; }

		
		
		
		public MinutesOnMap ( int minutes, string mapID )  : this()
		{
			
Minutes = minutes;  
MapID = mapID;  

		}
	}
	

	public struct ItemAttributeModel
	{
		
[XmlElement("p1")] 
 public int ItemID { get; set; }
[XmlElement("p2")] 
 public string Attribute { get; set; }
[XmlElement("p3")] 
 public string Value { get; set; }

		
		
		
		public ItemAttributeModel ( int itemID, string attribute, string value )  : this()
		{
			
ItemID = itemID;  
Attribute = attribute;  
Value = value;  

		}
	}
	

	public struct RecordBattleModel
	{
		
[XmlElement("p1")] 
 public int BattleID { get; set; }
[XmlElement("p2")] 
 public int HostPlayerID { get; set; }
[XmlElement("p3")] 
 public int WinningTeam { get; set; }
[XmlElement("p4")] 
 public int Result { get; set; }

		
		
		
		public RecordBattleModel ( int battleID, int hostPlayerID, int winningTeam, int result )  : this()
		{
			
BattleID = battleID;  
HostPlayerID = hostPlayerID;  
WinningTeam = winningTeam;  
Result = result;  

		}
	}
	

	public class Division
	{
		
[XmlElement("p1")] 
 public int PlayerLevel { get; set; }
[XmlElement("p2")] 
 public int PlayerID { get; set; }
[XmlElement("p3")] 
 public int DivisionID { get; set; }
[XmlElement("p4")] 
 public int Side { get; set; }
[XmlElement("p5")] 
 public int FactionID { get; set; }
[XmlElement("p6")] 
 public Guid UserID { get; set; }
[XmlElement("p7")] 
 public string UserName { get; set; }
[XmlElement("p8")] 
 public Name CharacterName { get; set; }
[XmlElement("p9")] 
 public int RD_Men_Preference { get; set; }
[XmlElement("p10")] 
 public int RD_Guns_Preference { get; set; }
[XmlElement("p11")] 
 public int CD_Regts_Preference { get; set; }
[XmlElement("p12")] 
 public int CD_Men_Preference { get; set; }
[XmlElement("p13")] 
 public int CD_Guns_Preference { get; set; }
[XmlElement("p14")] 
 public int RD_Men { get; set; }
[XmlElement("p15")] 
 public int RD_Guns { get; set; }
[XmlElement("p16")] 
 public int CD_Men { get; set; }
[XmlElement("p17")] 
 public int CD_Guns { get; set; }
[XmlElement("p18")] 
 public int CD_Regts_Rank { get; set; }
[XmlElement("p19")] 
 public int CD_Regts_Rank_Modified { get; set; }
[XmlElement("p20")] 
 public int CD_Men_Rank { get; set; }
[XmlElement("p21")] 
 public int CD_Guns_Rank { get; set; }
[XmlElement("p22")] 
 public int CD_Guns_Penalty { get; set; }
[XmlElement("p23")] 
 public string DivisionXmlPath { get; set; }
[XmlElement("p24")] 
 public int PlayerRank { get; set; }
[XmlElement("p25")] 
 public int PrefHighCommand { get; set; }
[XmlElement("p26")] 
 public int PrefCavalry { get; set; }
[XmlElement("p27")] 
 public Guid FilesVersion { get; set; }
[XmlElement("p28")] 
 public Guid LastRealBattleKey { get; set; }
[XmlElement("p29")] 
 public List<string> PlayerOrders { get; set; }

		
		 public Division () {} 
		
		public Division ( int playerLevel, int playerID, int divisionID, int side, int factionID, Guid userID, string userName, Name characterName, int rD_Men_Preference, int rD_Guns_Preference, int cD_Regts_Preference, int cD_Men_Preference, int cD_Guns_Preference, int rD_Men, int rD_Guns, int cD_Men, int cD_Guns, int cD_Regts_Rank, int cD_Regts_Rank_Modified, int cD_Men_Rank, int cD_Guns_Rank, int cD_Guns_Penalty, string divisionXmlPath, int playerRank, int prefHighCommand, int prefCavalry, Guid filesVersion, Guid lastRealBattleKey, List<string> playerOrders ) 
		{
			
PlayerLevel = playerLevel;  
PlayerID = playerID;  
DivisionID = divisionID;  
Side = side;  
FactionID = factionID;  
UserID = userID;  
UserName = userName;  
CharacterName = characterName;  
RD_Men_Preference = rD_Men_Preference;  
RD_Guns_Preference = rD_Guns_Preference;  
CD_Regts_Preference = cD_Regts_Preference;  
CD_Men_Preference = cD_Men_Preference;  
CD_Guns_Preference = cD_Guns_Preference;  
RD_Men = rD_Men;  
RD_Guns = rD_Guns;  
CD_Men = cD_Men;  
CD_Guns = cD_Guns;  
CD_Regts_Rank = cD_Regts_Rank;  
CD_Regts_Rank_Modified = cD_Regts_Rank_Modified;  
CD_Men_Rank = cD_Men_Rank;  
CD_Guns_Rank = cD_Guns_Rank;  
CD_Guns_Penalty = cD_Guns_Penalty;  
DivisionXmlPath = divisionXmlPath;  
PlayerRank = playerRank;  
PrefHighCommand = prefHighCommand;  
PrefCavalry = prefCavalry;  
FilesVersion = filesVersion;  
LastRealBattleKey = lastRealBattleKey;  
PlayerOrders = playerOrders;  

		}
	}
	

	public struct PrepareForBattleResponse
	{
		
[XmlElement("p1")] 
 public bool Success { get; set; }
[XmlElement("p2")] 
 public string ErrorMessage { get; set; }

		
		
		
		public PrepareForBattleResponse ( bool success, string errorMessage )  : this()
		{
			
Success = success;  
ErrorMessage = errorMessage;  

		}
	}
	

	public struct CreateBattleFromLobbyResponse
	{
		
[XmlElement("p1")] 
 public int HostPlayerID { get; set; }
[XmlElement("p2")] 
 public int BattleID { get; set; }
[XmlElement("p3")] 
 public bool Error { get; set; }
[XmlElement("p4")] 
 public string ErrorMessage { get; set; }
[XmlElement("p5")] 
 public int PlayersHash { get; set; }

		
		
		
		public CreateBattleFromLobbyResponse ( int hostPlayerID, int battleID, bool error, string errorMessage, int playersHash )  : this()
		{
			
HostPlayerID = hostPlayerID;  
BattleID = battleID;  
Error = error;  
ErrorMessage = errorMessage;  
PlayersHash = playersHash;  

		}
	}
	

	public struct AuthResult
	{
		
[XmlElement("p1")] 
 public bool Error { get; set; }
[XmlElement("p2")] 
 public string ErrorMessage { get; set; }
[XmlElement("p3")] 
 public Guid UserID { get; set; }
[XmlElement("p4")] 
 public int PlayerID { get; set; }
[XmlElement("p5")] 
 public string Username { get; set; }
[XmlElement("p6")] 
 public bool IsAdmin { get; set; }
[XmlElement("p7")] 
 public bool IsDeveloper { get; set; }
[XmlElement("p8")] 
 public int Div1 { get; set; }
[XmlElement("p9")] 
 public int Div2 { get; set; }
[XmlElement("p10")] 
 public int PcSpecsTier { get; set; }
[XmlElement("p11")] 
 public ItemAttributeList Settings { get; set; }
[XmlElement("p12")] 
 public int WabashLevel { get; set; }

		
		
		
		public AuthResult ( bool error, string errorMessage, Guid userID, int playerID, string username, bool isAdmin, bool isDeveloper, int div1, int div2, int pcSpecsTier, ItemAttributeList settings, int wabashLevel )  : this()
		{
			
Error = error;  
ErrorMessage = errorMessage;  
UserID = userID;  
PlayerID = playerID;  
Username = username;  
IsAdmin = isAdmin;  
IsDeveloper = isDeveloper;  
Div1 = div1;  
Div2 = div2;  
PcSpecsTier = pcSpecsTier;  
Settings = settings;  
WabashLevel = wabashLevel;  

		}
	}
	

	public class BattleState
	{
		
[XmlElement("p1")] 
 public int BattleID { get; set; }
[XmlElement("p2")] 
 public int Time { get; set; }
[XmlElement("p3")] 
 public int PlayerReporting { get; set; }
[XmlElement("p4")] 
 public DateTime TimeRecorded { get; set; }
[XmlElement("p5")] 
 public DateTime TimeWrittenTo { get; set; }
[XmlElement("p6")] 
 public List<SideState> Sides { get; set; }
[XmlElement("p7")] 
 public int NumPlayers { get; set; }
[XmlElement("p8")] 
 public bool IsSingleplayer { get; set; }
[XmlElement("p9")] 
 public string Host { get; set; }
[XmlElement("p10")] 
 public string Map { get; set; }
[XmlElement("p11")] 
 public DateTime BattleStarted { get; set; }

		
		 public BattleState () {} 
		
		public BattleState ( int battleID, int time, int playerReporting, DateTime timeRecorded, DateTime timeWrittenTo, List<SideState> sides, int numPlayers, bool isSingleplayer, string host, string map, DateTime battleStarted ) 
		{
			
BattleID = battleID;  
Time = time;  
PlayerReporting = playerReporting;  
TimeRecorded = timeRecorded;  
TimeWrittenTo = timeWrittenTo;  
Sides = sides;  
NumPlayers = numPlayers;  
IsSingleplayer = isSingleplayer;  
Host = host;  
Map = map;  
BattleStarted = battleStarted;  

		}
	}
	

	public class SideState
	{
		
[XmlElement("p1")] 
 public List<string> Players { get; set; }
[XmlElement("p2")] 
 public int Score { get; set; }
[XmlElement("p3")] 
 public int Men { get; set; }
[XmlElement("p4")] 
 public int Casualties { get; set; }
[XmlElement("p5")] 
 public int Inflicted { get; set; }
[XmlElement("p6")] 
 public int Side { get; set; }

		
		 public SideState () {} 
		
		public SideState ( List<string> players, int score, int men, int casualties, int inflicted, int side ) 
		{
			
Players = players;  
Score = score;  
Men = men;  
Casualties = casualties;  
Inflicted = inflicted;  
Side = side;  

		}
	}
	

	public struct SpCampaignBattle
	{
		
[XmlElement("p1")] 
 public int BattleID { get; set; }
[XmlElement("p2")] 
 public int PlayerFactionID { get; set; }
[XmlElement("p3")] 
 public bool CustomBattleSettings { get; set; }

		
		
		
		public SpCampaignBattle ( int battleID, int playerFactionID, bool customBattleSettings )  : this()
		{
			
BattleID = battleID;  
PlayerFactionID = playerFactionID;  
CustomBattleSettings = customBattleSettings;  

		}
	}
	

	public class ChangePlayerSettingRequest
	{
		
[XmlElement("p1")] 
 public int PlayerID { get; set; }
[XmlElement("p2")] 
 public string Name { get; set; }
[XmlElement("p3")] 
 public string Value { get; set; }

		
		 public ChangePlayerSettingRequest () {} 
		
		public ChangePlayerSettingRequest ( int playerID, string name, string value ) 
		{
			
PlayerID = playerID;  
Name = name;  
Value = value;  

		}
	}
	

	public struct GcmAuthRequest
	{
		
[XmlElement("p1")] 
 public string Username { get; set; }
[XmlElement("p2")] 
 public string Password { get; set; }
[XmlElement("p3")] 
 public int Version { get; set; }

		
		
		
		public GcmAuthRequest ( string username, string password, int version )  : this()
		{
			
Username = username;  
Password = password;  
Version = version;  

		}
	}
	

	public struct CreateDivisionModel
	{
		
[XmlElement("p1")] 
 public Name Name { get; set; }
[XmlElement("p2")] 
 public string State { get; set; }
[XmlElement("p3")] 
 public int FactionID { get; set; }
[XmlElement("p4")] 
 public int PlayerID { get; set; }

		
		
		
		public CreateDivisionModel ( Name name, string state, int factionID, int playerID )  : this()
		{
			
Name = name;  
State = state;  
FactionID = factionID;  
PlayerID = playerID;  

		}
	}
	

	public class DivisionReport
	{
		
[XmlElement("p1")] 
 public int PlayerID { get; set; }
[XmlElement("p2")] 
 public double TroopsWeight { get; set; }
[XmlElement("p3")] 
 public int PlayerLevel { get; set; }
[XmlElement("p4")] 
 public string PlayerName { get; set; }
[XmlElement("p5")] 
 public int FactionID { get; set; }
[XmlElement("p6")] 
 public int DivisionID { get; set; }
[XmlElement("p7")] 
 public int TotalInflicted { get; set; }
[XmlElement("p8")] 
 public int TotalCasualties { get; set; }
[XmlElement("p9")] 
 public int TotalWounded { get; set; }
[XmlElement("p10")] 
 public int TotalMissing { get; set; }
[XmlElement("p11")] 
 public int TotalKilled { get; set; }
[XmlElement("p12")] 
 public int TotalEngaged { get; set; }
[XmlElement("p13")] 
 public int TotalStrength { get; set; }
[XmlElement("p14")] 
 public double AverageExperienceOfInflictedCasualties { get; set; }
[XmlElement("p15")] 
 public double AverageExperienceOfCasualties { get; set; }

		
		 public DivisionReport () {} 
		
		public DivisionReport ( int playerID, double troopsWeight, int playerLevel, string playerName, int factionID, int divisionID, int totalInflicted, int totalCasualties, int totalWounded, int totalMissing, int totalKilled, int totalEngaged, int totalStrength, double averageExperienceOfInflictedCasualties, double averageExperienceOfCasualties ) 
		{
			
PlayerID = playerID;  
TroopsWeight = troopsWeight;  
PlayerLevel = playerLevel;  
PlayerName = playerName;  
FactionID = factionID;  
DivisionID = divisionID;  
TotalInflicted = totalInflicted;  
TotalCasualties = totalCasualties;  
TotalWounded = totalWounded;  
TotalMissing = totalMissing;  
TotalKilled = totalKilled;  
TotalEngaged = totalEngaged;  
TotalStrength = totalStrength;  
AverageExperienceOfInflictedCasualties = averageExperienceOfInflictedCasualties;  
AverageExperienceOfCasualties = averageExperienceOfCasualties;  

		}
	}
	

	public class TeamSelection
	{
		
[XmlElement("p1")] 
 public int[] Team1 { get; set; }
[XmlElement("p2")] 
 public int[] Team2 { get; set; }
[XmlElement("p3")] 
 public double LevelsRatio { get; set; }

		
		 public TeamSelection () {} 
		
		public TeamSelection ( int[] team1, int[] team2, double levelsRatio ) 
		{
			
Team1 = team1;  
Team2 = team2;  
LevelsRatio = levelsRatio;  

		}
	}
	

	public class SideReport
	{
		
[XmlElement("p1")] 
 public string SideName { get; set; }
[XmlElement("p2")] 
 public int FactionID { get; set; }
[XmlElement("p3")] 
 public int TotalInflicted { get; set; }
[XmlElement("p4")] 
 public int TotalCasualties { get; set; }
[XmlElement("p5")] 
 public int TotalWounded { get; set; }
[XmlElement("p6")] 
 public int TotalMissing { get; set; }
[XmlElement("p7")] 
 public int TotalKilled { get; set; }
[XmlElement("p8")] 
 public int TotalEngaged { get; set; }
[XmlElement("p9")] 
 public int TotalStrength { get; set; }
[XmlElement("p10")] 
 public double AverageExperienceOfInflictedCasualties { get; set; }
[XmlElement("p11")] 
 public double AverageExperienceOfCasualties { get; set; }
[XmlElement("p12")] 
 public List<DivisionReport> DivisionReports { get; set; }

		
		 public SideReport () {} 
		
		public SideReport ( string sideName, int factionID, int totalInflicted, int totalCasualties, int totalWounded, int totalMissing, int totalKilled, int totalEngaged, int totalStrength, double averageExperienceOfInflictedCasualties, double averageExperienceOfCasualties, List<DivisionReport> divisionReports ) 
		{
			
SideName = sideName;  
FactionID = factionID;  
TotalInflicted = totalInflicted;  
TotalCasualties = totalCasualties;  
TotalWounded = totalWounded;  
TotalMissing = totalMissing;  
TotalKilled = totalKilled;  
TotalEngaged = totalEngaged;  
TotalStrength = totalStrength;  
AverageExperienceOfInflictedCasualties = averageExperienceOfInflictedCasualties;  
AverageExperienceOfCasualties = averageExperienceOfCasualties;  
DivisionReports = divisionReports;  

		}
	}
	

	public class PlayerCacheInfo
	{
		
[XmlElement("p1")] 
 public int PlayerID { get; set; }
[XmlElement("p2")] 
 public string Name { get; set; }
[XmlElement("p3")] 
 public int Div1 { get; set; }
[XmlElement("p4")] 
 public int Div2 { get; set; }
[XmlElement("p5")] 
 public int PcSpecsTier { get; set; }

		
		 public PlayerCacheInfo () {} 
		
		public PlayerCacheInfo ( int playerID, string name, int div1, int div2, int pcSpecsTier ) 
		{
			
PlayerID = playerID;  
Name = name;  
Div1 = div1;  
Div2 = div2;  
PcSpecsTier = pcSpecsTier;  

		}
	}
	

	public struct SubmitScreenshotModel
	{
		
[XmlElement("p1")] 
 public int PlayerID { get; set; }
[XmlElement("p2")] 
 public string PlayerName { get; set; }
[XmlElement("p3")] 
 public string Caption { get; set; }
[XmlElement("p4")] 
 public Guid Hash { get; set; }
[XmlElement("p5")] 
 public int BattleID { get; set; }

		
		
		
		public SubmitScreenshotModel ( int playerID, string playerName, string caption, Guid hash, int battleID )  : this()
		{
			
PlayerID = playerID;  
PlayerName = playerName;  
Caption = caption;  
Hash = hash;  
BattleID = battleID;  

		}
	}
	

	public struct PlayerListInfo
	{
		
[XmlElement("p1")] 
 public int PlayerID { get; set; }
[XmlElement("p2")] 
 public int Level { get; set; }
[XmlElement("p3")] 
 public int Points { get; set; }
[XmlElement("p4")] 
 public int Balance { get; set; }
[XmlElement("p5")] 
 public string Name { get; set; }
[XmlElement("p6")] 
 public int Div1 { get; set; }
[XmlElement("p7")] 
 public int Div2 { get; set; }
[XmlElement("p8")] 
 public DateTime? LastBattle { get; set; }
[XmlElement("p9")] 
 public DateTime? LastLogin { get; set; }
[XmlElement("p10")] 
 public Guid FilesVersion { get; set; }

		
		
		
		public PlayerListInfo ( int playerID, int level, int points, int balance, string name, int div1, int div2, DateTime? lastBattle, DateTime? lastLogin, Guid filesVersion )  : this()
		{
			
PlayerID = playerID;  
Level = level;  
Points = points;  
Balance = balance;  
Name = name;  
Div1 = div1;  
Div2 = div2;  
LastBattle = lastBattle;  
LastLogin = lastLogin;  
FilesVersion = filesVersion;  

		}
	}
	

	public struct PlayerRankInfo
	{
		
[XmlElement("p1")] 
 public int PlayerID { get; set; }
[XmlElement("p2")] 
 public string Name { get; set; }
[XmlElement("p3")] 
 public int Level { get; set; }
[XmlElement("p4")] 
 public int Points { get; set; }

		
		
		
		public PlayerRankInfo ( int playerID, string name, int level, int points )  : this()
		{
			
PlayerID = playerID;  
Name = name;  
Level = level;  
Points = points;  

		}
	}
	

	public class SpCampaignStatus
	{
		
[XmlElement("p1")] 
 public bool Valid { get; set; }
[XmlElement("p2")] 
 public CampaignStatus Action { get; set; }
[XmlElement("p3")] 
 public SpCampaignBattle Battle { get; set; }
[XmlElement("p4")] 
 public SpCampaignTurn Turn { get; set; }
[XmlElement("p5")] 
 public SpCampaignChooseSides ChooseSides { get; set; }

		
		 public SpCampaignStatus () {} 
		
		public SpCampaignStatus ( bool valid, CampaignStatus action, SpCampaignBattle battle, SpCampaignTurn turn, SpCampaignChooseSides chooseSides ) 
		{
			
Valid = valid;  
Action = action;  
Battle = battle;  
Turn = turn;  
ChooseSides = chooseSides;  

		}
	}
	

	public struct CreateBattleFromLobbyRequest
	{
		
[XmlElement("p1")] 
 public int HostPlayerID { get; set; }
[XmlElement("p2")] 
 public int LastBattlePlayersHash { get; set; }

		
		
		
		public CreateBattleFromLobbyRequest ( int hostPlayerID, int lastBattlePlayersHash )  : this()
		{
			
HostPlayerID = hostPlayerID;  
LastBattlePlayersHash = lastBattlePlayersHash;  

		}
	}
	

	public struct ItemAttributeList
	{
		
[XmlElement("p1")] 
 public ItemAttributeModel[] Values { get; set; }

		
		
		
		public ItemAttributeList ( ItemAttributeModel[] values )  : this()
		{
			
Values = values;  

		}
	}
	

	public struct BattleTimeInfo
	{
		
[XmlElement("p1")] 
 public double LagFactor { get; set; }
[XmlElement("p2")] 
 public int GameSeconds { get; set; }
[XmlElement("p3")] 
 public DateTime RealStarted { get; set; }
[XmlElement("p4")] 
 public DateTime RealFinished { get; set; }
[XmlElement("p5")] 
 public int RealSeconds { get; set; }

		
		
		
		public BattleTimeInfo ( double lagFactor, int gameSeconds, DateTime realStarted, DateTime realFinished, int realSeconds )  : this()
		{
			
LagFactor = lagFactor;  
GameSeconds = gameSeconds;  
RealStarted = realStarted;  
RealFinished = realFinished;  
RealSeconds = realSeconds;  

		}
	}
	

	public struct RegimentListing
	{
		
[XmlElement("p1")] 
 public int Men { get; set; }
[XmlElement("p2")] 
 public int RegimentNumber { get; set; }
[XmlElement("p3")] 
 public string State { get; set; }
[XmlElement("p4")] 
 public int ID { get; set; }
[XmlElement("p5")] 
 public string Commander { get; set; }
[XmlElement("p6")] 
 public int OwnerID { get; set; }
[XmlElement("p7")] 
 public bool OwnerIsDivision { get; set; }
[XmlElement("p8")] 
 public int NumEngagements { get; set; }

		
		
		
		public RegimentListing ( int men, int regimentNumber, string state, int iD, string commander, int ownerID, bool ownerIsDivision, int numEngagements )  : this()
		{
			
Men = men;  
RegimentNumber = regimentNumber;  
State = state;  
ID = iD;  
Commander = commander;  
OwnerID = ownerID;  
OwnerIsDivision = ownerIsDivision;  
NumEngagements = numEngagements;  

		}
	}
	

	public class PlayerLag
	{
		
[XmlElement("p1")] 
 public int PlayerID { get; set; }
[XmlElement("p2")] 
 public int LagFactor { get; set; }
[XmlElement("p3")] 
 public int AvgLagFactor { get; set; }
[XmlElement("p4")] 
 public int NumBattles { get; set; }
[XmlElement("p5")] 
 public DateTime LastBattleDate { get; set; }
[XmlElement("p6")] 
 public string Name { get; set; }

		
		 public PlayerLag () {} 
		
		public PlayerLag ( int playerID, int lagFactor, int avgLagFactor, int numBattles, DateTime lastBattleDate, string name ) 
		{
			
PlayerID = playerID;  
LagFactor = lagFactor;  
AvgLagFactor = avgLagFactor;  
NumBattles = numBattles;  
LastBattleDate = lastBattleDate;  
Name = name;  

		}
	}
	

	public struct PlayerIDModel
	{
		
[XmlElement("p1")] 
 public int PlayerID { get; set; }
[XmlElement("p2")] 
 public string PlayerName { get; set; }

		
		
		
		public PlayerIDModel ( int playerID, string playerName )  : this()
		{
			
PlayerID = playerID;  
PlayerName = playerName;  

		}
	}
	

	public class SowgbLogInfo
	{
		
[XmlElement("p1")] 
 public List<string> MapLoaders { get; set; }
[XmlElement("p2")] 
 public List<PlayerDrop> PlayerDrops { get; set; }

		
		 public SowgbLogInfo () {} 
		
		public SowgbLogInfo ( List<string> mapLoaders, List<PlayerDrop> playerDrops ) 
		{
			
MapLoaders = mapLoaders;  
PlayerDrops = playerDrops;  

		}
	}
	

	public struct ScreenshotInfo
	{
		
[XmlElement("p1")] 
 public string PlayerName { get; set; }
[XmlElement("p2")] 
 public string Hash { get; set; }
[XmlElement("p3")] 
 public string Caption { get; set; }

		
		
		
		public ScreenshotInfo ( string playerName, string hash, string caption )  : this()
		{
			
PlayerName = playerName;  
Hash = hash;  
Caption = caption;  

		}
	}
	

	public struct CampaignInfo
	{
		
[XmlElement("p1")] 
 public BattleListInfo[] BattleList { get; set; }
[XmlElement("p2")] 
 public object Campaign { get; set; }

		
		
		
		public CampaignInfo ( BattleListInfo[] battleList, object campaign )  : this()
		{
			
BattleList = battleList;  
Campaign = campaign;  

		}
	}
	

	public struct SpCampaignChooseSides
	{
		
[XmlElement("p1")] 
 public DivisionChooseSidesInfo[] Divisions { get; set; }
[XmlElement("p2")] 
 public int HostID { get; set; }

		
		
		
		public SpCampaignChooseSides ( DivisionChooseSidesInfo[] divisions, int hostID )  : this()
		{
			
Divisions = divisions;  
HostID = hostID;  

		}
	}
	

	public struct FilesVersionCheck
	{
		
[XmlElement("p1")] 
 public int PlayerID { get; set; }
[XmlElement("p2")] 
 public Guid FilesVersion { get; set; }

		
		
		
		public FilesVersionCheck ( int playerID, Guid filesVersion )  : this()
		{
			
PlayerID = playerID;  
FilesVersion = filesVersion;  

		}
	}
	

	public struct DivisionChooseSidesInfo
	{
		
[XmlElement("p1")] 
 public int DivisionID { get; set; }
[XmlElement("p2")] 
 public int FactionID { get; set; }
[XmlElement("p3")] 
 public string PlayerName { get; set; }
[XmlElement("p4")] 
 public string DivisionName { get; set; }

		
		
		
		public DivisionChooseSidesInfo ( int divisionID, int factionID, string playerName, string divisionName )  : this()
		{
			
DivisionID = divisionID;  
FactionID = factionID;  
PlayerName = playerName;  
DivisionName = divisionName;  

		}
	}
	

	public class ChangePlayerSettingResponse
	{
		
[XmlElement("p1")] 
 public int PlayerID { get; set; }
[XmlElement("p2")] 
 public bool Success { get; set; }
[XmlElement("p3")] 
 public ItemAttributeList Settings { get; set; }

		
		 public ChangePlayerSettingResponse () {} 
		
		public ChangePlayerSettingResponse ( int playerID, bool success, ItemAttributeList settings ) 
		{
			
PlayerID = playerID;  
Success = success;  
Settings = settings;  

		}
	}
	

	public struct PrepareForBattleModel
	{
		
[XmlElement("p1")] 
 public int BattleID { get; set; }
[XmlElement("p2")] 
 public int HostPlayerID { get; set; }
[XmlElement("p3")] 
 public BattleTypes Type { get; set; }
[XmlElement("p4")] 
 public bool OverwriteBattles { get; set; }

		
		
		
		public PrepareForBattleModel ( int battleID, int hostPlayerID, BattleTypes type, bool overwriteBattles )  : this()
		{
			
BattleID = battleID;  
HostPlayerID = hostPlayerID;  
Type = type;  
OverwriteBattles = overwriteBattles;  

		}
	}
	

	public class PlayerDrop
	{
		
[XmlElement("p1")] 
 public string Name { get; set; }
[XmlElement("p2")] 
 public int GameSeconds { get; set; }

		
		 public PlayerDrop () {} 
		
		public PlayerDrop ( string name, int gameSeconds ) 
		{
			
Name = name;  
GameSeconds = gameSeconds;  

		}
	}
	

	public struct CreateSpCampaignBattleModel
	{
		
[XmlElement("p1")] 
 public ContinueSpCampaignRequest Request { get; set; }
[XmlElement("p2")] 
 public List<int> Divisions { get; set; }

		
		
		
		public CreateSpCampaignBattleModel ( ContinueSpCampaignRequest request, List<int> divisions )  : this()
		{
			
Request = request;  
Divisions = divisions;  

		}
	}
	

	public struct BattleListInfo
	{
		
[XmlElement("p1")] 
 public int BattleID { get; set; }
[XmlElement("p2")] 
 public byte NumPlayers { get; set; }
[XmlElement("p3")] 
 public byte Winner { get; set; }
[XmlElement("p4")] 
 public byte Result { get; set; }
[XmlElement("p5")] 
 public bool HasScreenshots { get; set; }
[XmlElement("p6")] 
 public bool HasReplay { get; set; }
[XmlElement("p7")] 
 public short LagPercent { get; set; }
[XmlElement("p8")] 
 public short GameMinutes { get; set; }
[XmlElement("p9")] 
 public string Map { get; set; }
[XmlElement("p10")] 
 public int TotalTroops { get; set; }
[XmlElement("p11")] 
 public DateTime? Date { get; set; }

		
		
		
		public BattleListInfo ( int battleID, byte numPlayers, byte winner, byte result, bool hasScreenshots, bool hasReplay, short lagPercent, short gameMinutes, string map, int totalTroops, DateTime? date )  : this()
		{
			
BattleID = battleID;  
NumPlayers = numPlayers;  
Winner = winner;  
Result = result;  
HasScreenshots = hasScreenshots;  
HasReplay = hasReplay;  
LagPercent = lagPercent;  
GameMinutes = gameMinutes;  
Map = map;  
TotalTroops = totalTroops;  
Date = date;  

		}
	}
	

	public struct GetListOfPlayersModel
	{
		
[XmlElement("p1")] 
 public int BattleID { get; set; }
[XmlElement("p2")] 
 public bool IsRanked { get; set; }

		
		
		
		public GetListOfPlayersModel ( int battleID, bool isRanked )  : this()
		{
			
BattleID = battleID;  
IsRanked = isRanked;  

		}
	}
	

	public struct SpCampaignTurn
	{
		
[XmlElement("p1")] 
 public int DivisionID { get; set; }
[XmlElement("p2")] 
 public int TurnID { get; set; }

		
		
		
		public SpCampaignTurn ( int divisionID, int turnID )  : this()
		{
			
DivisionID = divisionID;  
TurnID = turnID;  

		}
	}
	

	public struct ContinueSpCampaignRequest
	{
		
[XmlElement("p1")] 
 public int PlayerID { get; set; }
[XmlElement("p2")] 
 public int CampaignID { get; set; }

		
		
		
		public ContinueSpCampaignRequest ( int playerID, int campaignID )  : this()
		{
			
PlayerID = playerID;  
CampaignID = campaignID;  

		}
	}
	

	public struct TeamPlayer
	{
		
[XmlElement("p1")] 
 public int Level { get; set; }
[XmlElement("p2")] 
 public int PlayerID { get; set; }

		
		
		
		public TeamPlayer ( int level, int playerID )  : this()
		{
			
Level = level;  
PlayerID = playerID;  

		}
	}
	

	public struct DetailedInfoListModel
	{
		
[XmlElement("p1")] 
 public int ID { get; set; }
[XmlElement("p2")] 
 public bool UnitsHaveHistory { get; set; }
[XmlElement("p3")] 
 public bool ShowManagementOptions { get; set; }
[XmlElement("p4")] 
 public IEnumerable<Military.Organization> Organizations { get; set; }

		
		
		
		public DetailedInfoListModel ( int iD, bool unitsHaveHistory, bool showManagementOptions, IEnumerable<Military.Organization> organizations )  : this()
		{
			
ID = iD;  
UnitsHaveHistory = unitsHaveHistory;  
ShowManagementOptions = showManagementOptions;  
Organizations = organizations;  

		}
	}
	

	public class CreateSpCampaignModel
	{
		
[XmlElement("p1")] 
 public string Name { get; set; }
[XmlElement("p2")] 
 public CreateDivisionModel PlayerDivision { get; set; }

		
		 public CreateSpCampaignModel () {} 
		
		public CreateSpCampaignModel ( string name, CreateDivisionModel playerDivision ) 
		{
			
Name = name;  
PlayerDivision = playerDivision;  

		}
	}
	

	public class BattleSetup
	{
		
[XmlElement("p1")] 
 public int BattleID { get; set; }
[XmlElement("p2")] 
 public List<Division> Divisions { get; set; }
[XmlElement("p3")] 
 public List<MinutesOnMap> AvgMinutesOnMap { get; set; }

		
		 public BattleSetup () {} 
		
		public BattleSetup ( int battleID, List<Division> divisions, List<MinutesOnMap> avgMinutesOnMap ) 
		{
			
BattleID = battleID;  
Divisions = divisions;  
AvgMinutesOnMap = avgMinutesOnMap;  

		}
	}
	
}
