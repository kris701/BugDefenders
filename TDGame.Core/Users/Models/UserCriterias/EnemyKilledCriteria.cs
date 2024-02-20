namespace TDGame.Core.Users.Models.UserCriterias
{
    public class EnemyKilledCriteria : IUserCriteria
    {
        public Guid EnemyID { get; set; }
        public int Quantity { get; set; }

        public bool IsValid(StatsDefinition stats)
        {
            if (stats.KillsOfType.ContainsKey(EnemyID) && stats.KillsOfType[EnemyID] >= Quantity)
                return true;
            return false;
        }
    }
}
