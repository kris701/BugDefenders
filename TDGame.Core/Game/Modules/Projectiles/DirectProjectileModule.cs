using TDGame.Core.Game.Models.Entities.Enemies.Modules;
using TDGame.Core.Game.Models.Entities.Projectiles;
using TDGame.Core.Game.Models.Entities.Projectiles.Modules;
using TDGame.Core.Game.Models.GameStyles;
using TDGame.Core.Game.Models.Maps;

namespace TDGame.Core.Game.Modules.Projectiles
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
                projectile.Target = Game.GetBestEnemy(projectile);
            if (def.IsGuided && projectile.Target != null)
            {
                if (!Context.CurrentEnemies.Contains(projectile.Target))
                    projectile.Target = null;
                else
                    projectile.Angle = Game.GetAngle(projectile.Target, projectile);
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
                if (best.ModuleInfo is ISlowable slow)
                    SetSlowingFactor(slow, def.SlowingFactor, def.SlowingDuration);
                Game.DamageEnemy(best, GetModifiedDamage(best.GetDefinition(), def.Damage, def.DamageModifiers));
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
