using System.Text;

namespace TDGame.Core.Models.Entities.Turrets
{
    public class InvestmentTurretDefinition : ITurretType
    {
        public int MoneyPrWave { get; set; }

        public ITurretType Copy()
        {
            return new InvestmentTurretDefinition()
            {
                MoneyPrWave = MoneyPrWave
            };
        }

        public string GetDescriptionString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Type: Investment");
            if (MoneyPrWave != 0)
                sb.AppendLine($"Money Pr Wave: {MoneyPrWave}");
            return sb.ToString();
        }
    }
}
