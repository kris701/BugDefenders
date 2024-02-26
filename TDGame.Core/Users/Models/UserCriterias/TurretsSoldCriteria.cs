namespace TDGame.Core.Users.Models.UserCriterias
{
    public class TurretsSoldCriteria : IUserCriteria
    {
        public int Quantity { get; set; }

        public bool IsValid(StatsDefinition stats) => stats.TotalTurretsSold >= Quantity;
    }
}
