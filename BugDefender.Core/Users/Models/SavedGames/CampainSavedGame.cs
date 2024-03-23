using BugDefender.Core.Game;
using BugDefender.Core.Resources;

namespace BugDefender.Core.Users.Models.SavedGames
{
    public class CampaignSavedGame : ISavedGame
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public GameContext Context { get; set; }
        public Guid CampaignID { get; set; }
        public Guid ChapterID { get; set; }
        public bool SeenIntro { get; set; }
        public StatsDefinition Stats { get; set; }
        public bool IsCompleted { get; set; }

        public CampaignSavedGame(string name, DateTime date, GameContext context, Guid campaignID, Guid chapterID, bool seenIntro, StatsDefinition stats, bool isCompleted)
        {
            Name = name;
            Date = date;
            Context = context;
            CampaignID = campaignID;
            ChapterID = chapterID;
            SeenIntro = seenIntro;
            Stats = stats;
            IsCompleted = isCompleted;
        }

        public override string ToString()
        {
            var campaign = ResourceManager.Campaigns.GetResource(CampaignID);
            var chapter = campaign.Chapters.First(x => x.ID == ChapterID);
            return $"({campaign.Name}) {Name}, {chapter.Name}, {Date}";
        }
    }
}
