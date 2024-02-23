using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TDGame.OpenGL.ResourcePacks
{
    public class SongDefinition
    {
        public Guid ID { get; set; }
        public string Content { get; set; }

        [JsonIgnore]
        public Song LoadedContent { get; set; }
    }
}
