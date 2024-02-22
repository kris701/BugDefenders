﻿using TDGame.Core.Game.Modules.Projectiles.SubModules;

namespace TDGame.Core.Game.Modules.Projectiles
{
    public class ProjectilesModule : BaseSuperGameModule
    {
        public ProjectilesModule(GameContext context, GameEngine game) : base(context, game)
        {
            Modules = new List<IGameModule>()
            {
                new ExplosiveProjectileModule(Context, Game),
                new DirectProjectileModule(Context, Game)
            };
        }
    }
}
