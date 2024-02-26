namespace TDGame.Core.Users.Models.UserCriterias
{
    public class EnemiesKilledOfTypeCriteria : IUserCriteria
    {
        public Guid EnemyID { get; set; }
        public int Quantity { get; set; }

        public bool IsValid(StatsDefinition stats) => stats.KillsOfType.ContainsKey(EnemyID) && stats.KillsOfType[EnemyID] >= Quantity;
    }
}
