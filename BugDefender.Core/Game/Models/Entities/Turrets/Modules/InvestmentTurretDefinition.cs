using System.Text;

namespace BugDefender.Core.Game.Models.Entities.Turrets.Modules
{
    public class InvestmentTurretDefinition : ITurretModule
    {
        public int MoneyPrWave { get; set; }

        public ITurretModule Copy()
        {
            return new InvestmentTurretDefinition()
            {
                MoneyPrWave = MoneyPrWave
            };
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Type: Investment");
            if (MoneyPrWave != 0)
                sb.AppendLine($"Money Pr Wave: {MoneyPrWave}");
            return sb.ToString();
        }
    }
}
