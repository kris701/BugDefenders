using BugDefender.Core.Game.Models;
using BugDefender.OpenGL.Engine;
using BugDefender.OpenGL.Engine.Views;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace BugDefender.OpenGL.Views.GameView
{
    public class EntityUpdater<T, U> where U : IControl where T : notnull
    {
        public delegate void EntityHandler(U parent);
        public EntityHandler? OnDelete;
        public int Layer { get; set; }
        public IView Screen { get; set; }
        public int XOffset { get; set; }
        public int YOffset { get; set; }

        public int Count => _entities.Count;
        public U? GetItem(T index)
        {
            if (_entities.ContainsKey(index))
                return _entities[index];
            return default;
        }

        private readonly Dictionary<T, U> _entities = new Dictionary<T, U>();

        public EntityUpdater(int layer, IView screen, int xOffset, int yOffset)
        {
            Layer = layer;
            Screen = screen;
            XOffset = xOffset;
            YOffset = yOffset;
        }

        public void UpdateEntities(HashSet<T> entities, GameTime passed, Func<T, U> toControl, Action<T, U, GameTime>? updateOverride = null)
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
