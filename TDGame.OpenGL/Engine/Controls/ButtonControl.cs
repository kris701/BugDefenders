using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public ButtonControl(IScreen parent, ClickedHandler? clicked = null, ClickedHandler? clickedModifierA = null, ClickedHandler? clickedModifierB = null) : base(parent)
        {
            Clicked += clicked;
            ClickedModifierA += clickedModifierA;
            ClickedModifierB += clickedModifierB;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var targetColor = FillColor;
            if (_holding)
                targetColor = FillClickedColor;
            if (!IsEnabled)
                targetColor = FillDisabledColor;

            DrawTile(gameTime, spriteBatch, targetColor);
            DrawString(gameTime, spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            if (IsEnabled)
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
