﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TDGame.Core.Models.Entities.Enemies.Modules
{
    public class SingleEnemyDefinition : IEnemyModule, ISlowable
    {
        public float Speed { get; set; }
        [JsonIgnore]
        public float SlowingFactor { get; set; } = 1;
        [JsonIgnore]
        public int SlowingDuration { get; set; } = 0;

        public IEnemyModule Copy()
        {
            return new SingleEnemyDefinition()
            {
                Speed = Speed
            };
        }

        public string GetDescriptionString()
        {
            var sb = new StringBuilder();

            return sb.ToString();
        }

        public float GetSpeed()
        {
            if (SlowingFactor == 1)
                return Speed;
            if (SlowingDuration <= 0)
                SlowingFactor = 1;
            return Speed * SlowingFactor;
        }
    }
}