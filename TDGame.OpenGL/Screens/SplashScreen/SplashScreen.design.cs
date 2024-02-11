using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.OpenGL.Engine;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Engine.Screens;
using TDGame.OpenGL.Screens.GameSetupView;
using TDGame.OpenGL.Textures;

namespace TDGame.OpenGL.Screens.SplashScreen
{
    public partial class SplashScreen : BaseScreen
    {
        public override void Initialize()
        {
            AddControl(0, new TileControl(this)
            {
                FillColor = TextureBuilder.GetTexture(new Guid("a23f20ed-299a-4f94-b398-3dd00ff63bd5")),
                Width = 1000,
                Height = 1000
            });
            base.Initialize();
        }
    }
}
