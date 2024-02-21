using System.Text;
using System.Text.Json.Serialization;

namespace TDGame.Core.Game.Models.Entities.Enemies.Modules
{
    public class WaveEnemyDefinition : IEnemyModule, ISpeedAttribute, ISlowable
    {
        public float Speed { get; set; }
        public int WaveSize { get; set; }
        public float SpawnDelay { get; set; }
        [JsonIgnore]
        public float SlowingFactor { get; set; } = 1;
        [JsonIgnore]
        public int SlowingDuration { get; set; } = 0;
        [JsonIgnore]
        public List<EnemyInstance> Group { get; set; }

        public IEnemyModule Copy()
        {
            return new WaveEnemyDefinition()
            {
                Speed = Speed,
                WaveSize = WaveSize,
                SpawnDelay = SpawnDelay,
            };
        }

        public string GetDescriptionString()
        {
            var sb = new StringBuilder();

            return sb.ToString();
        }

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
