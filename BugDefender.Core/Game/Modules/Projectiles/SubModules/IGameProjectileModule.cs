using BugDefender.Core.Game.Models.Entities.Projectiles;
using BugDefender.Core.Game.Models.Entities.Projectiles.Modules;

namespace BugDefender.Core.Game.Modules.Projectiles.SubModules
{
    public interface IGameProjectileModule<T> : IGameModule where T : IProjectileModule
    {
        public bool UpdateProjectile(TimeSpan passed, ProjectileInstance projectile, T def);
    }
}
