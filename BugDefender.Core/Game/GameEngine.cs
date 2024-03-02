using BugDefender.Core.Game.Helpers;
using BugDefender.Core.Game.Modules;
using BugDefender.Core.Game.Modules.Enemies;
using BugDefender.Core.Game.Modules.Projectiles;
using BugDefender.Core.Game.Modules.Turrets;
using BugDefender.Core.Resources;

namespace BugDefender.Core.Game
{
    public delegate void GameEventHandler();

    public partial class GameEngine
    {
        public GameEventHandler? OnPlayerDamaged;

        private readonly GameTimer _mainLoopTimer;

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

#if DEBUG
            CheatsHelper.Cheats.Add(CheatTypes.InfiniteMoney);
            Context.Wave = 9999;
#endif
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

            if (CheatsHelper.Cheats.Contains(CheatTypes.InfiniteMoney))
                Context.Money = 99999999;
            if (CheatsHelper.Cheats.Contains(CheatTypes.MaxWaves))
                Context.Wave = 99999999;
        }

        internal void DamagePlayer()
        {
            OnPlayerDamaged?.Invoke();
            if (!CheatsHelper.Cheats.Contains(CheatTypes.Invincibility))
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
