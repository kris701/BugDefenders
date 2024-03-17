using BugDefender.Core.Game.Models;
using System.Text.Json.Serialization;

namespace BugDefender.Core.Campaign.Models
{
    public class CampaignDefinition : IDefinition
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<ChapterDefinition> Chapters { get; set; }
        public Dictionary<Guid, string> Speakers { get; set; }
        public int Reward { get; set; }

        public CutsceneDefinition CampaignOver { get; set; }

        [JsonConstructor]
        public CampaignDefinition(Guid iD, string name, string description, Dictionary<Guid, string> speakers, List<ChapterDefinition> chapters, CutsceneDefinition campaignOver, int reward)
        {
            ID = iD;
            Name = name;
            Description = description;
            Speakers = speakers;
            Chapters = chapters;
            CampaignOver = campaignOver;
            Reward = reward;
        }
    }
}
