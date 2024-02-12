using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TDGame.Core.Enemies;
using TDGame.Core.Models;
using static TDGame.Core.Enemies.EnemyDefinition;

namespace TDGame.Core.Turrets
{
    public class ProjectileDefinition : BasePositionModel, ITextured
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public override float Size { get; set; }
        public int Speed { get; set; }
        public int Damage { get; set; }
        public int Range { get; set; }
        public int SplashRange { get; set; }
        public int TriggerRange { get; set; }
        public double Acceleration { get; set; }
        public bool IsGuided { get; set; }
        public bool IsExplosive { get; set; }
        public List<EnemyTypes> StrongAgainst { get; set; }
        public List<EnemyTypes> WeakAgainst { get; set; }


        [JsonIgnore]
        public int Traveled { get; set; }

        [JsonIgnore]
        public override float Angle { get; set; }
        [JsonIgnore]
        public EnemyDefinition Target { get; set; }
        [JsonIgnore]
        public TurretDefinition Source { get; set; }
    }
}
