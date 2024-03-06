using BugDefender.Core.Resources;
using System.Text.Json.Serialization;

namespace BugDefender.Core.Users.Models.UserCriterias
{
    [JsonSerializable(typeof(TurretsSoldOfTypeCriteria))]
    public class TurretsSoldOfTypeCriteria : IUserCriteria
    {
        public Guid TurretID { get; set; }
        public int Quantity { get; set; }

        public bool IsValid(StatsDefinition stats) => stats.TotalTurretsSoldOfType.ContainsKey(TurretID) && stats.TotalTurretsSoldOfType[TurretID] >= Quantity;
        public override string ToString()
        {
            return $"Sell {Quantity} '{ResourceManager.Turrets.GetResource(TurretID).Name}' turrets.";
        }
    }
}
