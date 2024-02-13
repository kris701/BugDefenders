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
    public class EntityUpdater<T, U> where T : IPosition, IIdentifiable where U : TileControl
    {
        public delegate void EntityHandler(U parent);
        public EntityHandler? OnDelete;
        public int Layer { get; set; }
        public IScreen Screen { get; set; }
        public int XOffset { get; set; }
        public int YOffset { get; set; }

        public int Count => _entities.Count;
        public U GetItem(T index) => _entities[index];

        private Dictionary<T, U> _entities = new Dictionary<T, U>();

        public EntityUpdater(int layer, IScreen screen, int xOffset, int yOffset)
        {
            Layer = layer;
            Screen = screen;
            XOffset = xOffset;
            YOffset = yOffset;
        }

        public void UpdateEntities(List<T> entities, Func<T, U> toControl, Action<T, U> updateOverride = null)
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
                    U newControl = toControl(entity);
                    newControl.Initialize();
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
