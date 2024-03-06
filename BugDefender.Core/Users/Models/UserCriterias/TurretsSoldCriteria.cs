﻿using System.Text.Json.Serialization;

namespace BugDefender.Core.Users.Models.UserCriterias
{
    [JsonSerializable(typeof(TurretsSoldCriteria))]
    public class TurretsSoldCriteria : IUserCriteria
    {
        public int Quantity { get; set; }

        public bool IsValid(StatsDefinition stats) => stats.TotalTurretsSold >= Quantity;
        public override string ToString()
        {
            return $"Sell {Quantity} turrets.";
        }
    }
}
