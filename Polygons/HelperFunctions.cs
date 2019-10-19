using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polygons
{
    public static class HelperFunctions
    {
        public static bool InArea(Point p1, Point p2, int dist)
        {
            return ((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y)) < dist * dist;
        }

        public static bool InLineArea(List<Point> points, Point p, int dist)
        {
            foreach (Point pp in points)
                if (InArea(pp, p, dist))
                    return true;
            return false;
        }

        public static void DrawLine(List<Point> points, Brush color, Graphics g)
        {
            foreach (Point p in points)
                g.FillRectangle(color, p.X, p.Y, 1, 1);
        }

        public static List<Point> BresenhamAlgorithm(int x, int y, int x2, int y2)
        {
            List<Point> points = new List<Point>();
            int w = x2 - x;
            int h = y2 - y;
            int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
            if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
            if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
            if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
            int longest = Math.Abs(w);
            int shortest = Math.Abs(h);
            if (!(longest > shortest))
            {
                longest = Math.Abs(h);
                shortest = Math.Abs(w);
                if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
                dx2 = 0;
            }
            int numerator = longest >> 1;
            for (int i = 0; i <= longest; i++)
            {
                points.Add(new Point(x, y));
                numerator += shortest;
                if (!(numerator < longest))
                {
                    numerator -= longest;
                    x += dx1;
                    y += dy1;
                }
                else
                {
                    x += dx2;
                    y += dy2;
                }
            }
            return points;
        }

        public static void LinesEqual(Line lineToChange, Line lineToStay, int length)
        {
            if (lineToChange.P1 == lineToStay.P1 || lineToChange.P1 == lineToStay.P2) // moving P2
            {
                lineToChange.ReduceLine(length, false);
            }
            else // moving P1
            {
                lineToChange.ReduceLine(length, true);
            }
        }

        public static void ParallelLines(Line l1, Line l2)
        {
            Random rand = new Random();
            double a = (double)(l1.P2.Position.Y - l1.P1.Position.Y) / (double)(l1.P2.Position.X - l1.P1.Position.X);
            int length = l2.GetLineLength();
            int xd = (int)(length / Math.Sqrt(a * a + 1)) + rand.Next() % 2;
            int yd = (int)(a * xd) + rand.Next() % 2;
            l2.P2.Position = new Point(l2.P1.Position.X + xd, l2.P1.Position.Y + yd);
        }
    }
}
