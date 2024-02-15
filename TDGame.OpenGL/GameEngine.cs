﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using TDGame.Core.Resources;
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
        private static string _modsDir = "Mods";
        private static string _settingsFile = "settings.json";

#if FPS
        private TimeSpan _passed = TimeSpan.Zero;
        private int _currentFrames = 0;
        private int _frames = 0;
#endif

        public SettingsDefinition Settings { get; set; }
        public GraphicsDeviceManager Device { get; }
        public int ScreenWidth() => Window.ClientBounds.Width;
        public int ScreenHeight() => Window.ClientBounds.Height;
        public IScreen CurrentScreen { get; set; }

        private Func<GameEngine, IScreen> _screenToLoad;
        private SpriteBatch? _spriteBatch;

        public GameEngine(Func<GameEngine, IScreen> screen)
        {
            Device = new GraphicsDeviceManager(this);
            Content.RootDirectory = _contentDir;
            _screenToLoad = screen;
            IsMouseVisible = true;

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

            Window.Title = "TDGame";

            ApplySettings();
            BasicTextures.Initialize(GraphicsDevice);
            BasicFonts.Initialize(Content);
            if (!Directory.Exists(_modsDir))
                Directory.CreateDirectory(_modsDir);
            foreach(var folder in new DirectoryInfo(_modsDir).GetDirectories())
                ResourceManager.LoadResource(folder);

            CurrentScreen = _screenToLoad(this);
            CurrentScreen.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            CurrentScreen.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            if (_spriteBatch == null)
                throw new Exception("Error! Spritebatch was not initialized!");

            _spriteBatch.Begin();
            CurrentScreen.Draw(gameTime, _spriteBatch);
#if FPS
            _currentFrames++;
            _passed += gameTime.ElapsedGameTime;
            if (_passed >= TimeSpan.FromSeconds(1))
            {
                _passed = TimeSpan.Zero;
                _frames = _currentFrames;
                _currentFrames = 0;
            }
            _spriteBatch.DrawString(BasicFonts.GetFont(16), $"FPS: {_frames}", new Vector2(0,0), new Color(255, 0, 0, 255));
#endif
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public void ApplySettings()
        {
            Device.PreferredBackBufferHeight = (int)(Settings.Scale * 1000);
            Device.PreferredBackBufferWidth = (int)(Settings.Scale * 1000);
            Device.SynchronizeWithVerticalRetrace = Settings.IsVsync;
            Device.IsFullScreen = Settings.IsFullscreen;
            TextureBuilder.Initialize(Content);
            TextureBuilder.LoadTexturePack(Settings.TexturePack);
            Device.ApplyChanges();
        }

        public void SaveSettings() 
        {
            if (File.Exists(_settingsFile))
                File.Delete(_settingsFile);
            File.WriteAllText(_settingsFile, JsonSerializer.Serialize(Settings));
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