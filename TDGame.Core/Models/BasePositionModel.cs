﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDGame.Core.Models
{
    public class BasePositionModel : IPosition
    {
        public float X { get; set; }
        public float Y { get; set; }
    }
}
