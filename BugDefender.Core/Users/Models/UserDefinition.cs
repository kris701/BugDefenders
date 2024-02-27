namespace BugDefender.Core.Users.Models
{
    public class UserDefinition<T>
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public List<Guid> Buffs { get; set; } = new List<Guid>();
        public List<Guid> Achivements { get; set; } = new List<Guid>();
        public List<ScoreDefinition> HighScores { get; set; } = new List<ScoreDefinition>();
        public bool IsPrimary { get; set; } = false;
        public StatsDefinition Stats { get; set; } = new StatsDefinition();
        public T UserData { get; set; }

        public UserDefinition(Guid iD, string name, List<Guid> buffs, List<Guid> achivements, List<ScoreDefinition> highScores, bool isPrimary, StatsDefinition stats, T userData)
        {
            ID = iD;
            Name = name;
            Buffs = buffs;
            Achivements = achivements;
            HighScores = highScores;
            IsPrimary = isPrimary;
            Stats = stats;
            UserData = userData;
        }
    }
}
