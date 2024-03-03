using BugDefender.Core.Resources;

namespace BugDefender.Core.Users.Models.UserCriterias
{
    public class TurretKillsCriteria : IUserCriteria
    {
        public Guid TurretID { get; set; }
        public int Quantity { get; set; }

        public bool IsValid(StatsDefinition stats) => stats.TotalTurretKills.ContainsKey(TurretID) && stats.TotalTurretKills[TurretID] >= Quantity;
        public string GetDescriptionString()
        {
            return $"Kill {Quantity} enemies with a '{ResourceManager.Turrets.GetResource(TurretID).Name}' turret.";
        }
    }
}
