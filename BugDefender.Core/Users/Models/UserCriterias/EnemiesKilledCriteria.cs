namespace BugDefender.Core.Users.Models.UserCriterias
{
    public class EnemiesKilledCriteria : IUserCriteria
    {
        public int Quantity { get; set; }

        public bool IsValid(StatsDefinition stats) => stats.TotalKills >= Quantity;

        public override string ToString()
        {
            return $"Kill {Quantity} enemies.";
        }

        public string Progress(StatsDefinition stats)
        {
            return $"{Quantity - stats.TotalKills} more enemies to kill";
        }
    }
}
