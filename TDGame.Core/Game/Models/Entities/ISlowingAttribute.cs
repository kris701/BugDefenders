using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDGame.Core.Game.Models.Entities
{
    public interface ISlowingAttribute
    {
        public float SlowingFactor { get; set; }
        public int SlowingDuration { get; set; }
    }
}
