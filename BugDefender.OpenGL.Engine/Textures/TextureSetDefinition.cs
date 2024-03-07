using Microsoft.Xna.Framework.Graphics;
using System.Text.Json.Serialization;

namespace BugDefender.OpenGL.Engine.Textures
{
    public class TextureSetDefinition
    {
        public Guid ID { get; set; }
        public int FrameTime { get; set; }
        public List<string> Contents { get; set; }

        [JsonIgnore]
        public List<Texture2D> LoadedContents { get; set; } = new List<Texture2D>();

        public TextureSetDefinition(Guid iD, int frameTime, List<string> contents)
        {
            ID = iD;
            FrameTime = frameTime;
            Contents = contents;
        }
    }
}
