using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDGame.OpenGL.Helpers
{
    public static class GameScreenHelper
    {
        public static Texture2D TakeScreenCap(GraphicsDevice device, Game game)
        {
            int w = device.PresentationParameters.BackBufferWidth;
            int h = device.PresentationParameters.BackBufferHeight;

            //force a frame to be drawn (otherwise back buffer is empty) 
            game.RunOneFrame();
            //game.Draw(new GameTime());

            //pull the picture from the buffer 
            int[] backBuffer = new int[w * h];
            device.GetBackBufferData(backBuffer);

            //copy into a texture 
            Texture2D texture = new Texture2D(device, w, h, false, device.PresentationParameters.BackBufferFormat);
            texture.SetData(backBuffer);
            return texture;
        }
    }
}
