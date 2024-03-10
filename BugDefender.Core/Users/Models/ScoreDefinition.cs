namespace BugDefender.Core.Users.Models
{
    public class ScoreDefinition
    {
        public int Score { get; set; }
        public string GameTime { get; set; }
        public string Date { get; set; }
        public float DifficultyRating { get; set; }

        public ScoreDefinition(int score, string gameTime, string date, float difficultyRating)
        {
            Score = score;
            GameTime = gameTime;
            Date = date;
            DifficultyRating = difficultyRating;
        }
    }
}
