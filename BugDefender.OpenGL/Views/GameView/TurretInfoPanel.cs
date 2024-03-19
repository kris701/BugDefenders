using BugDefender.Core.Game.Models.Entities.Turrets;
using BugDefender.Core.Game.Models.GameStyles;
using BugDefender.OpenGL.Controls;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Helpers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using static BugDefender.Core.Game.Models.Entities.Turrets.TurretInstance;
using static BugDefender.OpenGL.Engine.Controls.ButtonControl;

namespace BugDefender.OpenGL.Views.GameView
{
    public class TurretInfoPanel : CollectionControl
    {
        public TurretInstance? SelectedTurret { get; set; }
        public GameStyleDefinition GameStyle { get; set; }

        private readonly BugDefenderGameWindow _parent;
        private readonly BugDefenderButtonControl _sellTurretButton;
        private readonly TextboxControl _turretStatsTextbox;
        private readonly List<BugDefenderButtonControl> _turretTargetingModes;

        public TurretInfoPanel(BugDefenderGameWindow parent, GameStyleDefinition gameStyle, ClickedHandler sellTurretClick)
        {
            Width = 650;
            Height = 330;
            _parent = parent;
            GameStyle = gameStyle;
            Children.Add(new TileControl()
            {
                FillColor = parent.TextureController.GetTexture(new Guid("90447608-bd7a-478c-9bfd-fddb26c731b7")),
                Height = Height,
                Width = Width
            });
            Children.Add(new LabelControl()
            {
                Text = "Turret Stats",
                Font = BasicFonts.GetFont(10),
                FontColor = Color.White,
                Y = 5,
                Height = 35,
                Width = Width
            });

            _sellTurretButton = new BugDefenderButtonControl(parent, sellTurretClick)
            {
                FillColor = parent.TextureController.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")),
                FillClickedColor = parent.TextureController.GetTexture(new Guid("78bbfd61-b6de-416a-80ba-e53360881759")),
                FillDisabledColor = parent.TextureController.GetTexture(new Guid("6fb75caf-80ca-4f03-a1bb-2485b48aefd8")),
                Font = BasicFonts.GetFont(10),
                FontColor = Color.White,
                Text = "Sell Turret",
                X = 10,
                Y = 40,
                Width = Width - 20,
                Height = 30,
                IsEnabled = false
            };
            Children.Add(_sellTurretButton);
            _turretStatsTextbox = new TextboxControl()
            {
                Font = BasicFonts.GetFont(8),
                FontColor = Color.White,
                Text = "Select a Turret",
                X = 10,
                Y = 75,
                Width = Width - 20,
                Height = Height - 115
            };
            Children.Add(_turretStatsTextbox);

            var values = Enum.GetValues(typeof(TargetingTypes)).Cast<TargetingTypes>().ToList();
            var buttonWidth = (Width - 20) / (values.Count - 1) - 5;
            var count = 0;
            _turretTargetingModes = new List<BugDefenderButtonControl>();
            foreach (TargetingTypes option in values.Skip(1))
            {
                var newControl = new BugDefenderButtonControl(parent, TargettingModeChanged)
                {
                    FillColor = parent.TextureController.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                    FillClickedColor = parent.TextureController.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
                    FillDisabledColor = parent.TextureController.GetTexture(new Guid("5e7e1313-fa7c-4f71-9a6e-e2650a7af968")),
                    Font = BasicFonts.GetFont(10),
                    FontColor = Color.White,
                    Text = $"{Enum.GetName(typeof(TargetingTypes), option)}",
                    X = 12 + (count++ * (buttonWidth + 5)),
                    Y = 285,
                    Height = 25,
                    Width = buttonWidth,
                    IsEnabled = false,
                    Tag = option
                };
                _turretTargetingModes.Add(newControl);
                Children.Add(newControl);
            }
        }

        public void Unselect()
        {
            SelectedTurret = null;
            ToggleButtonEnableness(false);
            _turretStatsTextbox.Text = "Select a Turret";
            _sellTurretButton.Text = "Sell Turret";
        }

        public void SelectInstance(TurretInstance instance)
        {
            SelectedTurret = instance;
            ToggleButtonEnableness(true);
            _turretStatsTextbox.Text = instance.ToString();
            _sellTurretButton.Text = $"[{instance.GetTurretWorth(GameStyle)}$] Sell Turret";
            foreach (var button in _turretTargetingModes)
                if (button.Tag is TargetingTypes target && instance.TargetingType == target)
                    button.FillColor = _parent.TextureController.GetTexture(new Guid("5b3e5e64-9c3d-4ba5-a113-b6a41a501c20"));
        }

        public void SelectDefinition(TurretDefinition definition)
        {
            ToggleButtonEnableness(false);
            _turretStatsTextbox.Text = new TurretInstance(definition).ToString();
        }

        private void ToggleButtonEnableness(bool to)
        {
            _sellTurretButton.IsEnabled = to;
            foreach (var button in _turretTargetingModes)
            {
                button.IsEnabled = to;
                button.FillColor = _parent.TextureController.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048"));
            }
        }

        private void TargettingModeChanged(ButtonControl e)
        {
            if (e.Tag is TargetingTypes newOption && SelectedTurret != null)
            {
                SelectedTurret.TargetingType = newOption;
                foreach (var button in _turretTargetingModes)
                    button.FillColor = _parent.TextureController.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")); ;
                e.FillColor = _parent.TextureController.GetTexture(new Guid("5b3e5e64-9c3d-4ba5-a113-b6a41a501c20"));
            }
        }
    }
}
