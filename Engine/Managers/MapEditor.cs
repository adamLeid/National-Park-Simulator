using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using National_Park_Simulator.World;
using System.Diagnostics;
using National_Park_Simulator.Engine;
using National_Park_Simulator.Engine.Managers;
using System.Collections.Generic;
using static National_Park_Simulator.World.Map;

namespace National_Park_Simulator.Engine.Managers
{
    public static class MapEditor
    {
        // Menu Level 1
        private const Keys BuildMode = Keys.B;
        public static bool Edit = false;

        // Menu Level 2
        private const Keys TrailMode = Keys.F1;
        private static bool PlaceTrail = false;
        private const Keys ParkingMode = Keys.F2;
        private static bool PlaceParking = false;
        private const Keys PlaceBuildings = Keys.F3;
        private static bool PlaceBuilding = false;

        // Menu Level 3
        private const Keys PlaceVisitorCenter_Key = Keys.V;
        private static bool PlaceVisitorCenter = false;
        private const Keys TrailHead_Key = Keys.H;
        private static bool PlaceTrailHead = false;
        private const Keys TrailPath = Keys.T;
        private static bool PlaceTrailPath = false;
        private const Keys ScanBuild_Key = Keys.Enter;
        private static bool PlaceTrailEnd = false;
        private const Keys TrailEnd_Key = Keys.E;

        // Building Placement
        private static bool footPrintValid = false;
        private static List<Point> FootPrint = new List<Point>();

        public static void Draw(SpriteBatch spriteBatch, SpriteFont font, Map currentMap)
        {

            // Draw the map in edit mode
            if (Edit)
            {
                // mouse highlight
                Point mosTile = GetMouseTilePos();
                Texture2D cursorOutline = new Texture2D(Globals.GraphicsDevice, 1, 1);
                cursorOutline.SetData(new Color[] { Color.Red });
                if (mosTile.X >= 0)
                    spriteBatch.Draw(cursorOutline, new Rectangle(mosTile.X * Globals.TileGridSize, mosTile.Y * Globals.TileGridSize, Globals.TileGridSize, Globals.TileGridSize), Color.White);
                Point p = new Point();
                for (int x = 0; x < currentMap.mapSize.X; x++)
                    for (int y = 0; y < currentMap.mapSize.Y; y++)
                    {
                        p.X = x;
                        p.Y = y;
                        if (currentMap.MapTiles[x, y].TrailTile != TrailTile.None)
                            DrawTrail(spriteBatch, font, currentMap.MapTiles[x, y].TrailTile, x, y, Color.Brown);
                        else if (currentMap.MapTiles[x, y].ParkingTile == true)
                            DrawParkingLot(spriteBatch, font, p, Color.Gray);
                        else if (currentMap.MapTiles[x, y].RoadTile == true)
                            DrawRoads(spriteBatch, font, p, Color.DarkGray);
                        else if (currentMap.MapTiles[x,y].BuildingTile != BuildingTile.None)
                            DrawBuilding(spriteBatch, font, p, currentMap);
                        else if (FootPrint.Contains(p))
                        {
                            if (footPrintValid)
                                DrawFootPrint(spriteBatch, font, "v", p, Color.Green);
                            else
                                DrawFootPrint(spriteBatch, font, "v", p, Color.Red);
                        }
                        else
                            DrawForestChar(spriteBatch, font, currentMap.MapTiles[x, y].Data, x, y);
                    }
            }
            if (Edit && !PlaceTrail && !PlaceParking && !PlaceBuilding)
                spriteBatch.DrawString(font, "*Build Mode* - (F1) Build Trail - (F2) Build Parking - (F3) Build Buildings - (Esc) Exit Build Mode", new Vector2(Globals.screenWidth / 4, 15), Color.White);
            else if (PlaceParking && !PlaceTrail && !PlaceBuilding)
                spriteBatch.DrawString(font, "*Build Parking Mode* - Left Click to Place Parking Spot - (Esc) To go back", new Vector2(Globals.screenWidth / 4, 15), Color.White);
            else if (PlaceBuilding && !PlaceTrail && !PlaceParking)
                spriteBatch.DrawString(font, "*Build Buildings Mode* - (V) Visitor Center - (Esc) To go back", new Vector2(Globals.screenWidth / 4, 15), Color.White);
            else if (PlaceVisitorCenter)
                spriteBatch.DrawString(font, "*Build Buildings Mode* - Left Click to Place Visitor Center - (Esc) To go back", new Vector2(Globals.screenWidth / 4, 15), Color.White);
            else if (PlaceTrail && !PlaceTrailHead && !PlaceTrailPath && !PlaceTrailEnd)
                spriteBatch.DrawString(font, "*Build Trail Mode* - (H) Place Trail Head - (T) Place Trail Path - (E) Place Trail End - (Esc) To go back", new Vector2(Globals.screenWidth / 4, 15), Color.White);
            else if (PlaceTrailHead)
                spriteBatch.DrawString(font, "*Build Trail Mode* - Left Click to Place Trail Head - (Esc) To go back", new Vector2(Globals.screenWidth / 4, 15), Color.White);
            else if (PlaceTrailPath)
                spriteBatch.DrawString(font, "*Build Trail Mode* - Left Click to Place Trail Path - (Esc) To go back", new Vector2(Globals.screenWidth / 4, 15), Color.White);
            else if (PlaceTrailEnd)
                spriteBatch.DrawString(font, "*Build Trail Mode* - Left Click to Place Trail End - (Esc) To go back", new Vector2(Globals.screenWidth / 4, 15), Color.White);
        }

