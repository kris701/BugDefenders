﻿using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

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
