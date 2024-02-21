using TDGame.Core.Game.Models.Entities.Projectiles;
using TDGame.Core.Game.Models.Entities.Projectiles.Modules;

namespace TDGame.Core.Game.Modules.Projectiles
{
    public interface IGameProjectileModule<T> : IGameModule where T : IProjectileModule
    {
        public bool UpdateProjectile(TimeSpan passed, ProjectileInstance projectile, T def);
    }
}
