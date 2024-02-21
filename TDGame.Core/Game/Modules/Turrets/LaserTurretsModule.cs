using TDGame.Core.Game.Models.Entities.Enemies.Modules;
using TDGame.Core.Game.Models.Entities.Turrets;
using TDGame.Core.Game.Models.Entities.Turrets.Modules;
using TDGame.Core.Game.Models.GameStyles;
using TDGame.Core.Game.Models.Maps;

namespace TDGame.Core.Game.Modules.Turrets
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

            var best = Game.GetBestEnemy(turret, def.Range);
            if (best != null)
            {
                if (Game.OnTurretShooting != null && turret.Targeting == null)
                    Game.OnTurretShooting.Invoke(turret);
                turret.Targeting = best;

                if (best.ModuleInfo is ISlowable slow)
                    SetSlowingFactor(slow, def.SlowingFactor, def.SlowingDuration);
                if (!Game.DamageEnemy(best, GetModifiedDamage(best.GetDefinition(), def.Damage, def.DamageModifiers)))
                    turret.Angle = Game.GetAngle(best, turret);
                else
                    turret.Kills++;
                def.CoolingFor = TimeSpan.FromMilliseconds(def.Cooldown);
            }
            else
            {
                if (Game.OnTurretIdle != null && turret.Targeting != null)
                    Game.OnTurretIdle.Invoke(turret);
                turret.Targeting = null;
            }
        }
    }
}
