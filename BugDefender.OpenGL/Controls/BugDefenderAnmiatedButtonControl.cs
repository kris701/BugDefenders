using BugDefender.OpenGL.Engine;
using BugDefender.OpenGL.Engine.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugDefender.OpenGL.Controls
{
    public class BugDefenderAnmiatedButtonControl : AnimatedButtonControl
    {
        public BugDefenderAnmiatedButtonControl(IWindow parent, ClickedHandler? clicked = null) : base(parent, clicked)
        {
            ClickSound = new Guid("2e3a4bbb-c0e5-4617-aee1-070e02e4b8ea");
        }
    }
}
