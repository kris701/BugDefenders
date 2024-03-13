using BugDefender.Core.Campain.Models;
using BugDefender.Core.Game;
using BugDefender.Core.Resources;
using BugDefender.Core.Users.Models.SavedGames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BugDefender.Core.Campain
{
    public class CampainEngine
    {
        public CampainDefinition CurrentCampain { get; }
        public GameEngine? CurrentGame { get; private set; }
        public ChapterDefinition? CurrentChapter { get; private set; }

        public CampainEngine(Guid campainID)
        {
            CurrentCampain = ResourceManager.Campains.GetResource(campainID);
            if (CurrentCampain.GameStyle == null)
                throw new Exception("Gamestyle was not set correctly!");
            CurrentChapter = CurrentCampain.Chapters[0];
            CurrentGame = new GameEngine(new GameContext(
                ResourceManager.Maps.GetResource(CurrentChapter.MapID),
                CurrentCampain.GameStyle),
                CurrentChapter.Criterias
                );
        }

        public CampainEngine(CampainSavedGame game)
        {
            CurrentCampain = ResourceManager.Campains.GetResource(game.CampainID);
            if (CurrentCampain.GameStyle == null)
                throw new Exception("Gamestyle was not set correctly!");
            foreach(var chapter in CurrentCampain.Chapters)
            {
                if (chapter.ID == game.ChapterID)
                    break;

                chapter.Effect.ApplyUpgradeEffectOnObject(CurrentCampain.GameStyle);
                CurrentChapter = chapter;
            }
            CurrentGame = new GameEngine(game.Context);
        }

        public void GameOver()
        {
            if (CurrentGame == null)
                throw new NullReferenceException("Game is null");
            if (CurrentChapter == null)
                throw new NullReferenceException("Chapter is null");
            if (!CurrentGame.GameOver)
                throw new Exception("Game is not over!");

            if (CurrentGame.Result == GameResult.Lost)
            {
                CurrentGame = null;
                return;
            }
            if (CurrentGame.Result == GameResult.Success && !CurrentChapter.IsValid(CurrentGame.Context.Stats))
                throw new Exception("Chapter requirements not valid!");

            if (CurrentChapter.NextChapterID == null)
            {

            }
            else
            {
                CurrentChapter = CurrentCampain.Chapters.First(x => x.ID == CurrentChapter.NextChapterID);
            }
        }
    }
}
