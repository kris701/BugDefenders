using System.Collections.Generic;

namespace TDGame.OpenGL.Engine.Screens
{
    public interface ILookup
    {
        public Dictionary<string, IControl> Lookup { get; }
        public T FindItem<T>(string name) where T : IControl;
        public void GenerateLookup();
    }
}
