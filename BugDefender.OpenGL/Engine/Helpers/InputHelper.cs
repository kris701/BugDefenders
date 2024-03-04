using BugDefender.Core.Game.Models.Maps;
using Microsoft.Xna.Framework.Input;

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
