namespace BugDefender.Core.Users.Models.UserCriterias
{
    public class TurretsSoldCriteria : IUserCriteria
    {
        public int Quantity { get; set; }

        public bool IsValid(StatsDefinition stats) => stats.TotalTurretsSold >= Quantity;
        public override string ToString()
        {
            return $"Sell {Quantity} turrets.";
        }

        public string Progress(StatsDefinition stats)
        {
            return $"{Quantity - stats.TotalTurretsSold} more turrets sold to go";
        }
    }
}
