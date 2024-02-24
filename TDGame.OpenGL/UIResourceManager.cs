using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Reflection;
using TDGame.Core.Game.Helpers;
using TDGame.OpenGL.ResourcePacks;
using TDGame.OpenGL.Textures;

namespace TDGame.OpenGL
{
    public static class UIResourceManager
    {
        public static BaseBuilder<ResourcePackDefinition> TexturePacks = new BaseBuilder<ResourcePackDefinition>("ResourcePacks.ResourcePacks", Assembly.GetExecutingAssembly());

        private static string _noTextureName = "notexture";
        private static Texture2D _noTexture;
        private static TextureSetDefinition _noTextureSet;

        private static ContentManager _contentManager;
        private static Dictionary<Guid, IEntityResource> _animationEntities = new Dictionary<Guid, IEntityResource>();
        private static Dictionary<Guid, IEntityResource> _soundEffectEntities = new Dictionary<Guid, IEntityResource>();
        private static Dictionary<Guid, TextureDefinition> _textures = new Dictionary<Guid, TextureDefinition>();
        private static Dictionary<Guid, TextureSetDefinition> _textureSets = new Dictionary<Guid, TextureSetDefinition>();
        private static Dictionary<Guid, SongDefinition> _songs = new Dictionary<Guid, SongDefinition>();
        private static Dictionary<Guid, SoundEffectDefinition> _soundEffects = new Dictionary<Guid, SoundEffectDefinition>();

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

        private static void LoadTexturePack(ResourcePackDefinition pack)
        {
            if (pack.BasedOn != null)
                LoadTexturePack(TexturePacks.GetResource((Guid)pack.BasedOn));
            foreach (var item in pack.Textures)
                LoadTexture(item);
            foreach (var item in pack.TextureSets)
                LoadTextureSet(item);
            foreach (var item in pack.Songs)
                LoadSong(item);
            foreach (var item in pack.SoundEffects)
                LoadSoundEffect(item);
            foreach (var item in pack.AnimationsEntities)
                LoadAnimationEntity(item);
            foreach (var item in pack.SoundEffectEntities)
                LoadSoundEffectEntity(item);
        }

        public static void LoadAnimationEntity(IEntityResource item)
        {
            if (_animationEntities.ContainsKey(item.Target))
                _animationEntities[item.Target] = item;
            else
                _animationEntities.Add(item.Target, item);
        }

        public static void LoadSoundEffectEntity(IEntityResource item)
        {
            if (_soundEffectEntities.ContainsKey(item.Target))
                _soundEffectEntities[item.Target] = item;
            else
                _soundEffectEntities.Add(item.Target, item);
        }

        public static void LoadTexture(TextureDefinition item)
        {
            if (_textures.ContainsKey(item.ID))
                _textures[item.ID].LoadedContent = _contentManager.Load<Texture2D>(item.Content);
            else
            {
                item.LoadedContent = _contentManager.Load<Texture2D>(item.Content);
                _textures.Add(item.ID, item);
            }
        }

        public static void LoadTextureSet(TextureSetDefinition item)
        {
            if (_textureSets.ContainsKey(item.ID))
                _textureSets.Remove(item.ID);
            _textureSets.Add(item.ID, item);
            foreach (var content in item.Contents)
                _textureSets[item.ID].LoadedContents.Add(_contentManager.Load<Texture2D>(content));
        }

        public static void LoadSong(SongDefinition item)
        {
            if (_songs.ContainsKey(item.ID))
                _songs.Remove(item.ID);
            item.LoadedContent = _contentManager.Load<Song>(item.Content);
            _songs.Add(item.ID, item);
        }

        public static void LoadSoundEffect(SoundEffectDefinition item)
        {
            if (_soundEffects.ContainsKey(item.ID))
                _soundEffects.Remove(item.ID);
            item.LoadedContent = _contentManager.Load<SoundEffect>(item.Content);
            _soundEffects.Add(item.ID, item);
        }

        public static ResourcePackDefinition GetTexturePack(Guid texturePack) => TexturePacks.GetResource(texturePack);

        public static Texture2D GetTexture(Guid id)
        {
            if (_textures.ContainsKey(id))
                return _textures[id].LoadedContent;
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
                        _textures[id].LoadedContent
                    }
                };
                _textureSets.Add(id, newSet);
                return newSet;
            }
            return _noTextureSet;
        }

        public static T GetAnimation<T>(Guid id) where T : IEntityResource
        {
            if (_animationEntities.ContainsKey(id))
            {
                var target = _animationEntities[id];
                if (target is T)
                    return (T)target;
            }
            throw new Exception("Animation not found!");
        }

        private static string _playing = "";
        public static void PlaySong(Guid id)
        {
            if (!_songs.ContainsKey(id))
                return;
            var song = _songs[id];
            if (song.Content == _playing)
                return;
            _playing = song.Content;
            MediaPlayer.Stop();
            MediaPlayer.Play(song.LoadedContent);
        }

        public static void PlaySoundEffect(Guid id)
        {
            if (_soundEffects.ContainsKey(id))
                return;
            _soundEffects[id].LoadedContent.Play();
        }
    }
}
