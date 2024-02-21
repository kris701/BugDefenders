using TDGame.Core.Game.Helpers;
using TDGame.Core.Game.Models.Entities;
using TDGame.Core.Game.Models.Entities.Enemies;
using TDGame.Core.Game.Models.Entities.Enemies.Modules;
using TDGame.Core.Game.Models.Entities.Projectiles;
using TDGame.Core.Game.Models.Entities.Projectiles.Modules;
using TDGame.Core.Game.Models.Entities.Turrets;
using TDGame.Core.Game.Models.Entities.Turrets.Modules;
using TDGame.Core.Game.Models.Maps;

namespace TDGame.Core.Game.Modules.Turrets
{
    public class ProjectileTurretsModule : BaseTurretModule<ProjectileTurretDefinition>
    {
        public ProjectileTurretsModule(GameEngine game) : base(game)
        {
        }

        public override void UpdateTurret(TimeSpan passed, TurretInstance turret, ProjectileTurretDefinition def)
        {
            def.CoolingFor -= passed;
            if (def.CoolingFor > TimeSpan.Zero)
                return;

            var best = Game.GetBestEnemy(turret, def.Range);
            if (best != null)
            {
                if (Game.OnTurretShooting != null && turret.Targeting == null)
                    Game.OnTurretShooting.Invoke(turret);
                turret.Targeting = best;

                var projectile = new ProjectileInstance(def.ProjectileID, def.ProjectileInfo);
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
                Game.Projectiles.Add(projectile);
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
            if (enemy.ModuleInfo is ISlowable slow && projectile.ProjectileInfo is ISpeedAttribute speed)
            {
                float x = enemy.CenterX;
                float y = enemy.CenterY;
                var dist = MathHelpers.Distance(enemy, projectile);
                var steps = dist / speed.Speed;
                var change = MathHelpers.GetPredictedLocation(enemy.Angle, slow.GetSpeed(), Game.GameStyle.EnemySpeedMultiplier);
                return new FloatPoint(x + change.X * (float)steps, y + change.Y * (float)steps);
            }
            else
                return new FloatPoint(enemy.CenterX, enemy.CenterY);
        }
    }
}
