using BugDefender.Core.Game.Models.Maps;
using BugDefender.OpenGL.Engine.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BugDefender.OpenGL.Engine.Controls
{
    public class AnimatedButtonControl : BaseControl
    {
        public delegate void OnAnimationDoneHandler(AnimatedButtonControl parent);
        public OnAnimationDoneHandler? OnAnimationDone;

        public delegate void ClickedHandler(AnimatedButtonControl parent);
        public event ClickedHandler? Clicked;

        public GameWindow Parent { get; set; }
        public Guid ClickSound { get; set; } = new Guid("2e3a4bbb-c0e5-4617-aee1-070e02e4b8ea");
        public List<Texture2D> TileSet { get; set; } = new List<Texture2D>();
        public int Frame { get; set; } = 0;
        public bool AutoPlay { get; set; } = true;
        public TimeSpan FrameTime { get; set; } = TimeSpan.FromMilliseconds(500);
        private bool _finished = false;
        private TimeSpan _currentFrameTime = TimeSpan.Zero;
        public Texture2D FillClickedColor { get; set; } = BasicTextures.GetBasicRectange(Color.Gray);
        public Texture2D FillDisabledColor { get; set; } = BasicTextures.GetBasicRectange(Color.DarkGray);
        public bool IsEnabled { get; set; } = true;
        private string _text = "";
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                if (value != _text)
                    _textChanged = true;
                _text = value;
            }
        }
        public Color FontColor { get; set; } = Color.Black;
        private SpriteFont _font = BasicFonts.GetFont(8);
        public SpriteFont Font
        {
            get
            {
                return _font;
            }
            set
            {
                _font = value;
                _textChanged = true;
            }
        }

        internal float _textX = 0;
        internal float _textY = 0;
        internal float _textWidth = 0;
        internal float _textHeight = 0;
        internal bool _textChanged = true;

        private bool _holding = false;
        private bool _blocked = false;

        public AnimatedButtonControl(GameWindow parent, ClickedHandler? clicked = null)
        {
            Parent = parent;
            Clicked += clicked;
        }

        internal void UpdateTextPositions()
        {
            var size = Font.MeasureString(Text);
            if (Width == 0)
                Width = size.X;
            if (Height == 0)
                Height = size.Y;
            ReAlign();
            _textWidth = size.X;
            _textHeight = size.Y;
            _textX = X + Width / 2 - _textWidth / 2;
            _textY = Y + Height / 2 - _textHeight / 2;
        }

        public override void Initialize()
        {
            if (TileSet.Count > 0)
            {
                Frame = 0;
                int targetW = TileSet[0].Width;
                int targetH = TileSet[0].Height;
                foreach (var tile in TileSet.Skip(1))
                    if (tile.Width != targetW || tile.Height != targetH)
                        throw new Exception("Animated tileset must have the same size!");

                if (Width == 0)
                    Width = TileSet[0].Width;
                if (Height == 0)
                    Height = TileSet[0].Height;
            }
            UpdateTextPositions();
            base.Initialize();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;

            DrawTile(gameTime, spriteBatch, TileSet[Frame]);
            DrawString(gameTime, spriteBatch);
            if (_holding)
                DrawTile(gameTime, spriteBatch, FillClickedColor);
            if (!IsEnabled)
                DrawTile(gameTime, spriteBatch, FillDisabledColor);
        }

        public override void Update(GameTime gameTime)
        {
            if (_textChanged && Text != "")
            {
                _textChanged = false;
                UpdateTextPositions();
            }
            if (IsEnabled && IsVisible && Parent.IsActive && Clicked != null)
            {
                var mouseState = Mouse.GetState();
                var translatedPos = InputHelper.GetRelativePosition(Parent.XScale, Parent.YScale);
                if (!_blocked && (translatedPos.X > X && translatedPos.X < X + Width &&
                    translatedPos.Y > Y && translatedPos.Y < Y + Height))
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
            if (!_finished)
            {
                _currentFrameTime += gameTime.ElapsedGameTime;
                if (_currentFrameTime >= FrameTime)
                {
                    Frame++;
                    if (Frame >= TileSet.Count)
                    {
                        if (AutoPlay)
                            Frame = 0;
                        else
                            _finished = true;
                        OnAnimationDone?.Invoke(this);
                    }
                    _currentFrameTime = TimeSpan.Zero;
                }
            }

            base.Update(gameTime);
        }

        internal void DrawString(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Text != "")
                spriteBatch.DrawString(
                    Font,
                    Text,
                    new Vector2(_textX + _textWidth / 2, _textY + _textHeight / 2),
                    new Color(FontColor.R, FontColor.G, FontColor.B, Alpha),
                    Rotation,
                    new Vector2(_textWidth / 2, _textHeight / 2),
                    1f,
                    SpriteEffects.None,
                    0);
#if DEBUG && TEXTBORDER
            spriteBatch.Draw(BasicTextures.GetBasicRectange(Color.Purple), new Vector2(_textX, _textY), new Rectangle((int)_textX, (int)_textY, (int)_textWidth, 1), GetAlphaColor());
            spriteBatch.Draw(BasicTextures.GetBasicRectange(Color.Purple), new Vector2(_textX, _textY), new Rectangle((int)_textX, (int)_textY, 1, (int)_textHeight), GetAlphaColor());
            spriteBatch.Draw(BasicTextures.GetBasicRectange(Color.Purple), new Vector2(_textX + _textWidth, _textY), new Rectangle((int)(_textX + _textWidth), (int)_textY, 1, (int)_textHeight), GetAlphaColor());
            spriteBatch.Draw(BasicTextures.GetBasicRectange(Color.Purple), new Vector2(_textX, _textY + _textHeight), new Rectangle((int)_textX, (int)(_textY + _textHeight), (int)(_textWidth + 1), 1), GetAlphaColor());
#endif
        }
    }
}
