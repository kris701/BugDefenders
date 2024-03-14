using BugDefender.Core.Game;
using BugDefender.Core.Resources;
using BugDefender.Core.Users.Models.SavedGames;
using BugDefender.OpenGL.Engine.Helpers;
using BugDefender.OpenGL.Engine.Views;
using BugDefender.OpenGL.Screens.GameScreen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugDefender.OpenGL.Views.GameView
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
            } else if (gameSave is ChallengeSavedGame challengeGame)
            {
                from.SwitchView(new GameScreen(
                    Parent,
                    gameSave,
                    OnChallengeOver));
            } else if (gameSave is CampainSavedGame campainGame)
            {
                from.SwitchView(new GameScreen(
                    Parent,
                    gameSave,
                    OnSurvivalGameOver));
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
                credits += (game.Context.Score / 100);
                Parent.UserManager.CurrentUser.Credits += credits;
                Parent.UserManager.CheckAndApplyAchivements();
                Parent.UserManager.SaveUser();
            }
#if RELEASE
            }
#endif
            Parent.UserManager.RemoveGame(gameSave);
            var screen = GameScreenHelper.TakeScreenCap(Parent.GraphicsDevice, Parent);
            view.SwitchView(new Screens.GameOverScreen.GameOverView(Parent, screen, game.Context, credits, game.Result, game.Context.Map.GetDifficultyRating() * game.Context.GameStyle.GetDifficultyRating(), "Challenge Over!"));
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
                credits += (game.Context.Score / 100);
                Parent.UserManager.CurrentUser.Credits += credits;
                Parent.UserManager.CheckAndApplyAchivements();
                Parent.UserManager.SaveUser();
            }
#if RELEASE
            }
#endif
            Parent.UserManager.RemoveGame(gameSave);
            var screen = GameScreenHelper.TakeScreenCap(Parent.GraphicsDevice, Parent);
            view.SwitchView(new Screens.GameOverScreen.GameOverView(Parent, screen, game.Context, credits, game.Result, game.Context.Map.GetDifficultyRating() * game.Context.GameStyle.GetDifficultyRating(), "Challenge Over!"));
        }
    }
}
