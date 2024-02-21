﻿using System.Text;
using TDGame.Core.Game.Models.Entities.Projectiles.Modules;
using TDGame.Core.Game.Models.Entities.Turrets;
using TDGame.Core.Game.Models.Entities.Turrets.Modules;

namespace TDGame.Core.Game.Models.Entities.Upgrades
{
    public class ExplosiveProjectileUpgrade : IUpgrade
    {
        public Guid ID { get; set; }
        public Guid? Requires { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
        public float DamageModifier { get; set; }
        public float SplashRangeModifier { get; set; }
        public float TriggerRangeModifier { get; set; }
        public float SlowingFactorModifier { get; set; }
        public float SlowingDurationModifier { get; set; }

        public void ApplyUpgrade(TurretInstance on)
        {
            if (on.TurretInfo is ProjectileTurretDefinition tur && tur.ProjectileInfo is ExplosiveProjectileDefinition item)
            {
                item.Damage *= DamageModifier;
                item.SplashRange *= SplashRangeModifier;
                item.TriggerRange *= TriggerRangeModifier;
                item.SlowingFactor *= SlowingFactorModifier;
                item.SlowingDuration = (int)(item.SlowingDuration * SlowingDurationModifier);
                on.HasUpgrades.Add(ID);
            }
            else
                throw new Exception("Invalid upgrade type for turret!");
        }

        public string GetDescriptionString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(Description);
            sb.AppendLine("Projectile get:");
            if (DamageModifier != 1)
                sb.AppendLine($"Damage {DamageModifier}x");
            if (SplashRangeModifier != 1)
                sb.AppendLine($"Splash Range {SplashRangeModifier}x");
            if (TriggerRangeModifier != 1)
                sb.AppendLine($"Trigger Range {TriggerRangeModifier}x");

            return sb.ToString();
        }
    }
}