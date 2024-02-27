namespace TDGame.Core.Game.Modules
{
    public abstract class BaseSuperGameModule : BaseGameModule, ISuperGameModule
    {
        public List<IGameModule> Modules { get; internal set; }

        protected BaseSuperGameModule(GameContext context, GameEngine game) : base(context, game)
        {
            Modules = new List<IGameModule>();
        }

        public override void Update(TimeSpan passed)
        {
            foreach (var module in Modules)
                module.Update(passed);
        }

        public override void Initialize()
        {
            foreach (var module in Modules)
                module.Initialize();
        }
    }
}
