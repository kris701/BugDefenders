using TDGame.Core.Helpers;
using TDGame.Core.Models.Entities.Enemies;
using TDGame.Core.Models.Entities.Enemies.Modules;
using TDGame.Core.Models.Entities.Turrets;
using TDGame.Core.Models.Entities.Turrets.Modules;

namespace TDGame.Core.Modules.Turrets
{
    public class AOETurretsModule : IGameModule
    {
        public Game Game { get; }

        public AOETurretsModule(Game game)
        {
            Game = game;
        }

        public void Update(TimeSpan passed)
        {
            foreach (var turret in Game.Turrets)
            {
                if (turret.TurretInfo is AOETurretDefinition def)
                {
                    def.CoolingFor -= passed;
                    if (def.CoolingFor <= TimeSpan.Zero)
                        UpdateTurret(turret, def);
                }
            }
        }

        private void UpdateTurret(TurretInstance turret, AOETurretDefinition def)
        {
            var closest = float.MaxValue;
            EnemyInstance? best = null;
            var targeting = new List<EnemyInstance>();
            foreach (var enemy in Game.CurrentEnemies)
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
                    {
                        if (def.SlowingFactor <= slow.SlowingFactor)
                        {
                            slow.SlowingFactor = def.SlowingFactor;
                            slow.SlowingDuration = def.SlowingDuration;
                        }
                    }
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
