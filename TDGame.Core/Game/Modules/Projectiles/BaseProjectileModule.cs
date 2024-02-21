using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.Core.Game.Models.Entities.Enemies;
using TDGame.Core.Game.Models.Entities.Projectiles;
using TDGame.Core.Game.Models.Entities.Projectiles.Modules;
using TDGame.Core.Game.Models.Entities.Turrets;

namespace TDGame.Core.Game.Modules.Projectiles
{
    public abstract class BaseProjectileModule<T> : IGameProjectileModule<T> where T : IProjectileModule
    {
        public GameEngine Game { get; }

        public BaseProjectileModule(GameEngine game)
        {
            Game = game;
        }

        public void Update(TimeSpan passed)
        {
            var toRemove = new List<ProjectileInstance>();
            foreach (var projectile in Game.Projectiles)
            {
                if (projectile.ProjectileInfo is T def)
                {
                    if (UpdateProjectile(passed, projectile, def))
                        toRemove.Add(projectile);
                }
            }
            foreach (var remove in toRemove)
                Game.Projectiles.Remove(remove);
        }

        public abstract bool UpdateProjectile(TimeSpan passed, ProjectileInstance projectile, T def);

        internal float GetModifiedDamage(EnemyDefinition enemyDef, float damage, List<DamageModifier> modifiers)
        {
            foreach (var modifier in modifiers)
                if (modifier.EnemyType == enemyDef.EnemyType)
                    damage *= modifier.Modifier;
            return damage;
        }

    }
}
