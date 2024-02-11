using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDGame.Core.Models
{
    public abstract class BasePositionModel : IPosition
    {
        public float X { get; set; }
        public float Y { get; set; }
        public abstract float Size { get; set; }
        public abstract float Angle { get; set; }
    }
}
