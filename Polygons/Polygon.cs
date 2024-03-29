﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polygons
{
    public class Polygon
    {
        public List<Vertex> Vertices { get; set; }
        public List<Line> Lines { get; set; }

        public void Print()
        {
            foreach (var v in Vertices)
            {
                Console.WriteLine("Verticle: " + v.Position);
            }
            foreach (var l in Lines)
            {
                Console.WriteLine("Line: " + l.P1.Position + " , " + l.P2.Position);
            }
        }
    }
}
