using BugDefender.Core.Campain.Models;
using BugDefender.Core.Game;
using BugDefender.Core.Resources;
using BugDefender.Core.Users.Models;
using BugDefender.Core.Users.Models.SavedGames;
using BugDefender.OpenGL.Engine.Helpers;
using BugDefender.OpenGL.Engine.Views;
using BugDefender.OpenGL.Screens.CutsceneView;
using BugDefender.OpenGL.Screens.GameScreen;
using BugDefender.OpenGL.Screens.MainMenu;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            else if (gameSave is CampainSavedGame campainGame)
            {
                if (campainGame.IsCompleted)
                    return;
                var campain = ResourceManager.Campains.GetResource(campainGame.CampainID);
                if (campainGame.ChapterID == Guid.Empty)
                {
                    var currentChapter = campain.Chapters[0];
                    campainGame.ChapterID = currentChapter.ID;
                    from.SwitchView(new CutsceneView(Parent, campainGame, currentChapter.Intro, OnCampainConversationOver));
                }
                else
                {
                    from.SwitchView(new GameScreen(
                        Parent,
                        gameSave,
                        OnCampainGameOver));
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

        private void OnCampainGameOver(IView view, GameEngine game, ISavedGame gameSave)
        {
            if (gameSave is CampainSavedGame campainSave)
            {
                var result = game.Result;
#if RELEASE
            if (CheatsHelper.Cheats.Count == 0)
            {
#endif
                var campain = ResourceManager.Campains.GetResource(campainSave.CampainID);
                var currentChapter = campain.Chapters.First(x => x.ID == campainSave.ChapterID);
                if (result == GameResult.Success)
                {
                    campainSave.Stats.Combine(game.Context.Stats);
                    AddStatsToUser(game.Context.Stats);
                    Parent.UserManager.SaveUser();
                    if (currentChapter.NextChapterID == null)
                    {
                        campainSave.ChapterID = Guid.Empty;
                        campainSave.IsCompleted = true;
                        campainSave.Context = null;
                        Parent.UserManager.SaveGame(gameSave);
                        Parent.UserManager.CurrentUser.Credits += campain.Reward;
                        Parent.UserManager.SaveUser();
                        view.SwitchView(new CutsceneView(Parent, campainSave, campain.CampainOver, OnCampainConversationOver));
                    }
                    else
                    {
                        currentChapter = campain.Chapters.First(x => x.ID == currentChapter.NextChapterID);
                        campainSave.ChapterID = currentChapter.ID;
                        gameSave.Context = campain.GetContextForChapter(currentChapter);
                        Parent.UserManager.SaveGame(gameSave);
                        view.SwitchView(new CutsceneView(Parent, campainSave, currentChapter.Intro, OnCampainConversationOver));
                    }
                }
                else
                {
                    gameSave.Context = campain.GetContextForChapter(currentChapter);
                    Parent.UserManager.SaveGame(gameSave);
                }
#if RELEASE
            }
#endif
                var screen = GameScreenHelper.TakeScreenCap(Parent.GraphicsDevice, Parent);
                view.SwitchView(new Screens.GameOverScreen.GameOverView(Parent, screen, campainSave.Stats, "Campaign Lost!"));
            }
        }

        private void OnCampainConversationOver(IView view, ISavedGame gameSave)
        {
            if (gameSave is CampainSavedGame campainSave)
            {
                if (campainSave.ChapterID == Guid.Empty)
                {
                    var screen = Parent.TextureController.GetTexture(campainSave.CampainID);
                    view.SwitchView(new Screens.GameOverScreen.GameOverView(Parent, screen, campainSave.Stats, "Campaign Won!"));
                }
                else
                {
                    var campain = ResourceManager.Campains.GetResource(campainSave.CampainID);
                    var currentChapter = campain.Chapters.First(x => x.ID == campainSave.ChapterID);
                    gameSave.Context = campain.GetContextForChapter(currentChapter);
                    Parent.UserManager.SaveGame(gameSave);
                    view.SwitchView(new GameScreen(
                        Parent,
                        gameSave,
                        OnCampainGameOver));
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
