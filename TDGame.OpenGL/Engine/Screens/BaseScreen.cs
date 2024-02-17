using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TDGame.OpenGL.Engine.Helpers;

namespace TDGame.OpenGL.Engine.Screens
{
    public abstract class BaseScreen : IScreen
    {
        public UIEngine Parent { get; set; }
        public FadeState State { get; set; } = FadeState.FadeIn;

        public int FadeInTime { get; set; } = 200;
        public int FadeOutTime { get; set; } = 200;

        public float ScaleValue { get; set; } = 1;
        public int Scale(int value) => (int)(value * ScaleValue);
        public double Scale(double value) => value * ScaleValue;
        public float Scale(float value) => (float)(value * ScaleValue);
        public int InvScale(int value) => (int)(value / ScaleValue);
        public float InvScale(float value) => value / ScaleValue;

        private double fadeTimer = 0;
        private int fadeValue = 255;
        private IScreen _switchTo;
        private Texture2D _fillColor = BasicTextures.GetBasicRectange(Color.Black);
        private Dictionary<int, List<IControl>> _viewLayers;

        public BaseScreen(UIEngine parent)
        {
            Parent = parent;
            _viewLayers = new Dictionary<int, List<IControl>>() {
                { 0, new List<IControl>() }
            };
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
            spriteBatch.FillScreen(_fillColor, Parent.ScreenWidth(), Parent.ScreenHeight(), fadeValue);
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

            switch (State)
            {
                case FadeState.FadeIn:
                    fadeTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                    fadeValue = 255 - (int)((fadeTimer / (double)FadeInTime) * 255);
                    if (fadeTimer >= FadeInTime)
                    {
                        fadeTimer = 0;
                        State = FadeState.Hold;
                        fadeValue = 0;
                    }
                    break;
                case FadeState.FadeOut:
                    fadeTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                    fadeValue = (int)((fadeTimer / (double)FadeOutTime) * 255);
                    if (fadeTimer >= FadeOutTime)
                    {
                        if (_switchTo != null)
                            Parent.CurrentScreen = _switchTo;
                        State = FadeState.PostHold;
                    }
                    break;
            }
            OnUpdate(gameTime);
        }

        public virtual void OnUpdate(GameTime gameTime)
        {

        }

        public void SwitchView(IScreen screen)
        {
            if (_switchTo == null)
            {
                _switchTo = screen;
                State = FadeState.FadeOut;
            }
        }
    }
}
