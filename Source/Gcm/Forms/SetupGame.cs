using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Utilities;

using GcmShared;

namespace Launcher.Forms {
  using Pair = KeyValuePair<string, string>;


  public partial class SetupGame : Dialog {
    OptionList OptionList;
    public SetupGame() {
      InitializeComponent();
    }

    public void Reset() {
      OptionList.SetDefaults();
    }

    public void Initialize(HashSet<string> flags, bool allPlayersUpToDate, BattleSetup setup) {
      Func<IData, bool> selector = l => {
            if (flags.Contains(l["gametype"]) && l["active"] == "1")
              return true;
            else
              return false;
          };

      var dict = new Dictionary<string, int>();
      var maps = Gcm.Data.GCSVs["maps"];

      foreach (var map in maps.Where(m => m["active"] == "1")) {
        var mapname = map["name"];
        var mapid = map["id"];
        map["optionname"] = mapname;
      }
      OptionList = new OptionList(Gcm.Data.GCSVs["game_options"], Gcm.Data.GCSVs.AsCollection(), Gcm.Data.VariableBin, selector, null, true);
      OptionList.SetToPanel(m_optionsPanel);

    }

    private void button1_Click(object sender, EventArgs e) {
      Reset();
    }

    private void SetupGame_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Enter) {
        this.m_OkButton.PerformClick();
      } 
    }
  }
}
