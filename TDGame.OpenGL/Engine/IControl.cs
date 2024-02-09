using System.Collections.Generic;

namespace TDGame.OpenGL.Engine
{
    public delegate void UIEvent();
    public interface IControl : IRenderable
    {
        public event UIEvent UIChanged;
        public string Name { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsVisible { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public int RowSpan { get; set; }
        public int ColumnSpan { get; set; }

        public List<IControl> GetLookupStack();
    }
}
