using TDGame.Core.Game.Models.GameStyles;
using TDGame.Core.Game.Models.Maps;

namespace TDGame.Core.Game.Modules
{
    public interface IGameModule
    {
        public GameContext Context { get; }
        public GameEngine Game { get; }
        public void Update(TimeSpan passed);
    }
}
