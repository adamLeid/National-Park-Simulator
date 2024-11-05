using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace National_Park_Simulator.World
{
    public class Road
    {
        public List<Point> road;
        public Road(List<Point> road)
        {
            this.road = road;
        }
    }
}
