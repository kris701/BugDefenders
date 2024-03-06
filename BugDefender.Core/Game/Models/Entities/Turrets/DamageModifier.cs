namespace BugDefender.Core.Game.Models.Entities.Turrets
{
    public class DamageModifier
    {
        public Guid EnemyType { get; set; }
        public float Modifier { get; set; }

        public DamageModifier(Guid enemyType, float modifier)
        {
            EnemyType = enemyType;
            Modifier = modifier;
        }
    }
}
