using Microsoft.Xna.Framework.Input;

namespace BugDefender.OpenGL.Engine.Input
{
    public class KeysWatcher
    {
        public List<Keys> Keys { get; set; }
        private bool _isDown = false;
        private readonly Action? _pressAction;
        private readonly Action? _unpressAction;

        public KeysWatcher(List<Keys> keys, Action? pressAction = null, Action? unpresAction = null)
        {
            Keys = keys;
            _pressAction = pressAction;
            _unpressAction = unpresAction;
        }

        public void Update(KeyboardState state)
        {
            if (!_isDown && Keys.All(state.IsKeyDown))
            {
                if (_pressAction != null)
                    _pressAction.Invoke();
                _isDown = true;
            }
            else if (_isDown && Keys.Any(state.IsKeyUp))
            {
                if (_unpressAction != null)
                    _unpressAction.Invoke();
                _isDown = false;
            }
        }
    }
}
