﻿using System.Text;

namespace BugDefender.Core.Game.Models.Entities.Upgrades
{
    public class UpdateModel : UpgradeEffectModel, IDefinition
    {
        public Guid ID { get; set; }
        public Guid? Requires { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }

        public UpdateModel(Guid iD, Guid? requires, string name, string description, int cost, List<EffectTarget> effects) : base(effects)
        {
            ID = iD;
            Requires = requires;
            Name = name;
            Description = description;
            Cost = cost;
        }

        public string GetDescriptionString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(Name);
            sb.AppendLine(Description);

            return sb.ToString();
        }
    }
}
