using BugDefender.Core.Game.Helpers;
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
                if (!Context.CurrentEnemies.Contains(projectile.Target))
                    projectile.Target = null;
                else
                    projectile.Angle = MathHelpers.GetAngle(projectile.Target, projectile);
            }

            var xMod = Math.Cos(projectile.Angle);
            var yMod = Math.Sin(projectile.Angle);
            if (def.Acceleration != 1)
            {
                def.Speed = (float)Math.Ceiling(def.Speed * def.Acceleration);
                if (def.Speed > Context.GameStyle.ProjectileSpeedCap)
                    def.Speed = Context.GameStyle.ProjectileSpeedCap;
            }

            if (projectile.Size >= 10)
            {
                projectile.X += (float)xMod * def.Speed;
                projectile.Y += (float)yMod * def.Speed;

                if (IsWithinTriggerRange(projectile, def) ||
                    projectile.X < 0 || projectile.X > Context.Map.Width ||
                    projectile.Y < 0 || projectile.Y > Context.Map.Height)
                    return true;
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    projectile.X += (float)xMod * ((float)def.Speed / 5);
                    projectile.Y += (float)yMod * ((float)def.Speed / 5);

                    if (IsWithinTriggerRange(projectile, def) ||
                        projectile.X < 0 || projectile.X > Context.Map.Width ||
                        projectile.Y < 0 || projectile.Y > Context.Map.Height)
                        return true;
                }
            }
            return false;
        }

        private bool IsWithinTriggerRange(ProjectileInstance projectile, ExplosiveProjectileDefinition def)
        {
            if (projectile.Source == null)
                return false;
            bool isWithin = false;
            foreach (var enemy in Context.CurrentEnemies)
            {
                if (MathHelpers.Distance(projectile, enemy) < def.TriggerRange)
                {
                    isWithin = true;
                    break;
                }
            }
            if (isWithin)
            {
                for (int i = 0; i < Context.CurrentEnemies.Count; i++)
                {
                    var dist = MathHelpers.Distance(projectile, Context.CurrentEnemies.ElementAt(i));
                    if (dist < def.SplashRange)
                    {
                        if (Context.CurrentEnemies.ElementAt(i).ModuleInfo is ISlowable slow)
                            SetSlowingFactor(slow, def.SlowingFactor, def.SlowingDuration);
                        if (Game.EnemiesModule.DamageEnemy(Context.CurrentEnemies.ElementAt(i), GetModifiedDamage(Context.CurrentEnemies.ElementAt(i).GetDefinition(), def.Damage, def.DamageModifiers), projectile.Source!.DefinitionID))
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
