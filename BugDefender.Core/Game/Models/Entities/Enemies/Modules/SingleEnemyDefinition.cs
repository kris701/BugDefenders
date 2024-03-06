using System.Text.Json.Serialization;

namespace BugDefender.Core.Game.Models.Entities.Enemies.Modules
{
    public class SingleEnemyDefinition : IEnemyModule, ISpeedAttribute, ISlowable
    {
        public float Speed { get; set; }
        [JsonIgnore]
        public float SlowingFactor { get; set; } = 1;
        [JsonIgnore]
        public int SlowingDuration { get; set; } = 0;

        public SingleEnemyDefinition(float speed)
        {
            Speed = speed;
        }

        public IEnemyModule Copy() => new SingleEnemyDefinition(Speed);

        public float GetSpeed()
        {
            if (SlowingFactor == 1)
                return Speed;
            if (SlowingDuration <= 0)
                SlowingFactor = 1;
            return Speed * SlowingFactor;
        }
    }
}
