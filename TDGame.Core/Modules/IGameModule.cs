namespace TDGame.Core.Modules
{
    public interface IGameModule
    {
        public Game Game { get; }
        public void Update(TimeSpan passed);
    }
}
