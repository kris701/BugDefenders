﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Engine.Screens;

namespace TDGame.OpenGL.Engine.Controls
{
    public class ButtonControl : LabelControl
    {
        public delegate void ClickedHandler(ButtonControl parent);
        public event ClickedHandler? Clicked;
        public event ClickedHandler? ClickedModifierA;
        public event ClickedHandler? ClickedModifierB;

        public Keys ModifierKeyA { get; set; } = Keys.LeftShift;
        public Keys ModifierKeyB { get; set; } = Keys.LeftControl;
        public Texture2D FillClickedColor { get; set; } = BasicTextures.GetBasicRectange(Color.Gray);
        public Texture2D FillDisabledColor { get; set; } = BasicTextures.GetBasicRectange(Color.DarkGray);
        public bool IsEnabled { get; set; } = true;

        private bool _holding = false;
        private bool _blocked = false;

        public ButtonControl(UIEngine parent, ClickedHandler? clicked = null, ClickedHandler? clickedModifierA = null, ClickedHandler? clickedModifierB = null) : base(parent)
        {
            Clicked += clicked;
            ClickedModifierA += clickedModifierA;
            ClickedModifierB += clickedModifierB;
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

        public override void Update(GameTime gameTime)
        {
            if (IsEnabled && IsVisible)
            {
                var mouseState = Mouse.GetState();
                if (!_blocked && (mouseState.X > X && mouseState.X < X + Width &&
                    mouseState.Y > Y && mouseState.Y < Y + Height))
                {
                    if (!_holding && mouseState.LeftButton == ButtonState.Pressed)
                        _holding = true;
                    else if (_holding && mouseState.LeftButton == ButtonState.Released)
                    {
                        var keyState = Keyboard.GetState();
                        if (keyState.IsKeyDown(ModifierKeyA))
                        {
                            if (ClickedModifierA != null)
                                ClickedModifierA.Invoke(this);
                        }
                        else if (keyState.IsKeyDown(ModifierKeyB))
                        {
                            if (ClickedModifierB != null)
                                ClickedModifierB.Invoke(this);
                        }
                        else
                        {
                            if (Clicked != null)
                                Clicked.Invoke(this);
                        }
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
