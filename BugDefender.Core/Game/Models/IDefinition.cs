namespace TDGame.Core.Game.Models
{
    public interface IDefinition : IIdentifiable
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
