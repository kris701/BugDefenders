using BugDefender.Core.Game.Helpers;
using BugDefender.Core.Game.Models;
using BugDefender.Core.Game.Models.Entities.Enemies;
using BugDefender.Core.Game.Models.Entities.Projectiles;
using BugDefender.Core.Game.Models.Entities.Turrets;
using BugDefender.Core.Game.Models.GameStyles;
using BugDefender.Core.Game.Models.Maps;
using BugDefender.Core.Users.Models;

namespace BugDefender.Core.Game
{
    public class GameContext
    {
        public MapDefinition Map { get; set; }
        public GameStyleDefinition GameStyle { get; set; }
        public List<List<Guid>> EnemiesToSpawn { get; set; } = new List<List<Guid>>();
        public bool AutoSpawn { get; set; } = false;
        public float Evolution { get; set; } = 1;
        public StatsDefinition Outcome { get; set; } = new StatsDefinition();
        public HashSet<EnemyInstance> CurrentEnemies { get; set; } = new HashSet<EnemyInstance>();
        public HashSet<TurretInstance> Turrets { get; set; } = new HashSet<TurretInstance>();
        public HashSet<ProjectileInstance> Projectiles { get; set; } = new HashSet<ProjectileInstance>();
        public int HP { get; set; }
        public int Money { get; set; }
        public int Score { get; set; }

        public int Wave { get; internal set; }
        public TimeSpan GameTime { get; set; }

        public GameContext(MapDefinition map, GameStyleDefinition gameStyle)
        {
            Map = map;
            GameStyle = gameStyle;

            HP = GameStyle.StartingHP;
            Money = GameStyle.StartingMoney;

        }
    }
}
