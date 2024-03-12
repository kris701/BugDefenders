using BugDefender.Core.Game.Helpers;
using BugDefender.Core.Game.Models.Entities.Enemies.Modules;
using BugDefender.Core.Game.Models.Entities.Projectiles;
using BugDefender.Core.Game.Models.Entities.Projectiles.Modules;

namespace BugDefender.Core.Game.Modules.Projectiles.SubModules
{
    public class FireProjectileModule : BaseProjectileModule<FireProjectileDefinition>
    {
        public FireProjectileModule(GameContext context, GameEngine game) : base(context, game)
        {
        }

        public override bool UpdateProjectile(TimeSpan passed, ProjectileInstance projectile, FireProjectileDefinition def)
        {
            def.LifeTimeMs -= (int)passed.TotalMilliseconds;
            if (def.LifeTimeMs <= 0)
                return true;

            if (def.IsGuided && projectile.Target == null)
                projectile.Target = Game.EnemiesModule.GetBestEnemy(projectile);
            if (def.IsGuided && projectile.Target != null)
            {
                if (!Context.CurrentEnemies.Contains(projectile.Target))
                    projectile.Target = null;
                else
                    projectile.Angle = MathHelpers.GetAngle(projectile.Target, projectile);
            }

            var xMod = (float)Math.Cos(projectile.Angle);
            var yMod = (float)Math.Sin(projectile.Angle);
            if (def.Acceleration != 1)
            {
                def.Speed = (float)Math.Floor(def.Speed * def.Acceleration);
                if (def.Speed > Context.GameStyle.ProjectileSpeedCap)
                {
                    def.Speed = Context.GameStyle.ProjectileSpeedCap;
                    def.Acceleration = 1;
                }
                else if (def.Speed <= 0)
                {
                    def.Speed = 0;
                    def.Acceleration = 1;
                }
            }

            var damageRange = (float)Math.Pow(def.DamageRange, 2);
            if (def.Speed > 0)
            {
                projectile.X += xMod * def.Speed;
                projectile.Y += yMod * def.Speed;

                if (projectile.X < 0 || projectile.X > Context.Map.Width ||
                    projectile.Y < 0 || projectile.Y > Context.Map.Height)
                    return true;
            }
            def.CoolingFor -= passed;
            if (def.CoolingFor <= TimeSpan.Zero)
            {
                def.CoolingFor = TimeSpan.FromMilliseconds(def.CooldownMs);
                DamageWithinRange(projectile, def, damageRange);
            }
            return false;
        }

        private void DamageWithinRange(ProjectileInstance projectile, FireProjectileDefinition def, float damageRange)
        {
            for (int i = 0; i < Context.CurrentEnemies.Count; i++)
            {
                if (!projectile.GetDefinition().CanTarget.Contains(Context.CurrentEnemies.ElementAt(i).GetDefinition().TerrainType))
                    continue;
                var dist = MathHelpers.SqrDistance(projectile, Context.CurrentEnemies.ElementAt(i));
                if (dist < damageRange)
                {
                    if (Context.CurrentEnemies.ElementAt(i).ModuleInfo is ISlowable slow)
                        SetSlowingFactor(slow, def.SlowingFactor, def.SlowingDuration);
                    if (Game.EnemiesModule.DamageEnemy(Context.CurrentEnemies.ElementAt(i), GetModifiedDamage(Context.CurrentEnemies.ElementAt(i).GetDefinition(), def.Damage, def.DamageModifiers), projectile.Source!.DefinitionID))
                    {
                        def.Speed /= 2;
                        if (projectile.Source != null)
                            projectile.Source.Kills++;
                        i--;
                    }
                }
            }
        }
    }
}
