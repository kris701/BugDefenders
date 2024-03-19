using Microsoft.Xna.Framework.Content;

namespace BugDefender.OpenGL.Engine
{
    public abstract class LoadableContent<T>
    {
        public bool IsDefered { get; set; } = false;

        private ContentManager? _manager;
        private T? _loadedContent;

        protected LoadableContent(bool isDefered)
        {
            IsDefered = isDefered;
        }

        public T GetLoadedContent()
        {
            if (_loadedContent == null)
            {
                if (IsDefered && _manager != null)
                    _loadedContent = LoadMethod(_manager);
                else
                    throw new Exception("Content not loaded!");
            }
            return _loadedContent;
        }

        public void LoadContent(ContentManager manager)
        {
            if (!IsDefered)
                _loadedContent = LoadMethod(manager);
            else
                _manager = manager;
        }
        public abstract T LoadMethod(ContentManager manager);
        public void SetContent(T content) => _loadedContent = content;
    }
}
