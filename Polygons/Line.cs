using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace Polygons
{
    public enum Relation { None, Equal, Parallel }

    public class Line
    {
        public Vertex P1 { get; set; }
        public Vertex P2 { get; set; }
        public Brush Color { get; set; }
        public Relation Relation { get; set; }
        public bool Marked { get; set; }

        public void RecolorLine()
        {
            switch (Relation)
            {
                case Relation.None:
                    Color = Brushes.Black;
                    break;
                case Relation.Equal:
                    Color = Brushes.Purple;
                    break;
                case Relation.Parallel:
                    Color = Brushes.Green;
                    break;
            }
            if (Marked == true)
                Color = Brushes.Blue;

        }
        public int GetLineLength()
        {
            return (int)Math.Sqrt((P1.Position.X - P2.Position.X) * (P1.Position.X - P2.Position.X) +
                                  (P1.Position.Y - P2.Position.Y) * (P1.Position.Y - P2.Position.Y));
        }

        public void ReduceLine(int length, bool moveP1)
        {
            if (moveP1)
            {
                var y = P1.Position.Y - ((GetLineLength() - length) * (P1.Position.Y - P2.Position.Y) / GetLineLength());
                var x = P1.Position.X - ((GetLineLength() - length) * (P1.Position.X - P2.Position.X) / GetLineLength());
                P1.Position = new Point(x, y);
            }
            else
            {
                var y = P2.Position.Y - ((GetLineLength() - length) * (P2.Position.Y - P1.Position.Y) / GetLineLength());
                var x = P2.Position.X - ((GetLineLength() - length) * (P2.Position.X - P1.Position.X) / GetLineLength());
                P2.Position = new Point(x, y);
            }
        }

    }
}
