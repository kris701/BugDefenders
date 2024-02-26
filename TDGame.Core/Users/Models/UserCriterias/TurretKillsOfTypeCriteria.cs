namespace TDGame.Core.Users.Models.UserCriterias
{
    public class TurretKillsOfTypeCriteria : IUserCriteria
    {
        public Guid TurretID { get; set; }
        public Guid EnemyID { get; set; }
        public int Quantity { get; set; }

        public bool IsValid(StatsDefinition stats)
        {
            if (stats.TotalTurretKillsOfType.ContainsKey(TurretID) &&
                stats.TotalTurretKillsOfType[TurretID].ContainsKey(EnemyID) &&
                stats.TotalTurretKillsOfType[TurretID][EnemyID] >= Quantity)
                return true;
            return false;
        }
    }
}
