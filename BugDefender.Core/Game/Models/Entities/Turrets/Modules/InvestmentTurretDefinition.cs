﻿using System.Text;
using System.Text.Json.Serialization;

namespace BugDefender.Core.Game.Models.Entities.Turrets.Modules
{
    [JsonSerializable(typeof(InvestmentTurretDefinition))]
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
