
	
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Xml.Serialization;

namespace GCM.Controllers
{
	
	

	public struct ArgGetTurnLinksForDivision
	{
		
[XmlElement("p1")] 
 public int DivisionID { get; set; }
[XmlElement("p2")] 
 public int NumberToShow { get; set; }

		
		
		
		public ArgGetTurnLinksForDivision ( int divisionID, int numberToShow )  : this()
		{
			
DivisionID = divisionID;  
NumberToShow = numberToShow;  

		}
	}
	

	public struct ArgLobbyChangeTeam
	{
		
[XmlElement("p1")] 
 public int RoomID { get; set; }
[XmlElement("p2")] 
 public int PlayerID { get; set; }
[XmlElement("p3")] 
 public int HostID { get; set; }
[XmlElement("p4")] 
 public int TeamID { get; set; }

		
		
		
		public ArgLobbyChangeTeam ( int roomID, int playerID, int hostID, int teamID )  : this()
		{
			
RoomID = roomID;  
PlayerID = playerID;  
HostID = hostID;  
TeamID = teamID;  

		}
	}
	

	public struct ArgGetDivisionHistory
	{
		
[XmlElement("p1")] 
 public int DivisionID { get; set; }
[XmlElement("p2")] 
 public int NumberToShow { get; set; }

		
		
		
		public ArgGetDivisionHistory ( int divisionID, int numberToShow )  : this()
		{
			
DivisionID = divisionID;  
NumberToShow = numberToShow;  

		}
	}
	

	public struct ArgLobbyKick
	{
		
[XmlElement("p1")] 
 public int RoomID { get; set; }
[XmlElement("p2")] 
 public int PlayerID { get; set; }
[XmlElement("p3")] 
 public int HostID { get; set; }

		
		
		
		public ArgLobbyKick ( int roomID, int playerID, int hostID )  : this()
		{
			
RoomID = roomID;  
PlayerID = playerID;  
HostID = hostID;  

		}
	}
	

	public struct ArgLobbyJoinRoom
	{
		
[XmlElement("p1")] 
 public int RoomID { get; set; }
[XmlElement("p2")] 
 public int PlayerID { get; set; }

		
		
		
		public ArgLobbyJoinRoom ( int roomID, int playerID )  : this()
		{
			
RoomID = roomID;  
PlayerID = playerID;  

		}
	}
	

	public struct ArgPlayerID
	{
		
[XmlElement("p1")] 
 public int PlayerID { get; set; }

		
		
		
		public ArgPlayerID ( int playerID )  : this()
		{
			
PlayerID = playerID;  

		}
	}
	

	public struct ArgGetBattlePlayersInfo
	{
		
[XmlElement("p1")] 
 public int BattleID { get; set; }

		
		
		
		public ArgGetBattlePlayersInfo ( int battleID )  : this()
		{
			
BattleID = battleID;  

		}
	}
	

	public struct ArgLobbyLeaveRoom
	{
		
[XmlElement("p1")] 
 public int RoomID { get; set; }
[XmlElement("p2")] 
 public int PlayerID { get; set; }

		
		
		
		public ArgLobbyLeaveRoom ( int roomID, int playerID )  : this()
		{
			
RoomID = roomID;  
PlayerID = playerID;  

		}
	}
	

	public struct ArgLobbySetOption
	{
		
[XmlElement("p1")] 
 public int RoomID { get; set; }
[XmlElement("p2")] 
 public int HostID { get; set; }
[XmlElement("p3")] 
 public string Key { get; set; }
[XmlElement("p4")] 
 public string Value { get; set; }

		
		
		
		public ArgLobbySetOption ( int roomID, int hostID, string key, string value )  : this()
		{
			
RoomID = roomID;  
HostID = hostID;  
Key = key;  
Value = value;  

		}
	}
	

	public struct ArgGetBattleLinksForDivision
	{
		
[XmlElement("p1")] 
 public int DivisionID { get; set; }
[XmlElement("p2")] 
 public int NumberToShow { get; set; }

		
		
		
		public ArgGetBattleLinksForDivision ( int divisionID, int numberToShow )  : this()
		{
			
DivisionID = divisionID;  
NumberToShow = numberToShow;  

		}
	}
	
}
