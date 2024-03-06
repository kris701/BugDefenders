using Microsoft.Xna.Framework.Audio;
using System;
using System.Text.Json.Serialization;

namespace BugDefender.OpenGL.ResourcePacks
{
    public class SoundEffectDefinition
    {
        public Guid ID { get; set; }
        public string Content { get; set; }

        [JsonIgnore]
        public SoundEffect LoadedContent { get; set; }

        public SoundEffectDefinition(Guid iD, string content)
        {
            ID = iD;
            Content = content;
        }
    }
}
