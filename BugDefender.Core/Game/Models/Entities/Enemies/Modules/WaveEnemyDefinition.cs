﻿using System.Text;
using System.Text.Json.Serialization;

namespace BugDefender.Core.Game.Models.Entities.Enemies.Modules
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
        public HashSet<EnemyInstance> Group { get; set; } = new HashSet<EnemyInstance>();

        public WaveEnemyDefinition(float speed, int waveSize, float spawnDelay)
        {
            Speed = speed;
            WaveSize = waveSize;
            SpawnDelay = spawnDelay;
        }

        public IEnemyModule Copy() => new WaveEnemyDefinition(Speed, WaveSize, SpawnDelay);

        public float GetSpeed()
        {
            if (SlowingFactor == 1)
                return Speed;
            if (SlowingDuration <= 0)
                SlowingFactor = 1;
            return Speed * SlowingFactor;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Type: Wave");
            sb.AppendLine($"Speed: {Speed}");
            sb.AppendLine($"Wave Size: {WaveSize}");

            return sb.ToString();
        }
    }
}
