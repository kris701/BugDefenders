namespace BugDefender.Core.Game.Helpers
{
    public class GameTimer
    {
        public TimeSpan Target { get; }
        private TimeSpan _last = TimeSpan.Zero;
        private readonly Action<TimeSpan> _func;

        public GameTimer(TimeSpan target, Action<TimeSpan> func)
        {
            Target = target;
            _func = func;
        }

        public void Update(TimeSpan passed)
        {
            _last += passed;
            if (_last > Target)
            {
                _func.Invoke(_last);
                _last = TimeSpan.Zero;
            }
        }
    }
}