        public static void Update(Map map)
        {
            // Enter edit mode
            if (InputManager.Instance.KeyPressed(BuildMode))
                Edit = !Edit;
            if (InputManager.Instance.KeyPressed(Keys.Escape))
            {
                if (PlaceTrail && !PlaceTrailHead && !PlaceTrailPath && !PlaceTrailEnd)
                    PlaceTrail = false;
                else if (PlaceParking)
                    PlaceParking = false;
                else if (PlaceBuilding && !PlaceVisitorCenter)
                    PlaceBuilding = false;
                else if (!PlaceTrail && !PlaceParking && !PlaceBuilding)
                    Edit = false;

                if (!Edit)
                    ScanMap(map);
            }
            if (Edit)
            {
                Point mosTile = GetMouseTilePos();

                // Set flag to place or remove trail tile
                if (InputManager.Instance.KeyPressed(TrailMode))
                {
                    PlaceTrail = !PlaceTrail;
                    PlaceParking = false;
                    PlaceBuilding = false;
                }
                else if (InputManager.Instance.KeyPressed(ParkingMode))
                {
                    PlaceParking = !PlaceParking;
                    PlaceTrail = false;
                    PlaceBuilding = false;
                }
                else if (InputManager.Instance.KeyPressed(PlaceBuildings))
                {
                    PlaceBuilding = !PlaceBuilding;
                    PlaceParking = false;
                    PlaceTrail = false;

                }

                // Building Mode
                if (PlaceTrail)
                {
                    if (InputManager.Instance.KeyPressed(ScanBuild_Key))
                        ScanTrails(map);
                    BuildTrail(map, mosTile);
                }
                else if (PlaceParking)
                {
                    if (InputManager.Instance.KeyPressed(ScanBuild_Key))
                        ScanParking(map);
                    BuildParking(map, mosTile);
                }
                else if (PlaceBuilding)
                {
                    BuildBuildings(map, mosTile);
                }
            }
        }

