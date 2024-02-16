using TDGame.Core.Models.Entities.Turrets.Modules;

namespace TDGame.Core.Modules.Turrets
{
    public class InvestmentTurretsModule : BaseTurretModule
    {
        private int _currentWave = 0;

        public InvestmentTurretsModule(Game game) : base(game)
        {
        }

        public override void Update(TimeSpan passed)
        {
            foreach (var turret in Game.Turrets)
            {
                if (turret.TurretInfo is InvestmentTurretDefinition def)
                {
                    if (Game.Spawned != _currentWave)
                        Game.Money += def.MoneyPrWave;
                }
            }
            _currentWave = Game.Spawned;
        }
    }
}
