using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace National_Park_Simulator.World
{
    public class Parking
    {
        public List<Point> ParkingLot;

        public Parking(List<Point> parkingLot)
        {
            ParkingLot = parkingLot;
        }
    }
}
