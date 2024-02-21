using TDGame.Core.Game.Models.Entities.Turrets;
using TDGame.Core.Game.Models.Entities.Turrets.Modules;

namespace TDGame.Core.Game.Modules.Turrets
{
    public class InvestmentTurretsModule : BaseTurretModule<InvestmentTurretDefinition>
    {
        private int _currentWave = 0;

        public InvestmentTurretsModule(GameEngine game) : base(game)
        {
        }

        public override void UpdateTurret(TimeSpan passed, TurretInstance turret, InvestmentTurretDefinition def)
        {
            if (Game.Wave != _currentWave)
                Game.Money += def.MoneyPrWave;
        }
    }
}
