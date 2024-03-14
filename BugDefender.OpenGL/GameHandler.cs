using BugDefender.Core.Campain.Models;
using BugDefender.Core.Game;
using BugDefender.Core.Resources;
using BugDefender.Core.Users.Models.SavedGames;
using BugDefender.OpenGL.Engine.Helpers;
using BugDefender.OpenGL.Engine.Views;
using BugDefender.OpenGL.Screens.CampainOverView;
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
            var credits = 0;
            var result = game.Result;
#if RELEASE
            if (CheatsHelper.Cheats.Count == 0)
            {
#endif
            if (gameSave is ChallengeSavedGame c)
            {
                var challenge = ResourceManager.Challenges.GetResource(c.ChallengeID);
                credits += challenge.Reward;
                Parent.UserManager.CurrentUser.CompletedChallenges.Add(c.ChallengeID);
            }

            if (result == GameResult.Success)
            {
                Parent.UserManager.CurrentUser.Stats.Combine(game.Context.Stats);
                credits += game.Context.Score / 100;
                Parent.UserManager.CurrentUser.Credits += credits;
                Parent.UserManager.CheckAndApplyAchivements();
                Parent.UserManager.SaveUser();
            }
#if RELEASE
            }
#endif
            Parent.UserManager.RemoveGame(gameSave);
            var screen = GameScreenHelper.TakeScreenCap(Parent.GraphicsDevice, Parent);
            view.SwitchView(new Screens.GameOverScreen.GameOverView(Parent, screen, game.Context, credits, game.Context.Map.GetDifficultyRating() * game.Context.GameStyle.GetDifficultyRating(), "Challenge Over!"));
        }

        private void OnSurvivalGameOver(IView view, GameEngine game, ISavedGame gameSave)
        {
            var credits = 0;
            var result = game.Result;
#if RELEASE
            if (CheatsHelper.Cheats.Count == 0)
            {
#endif
            if (result == GameResult.Success)
            {
                Parent.UserManager.CurrentUser.Stats.Combine(game.Context.Stats);
                credits += game.Context.Score / 100;
                Parent.UserManager.CurrentUser.Credits += credits;
                Parent.UserManager.CheckAndApplyAchivements();
                Parent.UserManager.SaveUser();
            }
#if RELEASE
            }
#endif
            Parent.UserManager.RemoveGame(gameSave);
            var screen = GameScreenHelper.TakeScreenCap(Parent.GraphicsDevice, Parent);
            view.SwitchView(new Screens.GameOverScreen.GameOverView(Parent, screen, game.Context, credits, game.Context.Map.GetDifficultyRating() * game.Context.GameStyle.GetDifficultyRating(), "Challenge Over!"));
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
                    Parent.UserManager.CurrentUser.Stats.Combine(game.Context.Stats);
                    Parent.UserManager.CheckAndApplyAchivements();
                    Parent.UserManager.SaveUser();
                    if (currentChapter.NextChapterID == null)
                    {
                        campainSave.ChapterID = Guid.Empty;
                        campainSave.IsCompleted = true;
                        campainSave.Context = null;
                        Parent.UserManager.SaveGame(gameSave);
                        view.SwitchView(new CutsceneView(Parent, campainSave, campain.CampainOver, OnCampainConversationOver));
                    }
                    else
                    {
                        currentChapter = campain.Chapters.First(x => x.ID == currentChapter.NextChapterID);
                        campainSave.ChapterID = currentChapter.ID;
                        gameSave.Context = GetContextForChapter(campain, currentChapter);
                        Parent.UserManager.SaveGame(gameSave);
                        view.SwitchView(new CutsceneView(Parent, campainSave, currentChapter.Intro, OnCampainConversationOver));
                    }
                }
                else
                {
                    gameSave.Context = GetContextForChapter(campain, currentChapter);
                    Parent.UserManager.SaveGame(gameSave);
                }
#if RELEASE
            }
#endif
                var screen = GameScreenHelper.TakeScreenCap(Parent.GraphicsDevice, Parent);
                view.SwitchView(new CampainOverView(Parent, campainSave, false));
            }
        }

        private void OnCampainConversationOver(IView view, ISavedGame gameSave)
        {
            if (gameSave is CampainSavedGame campainSave)
            {
                if (campainSave.ChapterID == Guid.Empty)
                {
                    view.SwitchView(new CampainOverView(Parent, campainSave, true));
                }
                else
                {
                    var campain = ResourceManager.Campains.GetResource(campainSave.CampainID);
                    var currentChapter = campain.Chapters.First(x => x.ID == campainSave.ChapterID);
                    gameSave.Context = GetContextForChapter(campain, currentChapter);
                    Parent.UserManager.SaveGame(gameSave);
                    view.SwitchView(new GameScreen(
                        Parent,
                        gameSave,
                        OnCampainGameOver));
                }
            }
        }

        private GameContext GetContextForChapter(CampainDefinition campain, ChapterDefinition currentChapter)
        {
            var baseGameStyle = ResourceManager.GameStyles.GetResource(campain.BaseGameStyle);
            foreach (var chapter in campain.Chapters)
            {
                if (chapter.ID == currentChapter.ID)
                    break;
                chapter.Apply(baseGameStyle);
            }
            return new GameContext(
                ResourceManager.Maps.GetResource(currentChapter.MapID),
                baseGameStyle);
        }
    }
}
