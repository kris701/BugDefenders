using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDGame.OpenGL.Textures
{
    public class TexturePackDefinition
    {
        public string Name { get; set; }
        public List<TextureDefinition> TextureSet { get; set; }

        public TexturePackDefinition(string name, List<TextureDefinition> textureSet)
        {
            Name = name;
            TextureSet = textureSet;
        }
    }
}
