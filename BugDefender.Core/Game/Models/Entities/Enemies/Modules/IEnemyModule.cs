using System.Text.Json.Serialization;

namespace TDGame.Core.Game.Models.Entities.Enemies.Modules
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "EnemyModule")]
    [JsonDerivedType(typeof(WaveEnemyDefinition), typeDiscriminator: "Wave")]
    [JsonDerivedType(typeof(SingleEnemyDefinition), typeDiscriminator: "Single")]
    public interface IEnemyModule : IModuleInfo<IEnemyModule>
    {
    }
}
