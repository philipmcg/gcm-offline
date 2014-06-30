
	
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Xml.Serialization;

namespace SceShared
{
	
	

	public struct GameInfoBrief
	{
		
[XmlElement("p1")] 
 public int GameID { get; set; }
[XmlElement("p2")] 
 public string GameName { get; set; }

		
		
		
		public GameInfoBrief ( int gameID, string gameName )  : this()
		{
			
GameID = gameID;  
GameName = gameName;  

		}
	}
	

	public struct SetPlayerFaction
	{
		
[XmlElement("p1")] 
 public int PlayerID { get; set; }
[XmlElement("p2")] 
 public int GameID { get; set; }
[XmlElement("p3")] 
 public int FactionID { get; set; }

		
		
		
		public SetPlayerFaction ( int playerID, int gameID, int factionID )  : this()
		{
			
PlayerID = playerID;  
GameID = gameID;  
FactionID = factionID;  

		}
	}
	

	public struct PollChatResult
	{
		
[XmlElement("p1")] 
 public ChatEntry[] Chats { get; set; }
[XmlElement("p2")] 
 public int LastChatID { get; set; }

		
		
		
		public PollChatResult ( ChatEntry[] chats, int lastChatID )  : this()
		{
			
Chats = chats;  
LastChatID = lastChatID;  

		}
	}
	

	public struct PollChat
	{
		
[XmlElement("p1")] 
 public int Room { get; set; }
[XmlElement("p2")] 
 public int LastChatID { get; set; }

		
		
		
		public PollChat ( int room, int lastChatID )  : this()
		{
			
Room = room;  
LastChatID = lastChatID;  

		}
	}
	

	public struct SubmitChat
	{
		
[XmlElement("p1")] 
 public int RoomID { get; set; }
[XmlElement("p2")] 
 public string Username { get; set; }
[XmlElement("p3")] 
 public string Message { get; set; }

		
		
		
		public SubmitChat ( int roomID, string username, string message )  : this()
		{
			
RoomID = roomID;  
Username = username;  
Message = message;  

		}
	}
	

	public struct GamePlayerInfo
	{
		
[XmlElement("p1")] 
 public PlayerInfo PlayerInfo { get; set; }
[XmlElement("p2")] 
 public int FactionID { get; set; }
[XmlElement("p3")] 
 public int TurnWatched { get; set; }

		
		
		
		public GamePlayerInfo ( PlayerInfo playerInfo, int factionID, int turnWatched )  : this()
		{
			
PlayerInfo = playerInfo;  
FactionID = factionID;  
TurnWatched = turnWatched;  

		}
	}
	

	public struct GameInfo
	{
		
[XmlElement("p1")] 
 public int GameID { get; set; }
[XmlElement("p2")] 
 public string GameName { get; set; }
[XmlElement("p3")] 
 public int ChatRoomID { get; set; }
[XmlElement("p4")] 
 public int MessageRoomID { get; set; }
[XmlElement("p5")] 
 public int HostPlayerID { get; set; }
[XmlElement("p6")] 
 public bool Started { get; set; }
[XmlElement("p7")] 
 public int Turn { get; set; }
[XmlElement("p8")] 
 public List<GamePlayerInfo> Players { get; set; }
[XmlElement("p9")] 
 public int NumFactions { get; set; }

		
		
		
		public GameInfo ( int gameID, string gameName, int chatRoomID, int messageRoomID, int hostPlayerID, bool started, int turn, List<GamePlayerInfo> players, int numFactions )  : this()
		{
			
GameID = gameID;  
GameName = gameName;  
ChatRoomID = chatRoomID;  
MessageRoomID = messageRoomID;  
HostPlayerID = hostPlayerID;  
Started = started;  
Turn = turn;  
Players = players;  
NumFactions = numFactions;  

		}
	}
	

	public struct ChatEntry
	{
		
[XmlElement("p1")] 
 public string Username { get; set; }
[XmlElement("p2")] 
 public string Message { get; set; }

		
		
		
		public ChatEntry ( string username, string message )  : this()
		{
			
Username = username;  
Message = message;  

		}
	}
	

