using BugDefender.OpenGL.Engine;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Helpers;
using BugDefender.OpenGL.Engine.Views;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace BugDefender.OpenGL.Views.Helpers
{
    public class PageHandler<T> where T : IControl
    {
        private readonly List<List<T>> _pages = new List<List<T>>();
        private int _pageIndex = 0;
        private ButtonControl _leftButton;
        private ButtonControl _rightButton;

        public float X { get; set; } = 0;
        public float Y { get; set; } = 0;
        public int ItemsPrPage { get; set; } = 5;
        public float Margin { get; set; } = 5;

        public float LeftButtonX { get; set; } = 0;
        public float LeftButtonY { get; set; } = 0;
        public float RightButtonX { get; set; } = 0;
        public float RightButtonY { get; set; } = 0;

        public void Initialize(List<T> items, BaseView parent)
        {
            _pages.Clear();
            int offset = 0;
            if (items.Count <= ItemsPrPage)
            {
                _pages.Add(new List<T>());
                foreach (var item in items)
                {
                    item.X = X;
                    item.Y = Y + offset++ * item.Height + Margin;
                    item.IsVisible = true;
                    parent.AddControl(2, item);
                }
                return;
            }

            int page = -1;
            int count = 0;
            foreach (var item in items)
            {
                if (count % ItemsPrPage == 0)
                {
                    page++;
                    _pages.Add(new List<T>());
                    offset = 0;
                }
                item.X = X;
                item.Y = Y + offset++ * item.Height + Margin;
                item.IsVisible = page == 0;
                _pages[page].Add(item);
                parent.AddControl(2, item);
                count++;
            }

            _leftButton = new ButtonControl(parent.Parent, clicked: (s) =>
            {
                _pageIndex--;
                if (_pageIndex < 0)
                    _pageIndex = 0;
                if (_pageIndex >= _pages.Count)
                    _pageIndex = _pages.Count - 1;
                UpdatePages();
            })
            {
                FillColor = parent.Parent.UIResources.GetTexture(new Guid("d86347e3-3834-4161-9bbe-0d761d1d27ae")),
                FillClickedColor = parent.Parent.UIResources.GetTexture(new Guid("2c220d3f-5e7a-44ec-b4da-459f104c1e4a")),
                FillDisabledColor = parent.Parent.UIResources.GetTexture(new Guid("2c220d3f-5e7a-44ec-b4da-459f104c1e4a")),
                FontColor = Color.White,
                Font = BasicFonts.GetFont(16),
                Text = $"<",
                X = LeftButtonX,
                Y = LeftButtonY,
                Height = 50,
                Width = 50,
                IsVisible = _pages.Count > 1,
                IsEnabled = false
            };
            parent.AddControl(1, _leftButton);
            _rightButton = new ButtonControl(parent.Parent, clicked: (s) =>
            {
                _pageIndex++;
                if (_pageIndex < 0)
                    _pageIndex = 0;
                if (_pageIndex >= _pages.Count)
                    _pageIndex = _pages.Count - 1;
                UpdatePages();
            })
            {
                FillColor = parent.Parent.UIResources.GetTexture(new Guid("d86347e3-3834-4161-9bbe-0d761d1d27ae")),
                FillClickedColor = parent.Parent.UIResources.GetTexture(new Guid("2c220d3f-5e7a-44ec-b4da-459f104c1e4a")),
                FillDisabledColor = parent.Parent.UIResources.GetTexture(new Guid("2c220d3f-5e7a-44ec-b4da-459f104c1e4a")),
                FontColor = Color.White,
                Font = BasicFonts.GetFont(16),
                Text = $">",
                X = RightButtonX,
                Y = RightButtonY,
                Height = 50,
                Width = 50,
                IsVisible = _pages.Count > 1
            };
            parent.AddControl(1, _rightButton);
        }

        public void UpdatePages()
        {
            foreach (var buttons in _pages)
                foreach (var control in buttons)
                    control.IsVisible = false;

            foreach (var control in _pages[_pageIndex])
                control.IsVisible = true;

            if (_pageIndex == 0)
                _leftButton.IsEnabled = false;
            else
                _leftButton.IsEnabled = true;
            if (_pageIndex == _pages.Count - 1)
                _rightButton.IsEnabled = false;
            else
                _rightButton.IsEnabled = true;
        }
    }
}
