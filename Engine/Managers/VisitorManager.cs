using National_Park_Simulator.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace National_Park_Simulator.Engine.Managers
{
    public static class VisitorManager
    {
        // Total number of visitors allowed
        private static int maxVisitors;
        // Current number of visitors
        private static int currentVisitors;
        // #Visitors per second
        private static int visitorRate;

        private static double timer;
        public static void Update(Map map, GameTime gameTime)
        {
            timer += gameTime.ElapsedGameTime.TotalSeconds;
            if (timer >= 1d)
            {
                timer = 0d;

                // Add more visitors
                maxVisitors = (int)(map.AvailableParking() + map.Trails.Count * map.ParkRating);
                visitorRate = (int)(map.ParkRating * .1f);
                if (currentVisitors + visitorRate > maxVisitors)
                    visitorRate = maxVisitors - currentVisitors;
                else if (currentVisitors >= maxVisitors)
                    visitorRate = 0;
                map.AddVisitors(GenerateNewVisitors(visitorRate));
                currentVisitors = map.Visitors.Count;
            }
            // Update visitors ai
            foreach (Visitor visitor in map.Visitors)
            {
                visitor.Update(gameTime);
            }
        }


        public static void Draw(Map map, SpriteBatch spriteBatch, SpriteFont font)
        {
            foreach(Visitor v in map.Visitors)
                spriteBatch.DrawString(font, "@", new Vector2(v.Location.X * Globals.TileGridSize, v.Location.Y * Globals.TileGridSize), Color.Purple);
        }

        private static List<Visitor> GenerateNewVisitors(int numVisitors)
        {
            List<Visitor> list = new List<Visitor>();
            for (int i = 0; i < numVisitors; i++)
                list.Add(new Visitor());
            return list;
        }
    }
}
