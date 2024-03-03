using BugDefender.Core.Game.Models.Maps;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugDefender.OpenGL.Engine.Helpers
{
    public static class InputHelper
    {
        public static FloatPoint GetRelativePosition(float scale)
        {
            var mouseState = Mouse.GetState();
            var translatedPos = new FloatPoint(mouseState.X / scale, mouseState.Y / scale);
            return translatedPos;
        }

        public static FloatPoint GetRelativePosition(float scaleX, float scaleY)
        {
            var mouseState = Mouse.GetState();
            var translatedPos = new FloatPoint(mouseState.X / scaleX, mouseState.Y / scaleY);
            return translatedPos;
        }
    }
}