        private static void BuildBuildings(Map map, Point mosTile)
        {
            // Within bounds of the screen
            if (mosTile.X >= 0)
            {
                int width = 0;
                int height = 0;
                if (InputManager.Instance.KeyPressed(PlaceVisitorCenter_Key))
                {
                    PlaceVisitorCenter = !PlaceVisitorCenter;
                }

                if (InputManager.Instance.KeyPressed(Keys.Escape))
                {
                    PlaceVisitorCenter = false;
                    width = 0;
                    height = 0;
                }

                // Grab a List of points for the footprint of the building wanting to be placed
                FootPrint.Clear();
                if (PlaceVisitorCenter)
                {
                    width = 5;
                    height = 3;
                }

                // Grab footprint
                if (width > 0 && height > 0)
                {
                    int w = (int)Math.Round((double)width / 2);
                    Point p;
                    for (int y = mosTile.Y; y > mosTile.Y - height; y--)
                    {
                        // -->
                        for (int x = mosTile.X; x <= mosTile.X + w; x++)
                        {
                            p = new Point(x, y);
                            if (!FootPrint.Contains(p) && x >= 0 && y >= 0)
                                FootPrint.Add(p);
                        }
                        // <--
                        for (int x = mosTile.X; x >= mosTile.X - w; x--)
                        {
                            p = new Point(x, y);
                            if (!FootPrint.Contains(p) && x >= 0 && y >= 0)
                                FootPrint.Add(p);
                        }
                    }

                    // Invalid placement so clear footprint
                    if (FootPrint.Count == (width * height))
                        footPrintValid = true;
                    else
                        footPrintValid = false;
                }



                //Left click to place building
                if (InputManager.Instance.LeftClick())
                {
                    if (footPrintValid)
                    {
                        if (PlaceVisitorCenter)
                        {
                            foreach (Point p in FootPrint)
                                map.MapTiles[p.X, p.Y].BuildingTile = BuildingTile.VisitorCenter;
                            List<Point> newBuilding = new List<Point>();
                            newBuilding.AddRange(FootPrint);
                            map.AddBuilding(new VisitorCenter(newBuilding));
                        }    
                    }
                }

                if (InputManager.Instance.RightClick())
                {
                    if (PlaceVisitorCenter)
                    {
                        List<Building> buildings = map.Buildings;
                        foreach (Building b in buildings)
                        {
                            if (b.footPrint.Contains(mosTile))
                            {
                                map.RemoveBuilding(b);
                                break;
                            }

                        }
                    }
                }
            }
        }

        private static void ScanMap(Map map)
        {
            List<Trail> trails = map.Trails;
            List<Parking> parking = map.ParkingLots;
            Point p;
            for (int x = 0; x < map.mapSize.X; x++)
                for (int y = 0; y < map.mapSize.Y; y++)
                {
                    p.X = x;
                    p.Y = y;
                    // check for non saved trail tiles
                    if (map.MapTiles[p.X, p.Y].TrailTile != TrailTile.None)
                    {
                        bool flag = false;
                        foreach (Trail t in trails)
                            if (t.trail.Contains(p))
                                flag = true;
                        if (!flag)
                            map.MapTiles[p.X, p.Y].TrailTile = TrailTile.None;
                    }

                    // check for non saved parking lot tiles
                    if (map.MapTiles[p.X, p.Y].ParkingTile == true)
                    {
                        bool flag = false;
                        foreach (Parking pl in parking)
                            if (pl.ParkingLot.Contains(p))
                                flag = true;
                        if (!flag)
                            map.MapTiles[p.X, p.Y].ParkingTile = false;
                    }
                }
        }

        /// <summary>
        /// Designate a parking lot area
        /// </summary>
        /// <param name="map"></param>
        /// <param name="mosTile"></param>
        private static void BuildParking(Map map, Point mosTile)
        {
            if (mosTile.X >= 0)
            {
                if (InputManager.Instance.LeftClick())
                {
                    map.MapTiles[mosTile.X, mosTile.Y].TrailTile = TrailTile.None;
                    map.MapTiles[mosTile.X, mosTile.Y].ParkingTile = true;

                }
                //Right click to remove parking tile
                else if (InputManager.Instance.RightClick())
                {
                    map.MapTiles[mosTile.X, mosTile.Y].TrailTile = TrailTile.None;
                    map.MapTiles[mosTile.X, mosTile.Y].ParkingTile = false;
                }
            }
        }

