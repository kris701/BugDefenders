using MonoGame.FormEngine.Core.Screens;
using MonoGame.FormEngine.Toolbox.Controls;
using MonoGame.FormEngine.Toolbox.Helpers;
using MonoGame.FormEngine.Toolbox.Interfaces;
using System.Collections.Generic;

namespace Project1.Screens.PathTest
{
    public partial class PathTestScreen : BaseScreen
    {
        public override void Initialize()
        {
            _moveTest = new PanelControl()
            {
                FillColor = BasicTextures.Black,
                Width = 10,
                Height = 10
            };
            _canvas = new CanvasControl() {
                Children = new List<IControl>()
                {
                    _moveTest
                }
            };
            Container = _canvas;
            base.Initialize();
        }
    }
}
