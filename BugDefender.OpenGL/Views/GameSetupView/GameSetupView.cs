﻿using BugDefender.Core.Game.Models.GameStyles;
using BugDefender.Core.Game.Models.Maps;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Input;
using BugDefender.OpenGL.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace BugDefender.OpenGL.Screens.GameSetupView
{
    public partial class GameSetupView : BaseBugDefenderView
    {
        private static readonly Guid _id = new Guid("1ccc48ee-6738-45cd-ae14-50d3d0896dc0");

        private Guid? _selectedGameStyle;
        private Guid? _selectedMap;
        private readonly KeyWatcher _escapeKeyWatcher;

        public GameSetupView(BugDefenderGameWindow parent) : base(parent, _id)
        {
            Initialize();
            _escapeKeyWatcher = new KeyWatcher(Keys.Escape, () => { SwitchView(new MainMenu.MainMenuView(Parent)); });
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
            if (sender.Tag is MapDefinition map)
            {
                if (_selectedMapButton != null)
                    _selectedMapButton.FillColor = Parent.TextureController.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048"));

                _selectedMapButton = sender;
                _selectedMapButton.FillColor = Parent.TextureController.GetTexture(new Guid("86911ca2-ebf3-408c-98f9-6221d9a322bc"));

                _selectedMap = map.ID;
                _mapPreviewTile.FillColor = Parent.TextureController.GetTexture(map.ID);
                _mapNameLabel.Text = map.Name;
                _mapDescriptionTextbox.Text = map.Description;

                if (_selectedGameStyle != null && _selectedMap != null)
                    _startButton.IsEnabled = true;
            }
        }

        private void SelectGameStyle_Click(ButtonControl sender)
        {
            if (sender.Tag is GameStyleDefinition gameStyle)
            {
                if (_selectedGameStyleButton != null)
                    _selectedGameStyleButton.FillColor = Parent.TextureController.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048"));

                _selectedGameStyleButton = sender;
                _selectedGameStyleButton.FillColor = Parent.TextureController.GetTexture(new Guid("86911ca2-ebf3-408c-98f9-6221d9a322bc"));

                _selectedGameStyle = gameStyle.ID;

                if (_selectedGameStyle != null && _selectedMap != null)
                    _startButton.IsEnabled = true;
            }
        }
    }
}
