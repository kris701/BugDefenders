using BugDefender.Core.Resources;
using BugDefender.Core.Resources.Integrity;
using BugDefender.Core.Users;
using BugDefender.OpenGL.BackgroundWorkers.FPSBackgroundWorker;
using BugDefender.OpenGL.BackgroundWorkers.NotificationBackroundWorker;
using BugDefender.OpenGL.BackgroundWorkers.NotificationBackroundWorker.Handles;
using BugDefender.OpenGL.Engine;
using BugDefender.OpenGL.Engine.Audio;
using BugDefender.OpenGL.Engine.BackgroundWorkers;
using BugDefender.OpenGL.Engine.Helpers;
using BugDefender.OpenGL.Engine.Textures;
using BugDefender.OpenGL.Engine.Views;
using BugDefender.OpenGL.ResourcePacks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace BugDefender.OpenGL
{
    public class BugDefenderGameWindow : Game, IWindow
    {
        private static readonly string _contentDir = "Content";
        private static readonly string _modsDir = "Mods";

        public float XScale { get; private set; }
        public float YScale { get; private set; }

        public GraphicsDeviceManager Device { get; }
        public IView CurrentScreen { get; set; }
        public UserEngine<SettingsDefinition> UserManager { get; private set; }
        public List<IBackgroundWorker> BackroundWorkers { get; set; } = new List<IBackgroundWorker>();

        public AudioController AudioController { get; private set; }
        public TextureController TextureController { get; private set; }
        public ResourcePackController ResourcePackController { get; private set; }

        private readonly Func<BugDefenderGameWindow, IView> _screenToLoad;
        private SpriteBatch? _spriteBatch;
        private readonly NotificationBackroundWorker _notificationWorker;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public BugDefenderGameWindow(Func<BugDefenderGameWindow, IView> screen) : base()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
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
            BasicTextures.Initialize(GraphicsDevice);
            BasicFonts.Initialize(Content, "DefaultFonts/DefaultFont");
            MediaPlayer.IsRepeating = true;
            SoundEffect.Initialize();
            _notificationWorker.Handles.Add(new AchivementsHandle(_notificationWorker));
            _notificationWorker.Handles.Add(new BuffsHandle(_notificationWorker));
            //_notificationWorker.Handles.Add(new GameUpdateHandle(_notificationWorker));
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
            foreach (var folder in new DirectoryInfo(_modsDir).GetDirectories())
            {
                // Load core resources
                ResourceManager.LoadResource(folder);

                // Load textures
                foreach (var subFolder in folder.GetDirectories())
                {
                    if (subFolder.Parent == null)
                        continue;
                    if (subFolder.Name.ToUpper() == "TEXTURES")
                    {
                        foreach (var file in subFolder.GetFiles())
                        {
                            var textureDef = JsonSerializer.Deserialize<List<TextureDefinition>>(File.ReadAllText(file.FullName));
                            if (textureDef != null)
                            {
                                foreach (var texture in textureDef)
                                {
                                    texture.Content = Path.Combine(subFolder.Parent.FullName, "Content", texture.Content);
                                    TextureController.LoadTexture(texture);
                                }
                            }
                        }
                    }
                    else if (subFolder.Name.ToUpper() == "TEXTURESETS")
                    {
                        foreach (var file in subFolder.GetFiles())
                        {
                            var textureSetDef = JsonSerializer.Deserialize<List<TextureSetDefinition>>(File.ReadAllText(file.FullName));
                            if (textureSetDef != null)
                            {
                                foreach (var textureSet in textureSetDef)
                                {
                                    for (int i = 0; i < textureSet.Contents.Count; i++)
                                        textureSet.Contents[i] = Path.Combine(subFolder.Parent.FullName, "Content", textureSet.Contents[i]);
                                    foreach (var content in textureSet.Contents)
                                        TextureController.LoadTextureSet(textureSet);
                                }
                            }
                        }
                    }
                    else if (subFolder.Name.ToUpper() == "SONGS")
                    {
                        foreach (var file in subFolder.GetFiles())
                        {
                            var songsDef = JsonSerializer.Deserialize<List<SongDefinition>>(File.ReadAllText(file.FullName));
                            if (songsDef != null)
                            {
                                foreach (var song in songsDef)
                                {
                                    song.Content = Path.Combine(subFolder.Parent.FullName, "Content", song.Content);
                                    AudioController.LoadSong(song);
                                }
                            }
                        }
                    }
                    else if (subFolder.Name.ToUpper() == "SOUNDEFFECTS")
                    {
                        foreach (var file in subFolder.GetFiles())
                        {
                            var soundEffectsDef = JsonSerializer.Deserialize<List<SoundEffectDefinition>>(File.ReadAllText(file.FullName));
                            if (soundEffectsDef != null)
                            {
                                foreach (var soundEffect in soundEffectsDef)
                                {
                                    soundEffect.Content = Path.Combine(subFolder.Parent.FullName, "Content", soundEffect.Content);
                                    AudioController.LoadSoundEffect(soundEffect);
                                }
                            }
                        }
                    }
                }
            }

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

            var matrix = Matrix.CreateScale(XScale, YScale, 1.0f);
            _spriteBatch!.Begin(transformMatrix: matrix);
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
        }
    }
}