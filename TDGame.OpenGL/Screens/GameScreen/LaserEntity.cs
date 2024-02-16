using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.Core.Models;

namespace TDGame.OpenGL.Screens.GameScreen
{
    public class LaserEntity
    {
        public IPosition From { get; set; }
        public IPosition To { get; set; }
    }
}
