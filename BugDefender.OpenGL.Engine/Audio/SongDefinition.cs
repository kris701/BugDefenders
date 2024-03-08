using Microsoft.Xna.Framework.Media;
using System.Text.Json.Serialization;

namespace BugDefender.OpenGL.Engine.Audio
{
    public class SongDefinition : LoadableContent<Song>
    {
        public Guid ID { get; set; }
        public string Content { get; set; }

        public SongDefinition(Guid iD, string content)
        {
            ID = iD;
            Content = content;
        }
    }
}
