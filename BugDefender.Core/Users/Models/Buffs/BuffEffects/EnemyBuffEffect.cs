namespace BugDefender.Core.Users.Models.Buffs.BuffEffects
{
    public class EnemyBuffEffect : IBuffEffect
    {
        public Guid EnemyID { get; set; }
        public List<ChangeTarget> Changes { get; set; }

        public EnemyBuffEffect(Guid enemyID, List<ChangeTarget> changes)
        {
            EnemyID = enemyID;
            Changes = changes;
        }
    }
}
