using Microsoft.Xna.Framework.Media;
using System;
using System.Text.Json.Serialization;

namespace TDGame.OpenGL.ResourcePacks
{
    public class SongDefinition
    {
        public Guid ID { get; set; }
        public string Content { get; set; }

        [JsonIgnore]
        public Song LoadedContent { get; set; }

        public SongDefinition(Guid iD, string content)
        {
            ID = iD;
            Content = content;
        }
    }
}
