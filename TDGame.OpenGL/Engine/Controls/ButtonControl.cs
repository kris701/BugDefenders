using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using TDGame.OpenGL.Engine.Helpers;

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
        public string FillColorName { get; set; } = "";
        public Texture2D FillColor { get; set; } = BasicTextures.Transparent;
        public string FillClickedColorName { get; set; } = "";
        public Texture2D FillClickedColor { get; set; } = BasicTextures.Transparent;

        private bool _isClicked = false;
        private bool _wasPressed = false;
        private int _textX = 0;
        private int _textY = 0;

        public ButtonControl()
        {
        }

        public ButtonControl(ClickedHandler? clicked = null, ClickedHandler? clickedModifierA = null, ClickedHandler? clickedModifierB = null)
        {
            Clicked += clicked;
            ClickedModifierA += clickedModifierA;
            ClickedModifierB += clickedModifierB;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;

            if (!_isClicked)
                spriteBatch.Draw(FillColor, new Rectangle(X, Y, Width, Height), Color.White);
            else
                spriteBatch.Draw(FillClickedColor, new Rectangle(X, Y, Width, Height), Color.White);
            spriteBatch.DrawString(Font, Text, new Vector2(_textX, _textY), FontColor);
        }

        public override void Refresh()
        {
            if (!IsVisible)
                return;
            if (!IsEnabled)
                return;

            base.Refresh();
            if (Font == null)
                throw new Exception("Font not loaded!");

            var size = Font.MeasureString(Text);
            _textX = X + (Width - (int)size.X) / 2;
            _textY = Y + (Height - (int)size.Y) / 2;
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);
            if (FillColorName != "")
                FillColor = content.Load<Texture2D>(FillColorName);
            if (FillClickedColorName != "")
                FillClickedColor = content.Load<Texture2D>(FillClickedColorName);
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsVisible)
                return;
            if (!IsEnabled)
                return;

            var mouseState = Mouse.GetState();
            if (mouseState.X > X && mouseState.X < X + Width &&
                mouseState.Y > Y && mouseState.Y < Y + Height &&
                !_wasPressed &&
                mouseState.LeftButton == ButtonState.Pressed)
            {
                _isClicked = true;
                _wasPressed = true;
            }

            if (mouseState.X > X && mouseState.X < X + Width &&
                mouseState.Y > Y && mouseState.Y < Y + Height &&
                _wasPressed &&
                mouseState.LeftButton != ButtonState.Pressed)
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
                _wasPressed = false;
                _isClicked = false;
            }

            if (!(mouseState.X > X && mouseState.X < X + Width &&
                mouseState.Y > Y && mouseState.Y < Y + Height))
            {
                _wasPressed = false;
                _isClicked = false;
            }

            base.Update(gameTime);
        }
    }
}
