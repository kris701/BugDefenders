namespace TDGame.Core.Models.EnemyTypes
{
    public class EnemyTypeDefinition : IDefinition
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
