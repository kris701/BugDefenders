using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BugDefender.OpenGL.Engine.Textures
{
    public class TextureDefinition : LoadableContent<Texture2D>
    {
        public Guid ID { get; set; }
        public string Content { get; set; }

        public TextureDefinition(Guid iD, string content, bool isDefered) : base(isDefered)
        {
            ID = iD;
            Content = content;
        }

        public override Texture2D LoadMethod(ContentManager manager) => manager.Load<Texture2D>(Content);
    }
}
