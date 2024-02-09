using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TDGame.Core.Models;

namespace TDGame.Core.Enemies
{
    public class EnemyDefinition : BaseGameModel
    {
        public int Health { get; set; }
        public double Speed { get; set; }
        public int Reward { get; set; }
        [JsonIgnore]
        public int X { get; set; }
        [JsonIgnore]
        public int Y { get; set; }
        [JsonIgnore]
        public int WayPointID { get; set; } = 0;
        [JsonIgnore]
        public Guid GroupID { get; set; }

        public EnemyDefinition(Guid id, string name, string description, int health, double speed, int reward) : base(id, name, description)
        {
            Health = health;
            Speed = speed;
            Reward = reward;
        }
    }
}
