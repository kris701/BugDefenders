using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Reflection;
using TDGame.Core.Game.Helpers;
using TDGame.OpenGL.ResourcePacks;

namespace TDGame.OpenGL
{
    public class UIResourceManager
    {
        public BaseBuilder<ResourcePackDefinition> TexturePacks = new BaseBuilder<ResourcePackDefinition>("ResourcePacks.ResourcePacks", Assembly.GetExecutingAssembly());

        private readonly string _noTextureName = "notexture";
        private readonly Texture2D _noTexture;
        private readonly TextureSetDefinition _noTextureSet;
        private readonly Dictionary<Guid, SoundEffectInstance> _instances = new Dictionary<Guid, SoundEffectInstance>();
        private string _playing = "";
        private readonly ContentManager _contentManager;
        private readonly Dictionary<Guid, List<IEntityResource>> _animationEntities = new Dictionary<Guid, List<IEntityResource>>();
        private readonly Dictionary<Guid, List<IEntityResource>> _soundEffectEntities = new Dictionary<Guid, List<IEntityResource>>();
        private readonly Dictionary<Guid, TextureDefinition> _textures = new Dictionary<Guid, TextureDefinition>();
        private readonly Dictionary<Guid, TextureSetDefinition> _textureSets = new Dictionary<Guid, TextureSetDefinition>();
        private readonly Dictionary<Guid, SongDefinition> _songs = new Dictionary<Guid, SongDefinition>();
        private readonly Dictionary<Guid, SoundEffectDefinition> _soundEffects = new Dictionary<Guid, SoundEffectDefinition>();

        public UIResourceManager(ContentManager contentManager)
        {
            _contentManager = contentManager;
            _noTexture = _contentManager.Load<Texture2D>(_noTextureName);
            _noTextureSet = new TextureSetDefinition(Guid.Empty, 1000, new List<string>())
            {
                LoadedContents = new List<Texture2D>()
                {
                    _noTexture
                }
            };
        }

        public List<Guid> GetTexturePacks() => TexturePacks.GetResources();

        public void LoadTexturePack(Guid texturePack)
        {
            _textures.Clear();
            LoadTexturePack(TexturePacks.GetResource(texturePack));
        }

        private void LoadTexturePack(ResourcePackDefinition pack)
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

        public void LoadAnimationEntity(IEntityResource item)
        {
            if (_animationEntities.ContainsKey(item.Target))
                _animationEntities[item.Target].Add(item);
            else
                _animationEntities.Add(item.Target, new List<IEntityResource>() { item });
        }

        public void LoadSoundEffectEntity(IEntityResource item)
        {
            if (_soundEffectEntities.ContainsKey(item.Target))
                _soundEffectEntities[item.Target].Add(item);
            else
                _soundEffectEntities.Add(item.Target, new List<IEntityResource>() { item });
        }

        public void LoadTexture(TextureDefinition item)
        {
            if (_textures.ContainsKey(item.ID))
                _textures[item.ID].LoadedContent = _contentManager.Load<Texture2D>(item.Content);
            else
            {
                item.LoadedContent = _contentManager.Load<Texture2D>(item.Content);
                _textures.Add(item.ID, item);
            }
        }

        public void LoadTextureSet(TextureSetDefinition item)
        {
            if (_textureSets.ContainsKey(item.ID))
                _textureSets.Remove(item.ID);
            _textureSets.Add(item.ID, item);
            foreach (var content in item.Contents)
                _textureSets[item.ID].LoadedContents.Add(_contentManager.Load<Texture2D>(content));
        }

        public void LoadSong(SongDefinition item)
        {
            if (_songs.ContainsKey(item.ID))
                _songs.Remove(item.ID);
            item.LoadedContent = _contentManager.Load<Song>(item.Content);
            _songs.Add(item.ID, item);
        }

        public void LoadSoundEffect(SoundEffectDefinition item)
        {
            if (_soundEffects.ContainsKey(item.ID))
                _soundEffects.Remove(item.ID);
            item.LoadedContent = _contentManager.Load<SoundEffect>(item.Content);
            _soundEffects.Add(item.ID, item);
        }

        public ResourcePackDefinition GetTexturePack(Guid texturePack) => TexturePacks.GetResource(texturePack);

        public Texture2D GetTexture(Guid id)
        {
            if (_textures.ContainsKey(id))
                return _textures[id].LoadedContent;
            return _noTexture;
        }

        public TextureSetDefinition GetTextureSet(Guid id)
        {
            if (_textureSets.ContainsKey(id))
                return _textureSets[id];
            if (_textures.ContainsKey(id))
            {
                var newSet = new TextureSetDefinition(id, 0, new List<string>())
                {
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

        public T GetAnimation<T>(Guid id) where T : IEntityResource
        {
            if (_animationEntities.ContainsKey(id))
            {
                var target = _animationEntities[id];
                foreach (var item in target)
                    if (item is T)
                        return (T)item;
            }
            throw new Exception("Animation not found!");
        }

        public T GetSoundEffects<T>(Guid id) where T : IEntityResource
        {
            if (_soundEffectEntities.ContainsKey(id))
            {
                var target = _soundEffectEntities[id];
                foreach (var item in target)
                    if (item is T)
                        return (T)item;
            }
            throw new Exception("Sound effect not found!");
        }

        public void PlaySong(Guid id)
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

        public Guid PlaySoundEffect(Guid id)
        {
            if (!_soundEffects.ContainsKey(id))
                return Guid.Empty;
            var newEffect = Guid.NewGuid();
            _instances.Add(newEffect, _soundEffects[id].LoadedContent.CreateInstance());
            _instances[newEffect].Volume = SoundEffect.MasterVolume;
            _instances[newEffect].IsLooped = true;
            _instances[newEffect].Play();
            return newEffect;
        }

        public void StopSoundEffect(Guid id)
        {
            if (!_instances.ContainsKey(id))
                return;
            _instances[id].Stop();
            _instances.Remove(id);
        }

        public void PauseSounds()
        {
            foreach (var key in _instances.Keys)
                _instances[key].Pause();
            MediaPlayer.Pause();
        }

        public void ResumeSounds()
        {
            foreach (var key in _instances.Keys)
                _instances[key].Resume();
            MediaPlayer.Resume();
        }

        public void StopSounds()
        {
            foreach (var key in _instances.Keys)
                _instances[key].Stop();
            _instances.Clear();
            MediaPlayer.Stop();
            _playing = "";
        }
    }
}
