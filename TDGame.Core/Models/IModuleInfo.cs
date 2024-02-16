using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.Core.Models.Entities.Turrets.Modules;

namespace TDGame.Core.Models
{
    public interface IModuleInfo<T>
    {
        public T Copy();
        public string GetDescriptionString();
    }
}
