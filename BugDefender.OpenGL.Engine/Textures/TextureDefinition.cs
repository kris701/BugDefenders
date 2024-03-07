﻿using Microsoft.Xna.Framework.Graphics;
using System.Text.Json.Serialization;

namespace BugDefender.OpenGL.Engine.Textures
{
    public class TextureDefinition
    {
        public Guid ID { get; set; }
        public string Content { get; set; }

        [JsonIgnore]
        public Texture2D LoadedContent { get; set; }

        public TextureDefinition(Guid iD, string content)
        {
            ID = iD;
            Content = content;
        }
    }
}