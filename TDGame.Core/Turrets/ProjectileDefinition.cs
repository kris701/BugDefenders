﻿using System;
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
    public class ProjectileDefinition : IDefinition
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Size { get; set; }
        public float Speed { get; set; }
        public float Damage { get; set; }
        public float Range { get; set; }
        public float SplashRange { get; set; }
        public float TriggerRange { get; set; }
        public double Acceleration { get; set; }
        public bool IsGuided { get; set; }
        public bool IsExplosive { get; set; }
        public List<DamageModifier> DamageModifiers { get; set; }
    }
}
