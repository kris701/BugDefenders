using BugDefender.Core.Game.Models.Entities.Turrets;
using BugDefender.Core.Game.Models.Entities.Turrets.Modules;

namespace BugDefender.Core.Game.Modules.Turrets.SubModules
{
    public interface IGameTurretModule<T> : IGameModule where T : ITurretModule
    {
        public void UpdateTurret(TimeSpan passed, TurretInstance turret, T def);
    }
}
