using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.Core.Helpers;
using TDGame.Core.Models.Entities.Enemies;
using TDGame.Core.Models.Entities.Projectiles;
using TDGame.Core.Models.Entities.Turrets;
using TDGame.Core.Models.Maps;
using TDGame.Core.Resources;

namespace TDGame.Core.Modules
{
    public class ProjectileTurretsModule : IGameModule
    {
        public Game Game { get; }
        public List<ProjectileInstance> Projectiles { get; set; }

        public ProjectileTurretsModule(Game game)
        {
            Projectiles = new List<ProjectileInstance>();
            Game = game;
        }

        public void Update(TimeSpan passed)
        {
            foreach(var turret in Game.Turrets)
            {
                if (turret.TurretInfo is ProjectileTurretDefinition def)
                {
                    def.CoolingFor -= passed;
                    if (def.CoolingFor <= TimeSpan.Zero)
                        UpdateTurret(turret, def);
                }
            }

            if (Projectiles.Count > 0)
                UpdateProjectiles();
        }

        private void UpdateTurret(TurretInstance turret, ProjectileTurretDefinition def)
        {
            var best = Game.GetNearestEnemy(turret, def.Range);
            if (best != null)
            {
                if (Game.OnTurretShooting != null && turret.Targeting == null)
                    Game.OnTurretShooting.Invoke(turret);
                turret.Targeting = best;

                var projectile = new ProjectileInstance(def.ProjectileDefinition);
                projectile.X = turret.X + turret.Size / 2;
                projectile.Y = turret.Y + turret.Size / 2;
                projectile.Source = turret;
                if (def.IsTrailing)
                    projectile.Angle = Game.GetAngle(
                        GetTrailingPoint(best, projectile),
                        turret);
                else
                    projectile.Angle = Game.GetAngle(best, turret);
                turret.Angle = projectile.Angle;
                Projectiles.Add(projectile);
                def.CoolingFor = TimeSpan.FromMilliseconds(def.Cooldown);
            }
            else
            {
                if (Game.OnTurretIdle != null && turret.Targeting != null)
                    Game.OnTurretIdle.Invoke(turret);
                turret.Targeting = null;
            }
        }

        private FloatPoint GetTrailingPoint(EnemyInstance enemy, ProjectileInstance projectile)
        {
            float x = enemy.X + enemy.Size / 2;
            float y = enemy.Y + enemy.Size / 2;
            var dist = MathHelpers.Distance(enemy, projectile);
            var steps = dist / projectile.GetDefinition().Speed;
            var change = Game.GetEnemyLocationChange(enemy.Angle, enemy.GetSpeed());
            return new FloatPoint(x + change.X * (float)steps, y + change.Y * (float)steps);
        }

        private void UpdateProjectiles()
        {
            var toRemove = new List<ProjectileInstance>();
            foreach (var projectile in Projectiles)
            {
                var projectileDef = projectile.GetDefinition();
                if (projectileDef.IsGuided && projectile.Target != null)
                {
                    if (!Game.CurrentEnemies.Contains(projectile.Target))
                    {
                        var best = Game.GetNearestEnemy(projectile);
                        if (best == null)
                        {
                            toRemove.Add(projectile);
                            continue;
                        }
                        projectile.Target = best;
                    }
                    projectile.Angle = Game.GetAngle(projectile.Target, projectile);
                }

                var xMod = Math.Cos(projectile.Angle);
                var yMod = Math.Sin(projectile.Angle);
                if (projectileDef.Acceleration != 1)
                {
                    projectileDef.Speed = (float)Math.Ceiling(projectileDef.Speed * projectileDef.Acceleration);
                    if (projectileDef.Speed > Game.GameStyle.ProjectileSpeedCap)
                        projectileDef.Speed = Game.GameStyle.ProjectileSpeedCap;
                }

                if (projectile.Size >= 10)
                {
                    projectile.X += (float)xMod * projectileDef.Speed;
                    projectile.Y += (float)yMod * projectileDef.Speed;

                    if (IsWithinTriggerRange(projectile) ||
                        projectile.X < 0 || projectile.X > Game.Map.Width ||
                        projectile.Y < 0 || projectile.Y > Game.Map.Height)
                        toRemove.Add(projectile);
                }
                else
                {
                    for (int i = 0; i < 5; i++)
                    {
                        projectile.X += (float)xMod * ((float)projectileDef.Speed / 5);
                        projectile.Y += (float)yMod * ((float)projectileDef.Speed / 5);

                        if (IsWithinTriggerRange(projectile) ||
                            projectile.X < 0 || projectile.X > Game.Map.Width ||
                            projectile.Y < 0 || projectile.Y > Game.Map.Height)
                        {
                            toRemove.Add(projectile);
                            break;
                        }
                    }
                }
            }
            foreach (var projectile in toRemove)
                Projectiles.Remove(projectile);
        }

        private bool IsWithinTriggerRange(ProjectileInstance projectile)
        {
            bool isWithin = false;
            foreach (var enemy in Game.CurrentEnemies)
            {
                if (MathHelpers.Distance(projectile.X + projectile.Size / 2, projectile.Y + projectile.Size / 2, enemy.X + enemy.Size / 2, enemy.Y + enemy.Size / 2) < projectile.GetDefinition().TriggerRange)
                {
                    isWithin = true;
                    break;
                }
            }
            if (isWithin)
            {
                var projDef = projectile.GetDefinition();
                for (int i = 0; i < Game.CurrentEnemies.Count; i++)
                {
                    var dist = MathHelpers.Distance(projectile, Game.CurrentEnemies[i]);
                    if (dist < projDef.SplashRange)
                    {
                        if (Game.DamageEnemy(Game.CurrentEnemies[i], GetModifiedDamage(Game.CurrentEnemies[i].GetDefinition(), projDef)))
                        {
                            if (projectile.Source != null)
                                projectile.Source.Kills++;
                            i--;
                        }
                    }
                }
                return true;
            }
            return false;
        }

        private float GetModifiedDamage(EnemyDefinition enemyDef, ProjectileDefinition def)
        {
            var damage = def.Damage;
            foreach (var modifier in def.DamageModifiers)
                if (modifier.EnemyType == enemyDef.EnemyType)
                    damage = damage * modifier.Modifier;
            return damage;
        }
    }
}
