using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static TDGame.Core.Game.Models.Entities.Enemies.EnemyDefinition;
using System.Xml.Linq;
using TDGame.Core.Game.Models.Entities.Turrets;
using TDGame.Core.Game.Models.Entities.Enemies;
using TDGame.Core.Resources;
using System.Text.Json.Serialization;

namespace TDGame.Core.Game.Models.Entities.Projectiles.Modules
{
    public class DirectProjectileDefinition : IProjectileModule, ISpeedAttribute, ISlowingAttribute, IDamageAttribute
    {
        public float Speed { get; set; }
        public float Damage { get; set; }
        public double Acceleration { get; set; }
        public bool IsGuided { get; set; }
        public float SlowingFactor { get; set; }
        public int SlowingDuration { get; set; }
        public List<DamageModifier> DamageModifiers { get; set; }

        public IProjectileModule Copy()
        {
            return new DirectProjectileDefinition()
            {
                Speed = Speed,
                Damage = Damage,
                Acceleration = Acceleration,
                IsGuided = IsGuided,
                SlowingFactor = SlowingFactor,
                SlowingDuration = SlowingDuration,
                DamageModifiers = DamageModifiers
            };
        }

        public string GetDescriptionString()
        {
            var sb = new StringBuilder();

            if (Speed != 0)
                sb.AppendLine($"Speed: {Speed}");
            if (Damage != 0)
                sb.AppendLine($"Damage: {Damage}");
            if (SlowingFactor != 1)
                sb.AppendLine($"Slowing Factor: {SlowingFactor}");
            if (SlowingDuration != 0)
                sb.AppendLine($"Slowing Duration: {SlowingDuration}");
            sb.AppendLine();
            if (DamageModifiers.Count > 0)
            {
                sb.AppendLine("Damage Modifiers:");
                foreach (var modifier in DamageModifiers)
                    sb.Append($"{ResourceManager.EnemyTypes.GetResource(modifier.EnemyType).Name}: {modifier.Modifier}x, ");
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
