
	
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Xml.Serialization;
using Utilities;

namespace GE
{
	
	

	public partial class HexOrder : Order
	{
		
[XmlElement("p1")] 
 public int OriginalTerrainID { get; set; }
[XmlElement("p2")] 
 public int NewTerrainID { get; set; }

		
		public HexOrder () : base()
		{
		}
		
		public HexOrder ( int id, int factionID, int turn, int originalTerrainID, int newTerrainID ) : base(id, factionID, turn)
		{
			
OriginalTerrainID = originalTerrainID;  
NewTerrainID = newTerrainID;  

		}
	}

    public static partial class OrdersExtensionMethods
    {
        public static void AddHexOrder(this FactionOrders me, int id, int originalTerrainID, int newTerrainID )
        {
            me.Order(me.Owner.HexOrders, id, new HexOrder(id, me.FactionID, G.Game.GameTurn, originalTerrainID, newTerrainID ));
        }
        public static bool HasHexOrder(this FactionOrders me, int id)
        {
			return me.Owner.HexOrders.ContainsKey(id);
        }
        public static HexOrder HexOrder(this FactionOrders me, int id)
        {
			return me.Owner.HexOrders[id];
        }
    }
	
}
