using BugDefender.Core.Game;
#if RELEASE
using BugDefender.Core.Game.Helpers;
#endif
using BugDefender.Core.Resources;
using BugDefender.Core.Users.Models;
using BugDefender.Core.Users.Models.SavedGames;
using BugDefender.OpenGL.Engine.Helpers;
using BugDefender.OpenGL.Engine.Views;
using BugDefender.OpenGL.Screens.CutsceneView;
using BugDefender.OpenGL.Screens.GameScreen;
using System;
using System.Linq;

namespace BugDefender.OpenGL
{
    public class GameHandler
    {
        public BugDefenderGameWindow Parent { get; set; }

        public GameHandler(BugDefenderGameWindow parent)
        {
            Parent = parent;
        }

        public void LoadGame(IView from, ISavedGame gameSave)
        {
            if (gameSave is SurvivalSavedGame survivalGame)
            {
                from.SwitchView(new GameScreen(
                    Parent,
                    gameSave,
                    OnSurvivalGameOver));
            }
            else if (gameSave is ChallengeSavedGame challengeGame)
            {
                from.SwitchView(new GameScreen(
                    Parent,
                    gameSave,
                    OnChallengeOver));
            }
            else if (gameSave is CampaignSavedGame campaignGame)
            {
                if (campaignGame.IsCompleted)
                    return;
                var campaign = ResourceManager.Campaigns.GetResource(campaignGame.CampaignID);
                if (campaignGame.ChapterID == Guid.Empty)
                {
                    var currentChapter = campaign.Chapters[0];
                    campaignGame.ChapterID = currentChapter.ID;
                    from.SwitchView(new CutsceneView(Parent, campaignGame, campaign, currentChapter.Intro, OnCampaignConversationOver));
                }
                else
                {
                    from.SwitchView(new GameScreen(
                        Parent,
                        gameSave,
                        OnCampaignGameOver));
                }
            }
        }

        private void OnChallengeOver(IView view, GameEngine game, ISavedGame gameSave)
        {
            var result = game.Result;
#if RELEASE
            if (CheatsHelper.Cheats.Count == 0)
            {
#endif
            if (result == GameResult.Success)
            {
                if (gameSave is ChallengeSavedGame c)
                {
                    var challenge = ResourceManager.Challenges.GetResource(c.ChallengeID);
                    Parent.UserManager.CurrentUser.Credits += challenge.Reward;
                    Parent.UserManager.CurrentUser.CompletedChallenges.Add(c.ChallengeID);
                }

                AddStatsToUser(game.Context.Stats);
                Parent.UserManager.SaveUser();
            }
#if RELEASE
            }
#endif
            Parent.UserManager.RemoveGame(gameSave);
            var screen = GameScreenHelper.TakeScreenCap(Parent.GraphicsDevice, Parent);
            view.SwitchView(new Screens.GameOverScreen.GameOverView(Parent, screen, game.Context.Stats, "Challenge Over!"));
        }

        private void OnSurvivalGameOver(IView view, GameEngine game, ISavedGame gameSave)
        {
            var result = game.Result;
#if RELEASE
            if (CheatsHelper.Cheats.Count == 0)
            {
#endif
            if (result == GameResult.Success)
            {
                AddStatsToUser(game.Context.Stats);
                AddToHighScore(game.Context.Stats);
                Parent.UserManager.SaveUser();
            }
#if RELEASE
            }
#endif
            Parent.UserManager.RemoveGame(gameSave);
            var screen = GameScreenHelper.TakeScreenCap(Parent.GraphicsDevice, Parent);
            view.SwitchView(new Screens.GameOverScreen.GameOverView(Parent, screen, game.Context.Stats, "Game Over!"));
        }

