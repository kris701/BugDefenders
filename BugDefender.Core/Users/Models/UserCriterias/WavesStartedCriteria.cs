namespace BugDefender.Core.Users.Models.UserCriterias
{
    public class WavesStartedCriteria : IUserCriteria
    {
        public int Quantity { get; set; }

        public bool IsValid(StatsDefinition stats) => stats.TotalWavesStarted >= Quantity;
    }
}
