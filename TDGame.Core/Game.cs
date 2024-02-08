using TDGame.Core.GameStyles;
using TDGame.Core.Maps;

namespace TDGame.Core
{
    public class Game
    {
        public static int RefreshRate = 10;

        public MapDefinition Map { get; }
        public GameStyle GameStyle { get; }
        public double Evolution { get; internal set; }

        private TimeSpan _target = TimeSpan.FromMilliseconds(1000 / RefreshRate);
        private TimeSpan _last = TimeSpan.Zero;
        private TimeSpan _total = TimeSpan.Zero;

        public Game(string mapName, string style)
        {
            Map = MapBuilder.GetMap(mapName);
            GameStyle = GameStyleBuilder.GetGameStyle(style);
        }

        public void Update(TimeSpan passed)
        {
            _last = _last.Add(passed);
            _total = _total.Add(passed);
            if (_last > _target)
            {

                _last = TimeSpan.Zero;
            }
        }
    }
}
