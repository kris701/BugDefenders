using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using TDGame.OpenGL.Engine.Helpers;

namespace TDGame.OpenGL.Engine.Screens
{
    public abstract class BaseScreen : IScreen, ILookup
    {
        public event SwitchEvent? EnteredView;
        public event SwitchEvent? ExitedView;
        public event UpdatedEvent? OnUpdate;
        public IEngine Parent { get; set; }
        public Dictionary<string, IControl> Lookup { get; } = new Dictionary<string, IControl>();
        public IChildrenContainer? Container { get; set; }
        public FadeState State { get; set; } = FadeState.FadeIn;

        public int FadeInTime { get; set; } = 200;
        public int FadeOutTime { get; set; } = 200;

        private double fadeTimer = 0;
        private int fadeValue = 255;
        private IScreen? _switchTo;

        public BaseScreen(IEngine parent)
        {
            Parent = parent;
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Container == null)
                throw new ArgumentNullException("Container is null!");

            Container.Draw(gameTime, spriteBatch);
            spriteBatch.FillScreen(BasicTextures.Black, Parent.ScreenWidth(), Parent.ScreenHeight(), fadeValue);
        }

        public virtual void Initialize()
        {
            if (Container != null)
            {
                GenerateLookup();
                Container.Initialize();
            }
        }

        public void GenerateLookup()
        {
            if (Container == null)
                throw new ArgumentNullException("Container is null!");

            Lookup.Clear();
            var lookupStack = Container.GetLookupStack();
            foreach (var item in lookupStack)
            {
                if (item.Name != "" && item.Name != null)
                    Lookup.Add(item.Name, item);
            }
        }

        public T FindItem<T>(string name) where T : IControl
        {
            if (Lookup.ContainsKey(name))
            {
                if (Lookup[name] is T item)
                    return item;
                else
                    throw new EntryPointNotFoundException($"Control named '{name}' was found, but it was not the type expected!");
            }
            throw new EntryPointNotFoundException($"Could not find control named '{name}'");
        }

        public virtual void LoadContent(ContentManager content)
        {
            if (Container == null)
                throw new ArgumentNullException("Container is null!");

            Container.X = 0;
            Container.Y = 0;
            Container.Width = Parent.ScreenWidth();
            Container.Height = Parent.ScreenHeight();
            Container.LoadContent(content);
        }

        public void Refresh()
        {
            if (Container == null)
                throw new ArgumentNullException("Container is null!");

            Container.Refresh();
        }

        public virtual void Update(GameTime gameTime)
        {
            if (Container == null)
                throw new ArgumentNullException("Container is null!");

            Container.Update(gameTime);
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
                        if (EnteredView != null)
                            EnteredView.Invoke();
                    }
                    break;
                case FadeState.FadeOut:
                    fadeTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                    fadeValue = (int)((fadeTimer / (double)FadeOutTime) * 255);
                    if (fadeTimer >= FadeOutTime)
                    {
                        if (_switchTo != null)
                        {
                            if (ExitedView != null)
                                ExitedView.Invoke();
                            Parent.SwitchView(_switchTo);
                        }
                        State = FadeState.PostHold;
                    }
                    break;
            }
            if (OnUpdate != null)
                OnUpdate.Invoke(gameTime);
        }

        public void SwitchView(IScreen? screen)
        {
            if (_switchTo == null)
            {
                _switchTo = screen;
                State = FadeState.FadeOut;
            }
        }
    }
}
