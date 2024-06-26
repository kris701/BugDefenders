﻿using BugDefender.Core.Game.Models.Entities.Turrets.Modules;
using BugDefender.Core.Game.Models.Entities.Upgrades;
using System.Text;
using static BugDefender.Core.Game.Models.Entities.Enemies.EnemyDefinition;
namespace BugDefender.Core.Game.Models.Entities.Turrets
{
    public class TurretDefinition : IDefinition
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Size { get; set; }
        public int Cost { get; set; }
        public ITurretModule ModuleInfo { get; set; }
        public HashSet<EnemyTerrrainTypes> CanTarget { get; set; }
        public List<UpgradeDefinition> Upgrades { get; set; }
        public int AvailableAtWave { get; set; }

        public TurretDefinition(Guid iD, string name, string description, float size, int cost, ITurretModule moduleInfo, HashSet<EnemyTerrrainTypes> canTarget, List<UpgradeDefinition> upgrades, int availableAtWave)
        {
            ID = iD;
            Name = name;
            Description = description;
            Size = size;
            Cost = cost;
            ModuleInfo = moduleInfo;
            CanTarget = canTarget;
            Upgrades = upgrades;
            AvailableAtWave = availableAtWave;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Name: {Name}");
            if (CanTarget.Count != 0)
            {
                sb.AppendLine("Can Target:");
                foreach (var item in CanTarget)
                    sb.Append($"{Enum.GetName(typeof(EnemyTerrrainTypes), item)}, ");
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
