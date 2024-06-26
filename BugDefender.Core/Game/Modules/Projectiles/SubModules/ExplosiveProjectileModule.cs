﻿using BugDefender.Core.Game.Helpers;
using BugDefender.Core.Game.Models.Entities.Enemies;
using BugDefender.Core.Game.Models.Entities.Enemies.Modules;
using BugDefender.Core.Game.Models.Entities.Projectiles;
using BugDefender.Core.Game.Models.Entities.Projectiles.Modules;

namespace BugDefender.Core.Game.Modules.Projectiles.SubModules
{
    public class ExplosiveProjectileModule : BaseProjectileModule<ExplosiveProjectileDefinition>
    {
        public ExplosiveProjectileModule(GameContext context, GameEngine game) : base(context, game)
        {
        }

        public override bool UpdateProjectile(TimeSpan passed, ProjectileInstance projectile, ExplosiveProjectileDefinition def)
        {
            if (def.IsGuided && projectile.Target == null)
                projectile.Target = Game.EnemiesModule.GetBestEnemy(projectile);
            if (def.IsGuided && projectile.Target != null)
            {
                if (!Context.CurrentEnemies.Enemies.Contains(projectile.Target))
                    projectile.Target = null;
                else
                    projectile.Angle = MathHelpers.GetAngle(projectile.Target, projectile);
            }

            var xMod = (float)Math.Cos(projectile.Angle);
            var yMod = (float)Math.Sin(projectile.Angle);
            if (def.Acceleration != 1)
            {
                def.Speed = (float)Math.Ceiling(def.Speed * def.Acceleration);
                if (def.Speed > Context.GameStyle.ProjectileSpeedCap)
                {
                    def.Speed = Context.GameStyle.ProjectileSpeedCap;
                    def.Acceleration = 1;
                }
            }

            var triggerRange = (float)Math.Pow(def.TriggerRange, 2);
            if (projectile.Size >= 10)
            {
                projectile.X += xMod * def.Speed;
                projectile.Y += yMod * def.Speed;

                if (IsWithinTriggerRange(projectile, def, triggerRange) ||
                    projectile.X < 0 || projectile.X > Context.Map.Width ||
                    projectile.Y < 0 || projectile.Y > Context.Map.Height)
                    return true;
            }
            else
            {
                var moved = 0f;
                var stepBy = projectile.Size;
                if (stepBy > def.Speed)
                    stepBy = def.Speed;
                while (moved < def.Speed)
                {
                    moved += stepBy;
                    projectile.X += xMod * stepBy;
                    projectile.Y += yMod * stepBy;

                    if (IsWithinTriggerRange(projectile, def, triggerRange) ||
                        projectile.X < 0 || projectile.X > Context.Map.Width ||
                        projectile.Y < 0 || projectile.Y > Context.Map.Height)
                        return true;
                }
            }
            return false;
        }

        private bool IsWithinTriggerRange(ProjectileInstance projectile, ExplosiveProjectileDefinition def, float triggerRange)
        {
            if (projectile.Source == null)
                return false;

            EnemyInstance? best = null;
            if (def.IsGuided && projectile.Target != null)
            {
                var dist = MathHelpers.SqrDistance(projectile, projectile.Target);
                if (dist < triggerRange)
                    best = projectile.Target;
            }
            else
                best = Game.EnemiesModule.GetBestEnemy(projectile, def.TriggerRange, projectile.Source.TargetingType, projectile.GetDefinition().CanTarget);
            if (best != null)
            {
                foreach (var canTarget in projectile.GetDefinition().CanTarget)
                {
                    for (int i = 0; i < Context.CurrentEnemies.EnemiesByTerrain[canTarget].Count; i++)
                    {
                        var enemy = Context.CurrentEnemies.EnemiesByTerrain[canTarget].ElementAt(i);
                        var dist = MathHelpers.SqrDistance(projectile, enemy);
                        if (dist < triggerRange)
                        {
                            if (enemy.ModuleInfo is ISlowable slow)
                                SetSlowingFactor(slow, def.SlowingFactor, def.SlowingDuration);
                            if (Game.EnemiesModule.DamageEnemy(enemy, GetModifiedDamage(enemy.GetDefinition(), def.Damage, def.DamageModifiers), projectile.Source!.DefinitionID))
                            {
                                if (projectile.Source != null)
                                    projectile.Source.Kills++;
                                i--;
                            }
                        }
                    }
                }
                return true;
            }
            return false;
        }
    }
}
