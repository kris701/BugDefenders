namespace TDGame.Core.Game.Models.EnemyTypes
{
    public class EnemyTypeDefinition : IDefinition
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
