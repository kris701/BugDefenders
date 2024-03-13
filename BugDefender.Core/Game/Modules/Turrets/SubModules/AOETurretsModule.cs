using BugDefender.Core.Game.Helpers;
using BugDefender.Core.Game.Models.Entities.Enemies;
using BugDefender.Core.Game.Models.Entities.Enemies.Modules;
using BugDefender.Core.Game.Models.Entities.Turrets;
using BugDefender.Core.Game.Models.Entities.Turrets.Modules;

namespace BugDefender.Core.Game.Modules.Turrets.SubModules
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
            var range = (float)Math.Pow(def.Range, 2);
            foreach (var enemy in Context.CurrentEnemies)
            {
                if (!turret.GetDefinition().CanTarget.Contains(enemy.GetDefinition().TerrainType))
                    continue;
                var dist = MathHelpers.SqrDistance(enemy, turret);
                if (dist <= range)
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
                Game.TurretsModule.OnTurretShooting?.Invoke(turret);
                turret.Targeting = best;

                foreach (var enemy in targeting)
                {
                    if (enemy.ModuleInfo is ISlowable slow)
                        SetSlowingFactor(slow, def.SlowingFactor, def.SlowingDuration);
                    if (Game.EnemiesModule.DamageEnemy(enemy, GetModifiedDamage(enemy.GetDefinition(), def), turret.DefinitionID))
                        turret.Kills++;
                }
                turret.Angle = MathHelpers.GetAngle(best, turret);
                def.CoolingFor = TimeSpan.FromMilliseconds(def.Cooldown);
            }
            else
            {
                if (Game.TurretsModule.OnTurretIdle != null && turret.Targeting != null)
                    Game.TurretsModule.OnTurretIdle.Invoke(turret);
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
