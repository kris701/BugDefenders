using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDGame.Core.Users.Models.UserCriterias
{
    public class TurretPlacedCriteria : IUserCriteria
    {
        public Guid TurretID { get; set; }
        public int Quantity { get; set; }

        public bool IsValid(StatsDefinition stats)
        {
            if (stats.TotalTurretsPlacedOfType.ContainsKey(TurretID) && stats.TotalTurretsPlacedOfType[TurretID] >= Quantity)
                return true;
            return false;
        }
    }
}
