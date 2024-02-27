namespace BugDefender.Core.Game.Modules
{
    public class EvolutionModule : BaseGameModule
    {
        private int _currentWave = 0;
        public EvolutionModule(GameContext context, GameEngine game) : base(context, game)
        {
        }

        public override void Update(TimeSpan passed)
        {
            if (Context.Wave != _currentWave)
            {
                _currentWave = Context.Wave;
                Context.Evolution *= Context.GameStyle.EvolutionRate;
            }
        }
    }
}
