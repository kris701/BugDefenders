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
        public List<string> EnemiesToSpawn { get; set; }
        public bool AutoSpawn { get; set; } = false;
        public double Evolution { get; set; } = 1;
        public bool Running { get; set; } = true;
        public bool GameOver { get; set; }
        public List<EnemyDefinition> CurrentEnemies { get; set; }
        public int HP { get; set; }
        public int Money { get; set; }
        public List<TurretDefinition> Turrets { get; set; }
        public List<ProjectileDefinition> Projectiles { get; set; }
    }
}
