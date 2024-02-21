using TDGame.Core.Game.Helpers;
using TDGame.Core.Game.Models.Entities;
using TDGame.Core.Game.Models.Entities.Enemies;
using TDGame.Core.Game.Models.Entities.Enemies.Modules;
using TDGame.Core.Game.Models.Entities.Projectiles;
using TDGame.Core.Game.Models.Entities.Turrets;
using TDGame.Core.Game.Models.Entities.Turrets.Modules;
using TDGame.Core.Game.Models.Maps;

namespace TDGame.Core.Game.Modules.Turrets
{
    public class ProjectileTurretsModule : BaseTurretModule<ProjectileTurretDefinition>
    {
        public ProjectileTurretsModule(GameContext context, GameEngine game) : base(context, game)
        {
        }

        public override void UpdateTurret(TimeSpan passed, TurretInstance turret, ProjectileTurretDefinition def)
        {
            def.CoolingFor -= passed;
            if (def.CoolingFor > TimeSpan.Zero)
                return;

            var best = Game.EnemiesModule.GetBestEnemy(turret, def.Range);
            if (best != null)
            {
                if (Game.TurretsModule.OnTurretShooting != null && turret.Targeting == null)
                    Game.TurretsModule.OnTurretShooting.Invoke(turret);
                turret.Targeting = best;

                var projectile = new ProjectileInstance(def.ProjectileID, def.ProjectileInfo);
                projectile.X = turret.CenterX - projectile.Size / 2;
                projectile.Y = turret.CenterY - projectile.Size / 2;
                projectile.Source = turret;
                projectile.Target = best;
                if (def.IsTrailing)
                    projectile.Angle = MathHelpers.GetAngle(
                        GetTrailingPoint(best, projectile),
                        turret);
                else
                    projectile.Angle = MathHelpers.GetAngle(best, turret);
                turret.Angle = projectile.Angle;
                Context.Projectiles.Add(projectile);
                def.CoolingFor = TimeSpan.FromMilliseconds(def.Cooldown);
            }
            else
            {
                if (Game.TurretsModule.OnTurretIdle != null && turret.Targeting != null)
                    Game.TurretsModule.OnTurretIdle.Invoke(turret);
                turret.Targeting = null;
            }
        }

        private FloatPoint GetTrailingPoint(EnemyInstance enemy, ProjectileInstance projectile)
        {
            if (enemy.ModuleInfo is ISlowable slow && projectile.ProjectileInfo is ISpeedAttribute speed)
            {
                float x = enemy.CenterX;
                float y = enemy.CenterY;
                var dist = MathHelpers.Distance(enemy, projectile);
                var steps = dist / speed.Speed;
                var change = MathHelpers.GetPredictedLocation(enemy.Angle, slow.GetSpeed(), Context.GameStyle.EnemySpeedMultiplier);
                return new FloatPoint(x + change.X * (float)steps, y + change.Y * (float)steps);
            }
            else
                return new FloatPoint(enemy.CenterX, enemy.CenterY);
        }
    }
}
