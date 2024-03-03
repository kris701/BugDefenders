﻿using BugDefender.Core.Resources;

namespace BugDefender.Core.Users.Models.UserCriterias
{
    public class TurretsPlacedOfTypeCriteria : IUserCriteria
    {
        public Guid TurretID { get; set; }
        public int Quantity { get; set; }

        public bool IsValid(StatsDefinition stats) => stats.TotalTurretsPlacedOfType.ContainsKey(TurretID) && stats.TotalTurretsPlacedOfType[TurretID] >= Quantity;
        public string GetDescriptionString()
        {
            return $"Place {Quantity} '{ResourceManager.Turrets.GetResource(TurretID).Name}' turrets.";
        }
    }
}
