using TDGame.Core.Game.Helpers;
using TDGame.Core.Game.Models.Entities.Enemies;
using TDGame.Core.Game.Models.Entities.Enemies.Modules;
using TDGame.Core.Game.Models.Entities.Turrets;
using TDGame.Core.Game.Models.Entities.Turrets.Modules;
using TDGame.Core.Game.Models.GameStyles;
using TDGame.Core.Game.Models.Maps;

namespace TDGame.Core.Game.Modules.Turrets
{
    public class AOETurretsModule : BaseTurretModule<AOETurretDefinition>
    {
        public AOETurretsModule(GameContext context, GameEngine game) : base(context, game)
        {
        }

        public override void UpdateTurret(TimeSpan passed, TurretInstance turret, AOETurretDefinition def)
        {
            def.CoolingFor -= passed;
            if (def.CoolingFor > TimeSpan.Zero)
                return;

            var closest = float.MaxValue;
            EnemyInstance? best = null;
            var targeting = new List<EnemyInstance>();
            foreach (var enemy in Context.CurrentEnemies)
            {
                if (!turret.GetDefinition().CanDamage.Contains(enemy.GetDefinition().TerrainType))
                    continue;
                var dist = MathHelpers.Distance(enemy, turret);
                if (dist <= def.Range)
                {
                    targeting.Add(enemy);
                    if (dist < closest)
                    {
                        closest = dist;
                        best = enemy;
                    }
                }
            }

            if (best != null)
            {
                if (Game.OnTurretShooting != null && turret.Targeting == null)
                    Game.OnTurretShooting.Invoke(turret);
                turret.Targeting = best;

                foreach (var enemy in targeting)
                {
                    if (enemy.ModuleInfo is ISlowable slow)
                        SetSlowingFactor(slow, def.SlowingFactor, def.SlowingDuration);
                    if (Game.DamageEnemy(enemy, GetModifiedDamage(enemy.GetDefinition(), def)))
                        turret.Kills++;
                }
                turret.Angle = Game.GetAngle(best, turret);
                def.CoolingFor = TimeSpan.FromMilliseconds(def.Cooldown);
            }
            else
            {
                if (Game.OnTurretIdle != null && turret.Targeting != null)
                    Game.OnTurretIdle.Invoke(turret);
                turret.Targeting = null;
            }
        }

        private float GetModifiedDamage(EnemyDefinition enemyDef, AOETurretDefinition def)
        {
            var damage = def.Damage;
            foreach (var modifier in def.DamageModifiers)
                if (modifier.EnemyType == enemyDef.EnemyType)
                    damage = damage * modifier.Modifier;
            return damage;
        }
    }
}
