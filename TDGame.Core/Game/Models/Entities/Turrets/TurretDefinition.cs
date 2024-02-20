using System.Text;
using TDGame.Core.Game.Models.Entities.Turrets.Modules;
using TDGame.Core.Game.Models.Entities.Upgrades;
using static TDGame.Core.Game.Models.Entities.Enemies.EnemyDefinition;
namespace TDGame.Core.Game.Models.Entities.Turrets
{
    public class TurretDefinition : IDefinition
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Size { get; set; }
        public int Cost { get; set; }
        public ITurretModule ModuleInfo { get; set; }
        public List<EnemyTerrrainTypes> CanDamage { get; set; }
        public List<IUpgrade> Upgrades { get; set; }
        public int AvailableAtWave { get; set; }

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
