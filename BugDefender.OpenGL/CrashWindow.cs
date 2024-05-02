using MonoGame.OpenGL.Formatter.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Text;

namespace BugDefender.OpenGL
{
    public class CrashWindow : Game
    {
        private static readonly string _logDir = "Crashes";
        private static readonly string _contentDir = "Content";
        public GraphicsDeviceManager Device { get; }

        private SpriteBatch? _spriteBatch;
        private readonly Exception _error;

        public CrashWindow(Exception error)
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
            BasicFonts.Initialize(Content, "DefaultFonts/DefaultFont");
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
            _spriteBatch.DrawString(BasicFonts.GetFont(12), "Game Have Crashed!", new Vector2(10, 10), Color.White);
            _spriteBatch.DrawString(BasicFonts.GetFont(10), "The error was:", new Vector2(10, 40), Color.White);
            _spriteBatch.DrawString(BasicFonts.GetFont(10), $"{_error.Message}", new Vector2(10, 65), Color.Red);
            _spriteBatch.DrawString(BasicFonts.GetFont(10), $"A log have been put in the '{_logDir}' directory", new Vector2(10, 90), Color.White);
            _spriteBatch.DrawString(BasicFonts.GetFont(10), "Consider sending the report to the developer", new Vector2(10, 115), Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
