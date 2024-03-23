using BugDefender.Core.Campaign.Models;
using BugDefender.Core.Game;
#if RELEASE
using BugDefender.Core.Game.Helpers;
#endif
using BugDefender.Core.Resources;
using BugDefender.Core.Users;
using BugDefender.Core.Users.Models;
using BugDefender.Core.Users.Models.SavedGames;

namespace BugDefender.Core
{
    public class GameManager<T> where T : new()
    {
        public delegate void GameStartedEvent(GameEngine game, ISavedGame savedGame);
        public event GameStartedEvent? OnGameStarted;
        public delegate void GameEndedEvent(StatsDefinition stats, ISavedGame savedGame, string title);
        public event GameEndedEvent? OnGameOver;

        public delegate void CutsceneStartedEvent(CutsceneDefinition cutscene, Dictionary<Guid, string> speakers, ISavedGame savedGame);
        public event CutsceneStartedEvent? OnCutsceneStarted;

        public UserEngine<T> UserManager { get; set; }

        private ISavedGame? _gameSave;
        private GameEngine? _game;

        public GameManager(UserEngine<T> userManager)
        {
            UserManager = userManager;
        }

        public void LoadGame(string name) => NewGame(UserManager.CurrentUser.SavedGames.First(x => x.Name == name));

        public void NewGame(ISavedGame savedGame)
        {
            _gameSave = savedGame;
            switch (_gameSave)
            {
                case SurvivalSavedGame g:
                    _game = new GameEngine(savedGame);
                    _game.OnGameOver += OnGameEngineFinished;
                    OnGameStarted?.Invoke(_game, g);
                    break;
                case ChallengeSavedGame g:
                    _game = new GameEngine(savedGame);
                    _game.OnGameOver += OnGameEngineFinished;
                    OnGameStarted?.Invoke(_game, g);
                    break;
                case CampaignSavedGame g:
                    var campaign = ResourceManager.Campaigns.GetResource(g.CampaignID);
                    if (g.IsCompleted)
                    {
                        if (!g.SeenIntro)
                        {
                            g.SeenIntro = true;
                            UserManager.SaveGame(g);
                            OnCutsceneStarted?.Invoke(campaign.CampaignOver, campaign.Speakers, g);
                        }
                        else
                            OnGameOver?.Invoke(g.Stats, g, "Campaign Won!");
                    }
                    else if (g.ChapterID == Guid.Empty)
                    {
                        var currentChapter = campaign.Chapters[0];
                        g.ChapterID = currentChapter.ID;
                        g.SeenIntro = true;
                        UserManager.SaveGame(g);
                        OnCutsceneStarted?.Invoke(currentChapter.Intro, campaign.Speakers, g);
                    }
                    else if (!g.SeenIntro)
                    {
                        g.SeenIntro = true;
                        var currentChapter = campaign.Chapters.First(x => x.ID == g.ChapterID);
                        UserManager.SaveGame(g);
                        OnCutsceneStarted?.Invoke(currentChapter.Intro, campaign.Speakers, g);
                    }
                    else
                    {
                        if (g.Context == null)
                        {
                            var currentChapter = campaign.Chapters.First(x => x.ID == g.ChapterID);
                            g.Context = currentChapter.GetContextForChapter();
                            UserManager.SaveGame(g);
                        }
                        _game = new GameEngine(savedGame);
                        _game.OnGameOver += OnGameEngineFinished;
                        OnGameStarted?.Invoke(_game, g);
                    }
                    break;
            }
        }

        private void OnGameEngineFinished()
        {
            if (_game != null && _game.GameOver && _gameSave != null)
            {
                switch (_gameSave)
                {
                    case SurvivalSavedGame g: OnSurvivalGameOver(_game, g); break;
                    case ChallengeSavedGame g: OnChallengeOver(_game, g); break;
                    case CampaignSavedGame g: OnCampaignGameOver(_game, g); break;
                }
            }
        }

