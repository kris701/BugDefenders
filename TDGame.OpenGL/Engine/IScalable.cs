using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDGame.OpenGL.Engine
{
    public interface IScalable
    {
        public UIEngine Parent { get; set; }
        public float ScaleValue { get; set; }

        public int Scale(int value);
        public double Scale(double value);
        public float Scale(float value);
        public int InvScale(int value);
        public float InvScale(float value);
    }
}
