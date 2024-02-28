using BugDefender.Core.Game.Models.Entities.Turrets.Modules;

namespace BugDefender.Core.Users.Models.Buffs.BuffEffects
{
    public class TurretBuffEffect : IBuffEffect
    {
        public Guid TurretID { get; set; }
        public List<ChangeTarget> Changes { get; set; }

        public TurretBuffEffect(Guid turretID, List<ChangeTarget> changes)
        {
            TurretID = turretID;
            Changes = changes;
        }
    }
}
