using System;
using System.Collections.Generic;
using TDGame.Core.Game.Models;
using TDGame.OpenGL.ResourcePacks;

namespace TDGame.OpenGL.Textures
{
    public class ResourcePackDefinition : IDefinition
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid? BasedOn { get; set; }
        public List<TextureDefinition> Textures { get; set; }
        public List<TextureSetDefinition> TextureSets { get; set; }
        public List<SongDefinition> Songs { get; set; }
        public List<SoundEffectDefinition> SoundEffects { get; set; }
        public List<IEntityResource> AnimationsEntities { get; set; }
        public List<IEntityResource> SoundEffectEntities { get; set; }
    }
}
