using Microsoft.Xna.Framework.Graphics;
using System;
using System.Text.Json.Serialization;

namespace TDGame.OpenGL.Textures
{
    public class TextureDefinition
    {
        public Guid ID { get; set; }
        public string Content { get; set; }

        [JsonIgnore]
        public Texture2D LoadedContent { get; set; }
    }
}
