using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.Core.Entities.Projectiles;
using TDGame.Core.Helpers;

namespace TDGame.Core
{
    public partial class Game
    {
        private void UpdateProjectiles()
        {
            var toRemove = new List<ProjectileInstance>();
            foreach (var projectile in Projectiles)
            {
                var projectileDef = projectile.GetDefinition();
                if (projectileDef.IsGuided && projectile.Target != null)
                {
                    if (!CurrentEnemies.Contains(projectile.Target))
                    {
                        var best = GetNearestEnemy(projectile);
                        if (best == null)
                        {
                            toRemove.Add(projectile);
                            continue;
                        }
                        projectile.Target = best;
                    }
                    projectile.Angle = GetAngle(projectile.Target, projectile);
                }

                var xMod = Math.Cos(projectile.Angle);
                var yMod = Math.Sin(projectile.Angle);
                if (projectileDef.Acceleration != 1)
                {
                    projectileDef.Speed = (float)Math.Ceiling(projectileDef.Speed * projectileDef.Acceleration);
                    if (projectileDef.Speed > GameStyle.ProjectileSpeedCap)
                        projectileDef.Speed = GameStyle.ProjectileSpeedCap;
                }
                projectile.Traveled += projectileDef.Speed;
                if (projectile.Traveled > projectileDef.Range)
                {
                    toRemove.Add(projectile);
                    continue;
                }

                if (projectile.Size >= 10)
                {
                    projectile.X += (float)xMod * projectileDef.Speed;
                    projectile.Y += (float)yMod * projectileDef.Speed;

                    if (IsWithinTriggerRange(projectile) ||
                        projectile.X < 0 || projectile.X > Map.Width ||
                        projectile.Y < 0 || projectile.Y > Map.Height)
                        toRemove.Add(projectile);
                }
                else
                {
                    for (int i = 0; i < 5; i++)
                    {
                        projectile.X += (float)xMod * ((float)projectileDef.Speed / 5);
                        projectile.Y += (float)yMod * ((float)projectileDef.Speed / 5);

                        if (IsWithinTriggerRange(projectile) ||
                            projectile.X < 0 || projectile.X > Map.Width ||
                            projectile.Y < 0 || projectile.Y > Map.Height)
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
            foreach (var enemy in CurrentEnemies)
            {
                if (MathHelpers.Distance(projectile.X + projectile.Size / 2, projectile.Y + projectile.Size / 2, enemy.X + enemy.Size / 2, enemy.Y + enemy.Size / 2) < projectile.GetDefinition().TriggerRange)
                {
                    isWithin = true;
                    break;
                }
            }
            if (isWithin)
            {
                for (int i = 0; i < CurrentEnemies.Count; i++)
                {
                    var dist = MathHelpers.Distance(projectile.X + projectile.Size / 2, projectile.Y + projectile.Size / 2, CurrentEnemies[i].X + CurrentEnemies[i].Size / 2, CurrentEnemies[i].Y + CurrentEnemies[i].Size / 2);
                    if (dist < projectile.GetDefinition().SplashRange)
                    {
                        if (DamageEnemy(CurrentEnemies[i], projectile.GetDefinition().Damage, projectile.GetDefinition().DamageModifiers))
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
    }
}
