using Microsoft.Xna.Framework.Graphics;

namespace BugDefender.OpenGL.Engine.Textures
{
    public class TextureSetDefinition : LoadableContent<List<Texture2D>>
    {
        public Guid ID { get; set; }
        public int FrameTime { get; set; }
        public List<string> Contents { get; set; }

        public TextureSetDefinition(Guid iD, int frameTime, List<string> contents)
        {
            ID = iD;
            FrameTime = frameTime;
            Contents = contents;
        }
    }
}
