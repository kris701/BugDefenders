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
        public List<List<T>> Pages = new List<List<T>>();
        public int PageIndex { get; set; } = 0;

        public float X { get; set; } = 0;
        public float Y { get; set; } = 0;
        public int ItemsPrPage { get; set; } = 5;
        public float Margin { get; set; } = 5;
        public bool IsVisible { get; set; } = true;

        public float ButtonSize { get; set; } = 50;
        public int ButtonFontSize { get; set; } = 16;
        public float LeftButtonX { get; set; } = 0;
        public float LeftButtonY { get; set; } = 0;
        public float RightButtonX { get; set; } = 0;
        public float RightButtonY { get; set; } = 0;
        public int MinPage { get; set; } = 0;
        public int MaxPage { get; set; } = int.MaxValue;
        public int MinItem { get; set; } = 0;
        public int MaxItem { get; set; } = int.MaxValue;
        public int Layer { get; set; } = 0;

        private ButtonControl? _leftButton;
        private ButtonControl? _rightButton;

        public void Initialize(List<T> items, BaseView parent)
        {
            foreach (var page in Pages)
                foreach (var item in page)
                    parent.RemoveControl(2, item);

            Pages.Clear();
            int offset = 0;
            if (items.Count <= ItemsPrPage)
            {
                Pages.Add(new List<T>());
                foreach (var item in items)
                {
                    item.X = X;
                    item.Y = Y + offset++ * (item.Height + Margin);
                    item.IsVisible = true;
                    Pages[0].Add(item);
                    parent.AddControl(Layer + 1, item);
                }
                return;
            }

            int pageIndex = -1;
            int count = 0;
            foreach (var item in items)
            {
                if (count % ItemsPrPage == 0)
                {
                    pageIndex++;
                    Pages.Add(new List<T>());
                    offset = 0;
                }
                item.X = X;
                item.Y = Y + offset++ * (item.Height + Margin);
                item.IsVisible = pageIndex == 0;
                Pages[pageIndex].Add(item);
                parent.AddControl(Layer + 1, item);
                count++;
            }

            _leftButton = new ButtonControl(parent.Parent, clicked: (s) =>
            {
                PageIndex--;
                if (PageIndex < MinPage)
                    PageIndex = MinPage;
                UpdatePages();
            })
            {
                FillColor = parent.Parent.TextureController.GetTexture(new Guid("d86347e3-3834-4161-9bbe-0d761d1d27ae")),
                FillClickedColor = parent.Parent.TextureController.GetTexture(new Guid("2c220d3f-5e7a-44ec-b4da-459f104c1e4a")),
                FillDisabledColor = parent.Parent.TextureController.GetTexture(new Guid("2c220d3f-5e7a-44ec-b4da-459f104c1e4a")),
                FontColor = Color.White,
                Font = BasicFonts.GetFont(ButtonFontSize),
                Text = $"<",
                X = LeftButtonX,
                Y = LeftButtonY,
                Height = ButtonSize,
                Width = ButtonSize,
                IsEnabled = false,
                IsVisible = false,
            };
            parent.AddControl(Layer, _leftButton);
            _rightButton = new ButtonControl(parent.Parent, clicked: (s) =>
            {
                PageIndex++;
                if (PageIndex >= MaxPage)
                    PageIndex = MaxPage;
                UpdatePages();
            })
            {
                FillColor = parent.Parent.TextureController.GetTexture(new Guid("d86347e3-3834-4161-9bbe-0d761d1d27ae")),
                FillClickedColor = parent.Parent.TextureController.GetTexture(new Guid("2c220d3f-5e7a-44ec-b4da-459f104c1e4a")),
                FillDisabledColor = parent.Parent.TextureController.GetTexture(new Guid("2c220d3f-5e7a-44ec-b4da-459f104c1e4a")),
                FontColor = Color.White,
                Font = BasicFonts.GetFont(ButtonFontSize),
                Text = $">",
                X = RightButtonX,
                Y = RightButtonY,
                Height = ButtonSize,
                Width = ButtonSize,
                IsVisible = false
            };
            parent.AddControl(Layer, _rightButton);

            if (MaxPage == int.MaxValue)
                MaxPage = Pages.Count;
            if (MaxPage - MinPage < 0)
                throw new Exception("Impossible page limits!");
            if (PageIndex < MinPage)
                PageIndex = MaxPage;

            UpdatePages();
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
