using TDGame.Core.Game.Helpers;
using TDGame.Core.Game.Models;
using TDGame.Core.Game.Models.Entities.Enemies;
using TDGame.Core.Game.Models.Entities.Projectiles;
using TDGame.Core.Game.Models.Entities.Turrets;
using TDGame.Core.Game.Models.Maps;
using TDGame.Core.Game.Modules;
using TDGame.Core.Game.Modules.Enemies;
using TDGame.Core.Game.Modules.Projectiles;
using TDGame.Core.Game.Modules.Turrets;
using TDGame.Core.Resources;
using static TDGame.Core.Game.Models.Entities.Enemies.EnemyDefinition;
using static TDGame.Core.Game.Models.Entities.Turrets.TurretInstance;

namespace TDGame.Core.Game
{


    public partial class GameEngine
    {
        private GameTimer _mainLoopTimer;

        public bool GameOver { get; set; }
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
        public GameContext Context { get; }
        public List<IGameModule> GameModules { get; }

        public EnemiesModule EnemiesModule { get; }
        public TurretsModule TurretsModule { get; }
        public ProjectilesModule ProjectilesModule { get; }

        public GameEngine(Guid mapID, Guid styleID)
        {
            Context = new GameContext(
                ResourceManager.Maps.GetResource(mapID),
                ResourceManager.GameStyles.GetResource(styleID));
            _mainLoopTimer = new GameTimer(TimeSpan.FromMilliseconds(30), MainLoop);

            EnemiesModule = new EnemiesModule(Context, this);
            TurretsModule = new TurretsModule(Context, this);
            ProjectilesModule = new ProjectilesModule(Context, this);

            GameModules = new List<IGameModule>()
            {
                new EvolutionModule(Context, this),
                TurretsModule,
                EnemiesModule,
                ProjectilesModule
            };

            foreach (var module in GameModules)
                module.Initialize();
        }

        public void Update(TimeSpan passed)
        {
            if (Running)
            {
                _mainLoopTimer.Update(passed);
                Context.GameTime += passed;
            }
        }

        private void MainLoop()
        {
            foreach (var module in GameModules)
                module.Update(_mainLoopTimer.Target);
        }

        internal void DamagePlayer()
        {
            Context.HP--;
            if (Context.HP <= 0)
            {
                Context.HP = 0;
                Running = false;
                GameOver = true;
            }
        }
    }
}
