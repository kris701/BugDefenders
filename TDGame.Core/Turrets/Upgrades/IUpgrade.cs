using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDGame.Core.Turrets.Upgrades
{
    public interface IUpgrade
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
        public bool HasUpgrade { get; set; }

        public string GetDescriptionString();
    }
}
