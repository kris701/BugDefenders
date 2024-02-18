using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TDGame.Core.Game.Models.Entities.Enemies.Modules;

namespace TDGame.Core.Users.Models.Buffs.BuffEffects
{
    public class EnemyBuffEffect : IBuffEffect
    {
        public Guid EnemyID { get; set; }
        public IEnemyModule Module { get; set; }

        [JsonIgnore]
        public Guid BuffID { get; set; }
    }
}
