﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TDGame.Core.Game.Models;

namespace TDGame.Core.Users.Models.Buffs.BuffEffects
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "BuffType")]
    [JsonDerivedType(typeof(EnemyBuffEffect), typeDiscriminator: "Enemy")]
    [JsonDerivedType(typeof(TurretBuffEffect), typeDiscriminator: "Turret")]
    public interface IBuffEffect
    {
        [JsonIgnore]
        public Guid BuffID { get; set; }
    }
}