using Microsoft.Xna.Framework.Media;

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
