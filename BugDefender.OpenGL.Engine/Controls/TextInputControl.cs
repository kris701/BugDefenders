using BugDefender.OpenGL.Engine.Controls.Elements;
using BugDefender.OpenGL.Engine.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BugDefender.OpenGL.Engine.Controls
{
    public class TextInputControl : LabelControl
    {
        public delegate void KeyEventHandler(TextInputControl parent);
        public event KeyEventHandler? OnEnter;
        public event KeyEventHandler? OnKeyDown;


        public IWindow Parent { get; set; }
        public Texture2D FillClickedColor { get; set; } = BasicTextures.GetBasicRectange(Color.Transparent);
        public Texture2D FillDisabledColor { get; set; } = BasicTextures.GetBasicRectange(Color.Transparent);
        public bool IsEnabled { get; set; } = true;
        public int Limit { get; set; } = 0;
        public Guid KeyDownSound
        {
            get => _keyDownSoundElement.SoundEffect;
            set => _keyDownSoundElement.SoundEffect = value;
        }
        public Guid EnterSound
        {
            get => _enterSoundElement.SoundEffect;
            set => _enterSoundElement.SoundEffect = value;
        }

        private readonly SoundEffectElement _keyDownSoundElement;
        private readonly SoundEffectElement _enterSoundElement;
        private bool _captured = false;
        private List<Keys> _lastKeys = new List<Keys>();
        private bool _holding = false;
        private bool _blocked = false;
        private bool _keyDown = false;
        private readonly List<Keys> _legalCharacters = new List<Keys>()
        {
            Keys.A, Keys.B, Keys.C,
            Keys.D, Keys.E, Keys.F, Keys.G,
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
        private TileControl _iBeam;
        private TimeSpan _iBeamTimer = TimeSpan.Zero;

        public TextInputControl(IWindow parent, KeyEventHandler? onEnter = null)
        {
            Parent = parent;
            OnEnter += onEnter;
            _keyDownSoundElement = new SoundEffectElement(parent);
            _enterSoundElement = new SoundEffectElement(parent);
            _iBeam = new TileControl()
            {
                FillColor = BasicTextures.GetBasicRectange(Color.White),
                Height = 30,
                Width = 3,
                IsVisible = false
            };
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;

            base.Draw(gameTime, spriteBatch);
            if (_holding || _keyDown)
                DrawTile(gameTime, spriteBatch, FillClickedColor);
            if (!IsEnabled)
                DrawTile(gameTime, spriteBatch, FillDisabledColor);
            _iBeam.Draw(gameTime, spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            if (IsEnabled && Parent.IsActive)
            {
                if (_captured)
                {
                    _keyDown = false;
                    var keyState = Keyboard.GetState();
                    var keys = keyState.GetPressedKeys();
                    var newText = Text;
                    var isCapital = keys.Any(x => x == Keys.LeftShift || x == Keys.RightShift);
                    foreach (var key in keys)
                    {
                        if (!_lastKeys.Contains(key) && _legalCharacters.Contains(key))
                        {
                            _keyDown = true;
                            if (!keys.Any(x => x == Keys.Enter))
                                _keyDownSoundElement.Trigger();
                            if (key == Keys.Back)
                            {
                                if (newText.Length > 0)
                                    newText = newText.Remove(newText.Length - 1);
                            }
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
                            _enterSoundElement.Trigger();
                            OnEnter?.Invoke(this);
                            _captured = false;
                            _holding = false;
                            return;
                        }
                    }
                    if (newText != Text)
                    {
                        Text = newText;
                        OnKeyDown?.Invoke(this);
                    }
                    _lastKeys.Clear();
                    _lastKeys = keys.ToList();

                    _iBeam.X = TextX + TextWidth;
                    _iBeam.Y = TextY;

                    _iBeamTimer -= gameTime.ElapsedGameTime;
                    if (_iBeamTimer <= TimeSpan.Zero)
                    {
                        _iBeamTimer = TimeSpan.FromMilliseconds(500);
                        _iBeam.IsVisible = !_iBeam.IsVisible;
                    }
                }

                var mouseState = Mouse.GetState();
                var translatedPos = InputHelper.GetRelativePosition(Parent.XScale, Parent.YScale);
                if (!_blocked && (translatedPos.X > X && translatedPos.X < X + Width &&
                    translatedPos.Y > Y && translatedPos.Y < Y + Height))
                {
                    if (!_captured)
                    {
                        _iBeam.IsVisible = true;
                        _iBeam.Height = TextHeight;
                        _iBeam.X = translatedPos.X - 10;
                        _iBeam.Y = translatedPos.Y - 15;
                    }
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
                    if (!_captured)
                        _iBeam.IsVisible = false;
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

            _iBeam.Update(gameTime);
            base.Update(gameTime);
        }
    }
}
