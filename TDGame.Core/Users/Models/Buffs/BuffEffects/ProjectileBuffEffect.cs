using TDGame.Core.Game.Models.Entities.Projectiles.Modules;
using TDGame.Core.Game.Models.Entities.Turrets.Modules;

namespace TDGame.Core.Users.Models.Buffs.BuffEffects
{
    public class ProjectileBuffEffect : IBuffEffect
    {
        public Guid ProjectileID { get; set; }
        public IProjectileModule Module { get; set; }

        public ProjectileBuffEffect(Guid projectileID, IProjectileModule module)
        {
            ProjectileID = projectileID;
            Module = module;
        }
    }
}
