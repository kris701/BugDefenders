using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDGame.Core.Models.Entities.Enemies.Modules
{
    public interface ISlowable
    {
        public float Speed { get; set; }
        public float SlowingFactor { get; set; }
        public int SlowingDuration { get; set; }

        public float GetSpeed();
    }
}
