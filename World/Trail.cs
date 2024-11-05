using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace National_Park_Simulator.World
{
    public enum TrailType
    {
        Invalid,
        OutandBack,
        Loop,
        Point2Point
    }
    public class Trail
    {
        public List<Point> trail;
        public TrailType trailType;

        public Trail(List<Point> trail, TrailType type)
        {
            this.trail = trail;
            trailType = type;
        }
    }
}
