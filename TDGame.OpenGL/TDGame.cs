using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TDGame.OpenGL.Engine;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Engine.Screens;

namespace TDGame.OpenGL
{
    public class TDGame : Game, IEngine
    {
        public GraphicsDeviceManager Device { get; }
        public IScreen? CurrentScreen { get; internal set; }

        private SpriteBatch? _spriteBatch;
        private Func<TDGame, IScreen> _initialScreen;

        public TDGame(Func<TDGame, IScreen> initialScreen)
        {
            Device = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _initialScreen = initialScreen;
        }

        protected override void Initialize()
        {
            base.Initialize();

            BasicTextures.Initialize(GraphicsDevice);
            BasicFonts.Initialize(Content);

            CurrentScreen = _initialScreen.Invoke(this);
            if (CurrentScreen != null)
                CurrentScreen.Parent = this;
            SwitchView(CurrentScreen);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            if (CurrentScreen != null)
            {
                CurrentScreen.LoadContent(Content);
                CurrentScreen.Refresh();
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (CurrentScreen != null)
                CurrentScreen.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            if (_spriteBatch == null)
                throw new Exception("Error! Spritebatch was not initialized!");

            _spriteBatch.Begin();
            if (CurrentScreen != null)
                CurrentScreen.Draw(gameTime, _spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public void SwitchView(IScreen? screen)
        {
            CurrentScreen = screen;
            if (CurrentScreen != null)
            {
                CurrentScreen.LoadContent(Content);
                CurrentScreen.Refresh();
            }
        }

        public int ScreenWidth() => Window.ClientBounds.Width;
        public int ScreenHeight() => Window.ClientBounds.Height;
    }
}