using System.Text.Json.Serialization;

namespace BugDefender.Core.Game.Models.Entities.Turrets
{
    [JsonSerializable(typeof(DamageModifier))]
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
