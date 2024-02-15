using Microsoft.Xna.Framework;
using System;
using TDGame.Core.Resources;
using TDGame.OpenGL.Engine;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Engine.Screens;
using TDGame.OpenGL.Screens.GameScreen;

namespace TDGame.OpenGL.Screens.GameSetupView
{
    public partial class GameSetupView : BaseScreen
    {
        private Guid? _selectedGameStyle;
        private ButtonControl? _selectedGameStyleButton;
        private Guid? _selectedMap;
        private ButtonControl? _selectedMapButton;

        public GameSetupView(GameEngine parent) : base(parent)
        {
            ScaleValue = parent.Settings.Scale;
            Initialize();
        }

        private void StartButton_Click(ButtonControl sender)
        {
            if (_selectedMap != null && _selectedGameStyle != null)
                SwitchView(new GameScreen.GameScreen(Parent, (Guid)_selectedMap, (Guid)_selectedGameStyle));
        }

        private void SelectMap_Click(ButtonControl sender)
        {
            if (sender.Tag is Guid mapName)
            {
                if (_selectedMapButton != null)
                    _selectedMapButton.FillColor = BasicTextures.GetBasicRectange(Color.DarkGray);

                _selectedMapButton = sender;
                _selectedMapButton.FillColor = BasicTextures.GetBasicRectange(Color.DarkGreen);

                _selectedMap = mapName;
                var map = ResourceManager.Maps.GetResource(mapName);
                _mapPreviewTile.FillColor = TextureManager.GetTexture(map.ID);
                _mapNameLabel.Text = map.Name;
                _mapDescriptionTextbox.Text = map.Description;
            }
        }

        private void SelectGameStyle_Click(ButtonControl sender)
        {
            if (sender.Tag is Guid gameStyleName)
            {
                if (_selectedGameStyleButton != null)
                    _selectedGameStyleButton.FillColor = BasicTextures.GetBasicRectange(Color.DarkGray);

                _selectedGameStyleButton = sender;
                _selectedGameStyleButton.FillColor = BasicTextures.GetBasicRectange(Color.DarkGreen);

                _selectedGameStyle = gameStyleName;
            }
        }
    }
}
