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
        private static TextureSetDefinition _noTextureSet;

        private static ContentManager _contentManager;
        private static Dictionary<Guid, IAnimationDefinition> _animations = new Dictionary<Guid, IAnimationDefinition>();
        private static Dictionary<Guid, Texture2D> _textures = new Dictionary<Guid, Texture2D>();
        private static Dictionary<Guid, TextureSetDefinition> _textureSets = new Dictionary<Guid, TextureSetDefinition>();

        public static void Initialize(ContentManager contentManager)
        {
            _contentManager = contentManager;
            _noTexture = _contentManager.Load<Texture2D>(_noTextureName);
            _noTextureSet = new TextureSetDefinition()
            {
                ID = Guid.Empty,
                FrameTime = 1000,
                LoadedContents = new List<Texture2D>()
                {
                    _noTexture
                }
            };
        }

        public static List<Guid> GetTexturePacks() => TexturePacks.GetResources();

        public static void LoadTexturePack(Guid texturePack)
        {
            _textures.Clear();
            LoadTexturePack(TexturePacks.GetResource(texturePack));
        }

        private static void LoadTexturePack(TexturePackDefinition pack)
        {
            if (pack.BasedOn != null)
                LoadTexturePack(TexturePacks.GetResource((Guid)pack.BasedOn));
            foreach (var item in pack.TexturesDefinition.Textures)
                LoadTexture(item);
            foreach (var item in pack.TexturesDefinition.TextureSets)
                LoadTexture(item);
            foreach (var item in pack.AnimationsDefinition)
                LoadAnimation(item);
        }

        public static void LoadAnimation(IAnimationDefinition item)
        {
            if (_animations.ContainsKey(item.Target))
                _animations[item.Target] = item;
            else
                _animations.Add(item.Target, item);
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
                _textureSets.Remove(item.ID);
            _textureSets.Add(item.ID, item);
            foreach (var content in item.Contents)
                _textureSets[item.ID].LoadedContents.Add(_contentManager.Load<Texture2D>(content));
        }

        public static TexturePackDefinition GetTexturePack(Guid texturePack) => TexturePacks.GetResource(texturePack);

        public static Texture2D GetTexture(Guid id)
        {
            if (_textures.ContainsKey(id))
                return _textures[id];
            return _noTexture;
        }

        public static TextureSetDefinition GetTextureSet(Guid id)
        {
            if (_textureSets.ContainsKey(id))
                return _textureSets[id];
            if (_textures.ContainsKey(id))
            {
                var newSet = new TextureSetDefinition()
                {
                    ID = id,
                    FrameTime = 0,
                    LoadedContents = new List<Texture2D>()
                    {
                        _textures[id]
                    }
                };
                _textureSets.Add(id, newSet);
                return newSet;
            }
            return _noTextureSet;
        }

        public static T GetAnimation<T>(Guid id) where T : IAnimationDefinition
        {
            if (_animations.ContainsKey(id))
            {
                var target = _animations[id];
                if (target is T)
                    return (T)target;
            }
            throw new Exception("Animation not found!");
        }
    }
}
