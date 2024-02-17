using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.Core.Game.Models.Entities.Turrets.Modules;

namespace TDGame.Core.Users.Models.Buffs
{
    public class TurretBuff : IBuff
    {
        public Guid TurretID { get; set; }
        public ITurretModule Module { get; set; }
    }
}
