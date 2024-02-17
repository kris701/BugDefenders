using TDGame.Core.Game.Models.Entities.Enemies;
using TDGame.Core.Game.Models.Entities.Enemies.Modules;
using TDGame.Core.Game.Models.Entities.Turrets;

namespace TDGame.Core.Game.Modules.Turrets
{
    public abstract class BaseTurretModule : IGameModule
    {
        public GameEngine Game { get; }

        public BaseTurretModule(GameEngine game)
        {
            Game = game;
        }

        public abstract void Update(TimeSpan passed);

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