        /// <summary>
        /// Building Trail Mode.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="mosTile"></param>
        private static void BuildTrail(Map map, Point mosTile)
        {
            // Within bounds of the screen
            if (mosTile.X >= 0)
            {
                if (InputManager.Instance.KeyPressed(TrailHead_Key))
                {
                    PlaceTrailHead = !PlaceTrailHead;
                    PlaceTrailPath = false;
                }
                if (InputManager.Instance.KeyPressed(TrailPath))
                {
                    PlaceTrailHead = false;
                    PlaceTrailPath = !PlaceTrailHead;
                }
                if (InputManager.Instance.KeyPressed(TrailEnd_Key))
                {
                    PlaceTrailHead = false;
                    PlaceTrailPath = false;
                    PlaceTrailEnd = !PlaceTrailEnd;
                }

                if (InputManager.Instance.KeyPressed(Keys.Escape))
                {
                    PlaceTrailHead = false;
                    PlaceTrailPath = false;
                    PlaceTrailEnd = false;
                }

                //Left click to place trail tile
                if (InputManager.Instance.LeftClick())
                {
                    if (PlaceTrailHead)
                    {
                        // Cant place trail heads adjacent to eachother
                        if (IsAdjacentTileTrailHead(map, mosTile).Count == 0)
                        {
                            map.MapTiles[mosTile.X, mosTile.Y].TrailTile = TrailTile.TrailHead;
                        }
                    }
                    else if (PlaceTrailPath)
                    {
                        // check if a trail path or trail head is adjacent to it
                        if (CheckAdjacentTrailTile(map, mosTile).Count > 0)
                        {
                            // If an adjacent tile is a trail head
                            if (IsAdjacentTileTrailHead(map, mosTile).Count > 0)
                            {
                                bool valid = true;
                                foreach (Point p in IsAdjacentTileTrailHead(map, mosTile))
                                {
                                    if (CheckAdjacentTrailTile(map, p).Count > 0)
                                        valid = false;
                                }
                                if (valid)
                                    map.MapTiles[mosTile.X, mosTile.Y].TrailTile = TrailTile.TrailPath;
                            }
                            else
                                map.MapTiles[mosTile.X, mosTile.Y].TrailTile = TrailTile.TrailPath;
                        }
                    }
                    else if (PlaceTrailEnd)
                    {
                        // check if a trail path or trail head is adjacent to it
                        if (CheckAdjacentTrailTile(map, mosTile).Count > 0)
                            map.MapTiles[mosTile.X, mosTile.Y].TrailTile = TrailTile.TrailEnd;
                    }
                }
                //Right click to remove trail tile
                else if (InputManager.Instance.RightClick())
                {
                    map.MapTiles[mosTile.X, mosTile.Y].TrailTile = TrailTile.None;
                }
            }
        }

