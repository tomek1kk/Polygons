using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Polygons
{
    public partial class Polygons : Form
    {
        public Polygons()
        {
            InitializeComponent();
        }

        private bool drawing = false;
        private Point from;

        List<Point> verticles = new List<Point>();
        List<(Point, Point)> lines = new List<(Point, Point)>();

        private bool InArea(Point p1, Point p2, int dist)
        {
            return ((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y)) < dist * dist;
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            foreach (Point ver in verticles)
            {
                if (InArea(e.Location, ver, 20)) // existing verticle clicked
                {
                    drawing = true;
                    from = ver;
                }
            }
            if (drawing == false) // add new verticle
            {
                verticles.Add(e.Location);
                Invalidate();
            }

        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (drawing == true && from != null)
            {
                foreach (var ver in verticles)
                {
                    if (InArea(e.Location, ver, 10))
                    {
                        lines.Add((ver, from));
                        Invalidate();
                        drawing = false;
                        return;
                    }
                }
                verticles.Add(e.Location);
                lines.Add((e.Location, from));
                Invalidate();
            }
            drawing = false;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            foreach (Point ver in verticles)
            {
                g.FillRectangle(Brushes.Red, ver.X, ver.Y, 5, 5);
            }

            foreach (var pair in lines)
            {
                //MidpointLine(pair.Item1, pair.Item2, g);
                line(pair.Item1.X, pair.Item1.Y, pair.Item2.X, pair.Item2.Y, g);
            }


        }

        public void line(int x, int y, int x2, int y2, Graphics g)
        {
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
                g.FillRectangle(Brushes.Black, x, y, 1, 1);
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
        }

    }
}

