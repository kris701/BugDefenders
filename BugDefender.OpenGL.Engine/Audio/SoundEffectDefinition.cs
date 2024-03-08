using Microsoft.Xna.Framework.Audio;

namespace BugDefender.OpenGL.Engine.Audio
{
    public class SoundEffectDefinition : LoadableContent<SoundEffect>
    {
        public Guid ID { get; set; }
        public string Content { get; set; }

        public SoundEffectDefinition(Guid iD, string content)
        {
            ID = iD;
            Content = content;
        }
    }
}
