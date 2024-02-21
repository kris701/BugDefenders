using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TDGame.Core.Resources;
using TDGame.Core.Resources.Integrity;
using TDGame.Core.Users;
using TDGame.Core.Users.Models;
using TDGame.OpenGL.BackgroundWorkers.AchivementBackroundWorker;
using TDGame.OpenGL.BackgroundWorkers.NotificationBackroundWorker.Handles;
using TDGame.OpenGL.Engine.BackgroundWorkers;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Engine.Screens;
using TDGame.OpenGL.Settings;
using TDGame.OpenGL.Textures;

namespace TDGame.OpenGL
{
    public class UIEngine : Game
    {
        private static string _contentDir = "Content";
        private static string _modsDir = "Mods";

#if FPS
        private TimeSpan _passed = TimeSpan.Zero;
        private int _currentFrames = 0;
        private int _frames = 0;
#endif

        public GraphicsDeviceManager Device { get; }
        public int ScreenWidth() => Window.ClientBounds.Width;
        public int ScreenHeight() => Window.ClientBounds.Height;
        public IScreen CurrentScreen { get; set; }
        public UserEngine<SettingsDefinition> UserManager { get; set; }
        public UserDefinition<SettingsDefinition> CurrentUser { get; set; }
        public List<IBackgroundWorker> BackroundWorkers { get; set; }

        private Func<UIEngine, IScreen> _screenToLoad;
        private SpriteBatch? _spriteBatch;
        private bool _isInitialized = false;
        private NotificationBackroundWorker _notificationWorker;

        public UIEngine(Func<UIEngine, IScreen> screen)
        {
            Device = new GraphicsDeviceManager(this);
            Content.RootDirectory = _contentDir;
            _screenToLoad = screen;
            IsMouseVisible = true;
            UserManager = new UserEngine<SettingsDefinition>();
            var allUsers = UserManager.GetAllUsers();
            if (allUsers.Count == 0)
            {
                var newUser = new UserDefinition<SettingsDefinition>()
                {
                    ID = Guid.NewGuid(),
                    Name = "Default",
                    IsPrimary = true,
                    UserData = new SettingsDefinition()
                };
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
            var newUser = new UserDefinition<SettingsDefinition>()
            {
                ID = Guid.NewGuid(),
                Name = name,
                IsPrimary = false,
                UserData = new SettingsDefinition()
            };
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

            TextureManager.Initialize(Content);
            BasicTextures.Initialize(GraphicsDevice);
            BasicFonts.Initialize(Content);
            ApplySettings();

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
                    if (subFolder.Name.ToUpper() == "TEXTURES")
                    {
                        foreach (var file in subFolder.GetFiles())
                        {
                            var textureSetDef = JsonSerializer.Deserialize<TexturesDefinition>(File.ReadAllText(file.FullName));
                            foreach (var texture in textureSetDef.Textures)
                            {
                                var textureFile = new FileInfo(Path.Combine(subFolder.Parent.FullName, "Content", texture.Content));
                                texture.Content = textureFile.FullName;
                                TextureManager.LoadTexture(texture);
                            }
                        }
                    }
                }
            }

            var checker = new ResourceIntegrityChecker();
            checker.CheckGameIntegrity();
            if (checker.Errors.Where(x => x.Severity > IntegrityError.SeverityLevel.Critical).Count() > 0)
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
            UserManager.SaveUser(CurrentUser);

            Device.PreferredBackBufferHeight = (int)(CurrentUser.UserData.Scale * 1000);
            Device.PreferredBackBufferWidth = (int)(CurrentUser.UserData.Scale * 1000);
            Device.SynchronizeWithVerticalRetrace = CurrentUser.UserData.IsVsync;
            Device.IsFullScreen = CurrentUser.UserData.IsFullscreen;
            TextureManager.LoadTexturePack(CurrentUser.UserData.TexturePack);
            Device.ApplyChanges();
            LoadMods();
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