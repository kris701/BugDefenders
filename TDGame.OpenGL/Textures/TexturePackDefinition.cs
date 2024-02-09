using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDGame.OpenGL.Textures
{
    public class TexturePackDefinition
    {
        public List<TextureDefinition> TextureSet { get; set; }

        public TexturePackDefinition(List<TextureDefinition> textureSet)
        {
            TextureSet = textureSet;
        }
    }
}
