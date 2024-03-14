using BugDefender.Core.Game.Models.Entities.Enemies;
using BugDefender.Core.Game.Models.Entities.Projectiles;
using BugDefender.Core.Game.Models.Entities.Turrets;
using BugDefender.Core.Game.Models.GameStyles;
using BugDefender.Core.Game.Models.Maps;
using BugDefender.Core.Game.Modules.Enemies;
using BugDefender.Core.Resources;
using BugDefender.Core.Users.Models;
using BugDefender.Core.Users.Models.Challenges;
using BugDefender.Core.Users.Models.UserCriterias;
using System.Text.Json;
using System.Text.Json.Serialization;
using static BugDefender.Core.Game.Models.Entities.Enemies.EnemyDefinition;

namespace BugDefender.Core.Game
{
    public enum GameResult { None, Lost, Success }
    public class GameContext
    {
        public MapDefinition Map { get; set; }
        public GameStyleDefinition GameStyle { get; set; }
        public List<List<Guid>> EnemiesToSpawn { get; set; } = new List<List<Guid>>();
        public bool AutoSpawn { get; set; } = false;
        public float Evolution { get; set; } = 1;
        public StatsDefinition Stats { get; set; } = new StatsDefinition();
        public CurrentEnemyContext CurrentEnemies { get; set; } = new CurrentEnemyContext();
        public HashSet<TurretInstance> Turrets { get; set; } = new HashSet<TurretInstance>();
        public HashSet<ProjectileInstance> Projectiles { get; set; } = new HashSet<ProjectileInstance>();
        public int HP { get; set; } = 0;
        public int Money { get; set; } = 0;
        public int Wave { get; set; } = 0;

        [JsonConstructor]
        public GameContext(MapDefinition map, GameStyleDefinition gameStyle, List<List<Guid>> enemiesToSpawn, bool autoSpawn, float evolution, StatsDefinition stats, CurrentEnemyContext currentEnemies, HashSet<TurretInstance> turrets, HashSet<ProjectileInstance> projectiles, int hP, int money, int wave) : this(map, gameStyle)
        {
            EnemiesToSpawn = enemiesToSpawn;
            AutoSpawn = autoSpawn;
            Evolution = evolution;
            Stats = stats;
            CurrentEnemies = currentEnemies;
            Turrets = turrets;
            Projectiles = projectiles;
            HP = hP;
            Money = money;
            Wave = wave;
        }

        public GameContext(Guid mapID, Guid gameStyleID) : this(
            ResourceManager.Maps.GetResource(mapID),
            ResourceManager.GameStyles.GetResource(gameStyleID))
        {
        }

        public GameContext(MapDefinition map, GameStyleDefinition gameStyle)
        {
            Map = map;
            GameStyle = gameStyle;
            HP = GameStyle.StartingHP;
            Money = GameStyle.StartingMoney;
        }

        public bool CanSave()
        {
            if (CurrentEnemies.Enemies.Count > 0)
                return false;
            if (Projectiles.Count > 0)
                return false;

            return true;
        }
    }
}
