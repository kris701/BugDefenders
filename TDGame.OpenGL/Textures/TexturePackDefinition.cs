using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.Core.Models;

namespace TDGame.OpenGL.Textures
{
    public class TexturePackDefinition : IDefinition
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid? BasedOn { get; set; }
        public TexturesDefinition TexturesDefinition { get; set; }
    }
}
