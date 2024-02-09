using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using TDGame.OpenGL.Engine.Helpers;

namespace TDGame.OpenGL.Engine.Controls
{
    public class ScrollStackPanelControl : StackPanelControl
    {
        public int ScrollButtonsSize { get; set; } = 25;
        public string ScrollBarIndicatorFillColorName { get; set; } = "";
        public Texture2D ScrollBarIndicatorFillColor { get; set; } = BasicTextures.GetBasicRectange(Color.Gray);
        public string ScrollButtonsFillColorName { get; set; } = "";
        public Texture2D ScrollButtonsFillColor { get; set; } = BasicTextures.GetBasicRectange(Color.Gray);
        public string ScrollButtonsClickedFillColorName { get; set; } = "";
        public Texture2D ScrollButtonsClickedFillColor { get; set; } = BasicTextures.GetBasicRectange(Color.White);
        public SpriteFont ScrollButtonsFont { get; set; }

        private ButtonControl _upButton;
        private ButtonControl _downButton;
        private PanelControl _scrollIndicator;
        private int _scrollIndex = 0;
        private int _maxScrollIndex = 0;
        private bool[] _visibilities = new bool[0];
        private int _previousScrollWheelValue = 0;
        private int _scrollWheelSensitivity = 10;
        private bool _needScroll = false;

        public ScrollStackPanelControl(SpriteFont scrollButtonsFont)
        {
            ScrollButtonsFont = scrollButtonsFont;
            _upButton = new ButtonControl();
            _downButton = new ButtonControl();
            _scrollIndicator = new PanelControl();
        }

        public override void Initialize()
        {
            base.Initialize();
            _upButton = new ButtonControl(clicked: (x) => ScrollUp())
            {
                IsVisible = false,
                Text = "^",
                Font = ScrollButtonsFont,
                Width = ScrollButtonsSize,
                Height = ScrollButtonsSize,
                FillColor = ScrollButtonsFillColor,
                FillClickedColor = ScrollButtonsClickedFillColor
            };
            _downButton = new ButtonControl(clicked: (x) => ScrollDown())
            {
                IsVisible = false,
                Text = "v",
                Font = ScrollButtonsFont,
                Width = ScrollButtonsSize,
                Height = ScrollButtonsSize,
                FillColor = ScrollButtonsFillColor,
                FillClickedColor = ScrollButtonsClickedFillColor
            };
            _scrollIndicator = new PanelControl()
            {
                IsVisible = false,
                FillColor = ScrollBarIndicatorFillColor
            };

            _visibilities = new bool[Children.Count];
            for (int i = 0; i < Children.Count; i++)
                _visibilities[i] = true;

            _maxScrollIndex = Children.Count;

            _previousScrollWheelValue = Mouse.GetState().ScrollWheelValue;
        }

        public void ScrollUp()
        {
            _scrollIndex--;
            if (_scrollIndex < 0)
                _scrollIndex = 0;
            Refresh();
        }

        public void ScrollDown()
        {
            _scrollIndex++;
            if (_scrollIndex > _maxScrollIndex)
                _scrollIndex--;
            Refresh();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;

            spriteBatch.Draw(FillColor, new Rectangle(X, Y, Width, Height), Color.White);

            if (_needScroll)
            {
                _upButton.Draw(gameTime, spriteBatch);
                _downButton.Draw(gameTime, spriteBatch);
                _scrollIndicator.Draw(gameTime, spriteBatch);

                int index = 0;
                foreach (var child in Children)
                {
                    if (child.IsVisible)
                    {
                        if (_visibilities[index])
                            child.Draw(gameTime, spriteBatch);
                        index++;
                    }
                }
            }
            else
            {
                foreach (var child in Children)
                    child.Draw(gameTime, spriteBatch);
            }


        }

        public override void Update(GameTime gameTime)
        {
            if (!IsVisible)
                return;
            if (!IsEnabled)
                return;

            if (_needScroll)
            {
                _downButton.Update(gameTime);
                _upButton.Update(gameTime);
                _scrollIndicator.Update(gameTime);
                CheckScrollWheel();
            }

            foreach (var child in Children)
                child.Update(gameTime);
        }

