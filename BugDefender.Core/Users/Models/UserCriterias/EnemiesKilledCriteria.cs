namespace BugDefender.Core.Users.Models.UserCriterias
{
    public class EnemiesKilledCriteria : IUserCriteria
    {
        public int Quantity { get; set; }

        public bool IsValid(StatsDefinition stats) => stats.TotalKills >= Quantity;

        public string GetDescriptionString()
        {
            return $"Kill {Quantity} enemies.";
        }
    }
}
