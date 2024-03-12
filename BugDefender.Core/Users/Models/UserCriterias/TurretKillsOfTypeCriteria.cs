using BugDefender.Core.Resources;

namespace BugDefender.Core.Users.Models.UserCriterias
{
    public class TurretKillsOfTypeCriteria : IUserCriteria
    {
        public Guid TurretID { get; set; }
        public Guid EnemyID { get; set; }
        public int Quantity { get; set; }

        public bool IsValid(StatsDefinition stats) =>
            stats.TotalTurretKillsOfType.ContainsKey(TurretID) &&
            stats.TotalTurretKillsOfType[TurretID].ContainsKey(EnemyID) &&
            stats.TotalTurretKillsOfType[TurretID][EnemyID] >= Quantity;

        public override string ToString()
        {
            return $"Kill {Quantity} '{ResourceManager.Enemies.GetResource(EnemyID).Name}' enemies with a '{ResourceManager.Turrets.GetResource(TurretID).Name}' turret.";
        }

        public string Progress(StatsDefinition stats)
        {
            if (!stats.TotalTurretKillsOfType.ContainsKey(TurretID) || !stats.TotalTurretKillsOfType[TurretID].ContainsKey(EnemyID))
                return $"{Quantity} more kills of '{ResourceManager.Enemies.GetResource(EnemyID).Name}' with a '{ResourceManager.Turrets.GetResource(TurretID).Name}' turret to go";
            return $"{Quantity - stats.TotalTurretKillsOfType[TurretID][EnemyID]} more kills of '{ResourceManager.Enemies.GetResource(EnemyID).Name}' with a '{ResourceManager.Turrets.GetResource(TurretID).Name}' turret to go";
        }
    }
}
