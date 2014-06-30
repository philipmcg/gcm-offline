using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GcmShared {
  public static class PcSpecsTiers {
    public const int Horrible = 2;
    public const int Bad = 3;
    public const int Normal = 5;
    public const int Good = 7;
  }


  public static class Systems {
    public const int Wabash = 1;
  }
  public static class PlayerIDs {
    public const int Computer = 138;
  }

  public static class Hashes {
    public static int HashPlayerList(IEnumerable<int> playerIDs) {
      return string.Join(",", playerIDs.OrderBy(p => p)).GetHashCode();
    }
  }
  public enum CampaignStatus {
    Invalid,
  }

  public static class GcmFormats {
    /// <summary>
    /// Formats names like '808 Garnier' and '808f Garnier' and '808.5 Muleskinner'
    /// to get rid of the version number.
    /// </summary>
    public static string FormatName(string originalName) {
      string name = originalName.Trim('"');
      if (!char.IsNumber(name[0])) {
        return name;
      }
      if (name.Split(' ').Length > 1) {
        var split = name.Split(' ');
        double version;
        if (double.TryParse(split[0], out version)) {
          // deal with GCM names that have version in them.
          return name.Substring(name.IndexOf(' ') + 1);
        } else {
          // Deal with FOW names
          int val;
          var sf = split[0].Split('f');
          if (sf.Length == 2 && int.TryParse(sf[0], out val) && int.TryParse(sf[0], out val)) {
            return name.Substring(name.IndexOf(' ') + 1);
          }
        }
      }
      return name;
    }
  }

  public static class LinqExt {

    public static IEnumerable<TSource> DistinctBy<TSource, TKey>
    (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) {
      HashSet<TKey> seenKeys = new HashSet<TKey>();
      foreach (TSource element in source) {
        if (seenKeys.Add(keySelector(element))) {
          yield return element;
        }
      }
    }

  }
}
