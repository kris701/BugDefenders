using System.Collections.Generic;
using TDGame.OpenGL.Engine;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Screens;

namespace Project1.Screens.PathTest
{
    public partial class PathTestScreen : BaseScreen
    {
        public override void Initialize()
        {
            _canvas = new CanvasControl() {
                Children = new List<IControl>()
                {
                }
            };
            Container = _canvas;
            base.Initialize();
        }
    }
}
