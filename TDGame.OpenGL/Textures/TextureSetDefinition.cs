using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TDGame.OpenGL.Textures
{
    public class TextureSetDefinition
    {
        public Guid ID { get; set; }
        public int FrameTime { get; set; }
        public List<string> Contents { get; set; }

        [JsonIgnore]
        public List<Texture2D> LoadedContents { get; set; } = new List<Texture2D>();
    }
}
