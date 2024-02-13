using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDGame.Core.Models
{
    public interface IInstance<T> : IIdentifiable where T : IDefinition
    {
        public Guid DefinitionID { get; set; }
        public T GetDefinition();
    }
}
