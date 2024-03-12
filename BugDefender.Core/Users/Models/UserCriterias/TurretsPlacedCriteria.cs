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

        public string Progress(StatsDefinition stats)
        {
            return $"{Quantity - stats.TotalTurretsPlaced} more turrets placed to go";
        }
    }
}
