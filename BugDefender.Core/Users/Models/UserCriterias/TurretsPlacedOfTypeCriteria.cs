using BugDefender.Core.Resources;

namespace BugDefender.Core.Users.Models.UserCriterias
{
    public class TurretsPlacedOfTypeCriteria : IUserCriteria
    {
        public Guid TurretID { get; set; }
        public int Quantity { get; set; }

        public bool IsValid(StatsDefinition stats) => stats.TotalTurretsPlacedOfType.ContainsKey(TurretID) && stats.TotalTurretsPlacedOfType[TurretID] >= Quantity;
        public override string ToString()
        {
            return $"Place {Quantity} '{ResourceManager.Turrets.GetResource(TurretID).Name}' turrets.";
        }

        public string Progress(StatsDefinition stats)
        {
            if (!stats.TotalTurretsPlacedOfType.ContainsKey(TurretID))
                return $"{Quantity} more '{ResourceManager.Turrets.GetResource(TurretID).Name}' turrets placed to go"; ;
            return $"{Quantity - stats.TotalTurretsPlacedOfType[TurretID]} more '{ResourceManager.Turrets.GetResource(TurretID).Name}' turrets placed to go"; ;
        }
    }
}
