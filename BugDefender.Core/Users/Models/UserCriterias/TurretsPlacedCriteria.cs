using BugDefender.Core.Resources;

namespace BugDefender.Core.Users.Models.UserCriterias
{
    public class TurretsPlacedCriteria : IUserCriteria
    {
        public int Quantity { get; set; }

        public bool IsValid(StatsDefinition stats) => stats.TotalTurretsPlaced >= Quantity;
        public string GetDescriptionString()
        {
            return $"Place {Quantity} turrets.";
        }
    }
}
