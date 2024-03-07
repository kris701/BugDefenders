using BugDefender.OpenGL.Engine.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace BugDefender.OpenGL.Engine.Views
{
    public abstract class BaseView : IView
    {
        public Guid ID { get; set; }
        public IWindow Parent { get; set; }
        private readonly SortedDictionary<int, List<IControl>> _viewLayers;

        public BaseView(IWindow parent, Guid id)
        {
            Parent = parent;
            _viewLayers = new SortedDictionary<int, List<IControl>>() {
                { 0, new List<IControl>() }
            };
            ID = id;
        }

        public void ClearLayer(int layer)
        {
            if (!_viewLayers.ContainsKey(layer))
                _viewLayers.Add(layer, new List<IControl>());
            _viewLayers[layer].Clear();
        }

        public void AddControl(int layer, IControl control)
        {
            if (!_viewLayers.ContainsKey(layer))
                _viewLayers.Add(layer, new List<IControl>());
            _viewLayers[layer].Add(control);
        }

        public void RemoveControl(int layer, IControl control)
        {
            if (!_viewLayers.ContainsKey(layer))
                _viewLayers.Add(layer, new List<IControl>());
            _viewLayers[layer].Remove(control);
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var key in _viewLayers.Keys)
                foreach (var control in _viewLayers[key])
                    control.Draw(gameTime, spriteBatch);
        }

        public virtual void Initialize()
        {
            foreach (var key in _viewLayers.Keys)
                foreach (var control in _viewLayers[key])
                    control.Initialize();
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (var key in _viewLayers.Keys)
                foreach (var control in _viewLayers[key])
                    control.Update(gameTime);
            OnUpdate(gameTime);
        }

        public virtual void OnUpdate(GameTime gameTime)
        {

        }

        public virtual void SwitchView(IView screen)
        {
            Parent.CurrentScreen = screen;
        }
    }
}
