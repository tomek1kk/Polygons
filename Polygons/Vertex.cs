using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Polygons
{
    public class Vertex
    {
        public int Id { get; set; }
        public Point Position { get; set; }
        public int Edges { get; set; }
        public bool block = false;

        public void Block()
        {
            block = true;
        }
    }
}
