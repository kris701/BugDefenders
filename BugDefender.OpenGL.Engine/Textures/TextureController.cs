using BugDefender.OpenGL.Engine.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BugDefender.OpenGL.Engine.Textures
{
    public class TextureController
    {
        private readonly ContentManager _contentManager;
        private readonly Dictionary<Guid, TextureDefinition> _textures = new Dictionary<Guid, TextureDefinition>();
        private readonly Dictionary<Guid, TextureSetDefinition> _textureSets = new Dictionary<Guid, TextureSetDefinition>();
        private Texture2D? _noTexture;
        private TextureSetDefinition? _noTextureSet;

        public TextureController(ContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        public void LoadTexture(TextureDefinition item)
        {
            if (_textures.ContainsKey(item.ID))
                _textures[item.ID].SetContent(_contentManager.Load<Texture2D>(item.Content));
            else
            {
                item.SetContent(_contentManager.Load<Texture2D>(item.Content));
                _textures.Add(item.ID, item);
            }
        }

        public void LoadTextureSet(TextureSetDefinition item)
        {
            if (_textureSets.ContainsKey(item.ID))
                _textureSets.Remove(item.ID);
            _textureSets.Add(item.ID, item);
            var textureSet = new List<Texture2D>();
            foreach (var content in item.Contents)
                textureSet.Add(_contentManager.Load<Texture2D>(content));
            _textureSets[item.ID].SetContent(textureSet);
        }

        public Texture2D GetTexture(Guid id)
        {
            if (_textures.ContainsKey(id))
                return _textures[id].GetLoadedContent();
            if (_noTexture == null)
                _noTexture = BasicTextures.GetBasicRectange(Color.HotPink);
            return _noTexture;
        }

        public TextureSetDefinition GetTextureSet(Guid id)
        {
            if (_textureSets.ContainsKey(id))
                return _textureSets[id];
            if (_textures.ContainsKey(id))
            {
                var newSet = new TextureSetDefinition(id, 0, new List<string>());
                newSet.SetContent(new List<Texture2D>() { _textures[id].GetLoadedContent() });
                _textureSets.Add(id, newSet);
                return newSet;
            }

            if (_noTextureSet == null)
            {
                if (_noTexture == null)
                    _noTexture = BasicTextures.GetBasicRectange(Color.HotPink);
                _noTextureSet = new TextureSetDefinition(Guid.Empty, 9999, new List<string>());
                _noTextureSet.SetContent(new List<Texture2D>() { _noTexture });
            }
            return _noTextureSet;
        }
    }
}
