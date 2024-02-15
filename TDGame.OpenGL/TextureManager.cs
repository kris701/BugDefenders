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
using TDGame.OpenGL.Textures;

namespace TDGame.OpenGL
{
    public static class TextureManager
    {
        public static BaseBuilder<TexturePackDefinition> TexturePacks = new BaseBuilder<TexturePackDefinition>("Textures.TexturePacks", Assembly.GetExecutingAssembly());

        private static string _noTextureName = "notexture";
        private static ContentManager _contentManager;
        private static Dictionary<Guid, Texture2D> _textures = new Dictionary<Guid, Texture2D>();

        public static void Initialize(ContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        public static List<Guid> GetTexturePacks() => TexturePacks.GetResources();

        public static void LoadTexturePack(Guid texturePack)
        {
            _textures.Clear();
            LoadPack(TexturePacks.GetResource(texturePack));
        }

        private static void LoadPack(TexturePackDefinition pack)
        {
            if (pack.BasedOn != null)
                LoadPack(TexturePacks.GetResource((Guid)pack.BasedOn));
            foreach (var item in pack.TextureDefinition.TextureSet)
                LoadTexture(item);
        }

        public static void LoadTexture(TextureDefinition item)
        {
            if (_textures.ContainsKey(item.ID))
                _textures[item.ID] = _contentManager.Load<Texture2D>(item.Content);
            else
                _textures.Add(item.ID, _contentManager.Load<Texture2D>(item.Content));
        }

        public static TexturePackDefinition GetTexturePack(Guid texturePack) => TexturePacks.GetResource(texturePack);

        public static Texture2D GetTexture(Guid id)
        {
            if (_textures.ContainsKey(id))
                return _textures[id];
            return _contentManager.Load<Texture2D>(_noTextureName);
        }

        public static List<Texture2D> GetTextureSet(List<Guid> ids)
        {
            var textureSet = new List<Texture2D>();
            foreach (var id in ids)
                textureSet.Add(GetTexture(id));
            return textureSet;
        }
    }
}
