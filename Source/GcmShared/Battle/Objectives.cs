using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using Utilities;
using Utilities.GCSV;

namespace GcmShared {

  // Cemetery Hill,SANDBOX0,minor,waypoint,100,39457.00,111062.00,213,621,1927,3,49,467,1,19:00,22:55,0:59,GFX_Obj_Minor,GFX_Obj_UMinor,GFX_Obj_CMinor
  public class Objectives {
    [Ninject.Inject]
    public ILog Log { get; set; }

    public Objectives() {
      OrderedLocations = new List<KeyValuePair<double, List<PointF>>>();
    }

    /// <summary>
    /// At noon
    /// </summary>
    static int EndMinutes = 720;

    int StartMinutes {
      get {
        return EndMinutes - Battle.LengthOfBattleInMinutes;
      }
    }

    public static string TimeToAbsolute(int minutes_from_noon) {
      int minutes = EndMinutes - minutes_from_noon;
      int hh = minutes / 60;
      int mm = minutes % 60;
      int ss = 0;
      return string.Format("{0,2:00}:{1,2:00}:{2,2:00}", hh, mm, ss);
    }

    const string Header = "Name,ID,Priority,Type,AI,loc x,loc z,radius,Men,Points,Fatigue,Morale,Ammo,OccMod,Beg,End,Interval,Sprite,Army1,Army2,Army3";

    List<PointF> Locations;
    List<PointF> BestLocations;
    List<KeyValuePair<double, List<PointF>>> OrderedLocations;
    int[] ObjectiveStartTimes;
    Battle Battle;
    PointF Center;
    double AverageStartTime;

    public static List<PointF> CreateMapLocationsFile(string path, Battle battle) {
      Objectives obj = Gcm.Provider.Get<Objectives>();
      obj.Battle = battle;
      return obj.CreateObjectives(path);
    }


    const int NumPhases = 6;
    static int[] ObjectivePhasePoints = { 1, 4, 10, 23, 31, 31 };

    int[] PhaseStartTimes;
    int[] PhaseLengths;
    int[] PhasePoints;
    int PhaseLength;

    void CreateObjectivePhases(int startTime, int length) {
      Log.Write("CreateObjectivePhases");
      double phaseLengthRaw = length / (double)NumPhases;
      PhaseLength = (int)Math.Ceiling(phaseLengthRaw);

      PhaseStartTimes = new int[NumPhases];
      PhaseLengths = new int[NumPhases];
      double elapsed = 0;
      int phasesSum = 0;
      for (int i = 0; i < NumPhases; i++) {
        elapsed += phaseLengthRaw;
        PhaseLengths[i] = (int)elapsed - phasesSum;
        phasesSum += PhaseLengths[i];
        PhaseStartTimes[i] = phasesSum - PhaseLengths[i];
      }

    }

    static readonly DateTime Today = DateTime.UtcNow;
    int GetTotalPointsForBattle() {
      // gradually increase the points from objectives.  They will increase by 100% of the base every year. 
      var timePassed = Today.Subtract(new DateTime(2013, 1, 1));
      double daysPassed = timePassed.TotalDays;
      double modifier = daysPassed / 365;
      int numYearsToIncreaseFrom2013 = 2;  // Once 2015 hits this will stop increasing.
      modifier = Math.Min(numYearsToIncreaseFrom2013, modifier);

      double MySneakyMultiplierToIncreaseThePointsInBattleWithoutAnyoneKnowing = 1 + modifier;
      // make sure the multiplier is between 1 and 2 if weirdness happens
      MySneakyMultiplierToIncreaseThePointsInBattleWithoutAnyoneKnowing = Math.Max(1, Math.Min(2, MySneakyMultiplierToIncreaseThePointsInBattleWithoutAnyoneKnowing));


      MySneakyMultiplierToIncreaseThePointsInBattleWithoutAnyoneKnowing *= 1.3;

      return (int)(Battle.TotalMen * MySneakyMultiplierToIncreaseThePointsInBattleWithoutAnyoneKnowing * double.Parse(Gcm.Var.Str["opt_s_objective_point_percentages"]) / 100);
    }

    const int SillySowDoublesThePoints = 2;
    int[] GetObjectivePhasePoints() {
      Log.Write("GetObjectivePhasePoints");
      int totalPoints = GetTotalPointsForBattle() / SillySowDoublesThePoints;

      int[] defaults = (int[])ObjectivePhasePoints.Clone();

      int totalPointsWithCurrentWeights = Enumerable.Range(0, NumPhases).Sum(i => defaults[i] * (PhaseLengths[i] - 1)) * Battle.NumObjectives; // phase length -1 because it's impossible to get exactly all of the points from a phase

      double multiplier = totalPoints / (double)totalPointsWithCurrentWeights;

      // Multiply the numbers by the multiplier.
      defaults = defaults.Select(i => (int)Math.Max(1, Math.Round(i * multiplier))).ToArray();
      return defaults;
    }

