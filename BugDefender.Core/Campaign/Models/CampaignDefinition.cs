using BugDefender.Core.Game;
using BugDefender.Core.Game.Models;
using BugDefender.Core.Game.Models.GameStyles;
using BugDefender.Core.Resources;
using System.Text.Json.Serialization;

namespace BugDefender.Core.Campaign.Models
{
    public class CampaignDefinition : IDefinition
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<ChapterDefinition> Chapters { get; set; }
        public Guid BaseGameStyle { get; set; }
        public GameStyleDefinition? GameStyle { get; set; }
        public int Reward { get; set; }

        public CutsceneDefinition CampaignOver { get; set; }

        [JsonConstructor]
        public CampaignDefinition(Guid iD, string name, string description, List<ChapterDefinition> chapters, Guid baseGameStyle, GameStyleDefinition? gameStyle, CutsceneDefinition campaignOver, int reward)
        {
            ID = iD;
            Name = name;
            Description = description;
            Chapters = chapters;
            BaseGameStyle = baseGameStyle;
            GameStyle = gameStyle;
            if (GameStyle == null)
                GameStyle = ResourceManager.GameStyles.GetResource(BaseGameStyle);
            CampaignOver = campaignOver;
            Reward = reward;
        }

        public GameContext GetContextForChapter(ChapterDefinition currentChapter)
        {
            var baseGameStyle = ResourceManager.GameStyles.GetResource(BaseGameStyle);
            foreach (var chapter in Chapters)
            {
                if (chapter.ID == currentChapter.ID)
                    break;
                chapter.Apply(baseGameStyle);
            }
            return new GameContext(
                ResourceManager.Maps.GetResource(currentChapter.MapID),
                baseGameStyle);
        }
    }
}
