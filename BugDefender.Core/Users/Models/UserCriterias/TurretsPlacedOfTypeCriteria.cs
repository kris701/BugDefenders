using BugDefender.Core.Resources;
using System.Text.Json.Serialization;

namespace BugDefender.Core.Users.Models.UserCriterias
{
    [JsonSerializable(typeof(TurretsPlacedOfTypeCriteria))]
    public class TurretsPlacedOfTypeCriteria : IUserCriteria
    {
        public Guid TurretID { get; set; }
        public int Quantity { get; set; }

        public bool IsValid(StatsDefinition stats) => stats.TotalTurretsPlacedOfType.ContainsKey(TurretID) && stats.TotalTurretsPlacedOfType[TurretID] >= Quantity;
        public override string ToString()
        {
            return $"Place {Quantity} '{ResourceManager.Turrets.GetResource(TurretID).Name}' turrets.";
        }
    }
}