        private void CheckScrollWheel()
        {
            var mouseState = Mouse.GetState();
            if (mouseState.X > X && mouseState.X < X + Width &&
                mouseState.Y > Y && mouseState.Y < Y + Height)
            {
                var newScrollValue = mouseState.ScrollWheelValue;
                if (Math.Abs(newScrollValue - _previousScrollWheelValue) >= _scrollWheelSensitivity)
                {
                    if (newScrollValue > _previousScrollWheelValue)
                        ScrollUp();
                    else
                        ScrollDown();
                    _previousScrollWheelValue = newScrollValue;
                }
            }
        }

        public override void Refresh()
        {
            if (!IsVisible)
                return;
            if (!IsEnabled)
                return;

            int yOffset = Y + Margin;
            int index = 0;
            InitializeVisualArray();

            foreach (var child in Children)
            {
                if (child.IsVisible)
                {
                    if (index >= _scrollIndex)
                        _visibilities[index] = true;
                    else
                        _visibilities[index] = false;

                    if (_visibilities[index])
                    {
                        child.IsEnabled = true;
                        child.X = X + Margin;
                        child.Y = yOffset;
                        child.Width = Width - 2 * Margin - ScrollButtonsSize - Margin;
                        child.Refresh();
                        if (child.Height == 0)
                            child.Height = 10;
                        yOffset += child.Height + Margin;

                        if (yOffset >= Y + Height)
                        {
                            child.IsEnabled = false;
                            _visibilities[index] = false;
                            _needScroll = true;
                        }
                        else
                        {
                            if (_scrollIndex == 0)
                                _needScroll = false;
                        }
                    }
                    else
                        child.IsEnabled = false;
                    index++;
                }
            }

            if (!_needScroll)
            {
                foreach (var child in Children)
                {
                    if (child.IsVisible)
                    {
                        child.Width = Width - 2 * Margin;
                        child.Refresh();
                    }
                }

                _upButton.IsVisible = false;
                _downButton.IsVisible = false;
                _scrollIndicator.IsVisible = false;
            }
            else
            {
                DetermineMaxScrollIndex();

                _upButton.IsVisible = true;
                _downButton.IsVisible = true;
                _scrollIndicator.IsVisible = true;

                _upButton.X = X + Width - ScrollButtonsSize;
                _upButton.Y = Y;

                _downButton.X = X + Width - ScrollButtonsSize;
                _downButton.Y = Y + Height - ScrollButtonsSize;

                _scrollIndicator.X = X + Width - ScrollButtonsSize;
                _scrollIndicator.Y = (Y + ScrollButtonsSize) + ((Height - 2 * ScrollButtonsSize) / (_maxScrollIndex + 1)) * _scrollIndex;
                _scrollIndicator.Width = ScrollButtonsSize;
                _scrollIndicator.Height = (Height - 2 * ScrollButtonsSize) / (_maxScrollIndex + 1);

                _upButton.Refresh();
                _downButton.Refresh();
                _scrollIndicator.Refresh();
            }
        }

        private void DetermineMaxScrollIndex()
        {
            _maxScrollIndex = 0;
            foreach (var child in Children)
            {
                if (child.IsVisible)
                {
                    _maxScrollIndex++;
                }
            }
            _maxScrollIndex--;
        }

        private void InitializeVisualArray()
        {
            int visibleChildren = 0;
            foreach (var child in Children)
            {
                if (child.IsVisible)
                {
                    visibleChildren++;
                }
            }

            _visibilities = new bool[visibleChildren];
            for (int i = 0; i < visibleChildren; i++)
                _visibilities[i] = true;
        }

        public override void LoadContent(ContentManager content)
        {
            if (ScrollBarIndicatorFillColorName != "")
                ScrollBarIndicatorFillColor = content.Load<Texture2D>(ScrollBarIndicatorFillColorName);
            if (ScrollButtonsFillColorName != "")
                ScrollButtonsFillColor = content.Load<Texture2D>(ScrollButtonsFillColorName);
            if (ScrollButtonsClickedFillColorName != "")
                ScrollButtonsClickedFillColor = content.Load<Texture2D>(ScrollButtonsClickedFillColorName);

            _upButton.LoadContent(content);
            _downButton.LoadContent(content);
            _scrollIndicator.LoadContent(content);

            foreach (var child in Children)
                child.LoadContent(content);
        }
    }
}
