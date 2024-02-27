using System.Text;
using BugDefender.Core.Game.Models.Entities.Turrets;
using BugDefender.Core.Game.Models.Entities.Turrets.Modules;

namespace BugDefender.Core.Game.Models.Entities.Upgrades
{
    public class InvestmentTurretUpgrade : IUpgrade
    {
        public Guid ID { get; set; }
        public Guid? Requires { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
        public float MoneyPrWaveModifier { get; set; }

        public InvestmentTurretUpgrade(Guid iD, Guid? requires, string name, string description, int cost, float moneyPrWaveModifier)
        {
            ID = iD;
            Requires = requires;
            Name = name;
            Description = description;
            Cost = cost;
            MoneyPrWaveModifier = moneyPrWaveModifier;
        }

        public void ApplyUpgrade(TurretInstance on)
        {
            if (on.TurretInfo is InvestmentTurretDefinition item)
            {
                item.MoneyPrWave = (int)(item.MoneyPrWave * MoneyPrWaveModifier);
                on.HasUpgrades.Add(ID);
            }
            else
                throw new Exception("Invalid upgrade type for turret!");
        }

        public string GetDescriptionString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(Description);
            sb.AppendLine("Turret get:");
            if (MoneyPrWaveModifier != 1)
                sb.AppendLine($"Money pr Wave {MoneyPrWaveModifier}x");

            return sb.ToString();
        }
    }
}
