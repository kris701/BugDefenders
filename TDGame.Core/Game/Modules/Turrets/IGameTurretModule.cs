using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.Core.Game.Models.Entities.Turrets.Modules;
using TDGame.Core.Game.Models.Entities.Turrets;

namespace TDGame.Core.Game.Modules.Turrets
{
    public interface IGameTurretModule<T> : IGameModule where T : ITurretModule
    {
        public void UpdateTurret(TimeSpan passed, TurretInstance turret, T def);
    }
}
