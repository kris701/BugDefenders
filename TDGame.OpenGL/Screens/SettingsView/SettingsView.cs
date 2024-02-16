using Microsoft.Xna.Framework;
using System;
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
            _isFullScreen.FillColor = BasicTextures.GetBasicRectange(Color.LightGray);
            _isVSync.FillColor = BasicTextures.GetBasicRectange(Color.LightGray);
            foreach (var button in _texturePacksButtons)
                button.FillColor = BasicTextures.GetBasicRectange(Color.LightGray);
            foreach (var button in _scaleButtons)
                button.FillColor = BasicTextures.GetBasicRectange(Color.LightGray);

            if (_settings.IsFullscreen)
                _isFullScreen.FillColor = BasicTextures.GetBasicRectange(Color.DarkGreen);
            if (_settings.IsVsync)
                _isVSync.FillColor = BasicTextures.GetBasicRectange(Color.DarkGreen);

            foreach (var button in _texturePacksButtons)
            {
                if (button.Tag is Guid str && str == _settings.TexturePack)
                {
                    button.FillColor = BasicTextures.GetBasicRectange(Color.DarkGreen);
                    break;
                }
            }
            foreach (var button in _scaleButtons)
            {
                if (button.Tag is float value && value == _settings.Scale)
                {
                    button.FillColor = BasicTextures.GetBasicRectange(Color.DarkGreen);
                    break;
                }
            }
        }
    }
}
