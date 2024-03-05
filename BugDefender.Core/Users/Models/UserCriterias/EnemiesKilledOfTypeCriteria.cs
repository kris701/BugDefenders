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
    }
}
