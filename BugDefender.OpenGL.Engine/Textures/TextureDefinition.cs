using Microsoft.Xna.Framework.Graphics;
using System.Text.Json.Serialization;

namespace BugDefender.OpenGL.Engine.Textures
{
    public class TextureDefinition : LoadableContent<Texture2D>
    {
        public Guid ID { get; set; }
        public string Content { get; set; }

        public TextureDefinition(Guid iD, string content)
        {
            ID = iD;
            Content = content;
        }
    }
}
