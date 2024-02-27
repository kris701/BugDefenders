using BugDefender.Core.Game.Models.Entities.Enemies;
using BugDefender.Core.Game.Models.Entities.Enemies.Modules;
using BugDefender.Core.Game.Models.Entities.Projectiles;
using BugDefender.Core.Game.Models.Entities.Projectiles.Modules;
using BugDefender.Core.Game.Models.Entities.Turrets;

namespace BugDefender.Core.Game.Modules.Projectiles.SubModules
{
    public abstract class BaseProjectileModule<T> : BaseGameModule, IGameProjectileModule<T> where T : IProjectileModule
    {
        protected BaseProjectileModule(GameContext context, GameEngine game) : base(context, game)
        {
        }

        public override void Update(TimeSpan passed)
        {
            var toRemove = new List<ProjectileInstance>();
            foreach (var projectile in Context.Projectiles)
            {
                if (projectile.ProjectileInfo is T def)
                {
                    if (UpdateProjectile(passed, projectile, def))
                        toRemove.Add(projectile);
                }
            }
            foreach (var remove in toRemove)
                Context.Projectiles.Remove(remove);
        }

        public abstract bool UpdateProjectile(TimeSpan passed, ProjectileInstance projectile, T def);

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
