using System.Reflection;
using System.Text.Json;
using TDGame.Core.Game.Helpers;
using TDGame.Core.Game.Models.EnemyTypes;
using TDGame.Core.Game.Models.Entities.Enemies;
using TDGame.Core.Game.Models.Entities.Projectiles;
using TDGame.Core.Game.Models.Entities.Turrets;
using TDGame.Core.Game.Models.GameStyles;
using TDGame.Core.Game.Models.Maps;
using TDGame.Core.Users.Models;

namespace TDGame.Core.Resources
{
    public static class ResourceManager
    {
        private static Guid _coreID = new Guid("e142d037-51ea-45a4-bcf5-45e5c559234d");

        public static BaseBuilder<EnemyTypeDefinition> EnemyTypes = new BaseBuilder<EnemyTypeDefinition>("Resources.Core.EnemyTypes", Assembly.GetExecutingAssembly());
        public static BaseBuilder<GameStyleDefinition> GameStyles = new BaseBuilder<GameStyleDefinition>("Resources.Core.GameStyles", Assembly.GetExecutingAssembly());
        public static BaseBuilder<MapDefinition> Maps = new BaseBuilder<MapDefinition>("Resources.Core.Maps", Assembly.GetExecutingAssembly());

        public static BaseBuilder<EnemyDefinition> Enemies = new BaseBuilder<EnemyDefinition>("Resources.Core.Enemies", Assembly.GetExecutingAssembly());
        public static BaseBuilder<ProjectileDefinition> Projectiles = new BaseBuilder<ProjectileDefinition>("Resources.Core.Projectiles", Assembly.GetExecutingAssembly());
        public static BaseBuilder<TurretDefinition> Turrets = new BaseBuilder<TurretDefinition>("Resources.Core.Turrets", Assembly.GetExecutingAssembly());

        public static BaseBuilder<BuffDefinition> Buffs = new BaseBuilder<BuffDefinition>("Resources.Core.Buffs", Assembly.GetExecutingAssembly());

        public static List<ResourceDefinition> LoadedResources { get; internal set; } = new List<ResourceDefinition>() {
            new ResourceDefinition()
            {
                ID = _coreID,
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
            resourceDefinition.Path = path.FullName;

            foreach (var folder in definitionFile.Directory.GetDirectories())
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

                if (folder.Name.ToUpper() == "BUFFS")
                    Buffs.LoadExternalResources(folder.GetFiles().ToList());
            }

            if (!LoadedResources.Any(x => x.ID == resourceDefinition.ID))
                LoadedResources.Add(resourceDefinition);

            CheckGameIntegrity();
        }

        public static void UnloadExternalResources()
        {
            EnemyTypes.Reload();
            GameStyles.Reload();
            Maps.Reload();

            Enemies.Reload();
            Projectiles.Reload();
            Turrets.Reload();

            Buffs.Reload();

            LoadedResources.Clear();
            LoadedResources = new List<ResourceDefinition>() {
                new ResourceDefinition()
                {
                    ID = _coreID,
                    Version = "1.0.0",
                    Name = "Core",
                    Description = "Core game components"
                }
            };
        }

        public static void ReloadResources()
        {
            EnemyTypes.Reload();
            GameStyles.Reload();
            Maps.Reload();

            Enemies.Reload();
            Projectiles.Reload();
            Turrets.Reload();

            Buffs.Reload();

            foreach (var resource in LoadedResources)
                if (resource.ID != _coreID)
                    LoadResource(new DirectoryInfo(resource.Path));
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
                foreach (var upgrade in turret.Upgrades)
                    if (upgrade.Requires != null)
                        if (!turret.Upgrades.Any(x => x.ID == upgrade.Requires))
                            throw new Exception($"Turret ({id}) has an upgrade ({upgrade.ID}) that requires another upgrade ({upgrade.Requires}) but it does not exist!");
            }
        }
    }
}
