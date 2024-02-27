using System.Text.Json.Serialization;
using TDGame.Core.Game.Models.Entities.Turrets;

namespace TDGame.Core.Game.Models.Entities.Upgrades
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "UpgradeType")]
    [JsonDerivedType(typeof(AOETurretUpgrade), typeDiscriminator: "AOETurret")]
    [JsonDerivedType(typeof(LaserTurretUpgrade), typeDiscriminator: "LaserTurret")]
    [JsonDerivedType(typeof(ProjectileTurretUpgrade), typeDiscriminator: "ProjectileTurret")]
    [JsonDerivedType(typeof(ExplosiveProjectileUpgrade), typeDiscriminator: "ExplosiveProjectile")]
    [JsonDerivedType(typeof(DirectProjectileUpgrade), typeDiscriminator: "DirectProjectile")]
    [JsonDerivedType(typeof(InvestmentTurretUpgrade), typeDiscriminator: "InvestmentTurret")]
    [JsonDerivedType(typeof(PassiveTurretUpgrade), typeDiscriminator: "PassiveTurret")]
    public interface IUpgrade : IDefinition
    {
        public int Cost { get; set; }
        public Guid? Requires { get; set; }

        public void ApplyUpgrade(TurretInstance on);

        public string GetDescriptionString();
    }
}
