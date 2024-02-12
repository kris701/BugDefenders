﻿using Microsoft.Xna.Framework;
using TDGame.OpenGL.Engine;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Engine.Screens;
using TDGame.OpenGL.Settings;

namespace TDGame.OpenGL.Screens.SettingsView
{
    public partial class SettingsView : BaseScreen
    {
        private SettingsDefinition _settings;
        public SettingsView(GameEngine parent) : base(parent)
        {
            _settings = parent.Settings.Copy();
            ScaleValue = parent.Settings.Scale;
            Initialize();
        }

        public void UpdateScreenSettingsButtons()
        {
            _scaleButtonOne.FillColor = BasicTextures.GetBasicRectange(Color.White);
            _scaleButtonTwo.FillColor = BasicTextures.GetBasicRectange(Color.White);
            _scaleButtonThree.FillColor = BasicTextures.GetBasicRectange(Color.White);
            _isFullScreen.FillColor = BasicTextures.GetBasicRectange(Color.White);
            _isVSync.FillColor = BasicTextures.GetBasicRectange(Color.White);
            foreach(var button in _texturePacksButtons)
                button.FillColor = BasicTextures.GetBasicRectange(Color.White);

            if (_settings.Scale == 0.5f)
                _scaleButtonOne.FillColor = BasicTextures.GetBasicRectange(Color.Yellow);
            if (_settings.Scale == 1f)
                _scaleButtonTwo.FillColor = BasicTextures.GetBasicRectange(Color.Yellow);
            if (_settings.Scale == 2f)
                _scaleButtonThree.FillColor = BasicTextures.GetBasicRectange(Color.Yellow);

            if (_settings.IsFullscreen)
                _isFullScreen.FillColor = BasicTextures.GetBasicRectange(Color.Yellow);
            if (_settings.IsVsync)
                _isVSync.FillColor = BasicTextures.GetBasicRectange(Color.Yellow);

            foreach(var button in _texturePacksButtons)
            {
                if (button.Tag is string str && str == _settings.TexturePack)
                {
                    button.FillColor = BasicTextures.GetBasicRectange(Color.Yellow);
                    break;
                }
            }
        }
    }
}
