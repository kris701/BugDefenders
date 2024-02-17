using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using TDGame.Core.Game.Models;
using TDGame.OpenGL.Engine;
using TDGame.OpenGL.Engine.Screens;

namespace TDGame.OpenGL.Screens.GameScreen
{
    public class EntityUpdater<T, U> where U : IControl
    {
        public delegate void EntityHandler(U parent);
        public EntityHandler? OnDelete;
        public int Layer { get; set; }
        public IScreen Screen { get; set; }
        public int XOffset { get; set; }
        public int YOffset { get; set; }

        public int Count => _entities.Count;
        public U? GetItem(T index)
        {
            if (_entities.ContainsKey(index))
                return _entities[index];
            return default(U);
        }

        private Dictionary<T, U> _entities = new Dictionary<T, U>();

        public EntityUpdater(int layer, IScreen screen, int xOffset, int yOffset)
        {
            Layer = layer;
            Screen = screen;
            XOffset = xOffset;
            YOffset = yOffset;
        }

        public void UpdateEntities(List<T> entities, GameTime passed, Func<T, U> toControl, Action<T, U, GameTime> updateOverride = null)
        {
            foreach (var entity in entities)
            {
                if (_entities.ContainsKey(entity))
                {
                    var toUpdate = _entities[entity];
                    if (updateOverride != null)
                        updateOverride(entity, toUpdate, passed);
                    else if (entity is IPosition pos)
                    {
                        toUpdate.X = pos.X + XOffset;
                        toUpdate.Y = pos.Y + YOffset;
                        toUpdate.Rotation = pos.Angle + (float)Math.PI / 2;
                    }
                    else
                        throw new NotImplementedException("Entity update not set for this type!");
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
