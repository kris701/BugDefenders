using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDGame.OpenGL.Settings
{
    public class SettingsDefinition
    {
        public float Scale { get; set; } = 1;
        public bool IsFullscreen { get; set; } = false;
        public bool IsVsync { get; set; } = true;

        public SettingsDefinition Copy()
        {
            return new SettingsDefinition()
            {
                Scale = Scale,
                IsFullscreen = IsFullscreen,
                IsVsync = IsVsync
            };
        }
    }
}
