using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using TDGame.OpenGL.Engine;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Engine.Screens;
using TDGame.OpenGL.Settings;
using TDGame.OpenGL.Textures;

namespace TDGame.OpenGL
{
    public class GameEngine : Game
    {
        private static string _contentDir = "Content";
        private static string _settingsFile = "settings.json";

        public SettingsDefinition Settings { get; set; }
        public GraphicsDeviceManager Device { get; }
        public int ScreenWidth() => Window.ClientBounds.Width;
        public int ScreenHeight() => Window.ClientBounds.Height;

        private Func<GameEngine, IScreen> _screenToLoad;
        private IScreen _currentScreen;
        private SpriteBatch? _spriteBatch;

        public GameEngine(Func<GameEngine, IScreen> screen)
        {
            Device = new GraphicsDeviceManager(this);
            Content.RootDirectory = _contentDir;
            _screenToLoad = screen;
            IsMouseVisible = true;
            Window.Title = "TDGame";

            if (File.Exists(_settingsFile))
            {
                var settings = JsonSerializer.Deserialize<SettingsDefinition>(File.ReadAllText(_settingsFile));
                if (settings != null)
                {
                    Settings = settings;
                }
            }
            else
                Settings = new SettingsDefinition();
        }

        protected override void Initialize()
        {
            base.Initialize();

            ApplySettings();
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

        public void ApplySettings()
        {
            Device.PreferredBackBufferHeight = (int)(Settings.Scale * 1000);
            Device.PreferredBackBufferWidth = (int)(Settings.Scale * 1000);
            Device.SynchronizeWithVerticalRetrace = Settings.IsVsync;
            Device.IsFullScreen = Settings.IsFullscreen;
            Device.ApplyChanges();
        }

        public void SaveSettings() 
        {
            if (File.Exists(_settingsFile))
                File.Delete(_settingsFile);
            File.WriteAllText(_settingsFile, JsonSerializer.Serialize(Settings));
        }

        public void SwitchView(IScreen screen)
        {
            _currentScreen = screen;
        }

        public Texture2D TakeScreenCap()
        {
            int w = GraphicsDevice.PresentationParameters.BackBufferWidth;
            int h = GraphicsDevice.PresentationParameters.BackBufferHeight;

            //force a frame to be drawn (otherwise back buffer is empty) 
            Draw(new GameTime());

            //pull the picture from the buffer 
            int[] backBuffer = new int[w * h];
            GraphicsDevice.GetBackBufferData(backBuffer);

            //copy into a texture 
            Texture2D texture = new Texture2D(GraphicsDevice, w, h, false, GraphicsDevice.PresentationParameters.BackBufferFormat);
            texture.SetData(backBuffer);
            return texture;
        }
    }
}