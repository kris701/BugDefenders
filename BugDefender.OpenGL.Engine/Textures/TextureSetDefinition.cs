using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Text.Json.Serialization;

namespace BugDefender.OpenGL.Engine.Textures
{
    public class TextureSetDefinition : LoadableContent<List<Texture2D>>
    {
        public Guid ID { get; set; }
        public int FrameTime { get; set; }
        public List<string> Contents { get; set; }

        public TextureSetDefinition(Guid iD, int frameTime, List<string> contents, bool isDefered) : base(isDefered)
        {
            ID = iD;
            FrameTime = frameTime;
            Contents = contents;
        }

        public override List<Texture2D> LoadMethod(ContentManager manager)
        {
            var returnList = new List<Texture2D>();
            foreach (var content in Contents)
                returnList.Add(manager.Load<Texture2D>(content));
            return returnList;
        }
    }
}
