using BugDefender.OpenGL.Engine;
using BugDefender.OpenGL.Engine.Controls;
using System;

namespace BugDefender.OpenGL.Controls
{
    public class BugDefenderButtonControl : ButtonControl
    {
        public BugDefenderButtonControl(IWindow parent, ClickedHandler? clicked = null) : base(parent, clicked)
        {
            ClickSound = new Guid("2e3a4bbb-c0e5-4617-aee1-070e02e4b8ea");
        }
    }
}
