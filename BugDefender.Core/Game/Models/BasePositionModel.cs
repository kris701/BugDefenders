using System.Drawing;

namespace BugDefender.Core.Game.Models
{
    public abstract class BasePositionModel : IPosition
    {
        private float _x;
        public float X
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
                CenterX = _x + Size / 2;
            }
        }
        private float _y;
        public float Y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
                CenterY = _y + Size / 2;
            }
        }
        private float _size;
        public float Size
        {
            get
            {
                return _size;
            }
            set
            {
                _size = value;
                CenterX = _x + Size / 2;
                CenterY = _y + Size / 2;
            }
        }
        public float Angle { get; set; }

        public float CenterX { get; private set; }
        public float CenterY { get; private set; }
    }
}
