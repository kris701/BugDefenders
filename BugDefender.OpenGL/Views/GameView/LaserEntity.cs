using TDGame.Core.Game.Models;

namespace TDGame.OpenGL.Views.GameView
{
    public class LaserEntity
    {
        public IPosition From { get; set; }
        public IPosition To { get; set; }

        public LaserEntity(IPosition from, IPosition to)
        {
            From = from;
            To = to;
        }
    }
}
