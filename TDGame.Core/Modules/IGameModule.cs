using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDGame.Core.Modules
{
    public interface IGameModule
    {
        public Game Game { get; }
        public void Update(TimeSpan passed);
    }
}
