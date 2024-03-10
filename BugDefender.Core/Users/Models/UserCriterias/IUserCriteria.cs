using System.Text.Json.Serialization;

namespace BugDefender.Core.Users.Models.UserCriterias
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "CriteriaType")]
    [JsonDerivedType(typeof(EnemiesKilledOfTypeCriteria), typeDiscriminator: "KillsOfType")]
    [JsonDerivedType(typeof(EnemiesKilledCriteria), typeDiscriminator: "Kills")]
    [JsonDerivedType(typeof(TurretsPlacedOfTypeCriteria), typeDiscriminator: "PlacedTurretsOfType")]
    [JsonDerivedType(typeof(TurretsPlacedCriteria), typeDiscriminator: "PlacedTurrets")]
    [JsonDerivedType(typeof(TurretsSoldOfTypeCriteria), typeDiscriminator: "SoldTurretsOfType")]
    [JsonDerivedType(typeof(TurretsSoldCriteria), typeDiscriminator: "SoldTurrets")]
    [JsonDerivedType(typeof(TurretKillsCriteria), typeDiscriminator: "TurretKills")]
    [JsonDerivedType(typeof(TurretKillsOfTypeCriteria), typeDiscriminator: "TurretKillsOfType")]
    [JsonDerivedType(typeof(MoneyEarnedCriteria), typeDiscriminator: "MoneyEarned")]
    [JsonDerivedType(typeof(WavesStartedCriteria), typeDiscriminator: "WavesStarted")]
    public interface IUserCriteria
    {
        public bool IsValid(StatsDefinition stats);
        public string Progress(StatsDefinition stats);
    }
}
