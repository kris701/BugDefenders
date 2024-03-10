using BugDefender.Core.Resources;

namespace BugDefender.Core.Users.Models.UserCriterias
{
    public class TurretsSoldOfTypeCriteria : IUserCriteria
    {
        public Guid TurretID { get; set; }
        public int Quantity { get; set; }

        public bool IsValid(StatsDefinition stats) => stats.TotalTurretsSoldOfType.ContainsKey(TurretID) && stats.TotalTurretsSoldOfType[TurretID] >= Quantity;
        public override string ToString()
        {
            return $"Sell {Quantity} '{ResourceManager.Turrets.GetResource(TurretID).Name}' turrets.";
        }

        public string Progress(StatsDefinition stats)
        {
            if (!stats.TotalTurretsPlacedOfType.ContainsKey(TurretID))
                return $"{Quantity} more '{ResourceManager.Turrets.GetResource(TurretID).Name}' turrets sold to go"; ;
            return $"{Quantity - stats.TotalTurretsSoldOfType[TurretID]} more '{ResourceManager.Turrets.GetResource(TurretID).Name}' turrets sold to go"; ;
        }
    }
}
