using System;
using TDGame.OpenGL.Engine.Views;

namespace TDGame.OpenGL.Screens.MainMenu
{
    public partial class MainMenuView : BaseView
    {
        private static readonly Guid _id = new Guid("9c52281c-3202-4b22-bfc9-dfc187fdbeb3");
        public MainMenuView(UIEngine parent) : base(parent, _id)
        {
            ScaleValue = parent.CurrentUser.UserData.Scale;
            Initialize();
            Parent.UIResources.PlaySong(ID);
        }
    }
}
