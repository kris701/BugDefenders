namespace TDGame.Core.Models.Entities.Enemies
{
    public class EnemyDefinition : IDefinition
    {
        public enum EnemyTerrrainTypes { None, Ground, Flying }

        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Size { get; set; }
        public float Health { get; set; }
        public float Speed { get; set; }
        public int Reward { get; set; }
        public int WaveSize { get; set; }
        public bool IsBoss { get; set; }
        public Guid EnemyType { get; set; }
        public EnemyTerrrainTypes TerrainType { get; set; }
    }
}