        /// <summary>
        /// Runs a scan for all possible trails made.
        /// </summary>
        /// <param name="map"></param>
        private static void ScanTrails(Map map)
        {
            List<Point> totalTrailTiles = new List<Point>();
            MapTile[,] mapTiles = map.MapTiles;
            for (int x = 0; x < map.mapSize.X; x++)
                for (int y = 0; y < map.mapSize.Y; y++)
                {
                    if (mapTiles[x, y].TrailTile == TrailTile.TrailHead)
                    {
                        // Option 1
                        Trail trail = CreateTrail(map, new Point(x, y));
                        if (trail.trailType != TrailType.Invalid)
                            totalTrailTiles.AddRange(trail.trail);
                        map.AddTrail(trail);
                    }
                }
            foreach (Trail trail in map.Trails)
            {
                if (trail.trailType == TrailType.Invalid)
                    foreach (Point p in trail.trail)
                    {
                        map.MapTiles[p.X, p.Y].TrailTile = TrailTile.None;
                    }
            }
            // Remove all invalid trails from the map trail list
            for (int i = 0; i < map.Trails.Count; i++)
            {
                if (map.Trails[i].trailType == TrailType.Invalid)
                {
                    map.Trails.Remove(map.Trails[i]);
                    i--;
                }

            }
        }
        private static Trail CreateTrail(Map map, Point startCord)
        {
            // New Trail list
            List<Point> newTrail = new List<Point>();
            // Add trail head to list
            newTrail.Add(startCord);
            // Check for adjacent trail paths only, passing a out of bound prev tile to ignore.
            // Set the next tile in the check
            Point next = CheckForTrailPath(map, startCord, new Point(-1, -1));
            // Set the previous tile to start
            Point prev = startCord;
            // If the trail contains at least one path and one head, go forward
            if (next.X > 0 && next.Y > 0)
            {
                // loop if the next tile has discovered a trail path or trail end
                while (next.X > 0 && next.Y > 0)
                {
                    newTrail.Add(next);
                    // The center tile we are on
                    Point center = next;
                    // Move the next tile up the path if a trail path or trail end exists
                    next = CheckForTrailPath(map, center, prev, true);
                    // Set the previous trail tile to center
                    prev = center;
                    // Double check we didnt rescan the prev tile
                    if (next.X > 0 && next.Y > 0 && next != prev)
                    {
                        // Loop trail
                        // If we scanned a next tile that was not the previous tile and is already in the trail list, 
                        // then we have looped back onto ourself, therefor this trail is a loop
                        if (newTrail.Contains(next))
                        {
                            //newTrail.Add(next);
                            return new Trail(newTrail, TrailType.Loop);
                        }
                        // Point to point trail
                        // If we scanned a tile that is flagged as a trail end tile, then we have mapped a point to point trail.
                        else if (map.MapTiles[next.X, next.Y].TrailTile == TrailTile.TrailEnd)
                        {
                            newTrail.Add(next);
                            return new Trail(newTrail, TrailType.Point2Point);
                        }
                        //newTrail.Add(next);
                    }
                }
                // We have not returned a loop or point to point, therefore this is a out and back trail
                return new Trail(newTrail, TrailType.OutandBack);
            }
            else
            {
                // No valid trail was scanned i.e. only one head trail was found with no adjacent trail path connecting it
                return new Trail(newTrail, TrailType.Invalid);
            }

        }
        private static void ScanParking(Map map)
        {
            List<Point> totalParkingTiles = new List<Point>();
            MapTile[,] mapTiles = map.MapTiles;
            for (int x = 0; x < map.mapSize.X; x++)
                for (int y = 0; y < map.mapSize.Y; y++)
                {
                    if (mapTiles[x, y].ParkingTile == true && !totalParkingTiles.Contains(new Point(x, y)))
                    {
                        Parking parkingLot = CreateParkingLot(map, new Point(x, y));
                        totalParkingTiles.AddRange(parkingLot.ParkingLot);
                        map.AddParkingLot(parkingLot);
                    }
                }
        }
        private static Parking CreateParkingLot(Map map, Point startCord)
        {
            List<Point> pTiles = GetAdjacentParkingTiles(map, startCord);
            Queue<Point> queue = new Queue<Point>(pTiles);
            while (queue.Count > 0)
            {
                Point next = queue.Dequeue();
                List<Point> temp = GetAdjacentParkingTiles(map, next);
                if (temp.Count > 0)
                {
                    foreach (Point p in temp)
                    {
                        if (!pTiles.Contains(p))
                        {
                            queue.Enqueue(p);
                            pTiles.Add(p);
                        }
                    }

                }
            }
            //pTiles.Add(startCord);
            return new Parking(pTiles);
        }
        private static List<Point> GetAdjacentParkingTiles(Map map, Point center)
        {
            List<Point> temp = new List<Point>();
            Point east = new Point(center.X + 1, center.Y);
            Point south = new Point(center.X, center.Y + 1);
            Point west = new Point(center.X - 1, center.Y);
            Point north = new Point(center.X, center.Y - 1);
            if (map.MapTiles[east.X, east.Y].ParkingTile == true)
                temp.Add(east);
            if (map.MapTiles[south.X, south.Y].ParkingTile == true)
                temp.Add(south);
            if (map.MapTiles[west.X, west.Y].ParkingTile == true)
                temp.Add(west);
            if (map.MapTiles[north.X, north.Y].ParkingTile == true)
                temp.Add(north);
            return temp;
        }
        private static List<Point> IsAdjacentTileTrailHead(Map map, Point center)
        {
            List<Point> temp = new List<Point>();
            Point east = new Point(center.X + 1, center.Y);
            Point south = new Point(center.X, center.Y + 1);
            Point west = new Point(center.X - 1, center.Y);
            Point north = new Point(center.X, center.Y - 1);
            if (map.MapTiles[east.X, east.Y].TrailTile == TrailTile.TrailHead)
                temp.Add(east);
            if (map.MapTiles[south.X, south.Y].TrailTile == TrailTile.TrailHead)
                temp.Add(south);
            if (map.MapTiles[west.X, west.Y].TrailTile == TrailTile.TrailHead)
                temp.Add(west);
            if (map.MapTiles[north.X, north.Y].TrailTile == TrailTile.TrailHead)
                temp.Add(north);
            return temp;
        }
        private static Point CheckForTrailPath(Map map, Point center, Point prev, bool CheckEndTileAndPath = false)
        {
            Point east = new Point(center.X + 1, center.Y);
            Point south = new Point(center.X, center.Y + 1);
            Point west = new Point(center.X - 1, center.Y);
            Point north = new Point(center.X, center.Y - 1);
            if (CheckEndTileAndPath)
            {
                if (prev != east && map.MapTiles[east.X, east.Y].TrailTile == TrailTile.TrailPath ||
                    map.MapTiles[east.X, east.Y].TrailTile == TrailTile.TrailEnd)
                    return east;
                else if (prev != south && map.MapTiles[south.X, south.Y].TrailTile == TrailTile.TrailPath ||
                    map.MapTiles[south.X, south.Y].TrailTile == TrailTile.TrailEnd)
                    return south;
                else if (prev != west && map.MapTiles[west.X, west.Y].TrailTile == TrailTile.TrailPath ||
                    map.MapTiles[west.X, west.Y].TrailTile == TrailTile.TrailEnd)
                    return west;
                else if (prev != north && map.MapTiles[north.X, north.Y].TrailTile == TrailTile.TrailPath ||
                    map.MapTiles[north.X, north.Y].TrailTile == TrailTile.TrailEnd)
                    return north;
            }
            else
            {
                if (prev != east && map.MapTiles[east.X, east.Y].TrailTile == TrailTile.TrailPath)
                    return east;
                else if (prev != south && map.MapTiles[south.X, south.Y].TrailTile == TrailTile.TrailPath)
                    return south;
                else if (prev != west && map.MapTiles[west.X, west.Y].TrailTile == TrailTile.TrailPath)
                    return west;
                else if (prev != north && map.MapTiles[north.X, north.Y].TrailTile == TrailTile.TrailPath)
                    return north;
            }
            return new Point(-1, -1);
        }

