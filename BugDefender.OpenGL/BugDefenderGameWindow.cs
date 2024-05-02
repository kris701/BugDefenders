using BugDefender.Core;
using BugDefender.Core.Resources;
using BugDefender.Core.Resources.Integrity;
using BugDefender.Core.Users;
using BugDefender.OpenGL.BackgroundWorkers.FPSBackgroundWorker;
using BugDefender.OpenGL.BackgroundWorkers.NotificationBackroundWorker;
using BugDefender.OpenGL.BackgroundWorkers.NotificationBackroundWorker.Handles;
using MonoGame.OpenGL.Formatter;
using MonoGame.OpenGL.Formatter.Audio;
using MonoGame.OpenGL.Formatter.BackgroundWorkers;
using MonoGame.OpenGL.Formatter.Helpers;
using MonoGame.OpenGL.Formatter.Textures;
using MonoGame.OpenGL.Formatter.Views;
using BugDefender.OpenGL.ResourcePacks;
using BugDefender.OpenGL.Screens.CutsceneView;
using BugDefender.OpenGL.Screens.GameOverScreen;
using BugDefender.OpenGL.Screens.GameScreen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BugDefender.OpenGL
{
    public class BugDefenderGameWindow : Game, IWindow
    {
        private static readonly string _contentDir = "Content";
        private static readonly string _modsDir = "Mods";

        public float XScale { get; private set; } = 1;
        public float YScale { get; private set; } = 1;

        public GraphicsDeviceManager Device { get; }
        public IView CurrentScreen { get; set; }
        public UserEngine<SettingsDefinition> UserManager { get; private set; }
        public List<IBackgroundWorker> BackroundWorkers { get; set; } = new List<IBackgroundWorker>();
        public GameManager<SettingsDefinition> GameManager { get; private set; }

        public AudioController AudioController { get; private set; }
        public TextureController TextureController { get; private set; }
        public ResourcePackController ResourcePackController { get; private set; }

        private readonly Func<BugDefenderGameWindow, IView> _screenToLoad;
        private SpriteBatch? _spriteBatch;
        private readonly NotificationBackroundWorker _notificationWorker;
        private Matrix _scaleMatrix;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public BugDefenderGameWindow(Func<BugDefenderGameWindow, IView> screen) : base()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            _scaleMatrix = Matrix.CreateScale(XScale, YScale, 1.0f);
            Device = new GraphicsDeviceManager(this);
            Content.RootDirectory = _contentDir;
            _screenToLoad = screen;
            IsMouseVisible = true;
            _notificationWorker = new NotificationBackroundWorker(this);
        }

        protected override void Initialize()
        {
            base.Initialize();

            var thisVersion = Assembly.GetEntryAssembly()?.GetName().Version!;
            var thisVersionStr = $"v{thisVersion.Major}.{thisVersion.Minor}.{thisVersion.Build}";
            Window.Title = $"Bug Defender {thisVersionStr}";

            AudioController = new AudioController(Content);
            TextureController = new TextureController(Content);
            ResourcePackController = new ResourcePackController(this);
            UserManager = new UserEngine<SettingsDefinition>();
            GameManager = new GameManager<SettingsDefinition>(UserManager);
            GameManager.OnGameStarted += (g, s) => CurrentScreen.SwitchView(new GameScreen(this, g));
            GameManager.OnCutsceneStarted += (c, p, s) => CurrentScreen.SwitchView(new CutsceneView(this, p, c, s));
            GameManager.OnGameOver += (g, s, t) =>
            {
                var screen = GameScreenHelper.TakeScreenCap(GraphicsDevice, this);
                CurrentScreen.SwitchView(new GameOverView(this, screen, g, t));
            };

            BasicTextures.Initialize(GraphicsDevice);
            BasicFonts.Initialize(Content, "DefaultFonts/DefaultFont");
            MediaPlayer.IsRepeating = true;
            SoundEffect.Initialize();
            _notificationWorker.Handles.Add(new AchivementsHandle(_notificationWorker));
            _notificationWorker.Handles.Add(new BuffsHandle(_notificationWorker));
            BackroundWorkers = new List<IBackgroundWorker>() {
                _notificationWorker,
                new FPSBackgroundWorker(this)
            };
            LoadMods();
            ApplySettings();

            foreach (var worker in BackroundWorkers)
                worker.Initialize();

            CurrentScreen = _screenToLoad(this);
            CurrentScreen.Initialize();
        }

        private void LoadMods()
        {
            if (!Directory.Exists(_modsDir))
                Directory.CreateDirectory(_modsDir);
            ResourcePackController.LoadMods(_modsDir);

            var checker = new ResourceIntegrityChecker();
            checker.CheckGameIntegrity();
            if (checker.Errors.Any(x => x.Severity > IntegrityError.SeverityLevel.Critical))
                _notificationWorker.AddManualNotification("Game Integrity", $"There where errors in the game integrity! Remove your mods or use the CLI tool to find the problem with your mods.");
            if (ResourceManager.LoadedResources.Count > 1)
                _notificationWorker.AddManualNotification("Mods", $"Currently have {ResourceManager.LoadedResources.Count - 1} loaded!");
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            CurrentScreen.Update(gameTime);
            foreach (var worker in BackroundWorkers)
                worker.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch!.Begin(transformMatrix: _scaleMatrix);
            CurrentScreen.Draw(gameTime, _spriteBatch);
            foreach (var worker in BackroundWorkers)
                worker.Draw(gameTime, _spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public void ApplySettings()
        {
            UserManager.SaveUser();

            Device.PreferredBackBufferHeight = UserManager.CurrentUser.UserData.ScreenHeight;
            Device.PreferredBackBufferWidth = UserManager.CurrentUser.UserData.ScreenWidth;
            Device.SynchronizeWithVerticalRetrace = UserManager.CurrentUser.UserData.IsVsync;
            Device.HardwareModeSwitch = false;
            Device.IsFullScreen = UserManager.CurrentUser.UserData.IsFullscreen;
            if (UserManager.CurrentUser.UserData.IsFullscreen)
            {
                Device.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                Device.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            }
            ResourcePackController.LoadResourcePack(UserManager.CurrentUser.UserData.TexturePack);
            MediaPlayer.Volume = UserManager.CurrentUser.UserData.MusicVolume;
            SoundEffect.MasterVolume = UserManager.CurrentUser.UserData.EffectsVolume;
            Device.ApplyChanges();
            XScale = (float)Device.PreferredBackBufferWidth / (float)IWindow.BaseScreenSize.X;
            YScale = (float)Device.PreferredBackBufferHeight / (float)IWindow.BaseScreenSize.Y;
            _scaleMatrix = Matrix.CreateScale(XScale, YScale, 1.0f);
        }
    }
}