using BugDefender.OpenGL.Engine.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BugDefender.OpenGL.Engine.Controls.Elements
{
    public class ScrollWatcherElement
    {
        public delegate void ScrollEventHandler(int oldValue, int newValue);
        public event ScrollEventHandler? ScrollChanged;

        public IWindow Parent { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Height { get; set; }
        public float Width { get; set; }
        public bool IsEnabled { get; set; } = true;

        private int _lastValue = 0;

        public ScrollWatcherElement(IWindow parent)
        {
            Parent = parent;
        }

        public void Update(GameTime gameTime)
        {
            if (IsEnabled && Parent.IsActive && ScrollChanged != null)
            {
                var mouseState = Mouse.GetState();
                var translatedPos = InputHelper.GetRelativePosition(Parent.XScale, Parent.YScale);
                if (translatedPos.X > X && translatedPos.X < X + Width &&
                    translatedPos.Y > Y && translatedPos.Y < Y + Height)
                {
                    if (mouseState.ScrollWheelValue != _lastValue)
                        ScrollChanged.Invoke(_lastValue, mouseState.ScrollWheelValue);
                }
                _lastValue = mouseState.ScrollWheelValue;
            }
        }
    }
}
