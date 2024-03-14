using BugDefender.Core.Game;
using BugDefender.Core.Game.Models;
using BugDefender.Core.Game.Models.GameStyles;
using BugDefender.Core.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BugDefender.Core.Campain.Models
{
    public class CampainDefinition : IDefinition
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<ChapterDefinition> Chapters { get; set; }
        public Guid BaseGameStyle { get; set; }
        public GameStyleDefinition? GameStyle { get; set; }
        public int Reward { get; set; }

        public CutsceneDefinition CampainOver { get; set; }

        [JsonConstructor]
        public CampainDefinition(Guid iD, string name, string description, List<ChapterDefinition> chapters, Guid baseGameStyle, GameStyleDefinition? gameStyle, CutsceneDefinition campainOver, int reward)
        {
            ID = iD;
            Name = name;
            Description = description;
            Chapters = chapters;
            BaseGameStyle = baseGameStyle;
            GameStyle = gameStyle;
            if (GameStyle == null)
                GameStyle = ResourceManager.GameStyles.GetResource(BaseGameStyle);
            CampainOver = campainOver;
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
