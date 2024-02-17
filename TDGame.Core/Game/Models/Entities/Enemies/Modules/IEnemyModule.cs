using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TDGame.Core.Game.Models;

namespace TDGame.Core.Game.Models.Entities.Enemies.Modules
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "EnemyModule")]
    [JsonDerivedType(typeof(WaveEnemyDefinition), typeDiscriminator: "Wave")]
    [JsonDerivedType(typeof(SingleEnemyDefinition), typeDiscriminator: "Single")]
    public interface IEnemyModule : IModuleInfo<IEnemyModule>
    {
    }
}
