﻿using BugDefender.Core.Resources;

namespace BugDefender.Core.Users.Models.UserCriterias
{
    public class TurretsSoldOfTypeCriteria : IUserCriteria
    {
        public Guid TurretID { get; set; }
        public int Quantity { get; set; }

        public bool IsValid(StatsDefinition stats) => stats.TotalTurretsSoldOfType.ContainsKey(TurretID) && stats.TotalTurretsSoldOfType[TurretID] >= Quantity;
        public string GetDescriptionString()
        {
            return $"Sell {Quantity} '{ResourceManager.Turrets.GetResource(TurretID).Name}' turrets.";
        }
    }
}
