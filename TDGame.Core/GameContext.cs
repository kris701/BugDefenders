using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.Core.Enemies;
using TDGame.Core.GameStyles;
using TDGame.Core.Maps;
using TDGame.Core.Turrets;

namespace TDGame.Core
{
    public partial class Game
    {
        public MapDefinition Map { get; set; }
        public GameStyleDefinition GameStyle { get; set; }
        public List<Guid> EnemiesToSpawn { get; set; }
        public bool AutoSpawn { get; set; } = false;
        public float Evolution { get; set; } = 1;
        private bool _running = true;
        public bool Running { 
            get { 
                return _running; 
            } set {
                if (!GameOver)
                    _running = value;
            } 
        }
        public List<EnemyInstance> CurrentEnemies { get; set; }
        public int HP { get; set; }
        public int Money { get; set; }
        public int Score { get; set; }
        public List<TurretInstance> Turrets { get; set; }
        public List<ProjectileInstance> Projectiles { get; set; }

        public bool GameOver { get; set; }
        public TimeSpan GameTime { get; set; }
    }
}
