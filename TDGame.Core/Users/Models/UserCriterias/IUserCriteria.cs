using System.Text.Json.Serialization;

namespace TDGame.Core.Users.Models.UserCriterias
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "CriteriaType")]
    [JsonDerivedType(typeof(EnemyKilledCriteria), typeDiscriminator: "TotalKillsOfType")]
    [JsonDerivedType(typeof(EnemiesKilledCriteria), typeDiscriminator: "TotalKills")]
    [JsonDerivedType(typeof(TurretPlacedCriteria), typeDiscriminator: "TotalPlacedTurretsOfType")]
    [JsonDerivedType(typeof(TurretsPlacedCriteria), typeDiscriminator: "TotalPlacedTurrets")]
    public interface IUserCriteria
    {
        public bool IsValid(StatsDefinition stats);
    }
}