    int GetPointsPerObjective() {
      return 50 * (int)Math.Round(GetTotalPointsForBattle() / 50.0 / (double)Battle.NumObjectives);
    }

    /// <summary>
    /// Creates objectives according the global variable settings, and saves them to a maplocations file with the path specified.
    /// </summary>
    List<PointF> CreateObjectives(string path) {
      Log.Write("CreateObjectives");
      // to avoid a crash..
      var a = Gcm.Var.Str["opt_s_objective_length", "0"];
      var b = Gcm.Var.Str["opt_s_objective_time_buffer", "100"];
      var c = Gcm.Var.Str["opt_s_objective_points", "100"];
      var d = Gcm.Var.Str["opt_s_objective_available_time", "100"];
      bool objectivesVisibleAtStart = Gcm.Var.Str["opt_s_objective_length"] == "1"; // magic constant for Points every minute visible at start.

      int objectiveStartInterval = 5;

      Locations = new List<PointF>();

      int time_buffer;
      var tb = Gcm.Var.Str["opt_s_objective_time_buffer"];
      tb = "p60";
      if (tb[0] == 'p')
        time_buffer = (int)((int.Parse(tb.Substring(1)) * Battle.LengthOfBattleInMinutes) / 100.0);
      else
        time_buffer = tb.ToInt();

      int time = Battle.LengthOfBattleInMinutes;

      int numObjectives = Battle.NumObjectives;

      int timeAvailable = (time - time_buffer) - (time % objectiveStartInterval);

      double interval = timeAvailable / (double)numObjectives;

      ObjectiveStartTimes = new int[numObjectives];
      double cur_time = 0;

      int StartMinutes = (EndMinutes - time);
      int MinutesObjectivesAreAvailable = (EndMinutes - timeAvailable);

      for (int k = 0; k < numObjectives; k++) {
        int timeForThisObjective = ((int)((cur_time / (double)objectiveStartInterval) + Rand.Double())) * objectiveStartInterval;

        ObjectiveStartTimes[k] = EndMinutes - (timeAvailable - timeForThisObjective + time_buffer);
        cur_time += interval;

        // If we want objectives visible from start, override the normal start time.
        if (objectivesVisibleAtStart)
          ObjectiveStartTimes[k] = StartMinutes;
      }

      AverageStartTime = ObjectiveStartTimes.Average();

      List<string> lines = new List<string>();
      lines.Add(Header);
      lines.Add("");

      Log.Write("Get Locations");
      if (Gcm.Var.Str["opt_s_objective_distribution"] == "even")
        GetBalancedLocations(numObjectives);
      else
        GetRandomLocations(numObjectives);

      if (Battle.Flags.Contains("custom_obj")) {
        CreateObjectivePhases(MinutesObjectivesAreAvailable, Battle.LengthOfBattleInMinutes);
        PhasePoints = GetObjectivePhasePoints();
      }

      Log.Write("Write Objectives");

      bool objectivesArePointsEveryMinute = Gcm.Var.Str["opt_s_objective_length"] == "0" || Gcm.Var.Str["opt_s_objective_length"] == "1";

      for (int k = 0; k < numObjectives; k++) {
        if (Battle.Flags.Contains("custom_obj")) {
          if (objectivesArePointsEveryMinute) {
            for (int i = 0; i < NumPhases; i++) {
              lines.Add(CreateObjective("custom", i + "", 0, BestLocations[k], k + 1));
            }
          } else
            lines.Add(CreateObjective("custom_waypoint", "", GetPointsPerObjective(), BestLocations[k], k + 1));

        } else {
          lines.Add(CreateObjective("random_loc", "Objective" + (k + 1), 1000, BestLocations[k], k + 1));
        }
      }

      for (int k = 0; k < numObjectives; k++) {
        if (Battle.Flags.Contains("custom_obj")) {
        } else {
          lines.Add(CreateObjective("random_obj", "Objective" + (k + 1), 1000, BestLocations[k], k + 1));
        }
      }

      float xmult = (float)Battle.MapSize.HMultiplier;
      float ymult = (float)Battle.MapSize.VMultiplier;
      float xoffset = (float)Battle.MapSize.HOffset;
      float yoffset = (float)Battle.MapSize.VOffset;
      float width = xmult * 8;
      float height = ymult * 8;

      if (Battle.BattleType != BattleTypes.SingleplayerCampaign) {
        lines.Add(CreateObjective("corner", "Northwest Corner", 0, new PointF(xoffset, yoffset), 0));
        lines.Add(CreateObjective("corner", "Southwest Corner", 0, new PointF(xoffset + width, yoffset), 0));
        lines.Add(CreateObjective("corner", "Northeast Corner", 0, new PointF(xoffset, yoffset + height), 0));
        lines.Add(CreateObjective("corner", "Southeast Corner", 0, new PointF(xoffset + width, yoffset + height), 0));
      }

      FileEx.WriteAllLines(path, lines);
      return BestLocations;
    }

