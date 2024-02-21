using TDGame.Core.Game.Helpers;
using TDGame.Core.Game.Models.Entities.Enemies.Modules;
using TDGame.Core.Game.Models.Entities.Projectiles;
using TDGame.Core.Game.Models.Entities.Projectiles.Modules;

namespace TDGame.Core.Game.Modules.Projectiles
{
    public class ExplosiveProjectileModule : BaseProjectileModule<ExplosiveProjectileDefinition>
    {
        public ExplosiveProjectileModule(GameEngine game) : base(game)
        {
        }

        public override bool UpdateProjectile(TimeSpan passed, ProjectileInstance projectile, ExplosiveProjectileDefinition def)
        {
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

                if (IsWithinTriggerRange(projectile, def) ||
                    projectile.X < 0 || projectile.X > Game.Map.Width ||
                    projectile.Y < 0 || projectile.Y > Game.Map.Height)
                    return true;
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    projectile.X += (float)xMod * ((float)def.Speed / 5);
                    projectile.Y += (float)yMod * ((float)def.Speed / 5);

                    if (IsWithinTriggerRange(projectile, def) ||
                        projectile.X < 0 || projectile.X > Game.Map.Width ||
                        projectile.Y < 0 || projectile.Y > Game.Map.Height)
                        return true;
                }
            }
            return false;
        }

        private bool IsWithinTriggerRange(ProjectileInstance projectile, ExplosiveProjectileDefinition def)
        {
            bool isWithin = false;
            foreach (var enemy in Game.CurrentEnemies)
            {
                if (MathHelpers.Distance(projectile.CenterX, projectile.CenterY, enemy.CenterX, enemy.CenterY) < def.TriggerRange)
                {
                    isWithin = true;
                    break;
                }
            }
            if (isWithin)
            {
                for (int i = 0; i < Game.CurrentEnemies.Count; i++)
                {
                    var dist = MathHelpers.Distance(projectile, Game.CurrentEnemies.ElementAt(i));
                    if (dist < def.SplashRange)
                    {
                        if (Game.CurrentEnemies.ElementAt(i).ModuleInfo is ISlowable slow)
                            SetSlowingFactor(slow, def.SlowingFactor, def.SlowingDuration);
                        if (Game.DamageEnemy(Game.CurrentEnemies.ElementAt(i), GetModifiedDamage(Game.CurrentEnemies.ElementAt(i).GetDefinition(), def.Damage, def.DamageModifiers)))
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
