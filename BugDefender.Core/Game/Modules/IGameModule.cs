namespace BugDefender.Core.Game.Modules
{
    public interface IGameModule
    {
        public GameContext Context { get; }
        public GameEngine Game { get; }
        public void Update(TimeSpan passed);
        public void Initialize();
    }
}
