using TDGame.Core.Game.Models.Entities.Turrets.Modules;

namespace TDGame.Core.Game.Modules.Turrets
{
    public class InvestmentTurretsModule : BaseTurretModule
    {
        private int _currentWave = 0;

        public InvestmentTurretsModule(GameEngine game) : base(game)
        {
        }

        public override void Update(TimeSpan passed)
        {
            foreach (var turret in Game.Turrets)
            {
                if (turret.TurretInfo is InvestmentTurretDefinition def)
                {
                    if (Game.Wave != _currentWave)
                        Game.Money += def.MoneyPrWave;
                }
            }
            _currentWave = Game.Wave;
        }
    }
}
