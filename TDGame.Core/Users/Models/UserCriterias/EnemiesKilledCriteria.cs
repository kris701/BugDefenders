namespace TDGame.Core.Users.Models.UserCriterias
{
    public class EnemiesKilledCriteria : IUserCriteria
    {
        public int Quantity { get; set; }

        public bool IsValid(StatsDefinition stats)
        {
            if (stats.TotalKills >= Quantity)
                return true;
            return false;
        }
    }
}
