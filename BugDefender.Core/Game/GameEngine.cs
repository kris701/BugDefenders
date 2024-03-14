using BugDefender.Core.Game.Helpers;
using BugDefender.Core.Game.Modules;
using BugDefender.Core.Game.Modules.Enemies;
using BugDefender.Core.Game.Modules.Projectiles;
using BugDefender.Core.Game.Modules.Turrets;
using BugDefender.Core.Resources;
using BugDefender.Core.Users.Models.Challenges;
using BugDefender.Core.Users.Models.SavedGames;
using BugDefender.Core.Users.Models.UserCriterias;

namespace BugDefender.Core.Game
{
    public delegate void GameEventHandler();

    public partial class GameEngine
    {
        public GameEventHandler? OnPlayerDamaged;

        private readonly GameTimer _mainLoopTimer;
        private readonly GameTimer _criteriaTimer;

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
        public List<IUserCriteria> Criterias { get; set; } = new List<IUserCriteria>();

        public GameEngine(ISavedGame save)
        {
            Context = save.Context;
            if (save is ChallengeSavedGame challengeSave)
                Criterias = ResourceManager.Challenges.GetResource(challengeSave.ChallengeID).Criterias;
            if (save is CampainSavedGame campainSave)
            {
                var campain = ResourceManager.Campains.GetResource(campainSave.CampainID);
                var chapter = campain.Chapters.First(x => x.ID == campainSave.ChapterID);
                Criterias = chapter.Criterias;
            }

            _mainLoopTimer = new GameTimer(TimeSpan.FromMilliseconds(30), MainLoop);
            _criteriaTimer = new GameTimer(TimeSpan.FromSeconds(1), CheckCriterias);

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

            if (Context.GameStyle.EnemyBlackList.Count != 0 && Context.GameStyle.EnemyWhiteList.Count != 0 ||
                Context.GameStyle.TurretBlackList.Count != 0 && Context.GameStyle.TurretWhiteList.Count != 0)
                throw new Exception("Cant have a black list and a white list at the same time!");

            Context.Stats.MoneyEarned(Context.GameStyle.StartingMoney);

            Initialize();
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
                if (Criterias.Count > 0)
                    _criteriaTimer.Update(passed);
                Context.GameTime += passed;
            }
        }

        private void MainLoop(TimeSpan passed)
        {
            foreach (var module in GameModules)
                module.Update(_mainLoopTimer.Target);

            if (CheatsHelper.Cheats.Contains(CheatTypes.InfiniteMoney))
                Context.Money = 99999999;
            if (CheatsHelper.Cheats.Contains(CheatTypes.MaxWaves))
                Context.Wave = 99999999;
        }

        private void CheckCriterias(TimeSpan passed)
        {
            foreach (var criteria in Criterias)
                if (!criteria.IsValid(Context.Stats))
                    return;
            Running = false;
            GameOver = true;
            Result = GameResult.Success;
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
                Result = GameResult.Lost;
            }
        }
    }
}
