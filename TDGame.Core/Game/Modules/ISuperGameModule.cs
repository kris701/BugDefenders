using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDGame.Core.Game.Modules
{
    public interface ISuperGameModule : IGameModule
    {
        public List<IGameModule> Modules { get; }
    }
}
