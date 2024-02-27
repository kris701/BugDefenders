using System;
using BugDefender.OpenGL.Engine.Views;

namespace BugDefender.OpenGL.Screens.MainMenu
{
    public partial class MainMenuView : BaseView
    {
        public static readonly Guid _id = new Guid("9c52281c-3202-4b22-bfc9-dfc187fdbeb3");
        public MainMenuView(UIEngine parent) : base(parent, _id)
        {
            ScaleValue = parent.CurrentUser.UserData.Scale;
            Initialize();
            Parent.UIResources.PlaySong(ID);
        }
    }
}
