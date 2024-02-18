using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.Core.Game.Models;
using TDGame.Core.Users.Models.Buffs;
using TDGame.Core.Users.Models.UpgradeRequirements;

namespace TDGame.Core.Users.Models
{
    public class BuffUpgrade : IDefinition
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid? Requires { get; set; }

        public IBuffCriteria Criteria { get; set; }
        public IBuff Effect { get; set; }
    }
}
