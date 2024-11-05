using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using National_Park_Simulator.World;
using System.Diagnostics;
using National_Park_Simulator.Engine;
using National_Park_Simulator.Engine.Managers;
using System.Collections.Generic;

namespace National_Park_Simulator.World
{
    public enum MapType
    {
        Forest
    }

    public enum Biome
    {
        Grass,
        Forest
    }

    public enum TrailTile
    {
        None,
        TrailHead,
        TrailPath,
        TrailEnd
    }

    public enum BuildingTile
    {
        None,
        VisitorCenter
    }

    public class Map
    {
        public Point mapSize;

        public string name;

        public List<Trail> Trails;

        public List<Parking> ParkingLots;

        public List<Road> Roads;

        public List<Building> Buildings;

        public List<Visitor> Visitors;

        public float ParkRating;

        private float Beauty;

        public struct MapTile
        {
            public MapTile(double value, Biome type, TrailTile trailTile, bool isParking = false, bool isRoad = false, BuildingTile building = BuildingTile.None)
            {
                Data = value;
                Type = type;
                TrailTile = trailTile;
                ParkingTile = isParking;
                RoadTile = isRoad;
                BuildingTile = building;
            }

            public double Data { get; set; }
            public Biome Type { get; set; }
            public TrailTile TrailTile { get; set; }
            public bool ParkingTile { get; set; }
            public bool RoadTile { get; set; }
            public BuildingTile BuildingTile { get; set; }

            public override string ToString() => $"Data: {Data}, Type: {Type}, TrailType: {TrailTile}, Parking Area: {ParkingTile}";
        }

        public MapTile[,] MapTiles;

        public Map(Point mapSize, string name, MapTile[,] data, Road road)
        {
            this.mapSize = mapSize;
            this.name = name;
            MapTiles = data;
            Trails = new List<Trail>();
            ParkingLots = new List<Parking>();
            Roads = new List<Road>();
            Roads.Add(road);
            Buildings = new List<Building>();
            Beauty = 100f;
        }

        public void AddTrail(Trail trail)
        {
            Trails.Add(trail);
        }

        public void AddVisitors(List<Visitor> visitors)
        {
            Visitors.AddRange(visitors);
        }

        public void AddParkingLot(Parking parkingLot)
        {
            ParkingLots.Add(parkingLot);
        }

        public void AddRoad(Road road)
        {
            Roads.Add(road);
        }

        public void AddBuilding(Building building)
        {
            Buildings.Add(building);
        }

        public void Update()
        {
            CalculateParkRating();
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            Point p = new Point();
            for (int x = 0; x < mapSize.X; x++)
                for (int y = 0; y < mapSize.Y; y++)
                {
                    p.X = x;
                    p.Y = y;
                    if (MapTiles[x, y].TrailTile != TrailTile.None)
                        MapEditor.DrawTrail(spriteBatch, font, MapTiles[x, y].TrailTile, x, y, Color.Blue);
                    else if (MapTiles[x, y].ParkingTile == true)
                        MapEditor.DrawParkingLot(spriteBatch, font, p, Color.White);
                    else if (MapTiles[x, y].RoadTile == true)
                        MapEditor.DrawRoads(spriteBatch, font, p, Color.DarkGray);
                    else if (IsBuildingCord(p))
                        DrawBuilding(spriteBatch, font, p);
                    else
                        MapEditor.DrawForestChar(spriteBatch, font, MapTiles[x, y].Data, x, y);
                }
        }

        private bool IsBuildingCord(Point p)
        {
            foreach(Building b in Buildings)
                if (b.footPrint.Contains(p))
                    return true;
            return false;
        }

        private void DrawBuilding(SpriteBatch spriteBatch, SpriteFont font, Point point)
        {
            BuildingTile type = MapTiles[point.X, point.Y].BuildingTile;
            if (type == BuildingTile.VisitorCenter)
                spriteBatch.DrawString(font, "v", new Vector2(point.X * Globals.TileGridSize, point.Y * Globals.TileGridSize), Color.Brown);
        }

        public void RemoveBuilding(Building b)
        {
            foreach(Point p in b.footPrint)
            {
                MapTiles[p.X, p.Y].BuildingTile = BuildingTile.None;
            }
            Buildings.Remove(b);
        }

        private void CalculateParkRating()
        {
            ParkRating = Trails.Count * .2f + ParkingLots.Count * .10f + Visitors.Count * .25f + Beauty * .3f;
        }

        public int AvailableParking()
        {
            int parkingSpots = 0;
            foreach(Parking p in ParkingLots)
            {
                parkingSpots += p.ParkingLot.Count;
            }
            return parkingSpots;
        }

        public bool InBounds(Point p)
        {
            if (p.X >= 0 && p.X < mapSize.X && p.Y >= 0 && p.Y < mapSize.Y)
                return true;
            return false;
        }

        /// <summary>
        /// If the point is passable for a visitor
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool Passable(Point p)
        {
            if (MapTiles[p.X, p.Y].RoadTile || MapTiles[p.X, p.Y].ParkingTile || (MapTiles[p.X, p.Y].TrailTile == TrailTile.TrailHead))
            {
                return true;
            }
            return false;
        }

        public double Cost(Point p)
        {
            foreach (Road road in Roads)
                if (road.road.Contains(p))
                    return 5;
            return 1;
                
        }
    }
}
