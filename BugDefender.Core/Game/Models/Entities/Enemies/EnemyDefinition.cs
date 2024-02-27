using TDGame.Core.Game.Models.Entities.Enemies.Modules;

namespace TDGame.Core.Game.Models.Entities.Enemies
{
    public class EnemyDefinition : IDefinition
    {
        public enum EnemyTerrrainTypes { None, Ground, Flying }

        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Health { get; set; }
        public float Size { get; set; }
        public int Reward { get; set; }
        public IEnemyModule ModuleInfo { get; set; }
        public Guid EnemyType { get; set; }
        public EnemyTerrrainTypes TerrainType { get; set; }
        public int AvailableAtWave { get; set; }

        public EnemyDefinition(Guid iD, string name, string description, float health, float size, int reward, IEnemyModule moduleInfo, Guid enemyType, EnemyTerrrainTypes terrainType, int availableAtWave)
        {
            ID = iD;
            Name = name;
            Description = description;
            Health = health;
            Size = size;
            Reward = reward;
            ModuleInfo = moduleInfo;
            EnemyType = enemyType;
            TerrainType = terrainType;
            AvailableAtWave = availableAtWave;
        }
    }
}
