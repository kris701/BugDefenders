using BugDefender.Core.Game.Modules.Projectiles.SubModules;

namespace BugDefender.Core.Game.Modules.Projectiles
{
    public class ProjectilesModule : BaseSuperGameModule
    {
        public ProjectilesModule(GameContext context, GameEngine game) : base(context, game)
        {
            Modules = new List<IGameModule>()
            {
                new ExplosiveProjectileModule(Context, Game),
                new DirectProjectileModule(Context, Game),
                new FireProjectileModule(Context, Game)
            };
        }
    }
}
