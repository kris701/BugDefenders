﻿using BugDefender.OpenGL.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.OpenGL.Formatter.Input;
using System;

namespace BugDefender.OpenGL.Screens.SettingsView
{
    public partial class SettingsView : BaseBugDefenderView
    {
        private static readonly Guid _id = new Guid("dd52b3a2-8d62-4733-8894-dde445408a02");
        private readonly SettingsDefinition _settings;
        private readonly KeyWatcher _escapeKeyWatcher;
        public SettingsView(BugDefenderGameWindow parent) : base(parent, _id)
        {
            _settings = parent.UserManager.CurrentUser.UserData.Copy();
            Initialize();
            _escapeKeyWatcher = new KeyWatcher(Keys.Escape, () => { SwitchView(new MainMenu.MainMenuView(Parent)); });
        }

        public override void OnUpdate(GameTime gameTime)
        {
            var keyState = Keyboard.GetState();
            _escapeKeyWatcher.Update(keyState);
        }

        public void UpdateScreenSettingsButtons()
        {
            var normal = Parent.Textures.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9"));
            var selected = Parent.Textures.GetTexture(new Guid("5b3e5e64-9c3d-4ba5-a113-b6a41a501c20"));
            _isFullScreen.FillColor = normal;
            _isVSync.FillColor = normal;
            _isFPSCounter.FillColor = normal;
            foreach (var button in _texturePacksButtons)
                button.FillColor = normal;
            foreach (var button in _scaleButtons)
            {
                button.FillColor = normal;
                button.IsEnabled = true;
            }
            foreach (var button in _musicButtons)
                button.FillColor = normal;
            foreach (var button in _soundEffectsButtons)
                button.FillColor = normal;

            if (_settings.IsFullscreen)
                _isFullScreen.FillColor = selected;
            if (_settings.IsVsync)
                _isVSync.FillColor = selected;
            if (_settings.FPSCounter)
                _isFPSCounter.FillColor = selected;

            foreach (var button in _texturePacksButtons)
            {
                if (button.Tag is Guid str && str == _settings.TexturePack)
                {
                    button.FillColor = selected;
                    break;
                }
            }
            if (_isFullScreen.FillColor == selected)
            {
                foreach (var button in _scaleButtons)
                    button.IsEnabled = false;
            }
            else
            {
                foreach (var button in _scaleButtons)
                {
                    if (button.Tag is Point value && value.X == _settings.ScreenWidth && value.Y == _settings.ScreenHeight)
                    {
                        button.FillColor = selected;
                        break;
                    }
                }
            }
            foreach (var button in _musicButtons)
            {
                if (button.Tag is float value && value == _settings.MusicVolume)
                {
                    button.FillColor = selected;
                    break;
                }
            }
            foreach (var button in _soundEffectsButtons)
            {
                if (button.Tag is float value && value == _settings.EffectsVolume)
                {
                    button.FillColor = selected;
                    break;
                }
            }
        }
    }
}
