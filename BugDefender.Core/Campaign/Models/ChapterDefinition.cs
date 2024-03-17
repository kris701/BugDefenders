using BugDefender.Core.Game;
using BugDefender.Core.Resources;
using BugDefender.Core.Users.Models;
using BugDefender.Core.Users.Models.UserCriterias;

namespace BugDefender.Core.Campaign.Models
{
    public class ChapterDefinition
    {
        public Guid ID { get; set; }
        public Guid? NextChapterID { get; set; }
        public string Name { get; set; }
        public Guid MapID { get; set; }
        public CutsceneDefinition Intro { get; set; }
        public Guid GameStyleID { get; set; }
        public List<IUserCriteria> Criterias { get; set; }

        public ChapterDefinition(Guid iD, Guid? nextChapterID, string name, Guid mapID, CutsceneDefinition intro, Guid gameStyleID, List<IUserCriteria> criterias)
        {
            ID = iD;
            NextChapterID = nextChapterID;
            Name = name;
            MapID = mapID;
            Intro = intro;
            GameStyleID = gameStyleID;
            Criterias = criterias;
        }

        public bool IsValid(StatsDefinition stats)
        {
            foreach (var criteria in Criterias)
                if (!criteria.IsValid(stats))
                    return false;
            return true;
        }

        public GameContext GetContextForChapter()
        {
            return new GameContext(
                ResourceManager.Maps.GetResource(MapID),
                ResourceManager.GameStyles.GetResource(GameStyleID));
        }
    }
}
