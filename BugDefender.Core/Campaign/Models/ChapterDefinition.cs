using BugDefender.Core.Game.Models.GameStyles;
using BugDefender.Core.Models;
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

        public List<IUserCriteria> Criterias { get; set; }
        public UpgradeEffectModel GameStyleEffect { get; set; }
        public List<Guid> AddsTurrets { get; set; }
        public List<Guid> AddsEnemies { get; set; }

        public ChapterDefinition(Guid iD, Guid? nextChapterID, string name, Guid mapID, CutsceneDefinition intro, List<IUserCriteria> criterias, UpgradeEffectModel gameStyleEffect, List<Guid> addsTurrets, List<Guid> addsEnemies)
        {
            ID = iD;
            NextChapterID = nextChapterID;
            Name = name;
            MapID = mapID;
            Intro = intro;
            Criterias = criterias;
            GameStyleEffect = gameStyleEffect;
            AddsTurrets = addsTurrets;
            AddsEnemies = addsEnemies;
        }

        public bool IsValid(StatsDefinition stats)
        {
            foreach (var criteria in Criterias)
                if (!criteria.IsValid(stats))
                    return false;
            return true;
        }

        public void Apply(GameStyleDefinition on)
        {
            on.TurretWhiteList.AddRange(AddsTurrets);
            on.EnemyWhiteList.AddRange(AddsEnemies);
            GameStyleEffect.ApplyUpgradeEffectOnObject(on);
        }
    }
}
