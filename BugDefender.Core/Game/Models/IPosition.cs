namespace BugDefender.Core.Game.Models
{
    public interface IPosition
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Size { get; set; }
        public float Angle { get; set; }

        public float CenterX { get; }
        public float CenterY { get; }
    }
}
