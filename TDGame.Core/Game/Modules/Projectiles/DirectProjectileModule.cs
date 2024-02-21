﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.Core.Game.Helpers;
using TDGame.Core.Game.Models.Entities.Projectiles;
using TDGame.Core.Game.Models.Entities.Projectiles.Modules;

namespace TDGame.Core.Game.Modules.Projectiles
{
    public class DirectProjectileModule : BaseProjectileModule<DirectProjectileDefinition>
    {
        public DirectProjectileModule(GameEngine game) : base(game)
        {
        }

        public override bool UpdateProjectile(TimeSpan passed, ProjectileInstance projectile, DirectProjectileDefinition def)
        {
            if (projectile.Source == null)
                return true;

            if (def.IsGuided && projectile.Target == null)
                projectile.Target = Game.GetBestEnemy(projectile);
            if (def.IsGuided && projectile.Target != null)
            {
                if (!Game.CurrentEnemies.Contains(projectile.Target))
                    projectile.Target = null;
                else
                    projectile.Angle = Game.GetAngle(projectile.Target, projectile);
            }

            var xMod = Math.Cos(projectile.Angle);
            var yMod = Math.Sin(projectile.Angle);
            if (def.Acceleration != 1)
            {
                def.Speed = (float)Math.Ceiling(def.Speed * def.Acceleration);
                if (def.Speed > Game.GameStyle.ProjectileSpeedCap)
                    def.Speed = Game.GameStyle.ProjectileSpeedCap;
            }

            if (projectile.Size >= 10)
            {
                projectile.X += (float)xMod * def.Speed;
                projectile.Y += (float)yMod * def.Speed;
                if (IsHit(projectile, def))
                    return true;
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    projectile.X += (float)xMod * ((float)def.Speed / 5);
                    projectile.Y += (float)yMod * ((float)def.Speed / 5);

                    if (IsHit(projectile, def))
                        return true;
                }
            }
            return false;
        }

        private bool IsHit(ProjectileInstance projectile, DirectProjectileDefinition def)
        {
            if (projectile.Source == null)
                return false;
            var best = Game.GetBestEnemy(projectile, projectile.Size / 2, projectile.Source.TargetingType, projectile.GetDefinition().CanDamage);
            if (best != null)
            {
                Game.DamageEnemy(best, GetModifiedDamage(best.GetDefinition(), def.Damage, def.DamageModifiers));
                return true;
            }
            else if (
                projectile.X < 0 || projectile.X > Game.Map.Width ||
                projectile.Y < 0 || projectile.Y > Game.Map.Height)
                return true;
            return false;
        }
    }
}
