using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDGame.Core.Game.Models
{
    public interface IModuleInfo<T>
    {
        public T Copy();
        public string GetDescriptionString();
    }
}
