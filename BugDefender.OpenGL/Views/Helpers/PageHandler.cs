using BugDefender.OpenGL.Controls;
using BugDefender.OpenGL.Helpers;
using Microsoft.Xna.Framework;
using MonoGame.OpenGL.Formatter.Controls;
using MonoGame.OpenGL.Formatter.Controls.Elements;
using MonoGame.OpenGL.Formatter.Views;
using System;
using System.Collections.Generic;

namespace BugDefender.OpenGL.Views.Helpers
{
    public class PageHandler<T> : CollectionControl where T : IControl
    {
        public List<List<T>> Pages = new List<List<T>>();
        public int PageIndex { get; set; } = 0;
        public int ItemsPrPage { get; set; } = 5;
        public float Margin { get; set; } = 5;
        public IView Parent { get; set; }

        public float ButtonSize { get; set; } = 50;
        public Guid ButtonFontSize { get; set; } = FontSizes.Ptx16;
        public float LeftButtonX { get; set; } = 0;
        public float LeftButtonY { get; set; } = 0;
        public float RightButtonX { get; set; } = 0;
        public float RightButtonY { get; set; } = 0;
        public int MinPage { get; set; } = 0;
        public int MaxPage { get; set; } = int.MaxValue;
        public int MinItem { get; set; } = 0;
        public int MaxItem { get; set; } = int.MaxValue;

        private readonly List<T> _items;
        private BugDefenderButtonControl? _leftButton;
        private BugDefenderButtonControl? _rightButton;
        private ScrollWatcherElement? _scrollWatcher;

        public PageHandler(BaseView parent, List<T> items)
        {
            Parent = parent;
            _items = items;
        }

        public override void Initialize()
        {
            foreach (var page in Pages)
                foreach (var item in page)
                    Parent.RemoveControl(2, item);

            _leftButton = new BugDefenderButtonControl(Parent.Parent, clicked: (s) =>
            {
                PageIndex--;
                if (PageIndex >= MaxPage)
                    PageIndex = MaxPage - 1;
                if (PageIndex < MinPage)
                    PageIndex = MinPage;
                UpdatePages();
            })
            {
                FillColor = Parent.Parent.Textures.GetTexture(new Guid("d86347e3-3834-4161-9bbe-0d761d1d27ae")),
                FillClickedColor = Parent.Parent.Textures.GetTexture(new Guid("2c220d3f-5e7a-44ec-b4da-459f104c1e4a")),
                FillDisabledColor = Parent.Parent.Textures.GetTexture(new Guid("2c220d3f-5e7a-44ec-b4da-459f104c1e4a")),
                FontColor = Color.White,
                Font = Parent.Parent.Fonts.GetFont(ButtonFontSize),
                Text = $"<",
                X = LeftButtonX,
                Y = LeftButtonY,
                Height = ButtonSize,
                Width = ButtonSize,
                IsEnabled = false,
                IsVisible = false,
            };
            Children.Add(_leftButton);
            _rightButton = new BugDefenderButtonControl(Parent.Parent, clicked: (s) =>
            {
                PageIndex++;
                if (PageIndex >= MaxPage)
                    PageIndex = MaxPage - 1;
                if (PageIndex < MinPage)
                    PageIndex = MinPage;
                UpdatePages();
            })
            {
                FillColor = Parent.Parent.Textures.GetTexture(new Guid("d86347e3-3834-4161-9bbe-0d761d1d27ae")),
                FillClickedColor = Parent.Parent.Textures.GetTexture(new Guid("2c220d3f-5e7a-44ec-b4da-459f104c1e4a")),
                FillDisabledColor = Parent.Parent.Textures.GetTexture(new Guid("2c220d3f-5e7a-44ec-b4da-459f104c1e4a")),
                FontColor = Color.White,
                Font = Parent.Parent.Fonts.GetFont(ButtonFontSize),
                Text = $">",
                X = RightButtonX,
                Y = RightButtonY,
                Height = ButtonSize,
                Width = ButtonSize,
                IsVisible = false
            };
            Children.Add(_rightButton);

            Pages.Clear();
            int offset = 0;
            if (_items.Count <= ItemsPrPage)
            {
                Pages.Add(new List<T>());
                foreach (var item in _items)
                {
                    item.Y = offset++ * (item.Height + Margin);
                    Pages[0].Add(item);
                    Children.Add(item);
                }
            }
            else
            {
                int pageIndex = -1;
                int count = 0;
                foreach (var item in _items)
                {
                    if (count % ItemsPrPage == 0)
                    {
                        pageIndex++;
                        Pages.Add(new List<T>());
                        offset = 0;
                    }
                    item.Y = offset++ * (item.Height + Margin);
                    item.IsVisible = pageIndex == 0;
                    Pages[pageIndex].Add(item);
                    Children.Add(item);
                    count++;
                }

                _scrollWatcher = new ScrollWatcherElement(Parent.Parent)
                {
                    X = X,
                    Y = Y,
                    Width = Width,
                    Height = Height
                };
                _scrollWatcher.ScrollChanged += (o, n) =>
                {
                    if (n > o)
                        _leftButton.DoClick();
                    else if (n < o)
                        _rightButton.DoClick();
                };
            }

            if (MaxPage == int.MaxValue)
                MaxPage = Pages.Count;
            if (MaxPage - MinPage < 0)
                throw new Exception("Impossible page limits!");
            if (PageIndex < MinPage)
                PageIndex = MaxPage;

            base.Initialize();
            UpdatePages();
        }

        public override void Update(GameTime gameTime)
        {
            if (_scrollWatcher != null)
            {
                _scrollWatcher.IsEnabled = IsVisible;
                _scrollWatcher.Update(gameTime);
            }
            base.Update(gameTime);
        }

        public void UpdatePages()
        {
            if (_leftButton == null || _rightButton == null)
                throw new NullReferenceException("Page handler was not initialized!");

            foreach (var items in Pages)
                foreach (var item in items)
                    item.IsVisible = false;
            _leftButton.IsVisible = false;
            _rightButton.IsVisible = false;

            if (!IsVisible)
                return;

            int count = PageIndex * ItemsPrPage;
            foreach (var item in Pages[PageIndex])
            {
                item.IsVisible = count >= MinItem && count < MaxItem;
                count++;
            }

            if (MinItem - MaxItem != 0 && MaxPage - MinPage != 1)
            {
                _leftButton.IsVisible = true;
                _rightButton.IsVisible = true;
                if (PageIndex == MinPage)
                    _leftButton.IsEnabled = false;
                else
                    _leftButton.IsEnabled = true;
                if (PageIndex == MaxPage - 1)
                    _rightButton.IsEnabled = false;
                else
                    _rightButton.IsEnabled = true;
            }
        }
    }
}
