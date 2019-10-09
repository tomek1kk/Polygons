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
    public partial class Form1 : Form
    {
        public Form1()
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
                MidpointLine(pair.Item1, pair.Item2, g);
            }


        }

        private void MidpointLine(Point p1, Point p2, Graphics g)
        {
            int dx = p2.X - p1.X;
            int dy = p2.Y - p1.Y;
            int d = 2 * dy - dx; //initial value of d
            int incrE = 2 * dy; //increment used for move to E
            int incrNE = 2 * (dy - dx); //increment used for move to NE
            int x = p1.X;
            int y = p1.Y;

            g.FillRectangle(Brushes.Black, x, y, 1, 1);

            while (x < p2.X)
            {
                if (d < 0) //choose E
                {
                    d += incrE;
                    x++;
                }
                else //choose NE
                {
                    d += incrNE;
                    x++;
                    y++;
                }

                g.FillRectangle(Brushes.Black, x, y, 1, 1);
            }
        }

    }
}

