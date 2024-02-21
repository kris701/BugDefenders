using TDGame.Core.Game.Models.Entities.Enemies;
using TDGame.Core.Game.Models.Entities.Enemies.Modules;
using TDGame.Core.Game.Models.Entities.Turrets;
using TDGame.Core.Game.Models.Entities.Turrets.Modules;

namespace TDGame.Core.Game.Modules.Turrets
{
    public abstract class BaseTurretModule<T> : IGameTurretModule<T> where T : ITurretModule
    {
        public GameEngine Game { get; }

        public BaseTurretModule(GameEngine game)
        {
            Game = game;
        }

        public void Update(TimeSpan passed)
        {
            foreach (var turret in Game.Turrets)
                if (turret.TurretInfo is T def)
                    UpdateTurret(passed, turret, def);
        }

        public abstract void UpdateTurret(TimeSpan passed, TurretInstance turret, T def);
        internal float GetModifiedDamage(EnemyDefinition enemyDef, float damage, List<DamageModifier> modifiers)
        {
            foreach (var modifier in modifiers)
                if (modifier.EnemyType == enemyDef.EnemyType)
                    damage *= modifier.Modifier;
            return damage;
        }

        internal void SetSlowingFactor(ISlowable item, float slowingFactor, int slowingDuration)
        {
            if (slowingFactor <= item.SlowingFactor)
            {
                item.SlowingFactor = slowingFactor;
                item.SlowingDuration = slowingDuration;
            }
        }
    }
}
