using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TDGame.Core.Models.Entities.Turrets
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "TurretModule")]
    [JsonDerivedType(typeof(AOETurretDefinition), typeDiscriminator: "AOETurret")]
    [JsonDerivedType(typeof(LaserTurretDefinition), typeDiscriminator: "LaserTurret")]
    [JsonDerivedType(typeof(ProjectileTurretDefinition), typeDiscriminator: "ProjectileTurret")]
    [JsonDerivedType(typeof(InvestmentTurretDefinition), typeDiscriminator: "InvestmentTurret")]
    public interface ITurretType
    {
        public ITurretType Copy();
        public string GetDescriptionString();
    }
}
