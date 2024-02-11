using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.Core.Models;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Screens;
using TDGame.OpenGL.Textures;
using static TDGame.OpenGL.Engine.Controls.ButtonControl;

namespace TDGame.OpenGL.Screens.GameScreen
{
    public class EntityUpdater<T> where T : IPosition, ITextured
    {
        public delegate void EntityHandler(ButtonControl parent);
        public EntityHandler? OnDelete;
        public int Layer { get; set; }
        public IScreen Screen { get; set; }
        public int XOffset { get; set; }
        public int YOffset { get; set; }

        private Dictionary<T, ButtonControl> _entities = new Dictionary<T, ButtonControl>();
        private ClickedHandler? _onClicked;

        public EntityUpdater(int layer, IScreen screen, int xOffset, int yOffset, ClickedHandler? onClicked = null)
        {
            Layer = layer;
            Screen = screen;
            XOffset = xOffset;
            YOffset = yOffset;
            _onClicked = onClicked;
        }

        public void UpdateEntities(List<T> entities, Func<T, ButtonControl> toControlOverride = null, Action<T, ButtonControl> updateOverride = null)
        {
            foreach(var entity in entities)
            {
                if (_entities.ContainsKey(entity)) 
                {
                    var toUpdate = _entities[entity];
                    if (updateOverride != null)
                        updateOverride(entity, toUpdate);
                    else
                    {
                        toUpdate.X = entity.X + XOffset;
                        toUpdate.Y = entity.Y + YOffset;
                        toUpdate.Rotation = entity.Angle + (float)Math.PI / 2;
                    }
                }
                else
                {
                    ButtonControl newControl;
                    if (toControlOverride != null)
                        newControl = toControlOverride(entity);
                    else
                    {
                        newControl = new ButtonControl(Screen, clicked: _onClicked)
                        {
                            IsEnabled = false,
                            FillClickedColor = TextureBuilder.GetTexture(entity.ID),
                            FillDisabledColor = TextureBuilder.GetTexture(entity.ID),
                            FillColor = TextureBuilder.GetTexture(entity.ID),
                            X = entity.X + XOffset,
                            Y = entity.Y + YOffset,
                            Width = entity.Size,
                            Height = entity.Size,
                            Rotation = entity.Angle + (float)Math.PI / 2,
                            Tag = entity
                        };
                    }
                    Screen.AddControl(Layer, newControl);
                    _entities.Add(entity, newControl);
                }
            }
            var toRemove = new List<T>();
            foreach (var entity in _entities.Keys)
                if (!entities.Contains(entity))
                    toRemove.Add(entity);
            foreach (var entity in toRemove)
            {
                if (OnDelete != null)
                    OnDelete.Invoke(_entities[entity]);
                Screen.RemoveControl(Layer, _entities[entity]);
                _entities.Remove(entity);
            }
        }
    }
}
