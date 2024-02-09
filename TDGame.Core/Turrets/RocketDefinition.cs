using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDGame.Core.Turrets
{
    public class RocketDefinition
    {
        public int X { get; set; }
        public int Y { get; set; }
        public double Angle { get; set; }
        public int Speed { get; set; }
        public int Damage { get; set; }
        public int Range { get; set; }
        public int SplashRange { get; set; }
        public int TriggerRange { get; set; }
        public double Acceleration { get; set; }
        public int Traveled { get; set; }
    }
}
