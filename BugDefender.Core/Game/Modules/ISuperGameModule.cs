namespace BugDefender.Core.Game.Modules
{
    public interface ISuperGameModule : IGameModule
    {
        public List<IGameModule> Modules { get; }
    }
}
