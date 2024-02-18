using TDGame.Core.Game.Models;
using TDGame.Core.Game.Models.Entities.Enemies;
using TDGame.Core.Game.Models.Entities.Turrets;
using TDGame.Core.Game.Models.GameStyles;
using TDGame.Core.Game.Models.Maps;
using TDGame.Core.Game.Modules;
using TDGame.Core.Users.Models;

namespace TDGame.Core.Game
{
    public partial class GameEngine
    {
        public MapDefinition Map { get; set; }
        public GameStyleDefinition GameStyle { get; set; }
        public List<List<Guid>> EnemiesToSpawn { get; set; }
        public bool AutoSpawn { get; set; } = false;
        public float Evolution { get; set; } = 1;
        private bool _running = true;
        public bool Running
        {
            get
            {
                return _running;
            }
            set
            {
                if (!GameOver)
                    _running = value;
            }
        }
        public StatsDefinition Outcome { get; set; } = new StatsDefinition();
        public List<EnemyInstance> CurrentEnemies { get; set; }
        public int HP { get; set; }
        public int Money { get; set; }
        public int Score { get; set; }
        public List<TurretInstance> Turrets { get; set; }

        public bool GameOver { get; set; }
        public TimeSpan GameTime { get; set; }

        public List<IGameModule> GameModules { get; }
    }
}
