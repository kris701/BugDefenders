using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TDGame.Core.Game.Models;

namespace TDGame.Core.Users.Models.Buffs
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "BuffType")]
    [JsonDerivedType(typeof(EnemyBuff), typeDiscriminator: "Enemy")]
    [JsonDerivedType(typeof(TurretBuff), typeDiscriminator: "Turret")]
    public interface IBuff
    {
        [JsonIgnore]
        public Guid BuffID { get; set; }
    }
}
