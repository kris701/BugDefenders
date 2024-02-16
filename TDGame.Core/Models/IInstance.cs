namespace TDGame.Core.Models
{
    public interface IInstance<T> : IIdentifiable where T : IDefinition
    {
        public Guid DefinitionID { get; set; }
        public T GetDefinition();
    }
}
