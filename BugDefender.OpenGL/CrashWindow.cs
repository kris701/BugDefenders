using MonoGame.OpenGL.Formatter.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Text;
using MonoGame.OpenGL.Formatter.Fonts;
using BugDefender.OpenGL.Helpers;

namespace BugDefender.OpenGL
{
    public class CrashWindow : Game
    {
        private static readonly string _logDir = "Crashes";
        private static readonly string _contentDir = "Content";
        public GraphicsDeviceManager Device { get; }
        public FontController Fonts { get; private set; }

        private SpriteBatch? _spriteBatch;
        private readonly Exception _error;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public CrashWindow(Exception error)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            _error = error;
            Content.RootDirectory = _contentDir;
            Device = new GraphicsDeviceManager(this);
            Device.PreferredBackBufferHeight = 150;
            Device.PreferredBackBufferWidth = 500;
            Device.ApplyChanges();
            IsMouseVisible = true;

            if (!Directory.Exists(_logDir))
                Directory.CreateDirectory(_logDir);

            var crashLog = Path.Combine(_logDir, $"crash_{DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss")}.txt");
            if (File.Exists(crashLog))
                File.Delete(crashLog);
            var sb = new StringBuilder();
            sb.AppendLine($"Main Exception: {error.Message}");
            sb.AppendLine();
            sb.AppendLine($"Stack Trace: {error.StackTrace}");
            sb.AppendLine();
            File.WriteAllText(crashLog, sb.ToString());
        }

        protected override void Initialize()
        {
            base.Initialize();
            Window.Title = $"Game Crashed!";
            Fonts = new FontController(Content, "DefaultFonts/DefaultFont");
            Fonts.LoadFont(new FontDefinition(new Guid("42f15b37-1da4-4b95-8c8e-2056743f172b"), "DefaultFonts/DefaultFont10", false));
            Fonts.LoadFont(new FontDefinition(new Guid("29fd3e67-78b3-4623-84be-0f6a7dc27d6d"), "DefaultFonts/DefaultFont12", false));
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);

            if (_spriteBatch == null)
                throw new Exception("Error! Spritebatch was not initialized!");

            _spriteBatch.Begin();
            _spriteBatch.DrawString(Fonts.GetFont(FontSizes.Ptx12), "Game Have Crashed!", new Vector2(10, 10), Color.White);
            _spriteBatch.DrawString(Fonts.GetFont(FontSizes.Ptx10), "The error was:", new Vector2(10, 40), Color.White);
            _spriteBatch.DrawString(Fonts.GetFont(FontSizes.Ptx10), $"{_error.Message}", new Vector2(10, 65), Color.Red);
            _spriteBatch.DrawString(Fonts.GetFont(FontSizes.Ptx10), $"A log have been put in the '{_logDir}' directory", new Vector2(10, 90), Color.White);
            _spriteBatch.DrawString(Fonts.GetFont(FontSizes.Ptx10), "Consider sending the report to the developer", new Vector2(10, 115), Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
