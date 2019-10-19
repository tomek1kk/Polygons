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

        const int CLICK_RADIUS = 10;
        const int VERTEX_SIZE = 5;
        private bool drawing = false;
        private bool movingVertex = false;
        private Vertex from;
        private Point currentPosition;

        private static int counter = 0;

        List<Vertex> Vertexs = new List<Vertex>();
        List<Line> lines = new List<Line>();
        List<(Line, Line)> relations = new List<(Line, Line)>();




        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                foreach (Vertex ver in Vertexs)
                {
                    if (HelperFunctions.InArea(e.Location, ver.Position, 20)) // existing Vertex clicked
                    {
                        if (ver.Edges >= 2 || Form.ModifierKeys != Keys.Control) // moving vertex
                        {
                            movingVertex = true;
                            from = ver;
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
                        if (HelperFunctions.InArea(e.Location, ver.Position, CLICK_RADIUS) && ver.Edges < 2)
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
                HelperFunctions.DrawLine(HelperFunctions.BresenhamAlgorithm(line.P1.Position.X, line.P1.Position.Y, line.P2.Position.X, line.P2.Position.Y), line.Color, g);
            }
            if (drawing == true)
            {
                HelperFunctions.DrawLine(HelperFunctions.BresenhamAlgorithm(from.Position.X, from.Position.Y, currentPosition.X, currentPosition.Y), Brushes.Black, g); // TODO
            }
            g.Dispose();

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
                                HelperFunctions.ParallelLines(rel.Item1, rel.Item2);
                            }
                            else // equal relation
                            {
                                HelperFunctions.LinesEqual(rel.Item2, rel.Item1, rel.Item1.GetLineLength());
                            }
                        }
                        else if (rel.Item2.P1 == from || rel.Item2.P2 == from)
                        {
                            if (rel.Item1.Relation == Relation.Parallel)
                            {
                                HelperFunctions.ParallelLines(rel.Item2, rel.Item1);
                            }
                            else // equal relation
                            {
                                HelperFunctions.LinesEqual(rel.Item1, rel.Item2, rel.Item2.GetLineLength());
                            }
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

        private void Polygons_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                foreach (var ver in Vertexs)
                {
                    if (HelperFunctions.InArea(ver.Position, e.Location, CLICK_RADIUS))
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
                    if (HelperFunctions.InLineArea(HelperFunctions.BresenhamAlgorithm(line.P1.Position.X, line.P1.Position.Y, line.P2.Position.X, line.P2.Position.Y), e.Location, CLICK_RADIUS))
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



        private void button1_Click(object sender, EventArgs e) // make marked lines equal length
        {
            var marked = lines.FindAll(line => line.Marked == true);
            if (marked.Count != 2)
                return;
            relations.Add((marked[0], marked[1]));
            var length = marked[0].GetLineLength() > marked[1].GetLineLength() ? marked[1].GetLineLength() : marked[0].GetLineLength(); // take shorter line
            Line lineToChange = marked[0].GetLineLength() > marked[1].GetLineLength() ? marked[0] : marked[1];
            Line lineToStay = marked[0].GetLineLength() > marked[1].GetLineLength() ? marked[1] : marked[0];
            HelperFunctions.LinesEqual(lineToChange, lineToStay, length);

            marked[0].Marked = false;
            marked[1].Marked = false;

            marked[0].Relation = Relation.Equal;
            marked[1].Relation = Relation.Equal;

            marked[0].RecolorLine();
            marked[1].RecolorLine();

            Invalidate();

        }


        private void button2_Click(object sender, EventArgs e) // make marked edges parallel
        {
            var marked = lines.FindAll(l => l.Marked == true);
            if (marked.Count != 2)
                return;
            if (marked[0].P1 == marked[1].P1 || marked[0].P1 == marked[1].P2 || marked[0].P2 == marked[1].P1 || marked[0].P2 == marked[1].P2)
                return;
            relations.Add((marked[0], marked[1]));

            HelperFunctions.ParallelLines(marked[0], marked[1]);

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

