using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.Core.Models;

namespace TDGame.Core.Entities.Turrets.Upgrades
{
    public interface IUpgrade : IDefinition
    {
        public int Cost { get; set; }
        public Guid? Requires { get; set; }

        public string GetDescriptionString();
    }
}
