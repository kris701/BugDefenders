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
        private static Texture2D _noTexture;
        private static ContentManager _contentManager;
        private static Dictionary<Guid, Texture2D> _textures = new Dictionary<Guid, Texture2D>();
        private static Dictionary<Guid, List<Texture2D>> _textureSets = new Dictionary<Guid, List<Texture2D>>();

        public static void Initialize(ContentManager contentManager)
        {
            _contentManager = contentManager;
            _noTexture = _contentManager.Load<Texture2D>(_noTextureName);
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
            foreach (var item in pack.TexturesDefinition.Textures)
                LoadTexture(item);
            foreach (var item in pack.TexturesDefinition.TextureSets)
                LoadTexture(item);
        }

        public static void LoadTexture(TextureDefinition item)
        {
            if (_textures.ContainsKey(item.ID))
                _textures[item.ID] = _contentManager.Load<Texture2D>(item.Content);
            else
                _textures.Add(item.ID, _contentManager.Load<Texture2D>(item.Content));
        }

        public static void LoadTexture(TextureSetDefinition item)
        {
            if (_textureSets.ContainsKey(item.ID))
                _textureSets[item.ID].Clear();
            else
                _textureSets.Add(item.ID, new List<Texture2D>());
            foreach (var content in item.Contents)
                _textureSets[item.ID].Add(_contentManager.Load<Texture2D>(content));
        }

        public static TexturePackDefinition GetTexturePack(Guid texturePack) => TexturePacks.GetResource(texturePack);

        public static Texture2D GetTexture(Guid id)
        {
            if (_textures.ContainsKey(id))
                return _textures[id];
            return _noTexture;
        }

        public static List<Texture2D> GetTextureSet(Guid id)
        {
            if (_textureSets.ContainsKey(id))
                return _textureSets[id];
            if (_textures.ContainsKey(id))
                return new List<Texture2D>() { _textures[id] };
            return new List<Texture2D>() { _noTexture };
        }
    }
}
