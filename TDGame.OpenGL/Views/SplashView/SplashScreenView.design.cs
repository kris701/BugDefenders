﻿using System;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Screens;

namespace TDGame.OpenGL.Screens.SplashScreen
{
    public partial class SplashScreenView : BaseScreen
    {
        public override void Initialize()
        {
            AddControl(0, new TileControl(Parent)
            {
                FillColor = Parent.UIResources.GetTexture(new Guid("a23f20ed-299a-4f94-b398-3dd00ff63bd5")),
                Width = 1000,
                Height = 1000
            });
            base.Initialize();
        }
    }
}