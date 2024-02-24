using Microsoft.Xna.Framework.Media;
using System;
using TDGame.OpenGL.Engine.Screens;

namespace TDGame.OpenGL.Screens.MainMenu
{
    public partial class MainMenu : BaseScreen
    {
        private static Guid _id = new Guid("9c52281c-3202-4b22-bfc9-dfc187fdbeb3");
        public MainMenu(UIEngine parent) : base(parent, _id)
        {
            ScaleValue = parent.CurrentUser.UserData.Scale;
            Initialize();
            UIResourceManager.PlaySong(ID);
        }
    }
}
