using BugDefender.Core.Game.Helpers;
using BugDefender.Core.Resources;
using MonoGame.OpenGL.Formatter.Audio;
using MonoGame.OpenGL.Formatter.Fonts;
using MonoGame.OpenGL.Formatter.Textures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace BugDefender.OpenGL.ResourcePacks
{
    public class ResourcePackController
    {
        public BugDefenderGameWindow Parent { get; }
        public BaseBuilder<ResourcePackDefinition> ResourcePacks = new BaseBuilder<ResourcePackDefinition>("ResourcePacks.ResourcePacks", Assembly.GetExecutingAssembly());
        private readonly Dictionary<Guid, List<IEntityResource>> _animationEntities = new Dictionary<Guid, List<IEntityResource>>();
        private readonly Dictionary<Guid, List<IEntityResource>> _soundEffectEntities = new Dictionary<Guid, List<IEntityResource>>();

        public ResourcePackController(BugDefenderGameWindow parent)
        {
            Parent = parent;
        }

        public List<Guid> GetResourcePacks() => ResourcePacks.GetResources();
        public ResourcePackDefinition GetResourcePack(Guid id) => ResourcePacks.GetResource(id);

        public void LoadResourcePack(Guid texturePack)
        {
            LoadResourcePack(ResourcePacks.GetResource(texturePack));
        }

        private void LoadResourcePack(ResourcePackDefinition pack)
        {
            if (pack.BasedOn != null)
                LoadResourcePack(ResourcePacks.GetResource((Guid)pack.BasedOn));
            foreach (var item in pack.Textures)
                Parent.Textures.LoadTexture(item);
            foreach (var item in pack.TextureSets)
                Parent.Textures.LoadTextureSet(item);
            foreach (var item in pack.Songs)
                Parent.Audio.LoadSong(item);
            foreach (var item in pack.SoundEffects)
                Parent.Audio.LoadSoundEffect(item);
            foreach (var item in pack.Fonts)
                Parent.Fonts.LoadFont(item);
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

        public bool HasSoundEffects<T>(Guid id) where T : IEntityResource
        {
            if (_soundEffectEntities.ContainsKey(id))
            {
                var target = _soundEffectEntities[id];
                foreach (var item in target)
                    if (item is T)
                        return true;
            }
            return false;
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

        public void LoadMods(string path)
        {
            foreach (var folder in new DirectoryInfo(path).GetDirectories())
            {
                // Load core resources
                ResourceManager.LoadResource(folder);

                // Load textures
                foreach (var subFolder in folder.GetDirectories())
                {
                    if (subFolder.Parent == null)
                        continue;
                    if (subFolder.Name.ToUpper() == "TEXTURES")
                    {
                        foreach (var file in subFolder.GetFiles())
                        {
                            var textureDef = JsonSerializer.Deserialize<List<TextureDefinition>>(File.ReadAllText(file.FullName));
                            if (textureDef != null)
                            {
                                foreach (var texture in textureDef)
                                {
                                    texture.Content = Path.Combine(subFolder.Parent.FullName, "Content", texture.Content);
                                    Parent.Textures.LoadTexture(texture);
                                }
                            }
                        }
                    }
                    else if (subFolder.Name.ToUpper() == "TEXTURESETS")
                    {
                        foreach (var file in subFolder.GetFiles())
                        {
                            var textureSetDef = JsonSerializer.Deserialize<List<TextureSetDefinition>>(File.ReadAllText(file.FullName));
                            if (textureSetDef != null)
                            {
                                foreach (var textureSet in textureSetDef)
                                {
                                    for (int i = 0; i < textureSet.Contents.Count; i++)
                                        textureSet.Contents[i] = Path.Combine(subFolder.Parent.FullName, "Content", textureSet.Contents[i]);
                                    foreach (var content in textureSet.Contents)
                                        Parent.Textures.LoadTextureSet(textureSet);
                                }
                            }
                        }
                    }
                    else if (subFolder.Name.ToUpper() == "SONGS")
                    {
                        foreach (var file in subFolder.GetFiles())
                        {
                            var songsDef = JsonSerializer.Deserialize<List<SongDefinition>>(File.ReadAllText(file.FullName));
                            if (songsDef != null)
                            {
                                foreach (var song in songsDef)
                                {
                                    song.Content = Path.Combine(subFolder.Parent.FullName, "Content", song.Content);
                                    Parent.Audio.LoadSong(song);
                                }
                            }
                        }
                    }
                    else if (subFolder.Name.ToUpper() == "FONTS")
                    {
                        foreach (var file in subFolder.GetFiles())
                        {
                            var fontDef = JsonSerializer.Deserialize<List<FontDefinition>>(File.ReadAllText(file.FullName));
                            if (fontDef != null)
                            {
                                foreach (var font in fontDef)
                                {
                                    font.Content = Path.Combine(subFolder.Parent.FullName, "Content", font.Content);
                                    Parent.Fonts.LoadFont(font);
                                }
                            }
                        }
                    }
                    else if (subFolder.Name.ToUpper() == "SOUNDEFFECTS")
                    {
                        foreach (var file in subFolder.GetFiles())
                        {
                            var soundEffectsDef = JsonSerializer.Deserialize<List<SoundEffectDefinition>>(File.ReadAllText(file.FullName));
                            if (soundEffectsDef != null)
                            {
                                foreach (var soundEffect in soundEffectsDef)
                                {
                                    soundEffect.Content = Path.Combine(subFolder.Parent.FullName, "Content", soundEffect.Content);
                                    Parent.Audio.LoadSoundEffect(soundEffect);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
