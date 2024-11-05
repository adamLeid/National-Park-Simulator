using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using National_Park_Simulator.World;
using System.Diagnostics;
using Microsoft.Xna.Framework.Content;
using National_Park_Simulator.Engine.MapGen;

namespace National_Park_Simulator.Engine.Managers
{
    public class MapManager
    {
        private static MapManager instance;

        public static MapManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MapManager();
                }
                return instance;
            }
        }

        Map currentMap;

        public ContentManager Content { get; private set; }

        SpriteFont font;
        Point mosTile;
        Vector2 mosPos;

        private Point GetMouseTilePos()
        {
            // Grab mouse tile position
            mosPos = InputManager.Instance.GetMousePos();
            if (mosPos.X > 0 && mosPos.X < Globals.screenWidth && mosPos.Y > 0 && mosPos.Y < Globals.screenHeight)
                return new Point((int)mosPos.X / Globals.TileGridSize, (int)mosPos.Y / Globals.TileGridSize);
            else
                return new Point(-1, 0);
        }

        public void LoadContent(ContentManager content)
        {
            Content = content;
            font = Content.Load<SpriteFont>("Default");
        }

        public void BuildMap(Point mapSize, string name, MapType mapType)
        {
            currentMap = MapGenerator.GenerateMap(mapSize, name, mapType);
        }

        public void Update(GameTime gameTime)
        {
            //remove
            mosTile = GetMouseTilePos();
            //Map Builder
            MapEditor.Update(currentMap);
            //Visitor Manager
            VisitorManager.Update(currentMap, gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            if (MapEditor.Edit)
                MapEditor.Draw(spriteBatch, font, currentMap);
            else
                currentMap.Draw(spriteBatch, font);
            VisitorManager.Draw(currentMap, spriteBatch, font);
            spriteBatch.DrawString(font, "" + mosTile, new Vector2(mosPos.X - 25, mosPos.Y - 25), Color.Red);
            spriteBatch.End();
        }

        Color GetColor(double point)
        {
            if (point >= .9)
                return new Color(255, 255, 255); //Mountain top
            else if (point >= .8)
                return new Color(230, 230, 230); // Dark Rock
            else if (point >= .7)
                return new Color(205, 205, 205); // Rock
            else if (point >= .6)
                return new Color(180, 180, 180); // Dark grass
            else if (point >= .5)
                return new Color(155, 155, 155); // Grass
            else if (point >= .4)
                return new Color(130, 130, 130); // Shallow Water
            else if (point >= .3)
                return new Color(105, 105, 105); // Ocean
            else if (point >= .2)
                return new Color(80, 80, 80); // Ocean
            else if (point >= .1)
                return new Color(50, 50, 50); // Ocean
            else
                return new Color(25, 25, 25); // Deap Ocean
        }

    }
}
