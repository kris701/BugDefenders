namespace TDGame.Core.Models
{
    public interface IDefinition : IIdentifiable
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
