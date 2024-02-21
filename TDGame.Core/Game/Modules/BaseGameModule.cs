namespace TDGame.Core.Game.Modules
{
    public abstract class BaseGameModule : IGameModule
    {
        public GameContext Context { get; }
        public GameEngine Game { get; }

        public BaseGameModule(GameContext context, GameEngine game)
        {
            Context = context;
            Game = game;
        }

        public abstract void Update(TimeSpan passed);
        public virtual void Initialize()
        {

        }
    }
}
