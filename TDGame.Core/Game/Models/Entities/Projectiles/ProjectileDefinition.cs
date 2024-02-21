﻿using System.Text;
using TDGame.Core.Game.Models.Entities.Projectiles.Modules;
using TDGame.Core.Game.Models.Entities.Turrets;
using TDGame.Core.Resources;
using static TDGame.Core.Game.Models.Entities.Enemies.EnemyDefinition;

namespace TDGame.Core.Game.Models.Entities.Projectiles
{
    public class ProjectileDefinition : IDefinition
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Size { get; set; }
        public IProjectileModule ModuleInfo { get; set; }
        public List<EnemyTerrrainTypes> CanDamage { get; set; }

        public string GetDescriptionString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Name: {Name}");
            if (CanDamage.Count != 0)
            {
                sb.AppendLine("Can Damage:");
                foreach (var item in CanDamage)
                    sb.Append($"{Enum.GetName(typeof(EnemyTerrrainTypes), item)}, ");
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
