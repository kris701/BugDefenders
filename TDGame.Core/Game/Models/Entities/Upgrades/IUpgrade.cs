using System.Text.Json.Serialization;
using TDGame.Core.Game.Models.Entities.Turrets;

namespace TDGame.Core.Game.Models.Entities.Upgrades
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "UpgradeType")]
    [JsonDerivedType(typeof(AOETurretUpgrade), typeDiscriminator: "AOETurret")]
    [JsonDerivedType(typeof(LaserTurretUpgrade), typeDiscriminator: "LaserTurret")]
    [JsonDerivedType(typeof(ProjectileTurretUpgrade), typeDiscriminator: "ProjectileTurret")]
    [JsonDerivedType(typeof(ProjectileUpgrade), typeDiscriminator: "Projectile")]
    [JsonDerivedType(typeof(InvestmentTurretUpgrade), typeDiscriminator: "InvestmentTurret")]
    public interface IUpgrade : IDefinition
    {
        public int Cost { get; set; }
        public Guid? Requires { get; set; }

        public void ApplyUpgrade(TurretInstance on);

        public string GetDescriptionString();
    }
}
