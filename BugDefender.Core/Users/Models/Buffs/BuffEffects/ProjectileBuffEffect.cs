using BugDefender.Core.Game.Models.Entities.Projectiles.Modules;

namespace BugDefender.Core.Users.Models.Buffs.BuffEffects
{
    public class ProjectileBuffEffect : IBuffEffect
    {
        public Guid ProjectileID { get; set; }
        public List<ChangeTarget> Changes { get; set; }

        public ProjectileBuffEffect(Guid projectileID, List<ChangeTarget> changes)
        {
            ProjectileID = projectileID;
            Changes = changes;
        }
    }
}