        private void OnCampaignGameOver(IView view, GameEngine game, ISavedGame gameSave)
        {
            if (gameSave is CampaignSavedGame campaignSave)
            {
                var result = game.Result;
#if RELEASE
            if (CheatsHelper.Cheats.Count == 0)
            {
#endif
                var campaign = ResourceManager.Campaigns.GetResource(campaignSave.CampaignID);
                var currentChapter = campaign.Chapters.First(x => x.ID == campaignSave.ChapterID);
                if (result == GameResult.Success)
                {
                    campaignSave.Stats.Combine(game.Context.Stats);
                    AddStatsToUser(game.Context.Stats);
                    Parent.UserManager.SaveUser();
                    if (currentChapter.NextChapterID == null)
                    {
                        campaignSave.ChapterID = Guid.Empty;
                        campaignSave.IsCompleted = true;
                        campaignSave.Context = null;
                        campaignSave.Stats.Credits += campaign.Reward;
                        Parent.UserManager.SaveGame(gameSave);
                        Parent.UserManager.CurrentUser.Credits += campaign.Reward;
                        Parent.UserManager.SaveUser();
                        view.SwitchView(new CutsceneView(Parent, campaignSave, campaign, campaign.CampaignOver, OnCampaignConversationOver));
                    }
                    else
                    {
                        currentChapter = campaign.Chapters.First(x => x.ID == currentChapter.NextChapterID);
                        campaignSave.ChapterID = currentChapter.ID;
                        gameSave.Context = currentChapter.GetContextForChapter();
                        Parent.UserManager.SaveGame(gameSave);
                        view.SwitchView(new CutsceneView(Parent, campaignSave, campaign, currentChapter.Intro, OnCampaignConversationOver));
                    }
                }
                else
                {
                    gameSave.Context = currentChapter.GetContextForChapter();
                    Parent.UserManager.SaveGame(gameSave);
                }
#if RELEASE
            }
#endif
                var screen = GameScreenHelper.TakeScreenCap(Parent.GraphicsDevice, Parent);
                view.SwitchView(new Screens.GameOverScreen.GameOverView(Parent, screen, campaignSave.Stats, "Campaign Lost!"));
            }
        }

        private void OnCampaignConversationOver(IView view, ISavedGame gameSave)
        {
            if (gameSave is CampaignSavedGame campaignSave)
            {
                if (campaignSave.IsCompleted)
                {
                    var screen = Parent.TextureController.GetTexture(campaignSave.CampaignID);
                    view.SwitchView(new Screens.GameOverScreen.GameOverView(Parent, screen, campaignSave.Stats, "Campaign Won!"));
                }
                else
                {
                    var campaign = ResourceManager.Campaigns.GetResource(campaignSave.CampaignID);
                    var currentChapter = campaign.Chapters.First(x => x.ID == campaignSave.ChapterID);
                    gameSave.Context = currentChapter.GetContextForChapter();
                    Parent.UserManager.SaveGame(gameSave);
                    view.SwitchView(new GameScreen(
                        Parent,
                        gameSave,
                        OnCampaignGameOver));
                }
            }
        }

        private void AddStatsToUser(StatsDefinition stats)
        {
            Parent.UserManager.CurrentUser.Stats.Combine(stats);
            Parent.UserManager.CurrentUser.Credits += stats.Credits;
            Parent.UserManager.CheckAndApplyAchivements();
        }

        private void AddToHighScore(StatsDefinition stats)
        {
            Parent.UserManager.CurrentUser.HighScores.Add(new ScoreDefinition(
                    stats.Score,
                    stats.GameTime.ToString("hh\\:mm\\:ss"),
                    DateTime.Now.Date.ToShortDateString(),
                    stats.Difficulty
                ));
            if (Parent.UserManager.CurrentUser.HighScores.Count > 10)
            {
                var smallest = int.MaxValue;
                ScoreDefinition? smallestDef = null;
                foreach (var scoreDef in Parent.UserManager.CurrentUser.HighScores)
                {
                    if (scoreDef.Score < smallest)
                    {
                        smallestDef = scoreDef;
                        smallest = scoreDef.Score;
                    }
                }
                if (smallestDef != null)
                    Parent.UserManager.CurrentUser.HighScores.Remove(smallestDef);
            }
        }
    }
}
