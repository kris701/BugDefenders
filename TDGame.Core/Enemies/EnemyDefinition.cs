using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TDGame.Core.Models;

namespace TDGame.Core.Enemies
{
    public class EnemyDefinition : BasePositionModel, ITextured
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public override float Size { get; set; }
        public int Health { get; set; }
        public double Speed { get; set; }
        public int Reward { get; set; }
        public double WaveSize { get; set; }
        public bool IsBoss { get; set; }
        [JsonIgnore]
        public int WayPointID { get; set; } = 0;
        [JsonIgnore]
        public override float Angle { get; set; } = 0;
        [JsonIgnore]
        public Guid GroupID { get; set; }

        public EnemyDefinition(Guid id, string name, string description, float size, int health, double speed, int reward)
        {
            ID = id;
            Name = name;
            Description = description;
            Size = size;
            Health = health;
            Speed = speed;
            Reward = reward;
        }
    }
}