    void GetBalancedLocations(int num) {
      int NumTries = 50 + num * num * 5;

      for (int k = 0; k < NumTries; k++) {
        GetSpreadLocations(num);
      }

      OrderedLocations = OrderedLocations
          .OrderByDescending(p => p.Key)
          .Take(10)
          .ToList();
      GetBestLocationsWithArmies();
    }

    void GetRandomLocations(int num) {
      Locations = new List<PointF>();

      float xmult = (float)Battle.MapSize.HMultiplier;
      float ymult = (float)Battle.MapSize.VMultiplier;
      float xoffset = (float)Battle.MapSize.HOffset;
      float yoffset = (float)Battle.MapSize.VOffset;

      for (int k = 0; k < num; k++) {
        int xarea = (int)(MapSize.MapRegions * xmult);
        int yarea = (int)(MapSize.MapRegions * ymult);
        PointF loc;

        if (Rand.Percent(50))
          loc = new PointF((float)Rand.Int(xarea), (float)Rand.Int(yarea));
        else
          loc = new PointF((float)Rand.Curved(xarea / 2, 40), (float)Rand.Curved(yarea / 2, 40));

        loc.X += xoffset;
        loc.Y += yoffset;
        Locations.Add(loc);
      }

      BestLocations = Locations;
    }
    void GetBestLocationsWithArmies() {
      Func<List<PointF>, double> f = (l) => {
        double dist1 = 0;
        double dist2 = 0;
        foreach (var p in l) {
          dist1 += Battle.Sides[1].Locations.DivPoints.Min(d => MapLocations.Distance(p, d));
          dist2 += Battle.Sides[2].Locations.DivPoints.Min(d => MapLocations.Distance(p, d));
        }
        return Math.Abs(dist1 - dist2);
      };

      var loc = OrderedLocations.OrderBy(l => f(l.Value)).First();
      BestLocations = loc.Value;
    }

    double GetDistanceFromCenter(PointF center, PointF pt) {
      double distFromCenter = MapLocations.Distance(center, pt);
      return Math.Sqrt(Math.Max(distFromCenter / 10000, 2));
    }

    public void GetSpreadLocations(int num) {
      Locations = new List<PointF>();
      double lowest = 0;

      float xmult = (float)Battle.MapSize.HMultiplier;
      float ymult = (float)Battle.MapSize.VMultiplier;
      float xoffset = (float)Battle.MapSize.HOffset;
      float yoffset = (float)Battle.MapSize.VOffset;

      float lengthOfShortestSide = Math.Min(ymult, xmult);
      Center = new PointF(4 * xmult + xoffset, 4 * ymult + yoffset);

      var distancesFromCenter = new double[num];
      //int distanceFromEdge = Gcm.Var.Str["opt_s_objective_distance_from_edge"].ToInt();
      double objectiveAreaMultiplier = double.Parse(Gcm.Data.GCSVs["objective_areas"][Gcm.Var.Str["opt_s_objective_area"]]["multiplier"]);
      objectiveAreaMultiplier = Math.Min(lengthOfShortestSide, objectiveAreaMultiplier);
      int distanceFromEdge = (int)((((lengthOfShortestSide-objectiveAreaMultiplier) / lengthOfShortestSide) * 100) / 2);
      double area = 8;
      double borderRatio = distanceFromEdge / 100d;
      double borderWidth = area * borderRatio;
      double randomArea = area - (borderWidth * 2);

      for (int k = 0; k < num; k++) {
        int d = 100 - (distanceFromEdge * 2);
        PointF loc;
        if (Rand.Percent(40))
          loc = new PointF((float)(Rand.CurvedDouble(4d, d) * xmult), (float)(Rand.CurvedDouble(4d, d) * ymult));
        else
          loc = new PointF((float)(((Rand.NextDouble() * randomArea) + borderWidth) * xmult), (float)(((Rand.NextDouble() * randomArea) + borderWidth) * ymult));
        loc.X += xoffset;
        loc.Y += yoffset;
        Locations.Add(loc);
        distancesFromCenter[k] = GetDistanceFromCenter(Center, loc);
      }


      for (int k = 0; k < num; k++) {
        for (int j = (k + 1); j < num; j++) {
          double dist = Math.Max(1, MapLocations.Distance(Locations[k], Locations[j]));
          // decided to forget about distance from center as it is unpredictable
          //dist = dist / ((distancesFromCenter[k] + distancesFromCenter[j]) / 2);
          if (lowest == 0 || dist < lowest)
            lowest = dist;
        }
      }

      OrderedLocations.Add(new KeyValuePair<double, List<PointF>>(lowest, Locations));
    }

