using BugDefender.Core.Game.Helpers;
using BugDefender.Core.Game.Models.Entities.Enemies;
using BugDefender.Core.Game.Models.Entities.Enemies.Modules;
using BugDefender.Core.Game.Models.Entities.Projectiles;
using BugDefender.Core.Game.Models.Entities.Projectiles.Modules;

namespace BugDefender.Core.Game.Modules.Projectiles.SubModules
{
    public class DirectProjectileModule : BaseProjectileModule<DirectProjectileDefinition>
    {
        public DirectProjectileModule(GameContext context, GameEngine game) : base(context, game)
        {
        }

        public override bool UpdateProjectile(TimeSpan passed, ProjectileInstance projectile, DirectProjectileDefinition def)
        {
            if (projectile.Source == null)
                return true;

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

            if (projectile.Size >= 10)
            {
                projectile.X += xMod * def.Speed;
                projectile.Y += yMod * def.Speed;
                if (IsHit(projectile, def))
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
            EnemyInstance? best = null;
            if (def.IsGuided && projectile.Target != null)
            {
                var dist = MathHelpers.Distance(projectile, projectile.Target);
                if (dist < projectile.Size / 2)
                    best = projectile.Target;
            }
            else
                best = Game.EnemiesModule.GetBestEnemy(projectile, projectile.Size / 2, projectile.Source.TargetingType, projectile.GetDefinition().CanTarget);
            if (best != null)
            {
                if (best.ModuleInfo is ISlowable slow)
                    SetSlowingFactor(slow, def.SlowingFactor, def.SlowingDuration);
                if (Game.EnemiesModule.DamageEnemy(best, GetModifiedDamage(best.GetDefinition(), def.Damage, def.DamageModifiers), projectile.Source.DefinitionID))
                    projectile.Source.Kills++;
                return true;
            }
            else if (
                projectile.X < 0 || projectile.X > Context.Map.Width ||
                projectile.Y < 0 || projectile.Y > Context.Map.Height)
                return true;
            return false;
        }
    }
}
