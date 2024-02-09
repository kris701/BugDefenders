using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TDGame.Core.Enemies;
using TDGame.Core.Models;

namespace TDGame.Core.Turrets
{
    public enum TurretType { None, Bullets, Rockets, Missile };
    public class TurretDefinition : BaseGameModel
    {
        public TurretType Type { get; set; }
        public int Size { get; set; }
        public int Cost { get; set; }
        public int Range { get; set; }
        public int Damage { get; set; }
        public int Cooldown { get; set; }
        public List<TurretLevel> Levels { get; set; }

        [JsonIgnore]
        public int X { get; set; }
        [JsonIgnore]
        public int Y { get; set; }
        [JsonIgnore]
        public TimeSpan CoolingFor { get; set; }
        [JsonIgnore]
        public EnemyDefinition? Targeting { get; set; }

        public TurretDefinition(Guid id, string name, string description, TurretType type, int size, int cost, int range, int damage, int cooldown, List<TurretLevel> levels) : base(id, name, description)
        {
            Type = type;
            Size = size;
            Cost = cost;
            Range = range;
            Damage = damage;
            Cooldown = cooldown;
            Levels = levels;

            foreach(var level in Levels)
                if (level.RequiresTurretLevel >= Levels.Count)
                    throw new Exception("Turret definition has an upgrade with a too high required index!");
        }
    }
}
