namespace BugDefender.Core.Users.Models.UserCriterias
{
    public class MoneyEarnedCriteria : IUserCriteria
    {
        public int Quantity { get; set; }

        public bool IsValid(StatsDefinition stats) => stats.TotalMoneyEarned >= Quantity;
    }
}
