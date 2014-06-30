using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using Utilities;


namespace GcmShared
{
    using PlayerInfo = Dictionary<string, string>;

    public struct SidePoints
    {
        public Point[] DivPoints;
        public PlayerInfo[] Players;
        public Point Main;
        public PointF Direction;
        public Battle Battle;
        public PointF CenterOfMapArea;

        int XGridMultiplier
        {
            get
            {
                return (int)(Battle.MapSize.HMultiplier * MapSize.MapRegions / MapLocations.GridSize);
            }
        }
        int YGridMultiplier
        {
            get
            {
                return (int)(Battle.MapSize.VMultiplier * MapSize.MapRegions / MapLocations.GridSize);
            }
        }
        int XOffset
        {
            get
            {
                return (int)Battle.MapSize.HOffset;
            }
        }
        int YOffset
        {
            get
            {
                return (int)Battle.MapSize.VOffset;
            }
        }


        public void Prepare(Battle battle)
        {
            Battle = battle;
            Main = PreparePoint(Main);
            for (int k = 0; k < DivPoints.Length; k++)
            {
                DivPoints[k] = PreparePoint(DivPoints[k]);
            }
            DivPoints.Shuffle();
            Players = new PlayerInfo[DivPoints.Length];
          CenterOfMapArea = new PointF(MapLocations.GridSize / 2 * XGridMultiplier + XOffset, MapLocations.GridSize / 2 * YGridMultiplier + YOffset);
        }

        private Point PreparePoint(Point p)
        {
            return new Point((p.X + 1) * XGridMultiplier + XOffset, (p.Y + 1) * YGridMultiplier + YOffset);
        }
    }

    public class MapLocations
    {
        static PointF[] Directions =
            
            {
                new PointF(0.71f,0.71f),
                new PointF(0.71f,-0.71f),
                new PointF(-0.71f,-0.71f),
                new PointF(-0.71f,0.71f),
                new PointF(1,0),
                new PointF(0,-1),
                new PointF(-1,0),
                new PointF(0,1)
            };


        public const int GridSize = 10;
        public const int Dist = GridSize - 2;
        public const int XL = GridSize - 1;
        public const int YL = GridSize - 1;
        const int NearCorner = 2;
        readonly int[,] Map;
        readonly int PercentSpread;

        public MapLocations(int numDivisions, int percentSpread)
        {
          percentSpread = Math.Max(0, Math.Min(percentSpread, 100 - numDivisions * 3));
            PercentSpread = percentSpread;
            Map = new int[XL, YL];
            int freeSpots = XL * YL - numDivisions;
            int blocked = (int)((freeSpots * percentSpread) / 100d);
            for (int i = 0; i < blocked; i++)
			{
                int x = Rand.Int(XL);
                int y = Rand.Int(YL);
                while(Map[x,y] != 0)
                {
                 x = Rand.Int(XL);
                 y = Rand.Int(YL);
                }
                Map[x,y] = -1;
			}
        }

