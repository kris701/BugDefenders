using BugDefender.Core.Resources;

namespace BugDefender.Core.Users.Models.UserCriterias
{
    public class EnemiesKilledOfTypeCriteria : IUserCriteria
    {
        public Guid EnemyID { get; set; }
        public int Quantity { get; set; }

        public bool IsValid(StatsDefinition stats) => stats.KillsOfType.ContainsKey(EnemyID) && stats.KillsOfType[EnemyID] >= Quantity;
        public override string ToString()
        {
            return $"Kill {Quantity} '{ResourceManager.Enemies.GetResource(EnemyID).Name}' enemies.";
        }

        public string Progress(StatsDefinition stats)
        {
            if (!stats.KillsOfType.ContainsKey(EnemyID))
                return $"{Quantity} more '{ResourceManager.Enemies.GetResource(EnemyID).Name}' enemies to kill";
            return $"{Quantity - stats.KillsOfType[EnemyID]} more '{ResourceManager.Enemies.GetResource(EnemyID).Name}' enemies to kill";
        }
    }
}
