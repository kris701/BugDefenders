using System;

namespace TDGame.OpenGL.Settings
{
    public class SettingsDefinition
    {
        public float Scale { get; set; } = 1;
        public bool IsFullscreen { get; set; } = false;
        public bool IsVsync { get; set; } = true;
        public bool FPSCounter { get; set; } = false;
        public Guid TexturePack { get; set; } = new Guid("f58b60d7-4aed-4865-a5bb-e240014649a7");
        public float MusicVolume { get; set; } = 0.2f;
        public float EffectsVolume { get; set; } = 0.2f;

        public SettingsDefinition Copy()
        {
            return new SettingsDefinition()
            {
                Scale = Scale,
                IsFullscreen = IsFullscreen,
                IsVsync = IsVsync,
                FPSCounter = FPSCounter,
                TexturePack = TexturePack,
                MusicVolume = MusicVolume,
                EffectsVolume = EffectsVolume
            };
        }
    }
}
