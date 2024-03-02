using BugDefender.Core.Game.Helpers;
using BugDefender.Core.Game.Modules;
using BugDefender.Core.Game.Modules.Enemies;
using BugDefender.Core.Game.Modules.Projectiles;
using BugDefender.Core.Game.Modules.Turrets;
using BugDefender.Core.Resources;
using BugDefender.Core.Users.Models;
using BugDefender.Core.Users.Models.Challenges;

namespace BugDefender.Core.Game
{
    public delegate void GameEventHandler();

    public partial class GameEngine
    {
        public GameEventHandler? OnPlayerDamaged;

        private readonly GameTimer _mainLoopTimer;
        private readonly GameTimer _challengeTimer;

        public GameResult Result { get; private set; } = GameResult.None;
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

        public GameEngine(ChallengeDefinition challenge) : this(challenge.MapID, challenge.GameStyleID)
        {
            Context.Challenge = challenge;
        }

        public GameEngine(Guid mapID, Guid gameStyleID) : this(
            new GameContext()
            {
                Map = ResourceManager.Maps.GetResource(mapID),
                GameStyle = ResourceManager.GameStyles.GetResource(gameStyleID)
            }
        )
        {
            Context.HP = Context.GameStyle.StartingHP;
            Context.Money = Context.GameStyle.StartingMoney;
        }

        public GameEngine(GameContext fromContext)
        {
            Context = fromContext;
            _mainLoopTimer = new GameTimer(TimeSpan.FromMilliseconds(30), MainLoop);
            _challengeTimer = new GameTimer(TimeSpan.FromSeconds(1), CheckChallenge);

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

            Initialize();

//#if DEBUG
//            CheatsHelper.Cheats.Add(CheatTypes.InfiniteMoney);
//            CheatsHelper.Cheats.Add(CheatTypes.MaxWaves);
//#endif
        }

        public void Initialize()
        {
            foreach (var module in GameModules)
                module.Initialize();
        }

        public void Update(TimeSpan passed)
        {
            if (Running)
            {
                _mainLoopTimer.Update(passed);
                if (Context.Challenge != null)
                    _challengeTimer.Update(passed);
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

        private void CheckChallenge()
        {
            if (Context.Challenge != null && Context.Challenge.IsValid(Context.Stats))
            {
                Running = false;
                GameOver = true;
                Result = GameResult.ChallengeSuccess;
            }
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
                if (Context.Challenge != null)
                    Result = GameResult.ChallengeLost;
                else
                    Result = GameResult.NormalLost;
            }
        }
    }
}
