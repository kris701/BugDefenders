using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDGame.Core.Models
{
    public class BaseGameModel
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public BaseGameModel(Guid iD, string name, string description)
        {
            ID = iD;
            Name = name;
            Description = description;
        }
    }
}
