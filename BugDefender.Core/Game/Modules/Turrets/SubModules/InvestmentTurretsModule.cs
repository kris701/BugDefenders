using BugDefender.Core.Game.Models.Entities.Turrets;
using BugDefender.Core.Game.Models.Entities.Turrets.Modules;

namespace BugDefender.Core.Game.Modules.Turrets.SubModules
{
    public class InvestmentTurretsModule : BaseTurretModule<InvestmentTurretDefinition>
    {
        private int _currentWave = 0;

        public InvestmentTurretsModule(GameContext context, GameEngine game) : base(context, game)
        {
        }

        public override void Update(TimeSpan passed)
        {
            foreach (var turret in Context.Turrets)
                if (turret.TurretInfo is InvestmentTurretDefinition def)
                    UpdateTurret(passed, turret, def);
            _currentWave = Context.Wave;
        }

        public override void UpdateTurret(TimeSpan passed, TurretInstance turret, InvestmentTurretDefinition def)
        {
            if (Context.Wave != _currentWave)
            {
                Context.Money += def.MoneyPrWave;
                Game.TurretsModule.OnTurretShooting?.Invoke(turret);
            }
        }
    }
}
