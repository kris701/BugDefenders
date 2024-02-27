﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TDGame.Core.Resources;
using TDGame.Core.Resources.Integrity;
using TDGame.Core.Users;
using TDGame.Core.Users.Models;
using TDGame.OpenGL.BackgroundWorkers.NotificationBackroundWorker;
using TDGame.OpenGL.BackgroundWorkers.NotificationBackroundWorker.Handles;
using TDGame.OpenGL.Engine.BackgroundWorkers;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Engine.Views;
using TDGame.OpenGL.ResourcePacks;
using TDGame.OpenGL.Settings;

namespace TDGame.OpenGL
{
    public class UIEngine : Game
    {
        private static readonly string _contentDir = "Content";
        private static readonly string _modsDir = "Mods";

        private TimeSpan _passed = TimeSpan.Zero;
        private int _currentFrames = 0;
        private int _frames = 0;

        public GraphicsDeviceManager Device { get; }
        public int ScreenWidth() => Window.ClientBounds.Width;
        public int ScreenHeight() => Window.ClientBounds.Height;
        public IView CurrentScreen { get; set; }
        public UserEngine<SettingsDefinition> UserManager { get; set; }
        public UserDefinition<SettingsDefinition> CurrentUser { get; set; }
        public List<IBackgroundWorker> BackroundWorkers { get; set; }
        public UIResourceManager UIResources { get; set; }

        private readonly Func<UIEngine, IView> _screenToLoad;
        private SpriteBatch? _spriteBatch;
        private bool _isInitialized = false;
        private readonly NotificationBackroundWorker _notificationWorker;

        public UIEngine(Func<UIEngine, IView> screen)
        {
            Device = new GraphicsDeviceManager(this);
            Content.RootDirectory = _contentDir;
            _screenToLoad = screen;
            IsMouseVisible = true;
            UserManager = new UserEngine<SettingsDefinition>();
            var allUsers = UserManager.GetAllUsers();
            if (allUsers.Count == 0)
            {
                var newUser = new UserDefinition<SettingsDefinition>(
                    Guid.NewGuid(),
                    "Default",
                    new List<Guid>(),
                    new List<Guid>(),
                    new List<ScoreDefinition>(),
                    true,
                    new StatsDefinition(),
                    new SettingsDefinition());
                UserManager.AddNewUser(newUser);
                ChangeUser(newUser);
            }
            else
            {
                foreach (var user in allUsers)
                {
                    if (user.IsPrimary)
                    {
                        ChangeUser(user);
                        break;
                    }
                }
                if (CurrentUser == null)
                    ChangeUser(allUsers[0]);
            }

            _notificationWorker = new NotificationBackroundWorker(this);
            _notificationWorker.Handles.Add(new AchivementsHandle(this));
            _notificationWorker.Handles.Add(new BuffsHandle(this));
            BackroundWorkers = new List<IBackgroundWorker>() {
                _notificationWorker
            };
        }

        public void CreateNewUser(string name)
        {
            var newUser = new UserDefinition<SettingsDefinition>(
                Guid.NewGuid(),
                name,
                new List<Guid>(),
                new List<Guid>(),
                new List<ScoreDefinition>(),
                false,
                new StatsDefinition(),
                new SettingsDefinition());
            UserManager.AddNewUser(newUser);
        }

        public void ChangeUser(UserDefinition<SettingsDefinition> toUser)
        {
            if (CurrentUser != null)
            {
                CurrentUser.IsPrimary = false;
                UserManager.SaveUser(CurrentUser);
            }

            CurrentUser = toUser;
            UserManager.ApplyBuffsToResources(toUser);
            toUser.IsPrimary = true;
            UserManager.SaveUser(toUser);
            if (_isInitialized)
                ApplySettings();
        }

        protected override void Initialize()
        {
            base.Initialize();

            Window.Title = "TDGame";

            UIResources = new UIResourceManager(Content);
            BasicTextures.Initialize(GraphicsDevice);
            BasicFonts.Initialize(Content);
            ApplySettings();
            MediaPlayer.IsRepeating = true;
            SoundEffect.Initialize();

            foreach (var worker in BackroundWorkers)
                worker.Initialize();

            CurrentScreen = _screenToLoad(this);
            CurrentScreen.Initialize();

            _isInitialized = true;
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
                                    UIResources.LoadTexture(texture);
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
                                        UIResources.LoadTextureSet(textureSet);
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
                                    UIResources.LoadSong(song);
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
                                    UIResources.LoadSoundEffect(soundEffect);
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

            if (_spriteBatch == null)
                throw new Exception("Error! Spritebatch was not initialized!");

            _spriteBatch.Begin();
            CurrentScreen.Draw(gameTime, _spriteBatch);
            foreach (var worker in BackroundWorkers)
                worker.Draw(gameTime, _spriteBatch);

            if (CurrentUser.UserData.FPSCounter)
            {
                _currentFrames++;
                _passed += gameTime.ElapsedGameTime;
                if (_passed >= TimeSpan.FromSeconds(1))
                {
                    _passed = TimeSpan.Zero;
                    _frames = _currentFrames;
                    _currentFrames = 0;
                }
                _spriteBatch.DrawString(BasicFonts.GetFont(16), $"FPS: {_frames}", new Vector2(0, 0), new Color(255, 0, 0, 255));
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public void ApplySettings()
        {
            UserManager.SaveUser(CurrentUser);

            Device.PreferredBackBufferHeight = (int)(CurrentUser.UserData.Scale * 1000);
            Device.PreferredBackBufferWidth = (int)(CurrentUser.UserData.Scale * 1000);
            Device.SynchronizeWithVerticalRetrace = CurrentUser.UserData.IsVsync;
            Device.IsFullScreen = CurrentUser.UserData.IsFullscreen;
            UIResources.LoadTexturePack(CurrentUser.UserData.TexturePack);
            MediaPlayer.Volume = CurrentUser.UserData.MusicVolume;
            SoundEffect.MasterVolume = CurrentUser.UserData.EffectsVolume;
            Device.ApplyChanges();
            LoadMods();
        }
    }
}