	public struct FinishTurn
	{
		
[XmlElement("p1")] 
 public int PlayerID { get; set; }
[XmlElement("p2")] 
 public int GameID { get; set; }
[XmlElement("p3")] 
 public int FactionID { get; set; }
[XmlElement("p4")] 
 public int Turn { get; set; }

		
		
		
		public FinishTurn ( int playerID, int gameID, int factionID, int turn )  : this()
		{
			
PlayerID = playerID;  
GameID = gameID;  
FactionID = factionID;  
Turn = turn;  

		}
	}
	

	public struct ReturnActivePlayer
	{
		
[XmlElement("p1")] 
 public int PlayerID { get; set; }
[XmlElement("p2")] 
 public int CurrentTurn { get; set; }
[XmlElement("p3")] 
 public bool Playing { get; set; }
[XmlElement("p4")] 
 public bool ReadyToRunTurn { get; set; }

		
		
		
		public ReturnActivePlayer ( int playerID, int currentTurn, bool playing, bool readyToRunTurn )  : this()
		{
			
PlayerID = playerID;  
CurrentTurn = currentTurn;  
Playing = playing;  
ReadyToRunTurn = readyToRunTurn;  

		}
	}
	

	public struct PlayerInfo
	{
		
[XmlElement("p1")] 
 public int ID { get; set; }
[XmlElement("p2")] 
 public string Name { get; set; }

		
		
		
		public PlayerInfo ( int iD, string name )  : this()
		{
			
ID = iD;  
Name = name;  

		}
	}
	

	public struct CreateGame
	{
		
[XmlElement("p1")] 
 public string Name { get; set; }
[XmlElement("p2")] 
 public Guid HostUserID { get; set; }
[XmlElement("p3")] 
 public int HostPlayerID { get; set; }
[XmlElement("p4")] 
 public int NumFactions { get; set; }

		
		
		
		public CreateGame ( string name, Guid hostUserID, int hostPlayerID, int numFactions )  : this()
		{
			
Name = name;  
HostUserID = hostUserID;  
HostPlayerID = hostPlayerID;  
NumFactions = numFactions;  

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
 public PlayerInfo PlayerInfo { get; set; }

		
		
		
		public AuthResult ( bool error, string errorMessage, Guid userID, PlayerInfo playerInfo )  : this()
		{
			
Error = error;  
ErrorMessage = errorMessage;  
UserID = userID;  
PlayerInfo = playerInfo;  

		}
	}
	

	public struct PlayerWatchedTurn
	{
		
[XmlElement("p1")] 
 public int PlayerID { get; set; }
[XmlElement("p2")] 
 public int Turn { get; set; }
[XmlElement("p3")] 
 public int GameID { get; set; }

		
		
		
		public PlayerWatchedTurn ( int playerID, int turn, int gameID )  : this()
		{
			
PlayerID = playerID;  
Turn = turn;  
GameID = gameID;  

		}
	}
	

	public struct GetActivePlayer
	{
		
[XmlElement("p1")] 
 public int GameID { get; set; }
[XmlElement("p2")] 
 public int FactionID { get; set; }

		
		
		
		public GetActivePlayer ( int gameID, int factionID )  : this()
		{
			
GameID = gameID;  
FactionID = factionID;  

		}
	}
	

	public struct AuthRequest
	{
		
[XmlElement("p1")] 
 public string Username { get; set; }
[XmlElement("p2")] 
 public string Password { get; set; }

		
		
		
		public AuthRequest ( string username, string password )  : this()
		{
			
Username = username;  
Password = password;  

		}
	}
	

	public struct StartGame
	{
		
[XmlElement("p1")] 
 public int GameID { get; set; }

		
		
		
		public StartGame ( int gameID )  : this()
		{
			
GameID = gameID;  

		}
	}
	

	public struct PassControlToPlayer
	{
		
[XmlElement("p1")] 
 public int CurrentPlayerID { get; set; }
[XmlElement("p2")] 
 public int DestPlayerID { get; set; }
[XmlElement("p3")] 
 public int GameID { get; set; }
[XmlElement("p4")] 
 public int FactionID { get; set; }

		
		
		
		public PassControlToPlayer ( int currentPlayerID, int destPlayerID, int gameID, int factionID )  : this()
		{
			
CurrentPlayerID = currentPlayerID;  
DestPlayerID = destPlayerID;  
GameID = gameID;  
FactionID = factionID;  

		}
	}
	
}
