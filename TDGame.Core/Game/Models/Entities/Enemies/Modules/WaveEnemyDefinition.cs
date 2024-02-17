using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TDGame.Core.Game.Models.Entities.Enemies.Modules
{
    public class WaveEnemyDefinition : IEnemyModule, ISlowable
    {
        public float Speed { get; set; }
        public int WaveSize { get; set; }
        [JsonIgnore]
        public float SlowingFactor { get; set; } = 1;
        [JsonIgnore]
        public int SlowingDuration { get; set; } = 0;
        [JsonIgnore]
        public Guid GroupID { get; set; }

        public IEnemyModule Copy()
        {
            return new WaveEnemyDefinition()
            {
                Speed = Speed,
                WaveSize = WaveSize
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
