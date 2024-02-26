using System.Text.Json.Serialization;

namespace TDGame.Core.Users.Models.UserCriterias
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "CriteriaType")]
    [JsonDerivedType(typeof(EnemiesKilledOfTypeCriteria), typeDiscriminator: "KillsOfType")]
    [JsonDerivedType(typeof(EnemiesKilledCriteria), typeDiscriminator: "Kills")]
    [JsonDerivedType(typeof(TurretsPlacedOfTypeCriteria), typeDiscriminator: "PlacedTurretsOfType")]
    [JsonDerivedType(typeof(TurretsPlacedCriteria), typeDiscriminator: "PlacedTurrets")]
    [JsonDerivedType(typeof(TurretKillsCriteria), typeDiscriminator: "TurretKills")]
    [JsonDerivedType(typeof(TurretKillsOfTypeCriteria), typeDiscriminator: "TurretKillsOfType")]
    public interface IUserCriteria
    {
        public bool IsValid(StatsDefinition stats);
    }
}