        public static PointF DirectionFromPointToPoint(PointF a, PointF b) {
          float deltaX = b.X - a.X;
          float deltaY = b.Y - a.Y;
          double angle = Math.Atan2(deltaY, deltaX);
          double angleInDegrees = angle * 180 / Math.PI;
          return new PointF((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        static PointF DirectionFromDegrees(int degrees)
        {
            degrees -= 180;
            degrees = 360 - degrees;
            float x = (float)Math.Sin(degrees * (Math.PI / 180));
            float y = (float)Math.Cos(degrees * (Math.PI / 180));

            x = ((float)(int)(x * 100)) / 100;
            y = ((float)(int)(y * 100)) / 100;

            var pt = new PointF(x, y);
            return pt;
        }


        static PointF GetDirection(string str)
        {
            int degrees;
            if (int.TryParse(str, out degrees))
                return DirectionFromDegrees(degrees);

            var dir = Gcm.Data.GCSVs["directions"][str];
            return new PointF(float.Parse(dir["x"]), float.Parse(dir["z"]));
        }



        Point LocationFromLine(IData l)
        {
            return new Point((int)float.Parse(l["locx"]), (int)float.Parse(l["locz"]));
        }


        /// <summary>
        /// ONLY WORKS FOR EDGE POINTS
        /// </summary>
        public static PointF GetDirection(Point p)
        {
            int ymax = YL - 1;
            int xmax = XL - 1;
            int ymin = 0;
            int xmin = 0;

            if (p.X < NearCorner && p.Y < NearCorner)
                return Directions[0];
            else if (p.X < NearCorner && p.Y > ymax - NearCorner)
                return Directions[1];
            else if (p.X > xmax - NearCorner && p.Y > ymax - NearCorner)
                return Directions[2];
            else if (p.X > xmax - NearCorner && p.Y < NearCorner)
                return Directions[3];
            else if (p.X == xmin)
                return Directions[4];
            else if (p.Y == ymax)
                return Directions[5];
            else if (p.X == xmax)
                return Directions[6];
            else if (p.Y == ymin)
                return Directions[7];

#if !DEBUG
            return Directions[0];
#else
            throw new ArgumentException("Point must be along the edge!");
#endif
        }

        /// <summary>
        /// Gets two points along the edge of the 
        /// map with a minimum distance between them
        /// </summary>
        public Point[] GetTwoEdgePoints()
        {
            Point[] a = new Point[2];
            a[0] = GetEdgePoint();
            a[1] = GetEdgePoint();
            while (Distance(a[0], a[1]) < Dist)
            {
                a[Rand.Int(2)] = GetEdgePoint();
            }


            FillMapPoints(a);
            return a;
        }

        /// <summary>
        /// Fills the points in the map with 1's
        /// </summary>
        public void FillMapPoints(Point[] pts)
        {
            foreach (var p in pts)
            {
                Map[p.X, p.Y] = 1;
            }
        }

        /// <summary>
        /// Distance between two points
        /// </summary>
        public static double Distance(Point a, Point b)
        {
            double dist = Math.Sqrt(Math.Pow((a.X - b.X), 2) + Math.Pow((a.Y - b.Y), 2));
            return dist;
        }
        /// <summary>
        /// Distance between two points
        /// </summary>
        public static double Distance(PointF a, PointF b)
        {
            double dist = Math.Sqrt(Math.Pow((a.X - b.X), 2) + Math.Pow((a.Y - b.Y), 2));
            return dist;
        }

        /// <summary>
        /// Get random point along the edge of the map
        /// </summary>
        public Point GetEdgePoint()
        {
            int z = 0;
            int x = 0;

            if (Rand.Bool()) // along z-border
            {
                x = Rand.Int(XL);
                z = Rand.Bool() ? 0 : YL - 1;
            }
            else // along x-border
            {
                x = Rand.Bool() ? 0 : XL - 1;
                z = Rand.Int(YL);
            }

            return new Point(x, z);
        }

        /// <summary>
        /// Get points near the starting point for num divisions
        /// </summary>
        public SidePoints GetLocationsForSide(Point start, int numDivisions, int side)
        {
            Point[] a = new Point[numDivisions];
            for (int k = 0; k < numDivisions; k++)
            {
                int percentChanceAvoidEnemy = 120;
               a[k] = MoveRandomlyToOpen(start);
               while (Neighbors(a[k].X, a[k].Y).Any(s => s > 0 && s != side) && Rand.Percent(percentChanceAvoidEnemy--))
               {
                   percentChanceAvoidEnemy = Math.Max(0, percentChanceAvoidEnemy);
                   a[k] = MoveRandomlyToOpen(start);
               }

               Map[a[k].X, a[k].Y] = side;
            }
            return new SidePoints() { Main = FindNearest(a, a.AveragePoint()), DivPoints = a, Direction = GetDirection(start) };
        }

        Point FindNearest(IEnumerable<Point> pts, Point pt)
        {
            return pts.OrderBy(p => MapLocations.Distance(p, pt)).First();
        }
        IEnumerable<int> Neighbors(int x, int y)
        {
            yield return GetValid(x-1, y-1);
            yield return GetValid(x-1, y);
            yield return GetValid(x-1, y+1);
            yield return GetValid(x, y+1);
            yield return GetValid(x, y - 1);
            yield return GetValid(x + 1, y - 1);
            yield return GetValid(x + 1, y);
            yield return GetValid(x + 1, y + 1);
        }

        int GetValid(int x, int y)
        {
            return Map[Math.Max(0, Math.Min(x, XL - 1)), Math.Max(0, Math.Min(y, YL - 1))];
        }

        /// <summary>
        /// Get a point close to p that is not occupied on the map
        /// </summary>
        public Point MoveRandomlyToOpen(Point p)
        {
            while ((p.X < 0 || p.X >= XL || p.Y < 0 || p.Y >= YL) || Map[p.X, p.Y] != 0 || Rand.Percent(PercentSpread))
            {
                p = MoveRandomly(p);
            }
            return p;
        }

        /// <summary>
        /// Move randomly in any direction from p
        /// </summary>
        public Point MoveRandomly(Point p)
        {
            if (Rand.Bool())
                if (p.X == 0)
                    p.X += 1;
                else if (p.X == XL - 1)
                    p.X -= 1;
                else
                    p.X += Rand.Bool() ? 1 : -1;
            else
                if (p.Y == 0)
                    p.Y += 1;
                else if (p.Y == YL - 1)
                    p.Y -= 1;
                else
                    p.Y += Rand.Bool() ? 1 : -1;

            return p;
        }

    }
}