        /// <summary>
        /// Check if adjacent tiles include a trail tile
        /// </summary>
        /// <param name="map"></param>
        /// <param name="center"></param>
        /// <returns></returns>
        private static List<Point> CheckAdjacentTrailTile(Map map, Point center)
        {
            List<Point> tiles = new List<Point>();
            if (map.MapTiles[center.X + 1, center.Y].TrailTile != TrailTile.None)
                tiles.Add(new Point(center.X + 1, center.Y));
            if (map.MapTiles[center.X - 1, center.Y].TrailTile != TrailTile.None)
                tiles.Add(new Point(center.X - 1, center.Y));
            if (map.MapTiles[center.X, center.Y + 1].TrailTile != TrailTile.None)
                tiles.Add(new Point(center.X, center.Y + 1));
            if (map.MapTiles[center.X, center.Y - 1].TrailTile != TrailTile.None)
                tiles.Add(new Point(center.X, center.Y - 1));
            return tiles;
        }

        private static Point GetMouseTilePos()
        {
            // Grab mouse tile position
            Vector2 mosPos = InputManager.Instance.GetMousePos();
            if (mosPos.X > 0 && mosPos.X < Globals.screenWidth && mosPos.Y > 0 && mosPos.Y < Globals.screenHeight)
                return new Point((int)mosPos.X / Globals.TileGridSize, (int)mosPos.Y / Globals.TileGridSize);
            else
                return new Point(-1, 0);
        }

