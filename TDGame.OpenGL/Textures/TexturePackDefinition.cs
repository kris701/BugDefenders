﻿using System;
using System.Collections.Generic;
using TDGame.Core.Game.Models;

namespace TDGame.OpenGL.Textures
{
    public class TexturePackDefinition : IDefinition
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid? BasedOn { get; set; }
        public TexturesDefinition TexturesDefinition { get; set; }
        public List<IEntityResource> AnimationsDefinition { get; set; }
        public List<IEntityResource> AudioDefinition { get; set; }
    }
}
