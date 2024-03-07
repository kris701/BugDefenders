using BugDefender.Core.Game.Helpers;
using BugDefender.OpenGL.Engine;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
                Parent.TextureController.LoadTexture(item);
            foreach (var item in pack.TextureSets)
                Parent.TextureController.LoadTextureSet(item);
            foreach (var item in pack.Songs)
                Parent.AudioController.LoadSong(item);
            foreach (var item in pack.SoundEffects)
                Parent.AudioController.LoadSoundEffect(item);
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
    }
}
