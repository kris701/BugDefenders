﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BugDefender.Core.Users.Models.SavedGames
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "SaveType")]
    [JsonDerivedType(typeof(SurvivalSavedGame), typeDiscriminator: "Survival")]
    [JsonDerivedType(typeof(CampainSavedGame), typeDiscriminator: "Campain")]
    [JsonDerivedType(typeof(ChallengeSavedGame), typeDiscriminator: "Challenge")]
    public interface ISavedGame
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
    }
}