        public static void DrawTrail(SpriteBatch spriteBatch, SpriteFont font, TrailTile type, int x, int y, Color color)
        {
            if (type == TrailTile.TrailPath)
                spriteBatch.DrawString(font, "t", new Vector2(x * Globals.TileGridSize, y * Globals.TileGridSize), color);
            else if (type == TrailTile.TrailHead)
                spriteBatch.DrawString(font, "h", new Vector2(x * Globals.TileGridSize, y * Globals.TileGridSize), color);
            else if (type == TrailTile.TrailEnd)
                spriteBatch.DrawString(font, "e", new Vector2(x * Globals.TileGridSize, y * Globals.TileGridSize), color);
        }

        public static void DrawParkingLot(SpriteBatch spriteBatch, SpriteFont font, Point point, Color color)
        {
            spriteBatch.DrawString(font, "p", new Vector2(point.X * Globals.TileGridSize, point.Y * Globals.TileGridSize), color);
        }

        public static void DrawRoads(SpriteBatch spriteBatch, SpriteFont font, Point point, Color color)
        {
            spriteBatch.DrawString(font, "r", new Vector2(point.X * Globals.TileGridSize, point.Y * Globals.TileGridSize), color);
        }

        public static void DrawForestChar(SpriteBatch spriteBatch, SpriteFont font, double point, int x, int y)
        {
            if (point >= .9)
                spriteBatch.DrawString(font, "-", new Vector2(x * Globals.TileGridSize, y * Globals.TileGridSize), Color.DarkGreen); // Forest hill 2
            else if (point >= .8)
                spriteBatch.DrawString(font, "^", new Vector2(x * Globals.TileGridSize, y * Globals.TileGridSize), Color.DarkGray); // elevation gain
            else if (point >= .7)
                spriteBatch.DrawString(font, "~", new Vector2(x * Globals.TileGridSize, y * Globals.TileGridSize), Color.ForestGreen); // Forest Hill
            else if (point >= .6)
                spriteBatch.DrawString(font, "^", new Vector2(x * Globals.TileGridSize, y * Globals.TileGridSize), Color.Gray); // elevation gain
            else
                spriteBatch.DrawString(font, "~", new Vector2(x * Globals.TileGridSize, y * Globals.TileGridSize), Color.Green); // Forest floor
        }

        public static void DrawFootPrint(SpriteBatch spriteBatch, SpriteFont font, string text, Point point, Color color)
        {
            spriteBatch.DrawString(font, text, new Vector2(point.X * Globals.TileGridSize, point.Y * Globals.TileGridSize), color);
        }

        public static void DrawBuilding(SpriteBatch spriteBatch, SpriteFont font, Point point, Map map)
        {
            BuildingTile type = map.MapTiles[point.X, point.Y].BuildingTile;
            if (type == BuildingTile.VisitorCenter)
                spriteBatch.DrawString(font, "v", new Vector2(point.X * Globals.TileGridSize, point.Y * Globals.TileGridSize), Color.Brown);
        }
    }
}
