namespace TDGame.Core.Game.Models
{
    public interface IModuleInfo<T>
    {
        public T Copy();
        public string GetDescriptionString();
    }
}
