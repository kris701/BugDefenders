using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDGame.Core.Helpers
{
    public class GameTimer
    {
        private TimeSpan Target { get; }
        private TimeSpan _last = TimeSpan.Zero;
        private Action _func;

        public GameTimer(TimeSpan target, Action func)
        {
            Target = target;
            _func = func;
        }

        public void Update(TimeSpan passed)
        {
            _last = _last.Add(passed);
            if (_last > Target)
            {
                _func.Invoke();
                _last = TimeSpan.Zero;
            }
        }
    }
}
