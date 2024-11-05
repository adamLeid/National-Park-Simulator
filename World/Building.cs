using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace National_Park_Simulator.World
{
    public class Building
    {
        public List<Point> footPrint;

        public Building(List<Point> footPrint)
        {
            this.footPrint = footPrint;
        }
    }
}
