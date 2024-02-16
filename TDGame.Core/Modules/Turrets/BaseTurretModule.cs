using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.Core.Models.Entities.Enemies;
using TDGame.Core.Models.Entities.Enemies.Modules;
using TDGame.Core.Models.Entities.Projectiles;
using TDGame.Core.Models.Entities.Turrets;

namespace TDGame.Core.Modules.Turrets
{
    public abstract class BaseTurretModule : IGameModule
    {
        public Game Game { get; }

        public BaseTurretModule(Game game)
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
