using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System.Text.Json.Serialization;

namespace BugDefender.OpenGL.Engine.Audio
{
    public class SongDefinition : LoadableContent<Song>
    {
        public Guid ID { get; set; }
        public string Content { get; set; }

        public SongDefinition(Guid iD, string content, bool isDefered) : base(isDefered)
        {
            ID = iD;
            Content = content;
        }

        public override Song LoadMethod(ContentManager manager) => manager.Load<Song>(Content);
    }
}
