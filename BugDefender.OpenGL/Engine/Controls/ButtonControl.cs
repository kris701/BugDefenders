using BugDefender.OpenGL.Engine.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace BugDefender.OpenGL.Engine.Controls
{
    public class ButtonControl : LabelControl
    {
        public delegate void ClickedHandler(ButtonControl parent);
        public event ClickedHandler? Clicked;

        public GameWindow Parent { get; set; }
        public Guid ClickSound { get; set; } = new Guid("2e3a4bbb-c0e5-4617-aee1-070e02e4b8ea");
        public Texture2D FillClickedColor { get; set; } = BasicTextures.GetBasicRectange(Color.Gray);
        public Texture2D FillDisabledColor { get; set; } = BasicTextures.GetBasicRectange(Color.DarkGray);
        public bool IsEnabled { get; set; } = true;

        private bool _holding = false;
        private bool _blocked = false;

        public ButtonControl(GameWindow parent, ClickedHandler? clicked = null)
        {
            Parent = parent;
            Clicked += clicked;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;

            DrawTile(gameTime, spriteBatch, FillColor);
            DrawString(gameTime, spriteBatch);
            if (_holding)
                DrawTile(gameTime, spriteBatch, FillClickedColor);
            if (!IsEnabled)
                DrawTile(gameTime, spriteBatch, FillDisabledColor);
        }

        public void DoClick()
        {
            _holding = true;
            Parent.UIResources.PlaySoundEffectOnce(ClickSound);
            Clicked?.Invoke(this);
        }

        public override void Update(GameTime gameTime)
        {
            if (IsEnabled && IsVisible && Parent.IsActive && Clicked != null)
            {
                var mouseState = Mouse.GetState();
                if (!_blocked && (mouseState.X > X && mouseState.X < X + Width &&
                    mouseState.Y > Y && mouseState.Y < Y + Height))
                {
                    if (!_holding && mouseState.LeftButton == ButtonState.Pressed)
                        _holding = true;
                    else if (_holding && mouseState.LeftButton == ButtonState.Released)
                    {
                        Parent.UIResources.PlaySoundEffectOnce(ClickSound);
                        Clicked?.Invoke(this);
                        _holding = false;
                    }
                }
                else
                {
                    if (_holding && mouseState.LeftButton == ButtonState.Released)
                        _holding = false;
                    if (mouseState.LeftButton == ButtonState.Pressed)
                        _blocked = true;
                    else
                        _blocked = false;
                }
            }

            base.Update(gameTime);
        }
    }
}
