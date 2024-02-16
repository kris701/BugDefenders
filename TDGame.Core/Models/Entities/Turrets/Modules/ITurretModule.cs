﻿using System.Text.Json.Serialization;

namespace TDGame.Core.Models.Entities.Turrets.Modules
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "TurretModule")]
    [JsonDerivedType(typeof(AOETurretDefinition), typeDiscriminator: "AOETurret")]
    [JsonDerivedType(typeof(LaserTurretDefinition), typeDiscriminator: "LaserTurret")]
    [JsonDerivedType(typeof(ProjectileTurretDefinition), typeDiscriminator: "ProjectileTurret")]
    [JsonDerivedType(typeof(InvestmentTurretDefinition), typeDiscriminator: "InvestmentTurret")]
    public interface ITurretModule : IModuleInfo<ITurretModule>
    {
    }
}