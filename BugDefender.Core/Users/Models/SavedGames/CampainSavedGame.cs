using BugDefender.Core.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugDefender.Core.Users.Models.SavedGames
{
    public class CampainSavedGame : ISavedGame
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public GameContext? Context { get; set; }
        public Guid CampainID { get; set; }
        public Guid ChapterID { get; set; }
        public bool IsCompleted { get; set; }

        public CampainSavedGame(string name, DateTime date, GameContext? context, Guid campainID, Guid chapterID, bool isCompleted)
        {
            Name = name;
            Date = date;
            Context = context;
            CampainID = campainID;
            ChapterID = chapterID;
            IsCompleted = isCompleted;
        }
    }
}
