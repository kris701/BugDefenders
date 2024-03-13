using BugDefender.Core.Models;
using BugDefender.Core.Users.Models;
using BugDefender.Core.Users.Models.UserCriterias;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugDefender.Core.Campain.Models
{
    public class ChapterDefinition
    {
        public Guid ID { get; set; }
        public Guid? NextChapterID { get; set; }
        public string Name { get; set; }
        public Guid MapID { get; set; }
        public CutsceneDefinition Intro { get; set; }

        public List<IUserCriteria> Criterias { get; set; }
        public UpgradeEffectModel Effect { get; set; }

        public ChapterDefinition(Guid iD, Guid? nextChapterID, string name, Guid mapID, CutsceneDefinition intro, List<IUserCriteria> criterias, UpgradeEffectModel effect)
        {
            ID = iD;
            NextChapterID = nextChapterID;
            Name = name;
            MapID = mapID;
            Intro = intro;
            Criterias = criterias;
            Effect = effect;
        }

        public bool IsValid(StatsDefinition stats)
        {
            foreach (var criteria in Criterias)
                if (!criteria.IsValid(stats))
                    return false;
            return true;
        }
    }
}
