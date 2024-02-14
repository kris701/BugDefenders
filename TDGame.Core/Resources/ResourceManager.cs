using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TDGame.Core.EnemyTypes;
using TDGame.Core.Entities.Enemies;
using TDGame.Core.Entities.Projectiles;
using TDGame.Core.Entities.Turrets;
using TDGame.Core.GameStyles;
using TDGame.Core.Helpers;
using TDGame.Core.Maps;

namespace TDGame.Core.Resources
{
    public static class ResourceManager
    {
        public static BaseBuilder<EnemyTypeDefinition> EnemyTypes = new BaseBuilder<EnemyTypeDefinition>("Resources.EnemyTypes", Assembly.GetExecutingAssembly());
        public static BaseBuilder<GameStyleDefinition> GameStypes = new BaseBuilder<GameStyleDefinition>("Resources.GameStyles", Assembly.GetExecutingAssembly());
        public static BaseBuilder<MapDefinition> Maps = new BaseBuilder<MapDefinition>("Resources.Maps", Assembly.GetExecutingAssembly());

        public static BaseBuilder<EnemyDefinition> Enemies = new BaseBuilder<EnemyDefinition>("Resources.Enemies", Assembly.GetExecutingAssembly());
        public static BaseBuilder<ProjectileDefinition> Projectiles = new BaseBuilder<ProjectileDefinition>("Resources.Projectiles", Assembly.GetExecutingAssembly());
        public static BaseBuilder<TurretDefinition> Turrets = new BaseBuilder<TurretDefinition>("Resources.Turrets", Assembly.GetExecutingAssembly());

        public static void ReloadResources()
        {
            EnemyTypes.Reload();
            GameStypes.Reload();
            Maps.Reload();

            Enemies.Reload();
            Projectiles.Reload();
            Turrets.Reload();
        }

        public static List<string> CheckGameIntegrity()
        {
            var issues = new List<string>();
            var enemyTypes = EnemyTypes.GetResources();
            foreach (var id in Enemies.GetResources())
            {
                var enemy = Enemies.GetResource(id);
                if (!enemyTypes.Contains(enemy.EnemyType))
                    issues.Add($"Enemy ({id}) has an unknown enemy type: {enemy.EnemyType}");
            }
            var projectiles = Projectiles.GetResources();
            foreach (var id in Turrets.GetResources())
            {
                var turret = Turrets.GetResource(id);
                if (turret.Type == TurretType.None)
                    issues.Add($"Turret ({id}) has no turret type set!");
                if (turret.Type == TurretType.Projectile)
                {
                    if (turret.ProjectileID == null)
                        issues.Add($"Turret ({id}) is set to be a projectile turret, but has no Projectile ID set!");
                    else if (!projectiles.Contains((Guid)turret.ProjectileID))
                        issues.Add($"Turret ({id}) has the projectile id {turret.ProjectileID} but it was not found in any projectile definitions!");
                }
                var allUpgrades = turret.GetAllUpgrades();
                foreach (var upgrade in allUpgrades)
                    if (upgrade.Requires != null)
                        if (!allUpgrades.Any(x => x.ID == upgrade.Requires))
                            issues.Add($"Turret ({id}) has upgrade ({upgrade.ID}) that requires an upgrade ID ({upgrade.Requires}) that does not exist!");
            }

            return issues;
        }
    }
}
