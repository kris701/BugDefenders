using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TDGame.Core.EnemyTypes;
using TDGame.Core.Entities.Enemies;
using TDGame.Core.Entities.Projectiles;
using TDGame.Core.Entities.Turrets;
using TDGame.Core.GameStyles;
using TDGame.Core.Maps;
using TDGame.Core.Resources;

namespace TDGame.Core.Mods
{
    public static class ModLoader
    {
        public static void LoadModPacks(DirectoryInfo modFolder)
        {
            foreach(var modPack in modFolder.GetFiles())
                LoadModPack(modPack);
        }

        public static void LoadModPack(FileInfo modFile)
        {
            var parsed = JsonSerializer.Deserialize<ModPackDefinition>(File.ReadAllText(modFile.FullName));
            if (parsed == null)
                throw new Exception("Error parsing mod pack!");

            if (parsed.EnemyTypes != null)
                ResourceManager.EnemyTypes.LoadModResources(parsed.EnemyTypes);
            if (parsed.GameStyles != null)
                ResourceManager.GameStypes.LoadModResources(parsed.GameStyles);
            if (parsed.Maps != null)
                ResourceManager.Maps.LoadModResources(parsed.Maps);

            if (parsed.Enemies != null)
                ResourceManager.Enemies.LoadModResources(parsed.Enemies);
            if (parsed.Projectiles != null)
                ResourceManager.Projectiles.LoadModResources(parsed.Projectiles);
            if (parsed.Turrets != null)
                ResourceManager.Turrets.LoadModResources(parsed.Turrets);

            Resources.ResourceManager.CheckGameIntegrity();
        }
    }
}
