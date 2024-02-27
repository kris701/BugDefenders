using BugDefender.Core.Game.Helpers;
using BugDefender.Core.Game.Models.Entities.Enemies.Modules;
using BugDefender.Core.Game.Models.Entities.Turrets;
using BugDefender.Core.Game.Models.Entities.Turrets.Modules;

namespace BugDefender.Core.Game.Modules.Turrets.SubModules
{
    public class LaserTurretsModule : BaseTurretModule<LaserTurretDefinition>
    {
        public LaserTurretsModule(GameContext context, GameEngine game) : base(context, game)
        {
        }

        public override void UpdateTurret(TimeSpan passed, TurretInstance turret, LaserTurretDefinition def)
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

                if (best.ModuleInfo is ISlowable slow)
                    SetSlowingFactor(slow, def.SlowingFactor, def.SlowingDuration);
                if (!Game.EnemiesModule.DamageEnemy(best, GetModifiedDamage(best.GetDefinition(), def.Damage, def.DamageModifiers), turret.DefinitionID))
                    turret.Angle = MathHelpers.GetAngle(best, turret);
                else
                    turret.Kills++;
                def.CoolingFor = TimeSpan.FromMilliseconds(def.Cooldown);
            }
            else
            {
                if (Game.TurretsModule.OnTurretIdle != null && turret.Targeting != null)
                    Game.TurretsModule.OnTurretIdle.Invoke(turret);
                turret.Targeting = null;
            }
        }
    }
}