        private void OnSurvivalGameOver(GameEngine game, SurvivalSavedGame save)
        {
#if RELEASE
            if (CheatsHelper.Cheats.Count == 0)
            {
#endif
            AddToHighScore(game.Context.Stats);
            if (game.Result == GameResult.Success)
            {
                AddStatsToUser(game.Context.Stats);
                UserManager.SaveUser();
            }
#if RELEASE
            }
#endif
            UserManager.RemoveGame(save);
            OnGameOver?.Invoke(game.Context.Stats, save, "Game Over!");
        }

        private void AddToHighScore(StatsDefinition stats)
        {
            UserManager.CurrentUser.HighScores.Add(new ScoreDefinition(
                    stats.Score,
                    stats.GameTime.ToString("hh\\:mm\\:ss"),
                    DateTime.Now.Date.ToShortDateString(),
                    stats.Difficulty
                ));
            if (UserManager.CurrentUser.HighScores.Count > 10)
            {
                var smallest = int.MaxValue;
                ScoreDefinition? smallestDef = null;
                foreach (var scoreDef in UserManager.CurrentUser.HighScores)
                {
                    if (scoreDef.Score < smallest)
                    {
                        smallestDef = scoreDef;
                        smallest = scoreDef.Score;
                    }
                }
                if (smallestDef != null)
                    UserManager.CurrentUser.HighScores.Remove(smallestDef);
            }
        }

        private void OnChallengeOver(GameEngine game, ChallengeSavedGame save)
        {
#if RELEASE
            if (CheatsHelper.Cheats.Count == 0)
            {
#endif
            if (game.Result == GameResult.Success)
            {
                var challenge = ResourceManager.Challenges.GetResource(save.ChallengeID);
                UserManager.CurrentUser.Credits += challenge.Reward;
                UserManager.CurrentUser.CompletedChallenges.Add(save.ChallengeID);
                AddStatsToUser(game.Context.Stats);
                UserManager.SaveUser();
                game.Context.Stats.Credits += challenge.Reward;
                UserManager.RemoveGame(save);
                OnGameOver?.Invoke(game.Context.Stats, save, "Challenge Won!");
                return;
            }
#if RELEASE
            }
#endif
            UserManager.RemoveGame(save);
            OnGameOver?.Invoke(game.Context.Stats, save, "Challenge Lost...");
        }

        private void AddStatsToUser(StatsDefinition stats)
        {
            UserManager.CurrentUser.Stats.Combine(stats);
            UserManager.CurrentUser.Credits += stats.Credits;
            UserManager.CheckAndApplyAchivements();
        }

        private void OnCampaignGameOver(GameEngine game, CampaignSavedGame save)
        {
            if (save.IsCompleted)
                return;
#if RELEASE
            if (CheatsHelper.Cheats.Count == 0)
            {
#endif
            var campaign = ResourceManager.Campaigns.GetResource(save.CampaignID);
            var currentChapter = campaign.Chapters.First(x => x.ID == save.ChapterID);
            if (game.Result == GameResult.Success)
            {
                save.Stats.Combine(game.Context.Stats);
                AddStatsToUser(game.Context.Stats);
                if (currentChapter.NextChapterID != null)
                {
                    save.SeenIntro = false;
                    save.Context = null;
                    save.ChapterID = (Guid)currentChapter.NextChapterID;
                    UserManager.SaveGame(save);
                    UserManager.SaveUser();
                    NewGame(save);
                    return;
                }
                else
                {
                    save.ChapterID = Guid.Empty;
                    save.IsCompleted = true;
                    save.Context = null;
                    save.Stats.Credits += campaign.Reward;
                    save.SeenIntro = false;
                    UserManager.SaveGame(save);
                    UserManager.CurrentUser.Credits += campaign.Reward;
                    UserManager.SaveUser();
                    NewGame(save);
                    return;
                }
            }
            else
            {
                save.Context = null;
                UserManager.SaveGame(save);
            }
#if RELEASE
            }
#endif
            game.Context.Stats.Combine(save.Stats);
            OnGameOver?.Invoke(save.Stats, save, "Campaign lost...");
        }
    }
}
