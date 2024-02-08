using Microsoft.Xna.Framework;
using MonoGame.FormEngine.Core;
using MonoGame.FormEngine.Core.Screens;
using MonoGame.FormEngine.Toolbox.Controls;
using MonoGame.FormEngine.Toolbox.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using TDGame.Core;

namespace Project1.Screens.PathTest
{
    public partial class PathTestScreen : BaseScreen
    {
        private TDGame.Core.Game _game;

        private CanvasControl _canvas;
        private PanelControl _moveTest;

        public PathTestScreen(IEngine parent) : base(parent)
        {
            parent.OnUpdate += OnUpdate;
            _game = new TDGame.Core.Game("map1", "easy");
            Initialize();
        }

        private void OnUpdate(GameTime gameTime)
        {
            _moveTest.X = _moveTest.X + 1;
            _game.Update(gameTime.ElapsedGameTime);
        }
    }
}
