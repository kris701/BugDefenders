using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TDGame.Core.Helpers;

namespace TDGame.OpenGL.Textures
{
    public static class TextureBuilder
    {
        private static ContentManager? _contentManager;
        private static Dictionary<Guid, Texture2D> _textures = new Dictionary<Guid, Texture2D>();
        private static BaseBuilder<TexturePackDefinition> _resourceFetcher = new BaseBuilder<TexturePackDefinition>("Textures.TexturePacks", Assembly.GetExecutingAssembly());

        public static void Initialize(ContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        public static List<string> GetTexturePacks() => _resourceFetcher.GetResources();

        public static void LoadTexturePack(string texturePack)
        {
            var pack = _resourceFetcher.GetResource(texturePack);
            foreach (var item in pack.TextureSet)
                _textures.Add(item.ID, _contentManager.Load<Texture2D>(item.Content));
        }

        public static Texture2D GetTexture(Guid id)
        {
            if (_textures.ContainsKey(id)) 
                return _textures[id];
            throw new Exception("Texture not found in texture pack!");
        }
    }
}
