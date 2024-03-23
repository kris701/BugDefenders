using BugDefender.Core.Game;

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

        public override string ToString()
        {
            return $"(Survival) {Name}, {Context.Map.Name}, {Date}";
        }
    }
}
