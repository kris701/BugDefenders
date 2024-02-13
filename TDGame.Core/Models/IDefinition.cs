using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDGame.Core.Models
{
    public interface IDefinition : IIdentifiable
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
