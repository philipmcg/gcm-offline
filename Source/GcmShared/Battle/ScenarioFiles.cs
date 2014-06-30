using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Military;

using Utilities;

namespace GcmShared
{

    class ScenarioFiles
    {
        static string AddIntroLine(string title, object text)
        {
            return string.Format("<FONT=HelpText,L,0,50,10>{0}<FONT=HelpText,L,8,12,24>  {1}\n", title, text);
        }

        static string AddPlayerLine(Division div)
        {
            string[] Colors = new[] { "", "0,5,170", "170,5,0" };

            return string.Format("<FONT=HelpText,L,{0}>{1}<FONT=HelpText,L,8,12,24>  {2}<FONT=HelpText,L,140,140,140>  {3}\n", Colors[div.Side], Gcm.Var.Str["side_abb_" + div.Side], div.UserName, div.CharacterName.Last);
        }

        public static void WriteIntro(string path, Battle battle, IEnumerable<Organization> armies)
        {
            List<string> lines = new List<string>();


            if (battle.BattleType == BattleTypes.SingleplayerCampaign)
                lines.Add(@"$Intro <FONT=HelpHeader,C,8,12,24>GCM Singleplayer Campaign Battle");
            else
                lines.Add(@"$Intro <FONT=HelpHeader,C,8,12,24>GCM Multiplayer Battle " + battle.BattleID);

            if (battle.BattleType != BattleTypes.SingleplayerCampaign) {
              lines.Add(AddIntroLine("Number of Objectives:", battle.NumObjectives));
              lines.Add(AddIntroLine("Ranked:", battle.Ranked ? "Yes" : "No"));
            }

            if (!battle.IsSingleplayer) {
              foreach (var army in armies.ToArray().OrderBy(a => a.Data.Side)) {
                foreach (var corps in army.Organizations) {
                  foreach (var div in corps.Organizations) {
                    int divisionID = battle.DivisionIndexById.First(p => p.Value == div).Key;
                    var divInfo = battle.Divisions.First(d => d.DivisionID == divisionID);
                    lines.Add(AddPlayerLine(divInfo));
                  }
                }
              }
              lines.Add("\n");
            }
            lines.Add(AddIntroLine("Battle Length:", battle.LengthOfBattleInMinutes + " Minutes"));
            lines.Add(AddIntroLine("Starting Time:", Objectives.TimeToAbsolute(battle.LengthOfBattleInMinutes)));
            lines.Add(AddIntroLine("Ending Time:", Objectives.TimeToAbsolute(0)));
            lines.Add(AddIntroLine("GCM Version:", battle.GcmVersion));

            if (battle.BattleType != BattleTypes.SingleplayerCampaign)
            {
                lines.Add(AddIntroLine("Map:", Gcm.Data.GCSVs["maps"][battle.Map]["name"]));
                lines.Add(AddIntroLine("Supply:", Gcm.Data.GCSVs["supply_options"][Gcm.Var.Str["opt_s_supply"]]["name"]));
                lines.Add(AddIntroLine("Armies:", battle.OOBTypeName));
                lines.Add(AddIntroLine("Host:", battle.HostUsername));

                lines.Add("\n");
                lines.Add("\n");

            }

            FileEx.WriteAllLines(path, lines);
        }
    }
}
