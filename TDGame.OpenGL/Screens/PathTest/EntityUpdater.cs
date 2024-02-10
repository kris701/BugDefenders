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

namespace TDGame.OpenGL.Screens.PathTest
{
    public class EntityUpdater<T> where T : IPosition, ITextured
    {
        public int Layer { get; set; }
        public IScreen Screen { get; set; }
        public int Size { get; set; }
        public int XOffset { get; set; }
        public int YOffset { get; set; }

        private Dictionary<T, ButtonControl> _entities = new Dictionary<T, ButtonControl>();
        private ClickedHandler? _onClicked;

        public EntityUpdater(int layer, IScreen screen, int size, int xOffset, int yOffset, ClickedHandler? onClicked = null)
        {
            Layer = layer;
            Screen = screen;
            Size = size;
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
                        toUpdate.X = XOffset + entity.X - Size / 2;
                        toUpdate.Y = YOffset + entity.Y - Size / 2;
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
                            ForceFit = true,
                            IsEnabled = false,
                            FillClickedColor = TextureBuilder.GetTexture(entity.ID),
                            FillDisabledColor = TextureBuilder.GetTexture(entity.ID),
                            FillColor = TextureBuilder.GetTexture(entity.ID),
                            X = entity.X - Size / 2,
                            Y = entity.Y - Size / 2,
                            Width = Size,
                            Height = Size,
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
                Screen.RemoveControl(Layer, _entities[entity]);
                _entities.Remove(entity);
            }
        }
    }
}
