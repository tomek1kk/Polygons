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
    }
}
