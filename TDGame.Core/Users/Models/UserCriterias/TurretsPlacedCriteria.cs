namespace TDGame.Core.Users.Models.UserCriterias
{
    public class TurretsPlacedCriteria : IUserCriteria
    {
        public int Quantity { get; set; }

        public bool IsValid(StatsDefinition stats)
        {
            if (stats.TotalTurretsPlaced >= Quantity)
                return true;
            return false;
        }
    }
}
