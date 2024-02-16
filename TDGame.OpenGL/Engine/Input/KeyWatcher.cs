using Microsoft.Xna.Framework.Input;
using System;

namespace TDGame.OpenGL.Engine.Input
{
    public class KeyWatcher
    {
        public Keys Key { get; set; }
        private bool _isDown = false;
        private Action? _pressAction;
        private Action? _unpressAction;

        public KeyWatcher(Keys key, Action? pressAction = null, Action? unpresAction = null)
        {
            Key = key;
            _pressAction = pressAction;
            _unpressAction = unpresAction;
        }

        public void Update(KeyboardState state)
        {
            if (!_isDown && state.IsKeyDown(Key))
            {
                if (_pressAction != null)
                    _pressAction.Invoke();
                _isDown = true;
            }
            else if (_isDown && state.IsKeyUp(Key))
            {
                if (_unpressAction != null)
                    _unpressAction.Invoke();
                _isDown = false;
            }
        }
    }
}
