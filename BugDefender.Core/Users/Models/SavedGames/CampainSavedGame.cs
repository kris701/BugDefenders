using BugDefender.Core.Game;

namespace BugDefender.Core.Users.Models.SavedGames
{
    public class CampaignSavedGame : ISavedGame
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public GameContext Context { get; set; }
        public Guid CampaignID { get; set; }
        public Guid ChapterID { get; set; }
        public StatsDefinition Stats { get; set; }
        public bool IsCompleted { get; set; }

        public CampaignSavedGame(string name, DateTime date, GameContext context, Guid campaignID, Guid chapterID, StatsDefinition stats, bool isCompleted)
        {
            Name = name;
            Date = date;
            Context = context;
            CampaignID = campaignID;
            ChapterID = chapterID;
            Stats = stats;
            IsCompleted = isCompleted;
        }
    }
}
