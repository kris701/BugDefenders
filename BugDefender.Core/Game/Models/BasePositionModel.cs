namespace TDGame.Core.Game.Models
{
    public abstract class BasePositionModel : IPosition
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Size { get; set; }
        public float Angle { get; set; }

        public float CenterX => X + Size / 2;
        public float CenterY => Y + Size / 2;
    }
}
