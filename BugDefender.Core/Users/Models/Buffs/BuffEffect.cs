using BugDefender.Core.Game.Models.Entities.Upgrades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugDefender.Core.Users.Models.Buffs
{
    public class BuffEffect : UpgradeEffectModel
    {
        public string TargetType { get; set; }
        public Guid Target { get; set; }

        public BuffEffect(string targetType, Guid target, List<EffectTarget> effects) : base(effects)
        {
            TargetType = targetType;
            Target = target;
        }
    }
}