    string GetTimeString(int starttime, int minutes) {
      return GetTimeString(minutes + starttime);
    }

    string GetTimeString(int minutes) {
      int hours = minutes / 60;
      minutes = minutes % 60;
      int seconds = 0;

      return string.Format("{0}:{1}:{2}", hours, minutes, seconds);
    }

    Dictionary<string, object> GetObjectiveEntries(string id, string name, int points, PointF loc, int n) {
      var d = new Dictionary<string, object>();

      d["locx"] = loc.X;
      d["locz"] = loc.Y;
      d["points"] = points;


      switch (id) {
        case "custom":
          int phase = name.ToInt();
          int l = PhaseLengths.Length - 1;
          int pointsInLastThird = (PhaseLengths[l - 1] * PhasePoints[l - 1] + PhaseLengths[l] * PhasePoints[l]) * 2;
          d["name"] = string.Format("[{0}] Objective {1} (phase {2}) ({3})", Battle.BattleID, n, phase + 1, pointsInLastThird);
          d["radius"] = Gcm.Var.Str["opt_s_objective_radius"];
          d["men"] = Gcm.Var.Str["opt_s_objective_men_to_hold"];
          if ((string)d["men"] == "0")
            d["men"] = "5";

          d["points"] = PhasePoints[phase];
          // int time = Gcm.Var.Str["opt_s_objective_length"].ToInt();

          int start = ObjectiveStartTimes[n - 1];
          int phaseStart = PhaseStartTimes[phase] + StartMinutes;
          start = Math.Max(start, phaseStart);

          d["beg"] = GetTimeString(start);
          d["end"] = GetTimeString(phaseStart, PhaseLengths[phase]);
          d["type"] = "hold";
          //d["end"] = GetTimeString(ObjectiveStartTimes[n - 1], (Rand.Int(4) * 10) + Gcm.Var.Str["opt_s_objective_available_time"].ToInt());

          d["interval"] = GetTimeString(1);
          break;

        case "custom_waypoint":
          d["name"] = string.Format("[{0}] Objective {1}", Battle.BattleID, n);
          d["radius"] = Gcm.Var.Str["opt_s_objective_radius"];
          d["men"] = Gcm.Var.Str["opt_s_objective_men_to_hold"];
          d["beg"] = GetTimeString(ObjectiveStartTimes[n - 1]);
          d["end"] = GetTimeString(ObjectiveStartTimes[n - 1], EndMinutes - ObjectiveStartTimes[n - 1] + 10);
          d["interval"] = GetTimeString(Gcm.Var.Str["opt_s_objective_length"].ToInt());
          d["points"] = points / 2;
          break;
        case "random_loc":
          d["name"] = string.Format("{0}", name);
          d["points"] = points;
          d["beg"] = GetTimeString(ObjectiveStartTimes[n - 1]);
          d["end"] = GetTimeString(EndMinutes, -10);
          d["interval"] = GetTimeString(600);
          break;
        case "random_obj":
          d["name"] = string.Format("{0} (cap)", name);
          d["points"] = points * 100;
          d["beg"] = GetTimeString(EndMinutes, -10);
          d["end"] = GetTimeString(EndMinutes);
          d["interval"] = GetTimeString(1);
          break;
        case "scenario_loc":
          d["name"] = string.Format("{0}", name);
          d["points"] = points;
          d["beg"] = GetTimeString(StartMinutes);
          d["end"] = GetTimeString(EndMinutes, -10);
          d["interval"] = GetTimeString(600);
          break;
        case "scenario_obj":
          d["name"] = string.Format("{0} (cap)", name);
          d["points"] = points * 100;
          d["beg"] = GetTimeString(EndMinutes, -10);
          d["end"] = GetTimeString(EndMinutes);
          d["interval"] = GetTimeString(1);
          break;
        case "corner":
          d["name"] = name;
          d["beg"] = GetTimeString(StartMinutes);
          d["end"] = GetTimeString(EndMinutes);
          d["interval"] = GetTimeString(600);
          break;
        default:
          break;
      }

      d["id"] = d["name"].ToString().Replace(' ', '_');
      return d;
    }
    string CreateObjective(string id, string name, int points, PointF loc, int n) {
      var csv = Gcm.Data.GCSVs["objective_templates"];

      GCSVLine line = new GCSVLine(csv.Header);
      var dict = GetObjectiveEntries(id, name, points, loc, n);

      foreach (var entry in csv.Header.Keys) {
        if (dict.ContainsKey(entry))
          line[entry] = dict[entry].ToString();
        else
          line[entry] = Gcm.Data.GCSVs["objective_templates"][id][entry];
      }

      return line.FieldArray.Implode(',');
    }
  }
}
