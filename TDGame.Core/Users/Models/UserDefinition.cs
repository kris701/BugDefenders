using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.Core.Game.Models;
using TDGame.Core.Users.Models.Buffs;

namespace TDGame.Core.Users.Models
{
    public class UserDefinition<T>
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public List<IBuff> Buffs { get; set; } = new List<IBuff>();
        public List<ScoreDefinition> HighScores { get; set; } = new List<ScoreDefinition>();
        public bool IsPrimary { get; set; } = false;
        public StatsDefinition Stats { get; set; } = new StatsDefinition();
        public T UserData { get; set; }
    }
}
