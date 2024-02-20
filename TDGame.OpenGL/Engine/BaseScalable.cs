﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDGame.OpenGL.Engine
{
    public abstract class BaseScalable : IScalable
    {
        public float ScaleValue { get; set; } = 1;
        public UIEngine Parent { get; set; }

        protected BaseScalable(UIEngine parent)
        {
            Parent = parent;
            ScaleValue = parent.CurrentUser.UserData.Scale;
        }

        public int Scale(int value) => (int)(value * ScaleValue);
        public double Scale(double value) => value * ScaleValue;
        public float Scale(float value) => (float)(value * ScaleValue);
        public int InvScale(int value) => (int)(value / ScaleValue);
        public float InvScale(float value) => value / ScaleValue;
    }
}