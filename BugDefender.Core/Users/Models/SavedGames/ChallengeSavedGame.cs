using BugDefender.Core.Game;

namespace BugDefender.Core.Users.Models.SavedGames
{
    public class ChallengeSavedGame : ISavedGame
    {
        public string Name { get; set; }
        public Guid ChallengeID { get; set; }
        public DateTime Date { get; set; }
        public GameContext Context { get; set; }

        public ChallengeSavedGame(string name, Guid challengeID, DateTime date, GameContext context)
        {
            Name = name;
            ChallengeID = challengeID;
            Date = date;
            Context = context;
        }
    }
}
