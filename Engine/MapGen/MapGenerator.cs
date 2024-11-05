using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using National_Park_Simulator.World;
using static National_Park_Simulator.World.Map;

namespace National_Park_Simulator.Engine.MapGen
{
    public static class WorldSize
    {
        public static Point Small = new Point(80, 80);
        public static Point Medium = new Point(200, 200);
        public static Point Large = new Point(300, 300);
    }

    public static class MapGenerator
    {
        private const double Forest_Floor = 0.5;
        //private const long Forest_Seed = 123;
        private const float Forest_Noise = 10;
        public static Map GenerateMap(Point mapSize, string name, MapType mapType)
        {
            // Generate Biome specific Map
            MapTile[,] mapTiles = new MapTile[mapSize.X, mapSize.Y];
            if (mapType == MapType.Forest)
                mapTiles = GenerateForestPark(mapSize, Forest_Noise, Forest_Floor);

            // Generate Main Road
            Road road = GenerateMainRoad(mapSize, mapTiles);
            return new Map(mapSize, name, mapTiles, road);
        }

        /// <summary>
        /// Create a randomly generated main road that acts as the entrance to the park map.
        /// Entrance road must be 2 lane with a minimum length of 4 tiles and maximum length of 1/4 length of the map.
        /// Each lane of the road (side by side) must be on the same elevation level.
        /// </summary>
        /// <param name="mapSize"></param>
        /// <param name="mapTiles"></param>
        private static Road GenerateMainRoad(Point mapSize, MapTile[,] mapTiles)
        {
            Random rng = new Random();

            // Pick road spawn direction
            double direc = rng.NextDouble();
            string spawnDirec = null;
            if (direc >= 0 && direc < 0.25)
                spawnDirec = "north";
            else if (direc >= .25 && direc < .5)
                spawnDirec = "south";
            else if (direc >= .5 && direc < .75)
                spawnDirec = "east";
            else
                spawnDirec = "west";
            int width = 2;

            // Pick dimensions
            int length;
            if (spawnDirec == "north" || spawnDirec == "south")
                length = rng.Next(4, mapSize.Y / 4);
            else
                length = rng.Next(4, mapSize.X / 4);
            int totalTiles = length * width;

            // Pick start coord
            List<Point> road = new List<Point>();
            if (spawnDirec == "north")
            {
                Point p1 = new Point(rng.Next(1, mapSize.X - 1), 0);
                Point p2 = new Point(p1.X + 1, 0);
                road.Add(p1);
                road.Add(p2);
                while (road.Count < totalTiles)
                {
                    p1 = new Point(p1.X, p1.Y + 1);
                    p2 = new Point(p2.X, p2.Y + 1);
                    road.Add(p1);
                    road.Add(p2);
                }
            }
            else if (spawnDirec == "south")
            {
                Point p1 = new Point(rng.Next(1, mapSize.X - 1), mapSize.Y - 1);
                Point p2 = new Point(p1.X + 1, mapSize.Y - 1);
                road.Add(p1);
                road.Add(p2);
                while (road.Count < totalTiles)
                {
                    p1 = new Point(p1.X, p1.Y - 1);
                    p2 = new Point(p2.X, p2.Y - 1);
                    road.Add(p1);
                    road.Add(p2);
                }
            }
            else if (spawnDirec == "east")
            {
                Point p1 = new Point(mapSize.X - 1, rng.Next(1, mapSize.Y - 1));
                Point p2 = new Point(mapSize.X - 1, p1.Y - 1);
                road.Add(p1);
                road.Add(p2);
                while (road.Count < totalTiles)
                {
                    p1 = new Point(p1.X - 1, p1.Y);
                    p2 = new Point(p2.X - 1, p2.Y);
                    road.Add(p1);
                    road.Add(p2);
                }
            }
            else
            {
                Point p1 = new Point(0, rng.Next(1, mapSize.Y - 1));
                Point p2 = new Point(0, p1.Y - 1);
                road.Add(p1);
                road.Add(p2);
                while (road.Count < totalTiles)
                {
                    p1 = new Point(p1.X + 1, p1.Y);
                    p2 = new Point(p2.X + 1, p2.Y);
                    road.Add(p1);
                    road.Add(p2);
                }
            }

            // assign road tiles to these points
            foreach (Point p in road)
                mapTiles[p.X, p.Y].RoadTile = true;
            return new Road(road);
        }

        private static MapTile[,] GenerateForestPark(Point mapSize, float noiseFactor, double floor)
        {
            MapTile[,] tiles = GetNoiseMap2D(mapSize, noiseFactor, floor);
            // TODO: Add trees, ponds and other things.
            //...
            // Set tile types
            for (int x = 0; x < mapSize.X; x++)
                for (int y = 0; y < mapSize.Y; y++)
                {
                    if (tiles[x, y].Data >= Forest_Floor && tiles[x, y].Data < 0.6)
                        tiles[x, y].Type = Biome.Grass;
                    else
                        tiles[x, y].Type = Biome.Forest;
                }

            return tiles;
        }

        private static MapTile[,] GetNoiseMap2D(Point mapSize, float noiseFactor, double floor)
        {
            MapTile[,] tiles = new MapTile[mapSize.X, mapSize.Y];
            OpenSimplexNoise oSimplexNoise = new OpenSimplexNoise();
            for (int x = 0; x < mapSize.X; x++)
                for (int y = 0; y < mapSize.Y; y++)
                {
                    double n = oSimplexNoise.Evaluate(x / noiseFactor, y / noiseFactor);
                    n += 1.0;
                    n /= 2.0;
                    if (n < floor)
                        n = floor;
                    tiles[x, y].Data = n;
                }
            return tiles;
        }
    }
}
