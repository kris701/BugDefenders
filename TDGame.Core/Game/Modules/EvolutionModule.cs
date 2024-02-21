namespace TDGame.Core.Game.Modules
{
    public class EvolutionModule : BaseGameModule
    {
        public TimeSpan TickRate { get; set; } = TimeSpan.FromSeconds(1);
        private TimeSpan _current = TimeSpan.Zero;
        public EvolutionModule(GameContext context, GameEngine game) : base(context, game)
        {
        }

        public override void Update(TimeSpan passed)
        {
            _current -= passed;
            if (_current <= TimeSpan.Zero)
            {
                _current = TickRate;
                Context.Evolution *= Context.GameStyle.EvolutionRate;
            }
        }
    }
}
