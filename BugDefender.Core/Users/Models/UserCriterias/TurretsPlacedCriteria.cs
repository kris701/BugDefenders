using System.Text.Json.Serialization;

namespace BugDefender.Core.Users.Models.UserCriterias
{
    public class TurretsPlacedCriteria : IUserCriteria
    {
        public int Quantity { get; set; }

        public bool IsValid(StatsDefinition stats) => stats.TotalTurretsPlaced >= Quantity;
        public override string ToString()
        {
            return $"Place {Quantity} turrets.";
        }
    }
}
