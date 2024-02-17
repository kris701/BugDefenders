using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TDGame.Core.Users.Models.Buffs
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "Target")]
    [JsonDerivedType(typeof(EnemyBuff), typeDiscriminator: "Enemy")]
    [JsonDerivedType(typeof(TurretBuff), typeDiscriminator: "Turret")]
    public interface IBuff
    {
    }
}
