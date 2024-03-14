using BugDefender.OpenGL.Engine;
using BugDefender.OpenGL.Engine.Controls;
using System;

namespace BugDefender.OpenGL.Controls
{
    public class BugDefendersTextInputControl : TextInputControl
    {
        public BugDefendersTextInputControl(IWindow parent, KeyEventHandler? onEnter = null) : base(parent, onEnter)
        {
            KeyDownSound = new Guid("2e3a4bbb-c0e5-4617-aee1-070e02e4b8ea");
            EnterSound = new Guid("2e3a4bbb-c0e5-4617-aee1-070e02e4b8ea");
        }
    }
}
