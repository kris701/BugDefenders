using BugDefender.Core.Users.Models.SavedGames;
using System.Text.Json.Serialization;

namespace BugDefender.Core.Users.Models
{
    public class UserDefinition<T> where T : new()
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public List<Guid> Buffs { get; set; } = new List<Guid>();
        public List<Guid> Achivements { get; set; } = new List<Guid>();
        public List<Guid> CompletedChallenges { get; set; } = new List<Guid>();
        public int ChallengeDaySeed { get; set; } = 0;
        public List<ScoreDefinition> HighScores { get; set; } = new List<ScoreDefinition>();
        public bool IsPrimary { get; set; } = false;
        public StatsDefinition Stats { get; set; } = new StatsDefinition();
        public int Credits { get; set; }
        public T UserData { get; set; }
        public List<ISavedGame> SavedGames { get; set; } = new List<ISavedGame>();

        public UserDefinition(string name)
        {
            ID = Guid.NewGuid();
            Name = name;
            Buffs = new List<Guid>();
            Achivements = new List<Guid>();
            CompletedChallenges = new List<Guid>();
            ChallengeDaySeed = 0;
            HighScores = new List<ScoreDefinition>();
            IsPrimary = false;
            Stats = new StatsDefinition();
            Credits = 0;
            UserData = new T();
            SavedGames = new List<ISavedGame>();
        }

        [JsonConstructor]
        public UserDefinition(Guid iD, string name, List<Guid> buffs, List<Guid> achivements, List<Guid> completedChallenges, int challengeDaySeed, List<ScoreDefinition> highScores, bool isPrimary, StatsDefinition stats, int credits, T userData, List<ISavedGame> savedGames)
        {
            ID = iD;
            Name = name;
            Buffs = buffs;
            Achivements = achivements;
            CompletedChallenges = completedChallenges;
            ChallengeDaySeed = challengeDaySeed;
            HighScores = highScores;
            IsPrimary = isPrimary;
            Stats = stats;
            Credits = credits;
            UserData = userData;
            SavedGames = savedGames;
        }
    }
}
