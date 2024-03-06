using System.Text.Json.Serialization;

namespace BugDefender.Core.Users.Models.UserCriterias
{
    [JsonSerializable(typeof(EnemiesKilledCriteria))]
    public class EnemiesKilledCriteria : IUserCriteria
    {
        public int Quantity { get; set; }

        public bool IsValid(StatsDefinition stats) => stats.TotalKills >= Quantity;

        public override string ToString()
        {
            return $"Kill {Quantity} enemies.";
        }
    }
}
