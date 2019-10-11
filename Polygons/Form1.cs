﻿using System;
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

        const int CLICK_RADIUS = 10;
        const int VERTEX_SIZE = 5;

        private bool drawing = false;
        private bool movingVertex = false;
        private Vertex from;
        private Point currentPosition;
        private Random rand = new Random();
        private static int counter = 0;

        List<Vertex> Vertexs = new List<Vertex>();
        List<Line> lines = new List<Line>();
        List<(Line, Line)> relations = new List<(Line, Line)>();

        private bool InArea(Point p1, Point p2, int dist)
        {
            return ((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y)) < dist * dist;
        }

        private bool InLineArea(int x, int y, int x2, int y2, Point p)
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
                if (InArea(new Point(x, y), p, 10))
                    return true;
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
            return false;
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                foreach (Vertex ver in Vertexs)
                {
                    if (InArea(e.Location, ver.Position, 20)) // existing Vertex clicked
                    {
                        if (ver.Edges >= 2 || Form.ModifierKeys != Keys.Control) // moving vertex
                        {
                            //if (lines.FindAll(l => l.P1 == ver || l.P2 == ver).All(li => li.Relation == Relation.None))
                            //{
                                movingVertex = true;
                                from = ver;
                            //}
                            //else
                             //   return;
                        }
                        else if (Form.ModifierKeys == Keys.Control && ver.Edges < 2) // ctrl is clicked - drawing line
                        {
                            ver.Edges++;
                            drawing = true;
                            from = ver;
                        }

                    }
                }
                if (drawing == false && movingVertex == false) // add new Vertex
                {
                    Console.WriteLine(e.Location);
                    Vertexs.Add(new Vertex { Position = e.Location, Id = Vertexs.Count + 1, Edges = 0 });
                    Invalidate();
                }
            }

        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {

                if (drawing == true && from != null)
                {
                    foreach (var ver in Vertexs)
                    {
                        if (InArea(e.Location, ver.Position, CLICK_RADIUS) && ver.Edges < 2)
                        {
                            ver.Edges++;
                            lines.Add(new Line { P1 = ver, P2 = from, Color = Brushes.Black, Relation = Relation.None });
                            Invalidate();
                            drawing = false;
                            return;
                        }
                    }
                    Vertexs.Add(new Vertex { Position = e.Location, Id = Vertexs.Count + 1, Edges = 1 });
                    lines.Add( new Line { P1 = Vertexs.Find(v => v.Id == Vertexs.Count), P2 = from,
                                            Color = Brushes.Black, Relation = Relation.None });
                    Invalidate();
                }
                drawing = false;
                movingVertex = false;
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            foreach (Vertex ver in Vertexs)
            {
                g.FillRectangle(Brushes.Red, ver.Position.X, ver.Position.Y, VERTEX_SIZE, VERTEX_SIZE);
            }

            foreach (var line in lines)
            {
                DrawLine(line.P1.Position.X, line.P1.Position.Y, line.P2.Position.X, line.P2.Position.Y, line.Color, g);
            }
            if (drawing == true)
            {
                DrawLine(from.Position.X, from.Position.Y, currentPosition.X, currentPosition.Y, Brushes.Black, g); // TODO
            }
            g.Dispose();

        }



        private void DrawLine(int x, int y, int x2, int y2, Brush color, Graphics g)
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
                g.FillRectangle(color, x, y, 1, 1);
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

        }

        private void Polygons_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left && movingVertex == true && from != null)
            {
                from.Position = e.Location;
                if (counter++ % 10 == 0)
                {
                    foreach (var rel in relations)
                    {
                        if (rel.Item1.P1 == from || rel.Item1.P2 == from) // moving vertex with relation
                        {
                            if (rel.Item1.Relation == Relation.Parallel)
                            {
                                ParallelLines(rel.Item1, rel.Item2);
                            }
                            else // equal relation
                            {
                                LinesEqual(rel.Item2, rel.Item1, rel.Item1.GetLineLength());
                            }
                            //break;
                        }
                        else if (rel.Item2.P1 == from || rel.Item2.P2 == from)
                        {
                            if (rel.Item1.Relation == Relation.Parallel)
                            {
                                ParallelLines(rel.Item2, rel.Item1);
                            }
                            else // equal relation
                            {
                                LinesEqual(rel.Item1, rel.Item2, rel.Item2.GetLineLength());
                            }
                            //break;
                        }

                    }
                }
                Invalidate();
            }
            
            if (drawing == true)
            {
                currentPosition = e.Location;
                Invalidate();
            }
        }

        private void Polygons_Click(object sender, EventArgs e)
        {
            
        }

        private void Polygons_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                foreach (var ver in Vertexs)
                {
                    if (InArea(ver.Position, e.Location, CLICK_RADIUS))
                    {
                        Vertexs.Remove(ver);
                        lines.RemoveAll(line =>
                            {
                                if (line.P1 == ver)
                                {
                                    line.P2.Edges--;
                                    return true;
                                }
                                else if (line.P2 == ver)
                                {
                                    line.P1.Edges--;
                                    return true;
                                }
                                return false;
                            });
                        Invalidate();
                        return;
                    }
                }
                foreach (var line in lines)
                {
                    if (InLineArea(line.P1.Position.X, line.P1.Position.Y, line.P2.Position.X, line.P2.Position.Y, e.Location))
                    {
                        if (line.Marked == false && line.Relation == Relation.None)
                        {
                            var markedLines = lines.FindAll(l => l.Marked == true).Count;
                            if (markedLines < 2)
                            {
                                line.Marked = true;
                                if (markedLines == 1)
                                {
                                    button1.Enabled = true;
                                    button2.Enabled = true;
                                }
                            }
                                
                        }
                        else
                        {
                            line.Relation = Relation.None;
                                relations.Remove(relations.FirstOrDefault(l => l.Item1 == line || l.Item2 == line));
                            line.Marked = false;
                        }
                        line.RecolorLine();
                        Invalidate();
                        return;

                    }
                }
                
            }
        }

        private void LinesEqual(Line lineToChange, Line lineToStay, int length)
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

        private void button1_Click(object sender, EventArgs e) // make marked lines equal length
        {
            var marked = lines.FindAll(line => line.Marked == true);
            if (marked.Count != 2)
                return;
            relations.Add((marked[0], marked[1]));
            var length = marked[0].GetLineLength() > marked[1].GetLineLength() ? marked[1].GetLineLength() : marked[0].GetLineLength(); // take shorter line
            Line lineToChange = marked[0].GetLineLength() > marked[1].GetLineLength() ? marked[0] : marked[1];
            Line lineToStay = marked[0].GetLineLength() > marked[1].GetLineLength() ? marked[1] : marked[0];
            LinesEqual(lineToChange, lineToStay, length);

            marked[0].Marked = false;
            marked[1].Marked = false;

            marked[0].Relation = Relation.Equal;
            marked[1].Relation = Relation.Equal;

            marked[0].RecolorLine();
            marked[1].RecolorLine();

            Invalidate();

        }

        private void ParallelLines(Line l1, Line l2)
        {
            double a = (double)(l1.P2.Position.Y - l1.P1.Position.Y) / (double)(l1.P2.Position.X - l1.P1.Position.X);
            int length = l2.GetLineLength();
            int xd = (int)(length / Math.Sqrt(a * a + 1)) + rand.Next() % 2;
            int yd = (int)(a * xd) + rand.Next() % 2;
            l2.P2.Position = new Point(l2.P1.Position.X + xd, l2.P1.Position.Y + yd);

        }

        private void button2_Click(object sender, EventArgs e) // make marked edges parallel
        {
            var marked = lines.FindAll(l => l.Marked == true);
            if (marked.Count != 2)
                return;
            if (marked[0].P1 == marked[1].P1 || marked[0].P1 == marked[1].P2 || marked[0].P2 == marked[1].P1 || marked[0].P2 == marked[1].P2)
                return;
            relations.Add((marked[0], marked[1]));

            ParallelLines(marked[0], marked[1]);

            marked[0].Marked = false;
            marked[1].Marked = false;

            marked[0].Relation = Relation.Parallel;
            marked[1].Relation = Relation.Parallel;

            marked[0].RecolorLine();
            marked[1].RecolorLine();

            Invalidate();
        }
    }
}

