using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugDefender.OpenGL.Engine
{
    public abstract class LoadableContent<T>
    {
        private T? _loadedContent;
        public T GetLoadedContent()
        {
            if (_loadedContent == null)
                throw new Exception("Content not loaded!");
            return _loadedContent;
        }
        public void SetContent(T content)
        {
            _loadedContent = content;
        }
    }
}
