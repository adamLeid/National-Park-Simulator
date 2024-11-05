using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using National_Park_Simulator.World;
using System.Diagnostics;
using National_Park_Simulator.Engine;
using National_Park_Simulator.Engine.Managers;
using National_Park_Simulator.Engine.MapGen;

namespace National_Park_Simulator
{
    public class Main : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;


        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            IsFixedTimeStep = false; //FPS locked
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Window.AllowUserResizing = true;
            _graphics.PreferredBackBufferWidth = 1600;
            _graphics.PreferredBackBufferHeight = 1600;
            Globals.screenWidth = 1600;
            Globals.screenHeight = 1600;
            Globals.GraphicsDevice = GraphicsDevice;
            _graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            MapManager.Instance.LoadContent(this.Content);
            MapManager.Instance.BuildMap(WorldSize.Small, "forest test", MapType.Forest);
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Tab))
                Exit();
            InputManager.Instance.Update();
            MapManager.Instance.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            MapManager.Instance.Draw(_spriteBatch);
            base.Draw(gameTime);
        }

    }
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Main())
                game.Run();
        }
    }
}

