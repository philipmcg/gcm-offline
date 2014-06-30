using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

using Utilities;

namespace GcmShared
{
    using PlayerInfo = Dictionary<string, string>;

    public class MapInfo
    {
        static void AddToDictionary(SidePoints s, Dictionary<PlayerInfo, Point> locations)
        {
            for (int k = 0; k < s.Players.Length; k++)
            {
                locations.Add(s.Players[k], s.DivPoints[k]);
            }
        }

        public static string Export(Battle battle)
        {
            string map = battle.Map;
            var objectives = battle.Objectives;


            XmlConstruct x = new XmlConstruct();
            x.Open("mapinfo");

            int size = (int)(double.Parse(Gcm.Data.GCSVs["maps"][map]["multiplier"]) * 8);
            string minimapid = Gcm.Data.GCSVs["maps"][map]["minimap"];

            x.Open("map");
            x.Insert("name", map);
            x.Insert("minimap", minimapid + "_MM");
            //x.Insert("minimap", map == "KS" ? Main.WebUI.GetVariable("randommap") : map + "_MM");
            x.Insert("randommap", map == "RandomMap" ? 1 : 0);
            x.Insert("sizex", size);
            x.Insert("sizey", size);
            x.Close();

            Dictionary<PlayerInfo, Point> locations = new Dictionary<Dictionary<string, string>, Point>();
            battle.Sides.ForEach(s => AddToDictionary(s.Locations, locations));

            foreach (var div in locations)
            {
                var plyr = div.Key;
                var loc = div.Value;
                x.Open("division");
                x.Insert("user", plyr["name"]);
                x.Insert("locx", loc.X);
                x.Insert("locy", loc.Y);
                x.Insert("side", plyr["side"]);
                x.Insert("character", "Unknown");
                x.Close();
            }

            int o = 1;
            foreach (var obj in objectives)
            {
                o++;
                x.Open("objective");
                x.Insert("name", o);
                x.Insert("locx", obj.X);
                x.Insert("locy", obj.Y);
                x.Close();
            }

            x.Close();

            return x.ToString();
        }
    }


}
