using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.Core.Game.Models.GameStyles;
using TDGame.Core.Game.Models.Maps;

namespace TDGame.Core.Game.Modules
{
    public abstract class BaseGameModule : IGameModule
    {
        public GameContext Context { get; }
        public GameEngine Game { get; }

        public BaseGameModule(GameContext context, GameEngine game)
        {
            Context = context;
            Game = game;
        }

        public abstract void Update(TimeSpan passed);
    }
}
