using BugDefender.Core.Resources;

namespace BugDefender.Core.Users.Models.UserCriterias
{
    public class TurretKillsCriteria : IUserCriteria
    {
        public Guid TurretID { get; set; }
        public int Quantity { get; set; }

        public bool IsValid(StatsDefinition stats) => stats.TotalTurretKills.ContainsKey(TurretID) && stats.TotalTurretKills[TurretID] >= Quantity;
        public override string ToString()
        {
            return $"Kill {Quantity} enemies with a '{ResourceManager.Turrets.GetResource(TurretID).Name}' turret.";
        }

        public string Progress(StatsDefinition stats)
        {
            if (!stats.TotalTurretKills.ContainsKey(TurretID))
                return $"{Quantity} more kills with a '{ResourceManager.Turrets.GetResource(TurretID).Name}' turret to go";
            return $"{Quantity - stats.TotalTurretKills[TurretID]} more kills with a '{ResourceManager.Turrets.GetResource(TurretID).Name}' turret to go";
        }
    }
}
