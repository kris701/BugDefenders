using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using TDGame.Core.Resources;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Input;
using TDGame.OpenGL.Engine.Screens;

namespace TDGame.OpenGL.Screens.GameSetupView
{
    public partial class GameSetupView : BaseScreen
    {
        private static Guid _id = new Guid("1ccc48ee-6738-45cd-ae14-50d3d0896dc0");

        private Guid? _selectedGameStyle;
        private ButtonControl? _selectedGameStyleButton;
        private Guid? _selectedMap;
        private ButtonControl? _selectedMapButton;
        private KeyWatcher _escapeKeyWatcher;

        public GameSetupView(UIEngine parent) : base(parent, _id)
        {
            ScaleValue = parent.CurrentUser.UserData.Scale;
            Initialize();
            _escapeKeyWatcher = new KeyWatcher(Keys.Escape, () => { SwitchView(new MainMenu.MainMenu(Parent)); });
        }

        public override void OnUpdate(GameTime gameTime)
        {
            var keyState = Keyboard.GetState();
            _escapeKeyWatcher.Update(keyState);
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
                    _selectedMapButton.FillColor = UIResourceManager.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048"));

                _selectedMapButton = sender;
                _selectedMapButton.FillColor = UIResourceManager.GetTexture(new Guid("86911ca2-ebf3-408c-98f9-6221d9a322bc"));

                _selectedMap = mapName;
                var map = ResourceManager.Maps.GetResource(mapName);
                _mapPreviewTile.FillColor = UIResourceManager.GetTexture(map.ID);
                _mapNameLabel.Text = map.Name;
                _mapDescriptionTextbox.Text = map.Description;

                if (_selectedGameStyle != null && _selectedMap != null)
                    _startButton.IsEnabled = true;
            }
        }

        private void SelectGameStyle_Click(ButtonControl sender)
        {
            if (sender.Tag is Guid gameStyleName)
            {
                if (_selectedGameStyleButton != null)
                    _selectedGameStyleButton.FillColor = UIResourceManager.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048"));

                _selectedGameStyleButton = sender;
                _selectedGameStyleButton.FillColor = UIResourceManager.GetTexture(new Guid("86911ca2-ebf3-408c-98f9-6221d9a322bc"));

                _selectedGameStyle = gameStyleName;

                if (_selectedGameStyle != null && _selectedMap != null)
                    _startButton.IsEnabled = true;
            }
        }

        private void UpdateMapSelectionPages()
        {
            foreach (var buttons in _mapPages)
                foreach (var control in buttons)
                    control.IsVisible = false;

            foreach (var control in _mapPages[_currentMapPage])
                control.IsVisible = true;
        }

        private void UpdateGameStyleSelectionPages()
        {
            foreach (var buttons in _gameStylePages)
                foreach (var control in buttons)
                    control.IsVisible = false;

            foreach (var control in _gameStylePages[_currentGameStylePage])
                control.IsVisible = true;
        }
    }
}
