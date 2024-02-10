using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using TDGame.OpenGL.Engine;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Engine.Screens;
using TDGame.OpenGL.Textures;

namespace TDGame.OpenGL
{
    public class GameEngine : Game
    {
        public GraphicsDeviceManager Device { get; }
        public int ScreenWidth() => Window.ClientBounds.Width;
        public int ScreenHeight() => Window.ClientBounds.Height;
        public float Scale { get; set; } = 1;

        private Func<GameEngine, IScreen> _screenToLoad;
        private IScreen _currentScreen;
        private SpriteBatch? _spriteBatch;

        public GameEngine(Func<GameEngine, IScreen> screen)
        {
            Device = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _screenToLoad = screen;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();

            BasicTextures.Initialize(GraphicsDevice);
            BasicFonts.Initialize(Content);
            TextureBuilder.Initialize(Content);
            TextureBuilder.LoadTexturePack(TextureBuilder.GetTexturePacks()[0]);

            _currentScreen = _screenToLoad(this);
            _currentScreen.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            _currentScreen.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            if (_spriteBatch == null)
                throw new Exception("Error! Spritebatch was not initialized!");

            _spriteBatch.Begin();
            _currentScreen.Draw(gameTime, _spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public void SwitchView(IScreen screen)
        {
            _currentScreen = screen;
        }
    }
}