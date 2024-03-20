using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace BugDefender.OpenGL.Engine.Audio
{
    public class SoundEffectDefinition : LoadableContent<SoundEffect>
    {
        public Guid ID { get; set; }
        public string Content { get; set; }

        public SoundEffectDefinition(Guid iD, string content, bool isDefered) : base(isDefered)
        {
            ID = iD;
            Content = content;
        }

        public override SoundEffect LoadMethod(ContentManager manager) => manager.Load<SoundEffect>(Content);
    }
}
