namespace TDGame.Core.Game.Modules
{
    public interface IGameModule
    {
        public GameEngine Game { get; }
        public void Update(TimeSpan passed);
    }
}
