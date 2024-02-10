﻿using Microsoft.Xna.Framework;
using TDGame.Core.Maps;
using TDGame.OpenGL.Engine;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Engine.Screens;
using TDGame.OpenGL.Textures;

namespace TDGame.OpenGL.Screens.GameSetupView
{
    public partial class GameSetupView : BaseScreen
    {
        private string _selectedGameStyle = "";
        private ButtonControl? _selectedGameStyleButton;
        private string _selectedMap = "";
        private ButtonControl? _selectedMapButton;

        public GameSetupView(TDGame parent) : base(parent)
        {
            ScaleValue = parent.Scale;
            Initialize();
        }

        private void StartButton_Click(ButtonControl sender)
        {
            //if (_selectedMap != "" && _selectedGameStyle != "")
            //    Parent.SwitchView(new PathTestScreen(Parent, _selectedMap, _selectedGameStyle));
        }

        private void SelectMap_Click(ButtonControl sender)
        {
            if (sender.Tag is string mapName)
            {
                if (_selectedMapButton != null)
                    _selectedMapButton.FillColor = BasicTextures.GetBasicRectange(Color.DarkRed);

                _selectedMapButton = sender;
                _selectedMapButton.FillColor = BasicTextures.GetBasicRectange(Color.Yellow);

                _selectedMap = mapName;
                var map = MapBuilder.GetMap(mapName);
                _mapPreviewTile.FillColor = TextureBuilder.GetTexture(map.ID);
            }
        }

        private void SelectGameStyle_Click(ButtonControl sender)
        {
            if (sender.Tag is string gameStyleName)
            {
                if (_selectedGameStyleButton != null)
                    _selectedGameStyleButton.FillColor = BasicTextures.GetBasicRectange(Color.DarkRed);

                _selectedGameStyleButton = sender;
                _selectedGameStyleButton.FillColor = BasicTextures.GetBasicRectange(Color.Yellow);

                _selectedGameStyle = gameStyleName;
            }
        }
    }
}
