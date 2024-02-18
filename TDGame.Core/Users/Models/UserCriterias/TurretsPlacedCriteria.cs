using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDGame.Core.Users.Models.UserCriterias
{
    public class TurretsPlacedCriteria : IUserCriteria
    {
        public int Quantity { get; set; }

        public bool IsValid(StatsDefinition stats)
        {
            if (stats.TotalTurretsPlaced >= Quantity)
                return true;
            return false;
        }
    }
}
