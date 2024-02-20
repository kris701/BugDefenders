﻿using TDGame.Core.Game.Models.Entities.Enemies.Modules;

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
    }
}