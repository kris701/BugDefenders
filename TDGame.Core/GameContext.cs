using TDGame.Core.Models.Entities.Enemies;
using TDGame.Core.Models.Entities.Turrets;
using TDGame.Core.Models.GameStyles;
using TDGame.Core.Models.Maps;
using TDGame.Core.Modules;

namespace TDGame.Core
{
    public partial class Game
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
