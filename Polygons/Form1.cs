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
            Vertexs.Add(new Vertex { Edges = 2, Id = 1, Position = new Point(200, 200) });
            Vertexs.Add(new Vertex { Edges = 2, Id = 2, Position = new Point(600, 100) });
            Vertexs.Add(new Vertex { Edges = 2, Id = 3, Position = new Point(800, 300) });
            Vertexs.Add(new Vertex { Edges = 2, Id = 4, Position = new Point(400, 500) });
            lines.Add(new Line { Color = Brushes.Black, Marked = false, Relation = Relation.None, P1 = Vertexs[0], P2 = Vertexs[1] });
            lines.Add(new Line { Color = Brushes.Black, Marked = false, Relation = Relation.None, P1 = Vertexs[1], P2 = Vertexs[2] });
            lines.Add(new Line { Color = Brushes.Black, Marked = false, Relation = Relation.None, P1 = Vertexs[2], P2 = Vertexs[3] });
            lines.Add(new Line { Color = Brushes.Black, Marked = false, Relation = Relation.None, P1 = Vertexs[3], P2 = Vertexs[0] });
            polygons.Add(new Polygon { Vertices = new List<Vertex>(Vertexs), Lines = new List<Line>(lines) });
        }

        const int CLICK_RADIUS = 10;
        const int VERTEX_SIZE = 5;
        private bool drawing = false;
        private bool movingVertex = false;
        private bool movingPolygon = false;
        private bool movingEdge = false;
        private Polygon polygonToMove;
        private Vertex from;
        private Point currentPosition;
        private Line lineToMove;
        private Point startPoint;

        private static int counter = 0;

        List<Vertex> Vertexs = new List<Vertex>();
        List<Line> lines = new List<Line>();
        List<Polygon> polygons = new List<Polygon>();
        List<(Line, Line)> relations = new List<(Line, Line)>();


        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                foreach (Vertex ver in Vertexs)
                {
                    if (HelperFunctions.InArea(e.Location, ver.Position, 20)) // existing Vertex clicked
                    {
                        if (Form.ModifierKeys == Keys.Shift) // space is clicked - moving polygon
                        {
                            foreach (var polygon in polygons) // check if vertex is part of any polygon
                            {
                                if (polygon.Vertices.Contains(ver))
                                {
                                    movingPolygon = true;
                                    polygonToMove = polygon;
                                    from = ver;
                                }
                            }
                        }
                        else if (ver.Edges >= 2 || Form.ModifierKeys != Keys.Control) // moving vertex
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
                if (Form.ModifierKeys == Keys.Alt)
                {
                    foreach (var line in lines)
                    {
                        if (HelperFunctions.InLineArea(HelperFunctions.BresenhamAlgorithm(line.P1.Position.X, line.P1.Position.Y, line.P2.Position.X, line.P2.Position.Y), e.Location, CLICK_RADIUS))
                        {
                            movingEdge = true;
                            startPoint = e.Location;
                            lineToMove = line;
                        }
                    }
                }
                if (drawing == false && movingVertex == false && movingPolygon == false && movingEdge == false) // add new Vertex
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

                if (drawing == true && from != null && movingPolygon == false)
                {
                    foreach (var ver in Vertexs)
                    {
                        if (HelperFunctions.InArea(e.Location, ver.Position, CLICK_RADIUS) && ver.Edges < 2) // line went to existing vertex
                        {
                            ver.Edges++;
                            lines.Add(new Line { P1 = from, P2 = ver, Color = Brushes.Black, Relation = Relation.None });
                            Invalidate();
                            drawing = false;
                            // CHECK IF NEW POLYGON WAS CREATED
                            Polygon polygon = GeneratePolygon(ver, from);
                            if (polygon != null)
                            {
                                polygons.Add(polygon);
                            }
                            
  
                            return;
                        }
                    }
                    Vertexs.Add(new Vertex { Position = e.Location, Id = Vertexs.Count + 1, Edges = 1 }); // create new vertex
                    lines.Add( new Line { P1 = from, P2 = Vertexs.Find(v => v.Id == Vertexs.Count),
                                            Color = Brushes.Black, Relation = Relation.None });
                    Invalidate();
                }
                drawing = false;
                movingPolygon = false;
                movingVertex = false;
                movingEdge = false;
            }
        }

        private Polygon GeneratePolygon(Vertex p1, Vertex p2)
        {
            List<Vertex> polygonVertices = new List<Vertex>();
            List<Line> polygonLines = new List<Line>();

            Vertex from = p1;

            polygonVertices.Add(from);
            while (from != p2)
            {
                Line line = lines.Find(l => l.P1 == from);
                if (line == null)
                    return null;
                from = line.P2;
                polygonVertices.Add(from);
                polygonLines.Add(line);
            }
            polygonLines.Add(lines.Find(l => l.P1 == p2));

            return new Polygon { Vertices = polygonVertices, Lines = polygonLines };
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            foreach (var polygon in polygons)
            {
                List<Point> points = new List<Point>();
                foreach (var vertex in polygon.Vertices)
                    points.Add(vertex.Position);
                g.FillPolygon(Brushes.Aqua, points.ToArray());
            }

            foreach (var line in lines)
            {
                HelperFunctions.DrawLine(HelperFunctions.BresenhamAlgorithm(line.P1.Position.X, line.P1.Position.Y, line.P2.Position.X, line.P2.Position.Y), line.Color, g);
            }

            foreach (Vertex ver in Vertexs)
            {
                g.FillRectangle(Brushes.Red, ver.Position.X, ver.Position.Y, VERTEX_SIZE, VERTEX_SIZE);
            }

            if (drawing == true)
            {
                HelperFunctions.DrawLine(HelperFunctions.BresenhamAlgorithm(from.Position.X, from.Position.Y, currentPosition.X, currentPosition.Y), Brushes.Black, g); // TODO
            }

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
            if (movingPolygon == true)
            {
                var dx = e.Location.X - from.Position.X;
                var dy = e.Location.Y - from.Position.Y;
                foreach (var ver in polygonToMove.Vertices)
                {
                    var point = new Point(ver.Position.X + dx, ver.Position.Y + dy);
                    ver.Position = point;
                }
                Invalidate();
            }
            if (movingEdge == true)
            {
                var dx = e.Location.X - startPoint.X;
                var dy = e.Location.Y - startPoint.Y;

                lineToMove.P1.Position = new Point(lineToMove.P1.Position.X + dx, lineToMove.P1.Position.Y + dy);
                lineToMove.P2.Position = new Point(lineToMove.P2.Position.X + dx, lineToMove.P2.Position.Y + dy);
                startPoint = e.Location;
                Invalidate();
            }
        }

        private void Polygons_MouseClick(object sender, MouseEventArgs e)
        {    
            if (e.Button == MouseButtons.Right)
            {
                foreach (var ver in Vertexs) // deleting vertex
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
                        foreach (var pol in polygons)
                        {
                            if (pol.Vertices.Contains(ver))
                            {
                                polygons.Remove(pol);
                                break;
                            }
                        }
                        Invalidate();
                        return;
                    }
                }
                foreach (var line in lines) // mark line
                {
                    if (HelperFunctions.InLineArea(HelperFunctions.BresenhamAlgorithm(line.P1.Position.X, line.P1.Position.Y, line.P2.Position.X, line.P2.Position.Y), e.Location, CLICK_RADIUS))
                    {
                        if (line.Marked == false && line.Relation == Relation.None)
                        {
                            var markedLines = lines.FindAll(l => l.Marked == true).Count;
                            if (markedLines < 2)
                            {
                                line.Marked = true;
                                if (markedLines == 0)
                                {
                                    button3.Enabled = true;
                                }
                                if (markedLines == 1)
                                {
                                    button1.Enabled = true;
                                    button2.Enabled = true;
                                    button3.Enabled = false;
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

        private void button3_Click(object sender, EventArgs e)
        {
            var marked = lines.FindAll(l => l.Marked == true);
            if (marked.Count != 1)
                return;
            var line = marked[0];
            Point center = new Point((line.P2.Position.X - line.P1.Position.X) / 2 + line.P1.Position.X,
                                     (line.P2.Position.Y - line.P1.Position.Y) / 2 + line.P1.Position.Y);
            Vertex ver = new Vertex { Position = center, Edges = 2, Id = Vertexs.Count + 1 };
            Vertexs.Add(ver);
            Line l1, l2;
            l1 = new Line { Color = Brushes.Black, Relation = Relation.None, Marked = false, P1 = line.P1, P2 = ver };
            l2 = new Line { Color = Brushes.Black, Relation = Relation.None, Marked = false, P1 = ver, P2 = line.P2 };
            lines.Add(l1);
            lines.Add(l2);
            lines.Remove(line);

            foreach (var polygon in polygons)
            {
                if (polygon.Lines.Contains(line))
                {
                    polygon.Lines.Remove(line);
                    polygon.Lines.Add(l1);
                    polygon.Lines.Add(l2);
                    polygon.Vertices.Insert(polygon.Vertices.FindIndex(v => v == line.P1) + 1, ver);
                }
            }

            Invalidate();
        }
    }
}

