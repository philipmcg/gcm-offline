using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;

namespace GcmShared
{
  public static partial class GcmExtensions {
    public static ControlledRandom GetRealBattleRandom(this Division me) {
      return new ControlledRandom(me.LastRealBattleKey.GetHashCode());
    }
    
  }
  public static partial class FormatExtensions
    {
        const int SECOND = 1;
        const int MINUTE = 60 * SECOND;
        const int HOUR = 60 * MINUTE;
        const int DAY = 24 * HOUR;
        const int MONTH = 30 * DAY;

        static string NiceTime(int delta, TimeSpan ts)
        {
            if (delta < 0)
            {
                return "not yet";
            }
            if (delta < 1 * 50)
            {
                return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";
            }
            if (delta < 2 * MINUTE)
            {
                return "a minute ago";
            }
            if (delta < 45 * MINUTE)
            {
                return ts.Minutes + " minutes ago";
            }
            if (delta < 2 * HOUR)
            {
                return "one hour ago";
            }
            if (delta < 24 * HOUR)
            {
                return ts.Hours + " hours ago";
            }
            if (delta < 48 * HOUR)
            {
                return "yesterday";
            }
            if (delta < 28 * DAY)
            {
                return ts.Days + " days ago";
            }
            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month ago" : months + " months ago";
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? "one year ago" : years + " years ago";
            }

        }
        public static string ToGcmNiceDateString(this DateTime me) {
          TimeSpan ts = DateTime.UtcNow.Subtract(me);
          int delta = (int)ts.TotalSeconds;
          return NiceTime(delta, ts);
        }
        public static string ToGcmDateString(this DateTime me)
        {
            TimeSpan ts = DateTime.UtcNow.Subtract(me);
            int delta = (int)ts.TotalSeconds;
            if (delta < DAY)
                return NiceTime(delta, ts);
            if (me.Year != DateTime.Now.Year)
                return string.Format("{0:M-d-yy}", me);
            else
                return string.Format("{0:MMM d}", me);
        }
        public static string TimeSpanToHHMMSS(this TimeSpan me) {
          return me.ToString("hh':'mm':'ss");
}
        public static string ToGcmPreciseDateString(this DateTime me) {
          return me.ToString(GcmPreciseDateFormat);
        }
        public static string ToGcmFileDateString(this DateTime me) {
          return me.ToString(GcmFileDateFormat);
        }
        public static readonly string GcmPreciseDateFormat = "yyyy-MM-dd HH:mm:ss.fff";
        public static readonly string GcmFileDateFormat = "yyyy_MM_dd_HH_mm_ss";
    }
}
