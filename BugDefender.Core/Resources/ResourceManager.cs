using BugDefender.Core.Campaign.Models;
using BugDefender.Core.Game.Helpers;
using BugDefender.Core.Game.Models.EnemyTypes;
using BugDefender.Core.Game.Models.Entities.Enemies;
using BugDefender.Core.Game.Models.Entities.Projectiles;
using BugDefender.Core.Game.Models.Entities.Turrets;
using BugDefender.Core.Game.Models.GameStyles;
using BugDefender.Core.Game.Models.Maps;
using BugDefender.Core.Users.Models;
using BugDefender.Core.Users.Models.Buffs;
using BugDefender.Core.Users.Models.Challenges;
using System.Reflection;
using System.Text.Json;

namespace BugDefender.Core.Resources
{
    public static class ResourceManager
    {
        private static readonly Guid _coreID = new Guid("e142d037-51ea-45a4-bcf5-45e5c559234d");

        public static BaseBuilder<EnemyTypeDefinition> EnemyTypes = new BaseBuilder<EnemyTypeDefinition>("Resources.Core.EnemyTypes", Assembly.GetExecutingAssembly());
        public static BaseBuilder<GameStyleDefinition> GameStyles = new BaseBuilder<GameStyleDefinition>("Resources.Core.GameStyles", Assembly.GetExecutingAssembly());
        public static BaseBuilder<MapDefinition> Maps = new BaseBuilder<MapDefinition>("Resources.Core.Maps", Assembly.GetExecutingAssembly());

        public static BaseBuilder<EnemyDefinition> Enemies = new BaseBuilder<EnemyDefinition>("Resources.Core.Enemies", Assembly.GetExecutingAssembly());
        public static BaseBuilder<ProjectileDefinition> Projectiles = new BaseBuilder<ProjectileDefinition>("Resources.Core.Projectiles", Assembly.GetExecutingAssembly());
        public static BaseBuilder<TurretDefinition> Turrets = new BaseBuilder<TurretDefinition>("Resources.Core.Turrets", Assembly.GetExecutingAssembly());

        public static BaseBuilder<BuffDefinition> Buffs = new BaseBuilder<BuffDefinition>("Resources.Core.Buffs", Assembly.GetExecutingAssembly());
        public static BaseBuilder<AchivementDefinition> Achivements = new BaseBuilder<AchivementDefinition>("Resources.Core.Achivements", Assembly.GetExecutingAssembly());
        public static BaseBuilder<ChallengeDefinition> Challenges = new BaseBuilder<ChallengeDefinition>("Resources.Core.Challenges", Assembly.GetExecutingAssembly());
        public static BaseBuilder<CampaignDefinition> Campaigns = new BaseBuilder<CampaignDefinition>("Resources.Core.Campaigns", Assembly.GetExecutingAssembly());

        public static List<ResourceDefinition> LoadedResources { get; internal set; } = new List<ResourceDefinition>() {
            new ResourceDefinition(_coreID, "1.0.0", "Core", "Core Game Components")
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
                if (folder.Name.ToUpper() == "ACHIVEMENTS")
                    Achivements.LoadExternalResources(folder.GetFiles().ToList());
                if (folder.Name.ToUpper() == "CHALLENGES")
                    Challenges.LoadExternalResources(folder.GetFiles().ToList());
                if (folder.Name.ToUpper() == "CAMPAINS")
                    Campaigns.LoadExternalResources(folder.GetFiles().ToList());
            }

            if (!LoadedResources.Any(x => x.ID == resourceDefinition.ID))
                LoadedResources.Add(resourceDefinition);
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
            Achivements.Reload();
            Challenges.Reload();
            Campaigns.Reload();

            LoadedResources.Clear();
            LoadedResources = new List<ResourceDefinition>() {
                new ResourceDefinition(_coreID, "1.0.0", "Core", "Core Game Components")
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
            Achivements.Reload();
            Challenges.Reload();
            Campaigns.Reload();

            foreach (var resource in LoadedResources)
                if (resource.ID != _coreID)
                    LoadResource(new DirectoryInfo(resource.Path));
        }
    }
}
