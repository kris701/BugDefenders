using BugDefender.OpenGL.Engine.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace BugDefender.OpenGL.Engine.Controls
{
    public class TextInputControl : LabelControl
    {
        public delegate void EnterHandler(TextInputControl parent);
        public event EnterHandler? OnEnter;
        public GameWindow Parent { get; set; }
        public Texture2D FillClickedColor { get; set; } = BasicTextures.GetBasicRectange(Color.Transparent);
        public Texture2D FillDisabledColor { get; set; } = BasicTextures.GetBasicRectange(Color.Transparent);
        public bool IsEnabled { get; set; } = true;
        public int Limit { get; set; } = 0;

        private bool _captured = false;
        private List<Keys> _lastKeys = new List<Keys>();
        private bool _holding = false;
        private bool _blocked = false;
        private readonly List<Keys> _legalCharacters = new List<Keys>()
        {
            Keys.A, Keys.B, Keys.C,
            Keys.D, Keys.E, Keys.F,
            Keys.H, Keys.I, Keys.J,
            Keys.K, Keys.L, Keys.M,
            Keys.N, Keys.O, Keys.P,
            Keys.Q, Keys.R, Keys.S,
            Keys.T, Keys.U, Keys.V,
            Keys.W, Keys.X, Keys.Y,
            Keys.Z, Keys.Back, Keys.Space,
            Keys.D0, Keys.D1, Keys.D2,
            Keys.D3, Keys.D4, Keys.D5,
            Keys.D6, Keys.D7, Keys.D8,
            Keys.D9
        };

        public TextInputControl(GameWindow parent, EnterHandler? onEnter = null)
        {
            Parent = parent;
            OnEnter += onEnter;
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
            if (IsEnabled && Parent.IsActive)
            {
                if (_captured)
                {
                    var keyState = Keyboard.GetState();
                    var keys = keyState.GetPressedKeys();
                    var newText = Text;
                    var isCapital = keys.Any(x => x == Keys.LeftShift || x == Keys.RightShift);
                    foreach (var key in keys)
                    {
                        if (!_lastKeys.Contains(key) && _legalCharacters.Contains(key))
                        {
                            if (newText.Length > 0 && key == Keys.Back)
                                newText = newText.Remove(newText.Length - 1);
                            else if (Limit != 0 && newText.Length < Limit)
                            {
                                switch (key)
                                {
                                    case Keys.Space: newText += " "; break;
                                    case Keys.D0: newText += "0"; break;
                                    case Keys.D1: newText += "1"; break;
                                    case Keys.D2: newText += "2"; break;
                                    case Keys.D3: newText += "3"; break;
                                    case Keys.D4: newText += "4"; break;
                                    case Keys.D5: newText += "5"; break;
                                    case Keys.D6: newText += "6"; break;
                                    case Keys.D7: newText += "7"; break;
                                    case Keys.D8: newText += "8"; break;
                                    case Keys.D9: newText += "9"; break;
                                    default:
                                        if (isCapital)
                                            newText += $"{key}".ToUpper();
                                        else
                                            newText += $"{key}".ToLower();
                                        break;
                                }
                            }
                        }
                        if (key == Keys.Enter)
                        {
                            OnEnter?.Invoke(this);
                            _captured = false;
                            _holding = false;
                            return;
                        }
                    }
                    Text = newText;
                    _lastKeys.Clear();
                    _lastKeys = keys.ToList();
                }

                var mouseState = Mouse.GetState();
                var translatedPos = InputHelper.GetRelativePosition(Parent.XScale, Parent.YScale);
                if (!_blocked && (translatedPos.X > X && translatedPos.X < X + Width &&
                    translatedPos.Y > Y && translatedPos.Y < Y + Height))
                {
                    if (!_holding && mouseState.LeftButton == ButtonState.Pressed)
                        _holding = true;
                    else if (_holding && mouseState.LeftButton == ButtonState.Released)
                    {
                        _captured = true;
                        _holding = false;
                    }
                }
                else
                {
                    if (_holding && mouseState.LeftButton == ButtonState.Released)
                        _holding = false;
                    if (mouseState.LeftButton == ButtonState.Pressed)
                    {
                        _blocked = true;
                        _captured = false;
                    }
                    else
                        _blocked = false;
                }
            }

            base.Update(gameTime);
        }
    }
}
