using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.Core.Game.Modules.Projectiles;

namespace TDGame.Core.Game.Modules
{
    public class ProjectilesModule : BaseSuperGameModule
    {
        public ProjectilesModule(GameContext context, GameEngine game) : base(context, game)
        {
        }

        public override void Initialize()
        {
            Modules = new List<IGameModule>()
            {
                new ExplosiveProjectileModule(Context, Game),
                new DirectProjectileModule(Context, Game)
            };
            base.Initialize();
        }
    }
}
