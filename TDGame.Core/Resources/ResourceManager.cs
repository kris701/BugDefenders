using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TDGame.Core.Helpers;
using TDGame.Core.Models.EnemyTypes;
using TDGame.Core.Models.Entities.Enemies;
using TDGame.Core.Models.Entities.Projectiles;
using TDGame.Core.Models.Entities.Turrets;
using TDGame.Core.Models.GameStyles;
using TDGame.Core.Models.Maps;

namespace TDGame.Core.Resources
{
    public static class ResourceManager
    {
        public static BaseBuilder<EnemyTypeDefinition> EnemyTypes = new BaseBuilder<EnemyTypeDefinition>("Resources.Core.EnemyTypes", Assembly.GetExecutingAssembly());
        public static BaseBuilder<GameStyleDefinition> GameStyles = new BaseBuilder<GameStyleDefinition>("Resources.Core.GameStyles", Assembly.GetExecutingAssembly());
        public static BaseBuilder<MapDefinition> Maps = new BaseBuilder<MapDefinition>("Resources.Core.Maps", Assembly.GetExecutingAssembly());

        public static BaseBuilder<EnemyDefinition> Enemies = new BaseBuilder<EnemyDefinition>("Resources.Core.Enemies", Assembly.GetExecutingAssembly());
        public static BaseBuilder<ProjectileDefinition> Projectiles = new BaseBuilder<ProjectileDefinition>("Resources.Core.Projectiles", Assembly.GetExecutingAssembly());
        public static BaseBuilder<TurretDefinition> Turrets = new BaseBuilder<TurretDefinition>("Resources.Core.Turrets", Assembly.GetExecutingAssembly());

        public static List<ResourceDefinition> LoadedResources { get; internal set; } = new List<ResourceDefinition>() {
            new ResourceDefinition()
            {
                ID = new Guid("e142d037-51ea-45a4-bcf5-45e5c559234d"),
                Version = "1.0.0",
                Name = "Core",
                Description = "Core game components"
            }
        };

        public static void LoadResource(DirectoryInfo path)
        {
            FileInfo? definitionFile = null;
            foreach (var file in path.GetFiles()) 
            {
                if (file.Extension.ToUpper() == ".JSON")
                {
                    definitionFile = file;
                    break;
                }
            }
            if (definitionFile == null || definitionFile.Directory == null)
                throw new FileNotFoundException("No resource definition found!");
            var resourceDefinition = JsonSerializer.Deserialize<ResourceDefinition>(File.ReadAllText(definitionFile.FullName));
            if (resourceDefinition == null)
                throw new Exception("Resource definition is malformed!");

            foreach(var folder in definitionFile.Directory.GetDirectories())
            {
                if (folder.Name.ToUpper() == "ENEMYTYPES")
                    EnemyTypes.LoadExternalResources(folder.GetFiles().ToList());
                if (folder.Name.ToUpper() == "GAMESTYLES")
                    GameStyles.LoadExternalResources(folder.GetFiles().ToList());
                if (folder.Name.ToUpper() == "MAPS")
                    Maps.LoadExternalResources(folder.GetFiles().ToList());

                if (folder.Name.ToUpper() == "ENEMIES")
                    Enemies.LoadExternalResources(folder.GetFiles().ToList());
                if (folder.Name.ToUpper() == "PROJECTILES")
                    Projectiles.LoadExternalResources(folder.GetFiles().ToList());
                if (folder.Name.ToUpper() == "TURRETS")
                    Turrets.LoadExternalResources(folder.GetFiles().ToList());
            }

            LoadedResources.Add(resourceDefinition);

            CheckGameIntegrity();
        }

        public static void ReloadResources()
        {
            EnemyTypes.Reload();
            GameStyles.Reload();
            Maps.Reload();

            Enemies.Reload();
            Projectiles.Reload();
            Turrets.Reload();

            LoadedResources.Clear();
            LoadedResources = new List<ResourceDefinition>() {
                new ResourceDefinition()
                {
                    ID = new Guid("e142d037-51ea-45a4-bcf5-45e5c559234d"),
                    Version = "1.0.0",
                    Name = "Core",
                    Description = "Core game components"
                }
            };
        }

        public static void CheckGameIntegrity()
        {
            var enemyTypes = EnemyTypes.GetResources();
            foreach (var id in Enemies.GetResources())
            {
                var enemy = Enemies.GetResource(id);
                if (!enemyTypes.Contains(enemy.EnemyType))
                    throw new Exception($"Enemy ({id}) has an unknown enemy type: {enemy.EnemyType}");
            }
            var projectiles = Projectiles.GetResources();
            foreach (var id in Turrets.GetResources())
            {
                var turret = Turrets.GetResource(id);
                if (turret.Type == TurretType.None)
                    throw new Exception($"Turret ({id}) has no turret type set!");
                if (turret.Type == TurretType.Projectile)
                {
                    if (turret.ProjectileID == null)
                        throw new Exception($"Turret ({id}) is set to be a projectile turret, but has no Projectile ID set!");
                    else if (!projectiles.Contains((Guid)turret.ProjectileID))
                        throw new Exception($"Turret ({id}) has the projectile id {turret.ProjectileID} but it was not found in any projectile definitions!");
                }
                var allUpgrades = turret.GetAllUpgrades();
                foreach (var upgrade in allUpgrades)
                    if (upgrade.Requires != null)
                        if (!allUpgrades.Any(x => x.ID == upgrade.Requires))
                            throw new Exception($"Turret ({id}) has upgrade ({upgrade.ID}) that requires an upgrade ID ({upgrade.Requires}) that does not exist!");
            }
        }
    }
}
