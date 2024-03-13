using BugDefender.Core.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugDefender.Core.Users.Models.SavedGames
{
    public class SurvivalSavedGame : ISavedGame
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public GameContext Context { get; set; }

        public SurvivalSavedGame(string name, DateTime date, GameContext context)
        {
            Name = name;
            Date = date;
            Context = context;
        }
    }
}
