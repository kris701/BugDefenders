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

namespace TDGame.OpenGL.Screens.PathTest
{
    public class EntityUpdater<T> where T : IPosition, ITextured
    {
        public int Layer { get; set; }
        public IScreen Screen { get; set; }
        public int Size { get; set; }

        private Dictionary<T, TileControl> _entities = new Dictionary<T, TileControl>();

        public EntityUpdater(int layer, IScreen screen, int size)
        {
            Layer = layer;
            Screen = screen;
            Size = size;
        }

        public void UpdateEntities(List<T> entities)
        {
            foreach(var entity in entities)
            {
                if (_entities.ContainsKey(entity)) 
                {
                    var toUpdate = _entities[entity];
                    toUpdate.X = entity.X - Size / 2;
                    toUpdate.Y = entity.Y - Size / 2;
                }
                else
                {
                    var newControl = new TileControl(Screen)
                    {
                        ForceFit = true,
                        FillColor = TextureBuilder.GetTexture(entity.ID),
                        X = entity.X - Size / 2,
                        Y = entity.Y - Size / 2,
                        Width = Size,
                        Height = Size
                    };
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
