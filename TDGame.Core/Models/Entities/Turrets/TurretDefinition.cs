using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
using TDGame.Core.Models;
using TDGame.Core.Models.Entities.Upgrades;
using static TDGame.Core.Models.Entities.Enemies.EnemyDefinition;
namespace TDGame.Core.Models.Entities.Turrets
{
    public class TurretDefinition : IDefinition
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Size { get; set; }
        public int Cost { get; set; }
        public ITurretType TurretType { get; set; }
        public List<EnemyTerrrainTypes> CanDamage { get; set; }

        public List<IUpgrade> Upgrades { get; set; }

        public string GetDescriptionString()
        {
            var sb = new StringBuilder();

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
