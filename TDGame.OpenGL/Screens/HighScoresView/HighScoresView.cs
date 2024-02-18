using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using TDGame.OpenGL.Engine.Screens;

namespace TDGame.OpenGL.Screens.HighScoresView
{
    public partial class HighScoresView : BaseScreen
    {
        public HighScoresView(UIEngine parent) : base(parent)
        {
            ScaleValue = parent.CurrentUser.UserData.Scale;
            Initialize();
        }
    }
}
