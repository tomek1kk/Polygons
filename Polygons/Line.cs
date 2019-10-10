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
        public Color Color { get; set; }
        public Relation Relation { get; set; }
    }
}
