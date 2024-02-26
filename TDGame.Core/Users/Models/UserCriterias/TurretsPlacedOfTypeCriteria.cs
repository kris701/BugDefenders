﻿namespace TDGame.Core.Users.Models.UserCriterias
{
    public class TurretsPlacedOfTypeCriteria : IUserCriteria
    {
        public Guid TurretID { get; set; }
        public int Quantity { get; set; }

        public bool IsValid(StatsDefinition stats) => stats.TotalTurretsPlacedOfType.ContainsKey(TurretID) && stats.TotalTurretsPlacedOfType[TurretID] >= Quantity;
    }
}
