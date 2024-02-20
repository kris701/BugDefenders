using TDGame.Core.Game.Helpers;
using TDGame.Core.Game.Models.Entities.Enemies;
using TDGame.Core.Game.Models.Entities.Enemies.Modules;
using TDGame.Core.Game.Models.Entities.Projectiles;
using TDGame.Core.Game.Models.Entities.Turrets;
using TDGame.Core.Game.Models.Entities.Turrets.Modules;
using TDGame.Core.Game.Models.Maps;

namespace TDGame.Core.Game.Modules.Turrets
{
    public class ProjectileTurretsModule : BaseTurretModule
    {
        public HashSet<ProjectileInstance> Projectiles { get; set; }

        public ProjectileTurretsModule(GameEngine game) : base(game)
        {
            Projectiles = new HashSet<ProjectileInstance>();
        }

        public override void Update(TimeSpan passed)
        {
            foreach (var turret in Game.Turrets)
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
            var best = Game.GetBestEnemy(turret, def.Range);
            if (best != null)
            {
                if (Game.OnTurretShooting != null && turret.Targeting == null)
                    Game.OnTurretShooting.Invoke(turret);
                turret.Targeting = best;

                var projectile = new ProjectileInstance(def.ProjectileDefinition);
                projectile.X = turret.CenterX - projectile.Size / 2;
                projectile.Y = turret.CenterY - projectile.Size / 2;
                projectile.Source = turret;
                projectile.Target = best;
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
            if (enemy.ModuleInfo is ISlowable slow)
            {
                float x = enemy.CenterX;
                float y = enemy.CenterY;
                var dist = MathHelpers.Distance(enemy, projectile);
                var steps = dist / projectile.GetDefinition().Speed;
                var change = MathHelpers.GetPredictedLocation(enemy.Angle, slow.GetSpeed(), Game.GameStyle.EnemySpeedMultiplier);
                return new FloatPoint(x + change.X * (float)steps, y + change.Y * (float)steps);
            }
            else
                return new FloatPoint(enemy.CenterX, enemy.CenterY);
        }

        private void UpdateProjectiles()
        {
            var toRemove = new List<ProjectileInstance>();
            foreach (var projectile in Projectiles)
            {
                var projectileDef = projectile.GetDefinition();
                if (projectileDef.IsGuided && projectile.Target == null)
                    projectile.Target = Game.GetBestEnemy(projectile);
                if (projectileDef.IsGuided && projectile.Target != null)
                {
                    if (!Game.CurrentEnemies.Contains(projectile.Target))
                        projectile.Target = null;
                    else
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
                if (MathHelpers.Distance(projectile.CenterX, projectile.CenterY, enemy.CenterX, enemy.CenterY) < projectile.GetDefinition().TriggerRange)
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
                    var dist = MathHelpers.Distance(projectile, Game.CurrentEnemies.ElementAt(i));
                    if (dist < projDef.SplashRange)
                    {
                        if (Game.DamageEnemy(Game.CurrentEnemies.ElementAt(i), GetModifiedDamage(Game.CurrentEnemies.ElementAt(i).GetDefinition(), projDef.Damage, projDef.DamageModifiers)))
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
