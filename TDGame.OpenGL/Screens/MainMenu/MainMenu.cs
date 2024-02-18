using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TDGame.Core.Users.Models;
using TDGame.OpenGL.Engine.Screens;
using TDGame.OpenGL.Settings;

namespace TDGame.OpenGL.Screens.MainMenu
{
    public partial class MainMenu : BaseScreen
    {
        public MainMenu(UIEngine parent) : base(parent)
        {
            ScaleValue = parent.CurrentUser.UserData.Scale;
            Initialize();
        }
    }
}
