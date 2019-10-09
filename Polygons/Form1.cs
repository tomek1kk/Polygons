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
        private bool movingVerticle = false;
        private Verticle from;

        List<Verticle> verticles = new List<Verticle>();
        List<(Verticle, Verticle)> lines = new List<(Verticle, Verticle)>();

        private bool InArea(Point p1, Point p2, int dist)
        {
            return ((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y)) < dist * dist;
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            foreach (Verticle ver in verticles)
            {
                if (InArea(e.Location, ver.Position, 20)) // existing verticle clicked
                {
                    if (Form.ModifierKeys == Keys.Control) // ctrl is clicked - drawing line
                    {
                        drawing = true;
                        from = ver;
                    }
                    else // moving selected verticle
                    {
                        movingVerticle = true;
                        from = ver;
                    }

                }
            }
            if (drawing == false && movingVerticle == false) // add new verticle
            {
                Console.WriteLine(e.Location);
                verticles.Add(new Verticle { Position = e.Location, Id = verticles.Count + 1 });
                Invalidate();
            }

        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (drawing == true && from != null)
            {
                foreach (var ver in verticles)
                {
                    if (InArea(e.Location, ver.Position, 10))
                    {
                        lines.Add((ver, from));
                        Invalidate();
                        drawing = false;
                        return;
                    }
                }
                verticles.Add(new Verticle { Position = e.Location, Id = verticles.Count + 1 });
                lines.Add((verticles.Find(v => v.Id == verticles.Count), from));
                Invalidate();
            }
            drawing = false;
            movingVerticle = false;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            foreach (Verticle ver in verticles)
            {
                g.FillRectangle(Brushes.Red, ver.Position.X, ver.Position.Y, 5, 5);
            }

            foreach (var pair in lines)
            {
                DrawLine(pair.Item1.Position.X, pair.Item1.Position.Y, pair.Item2.Position.X, pair.Item2.Position.Y, g);
            }


        }

        private void DrawLine(int x, int y, int x2, int y2, Graphics g)
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

        private void Polygons_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 'w')
            {
                Console.WriteLine("w clicked");
                Console.WriteLine(Polygons.MousePosition.X + " , " + Polygons.MousePosition.Y);
                //Polygons.MousePosition
                
            }
        }

        private void Polygons_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left && movingVerticle == true && from != null)
            {
                from.Position = e.Location;
   
                Console.WriteLine(from);
                Invalidate();
                //pictureBox1.Left = e.X + pictureBox1.Left - MouseDownLocation.X;
                //pictureBox1.Top = e.Y + pictureBox1.Top - MouseDownLocation.Y;
            }
        }
    }
}

