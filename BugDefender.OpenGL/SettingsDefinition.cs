using System;

namespace BugDefender.OpenGL
{
    public class SettingsDefinition
    {
        public int ScreenWidth { get; set; } = 1280;
        public int ScreenHeight { get; set; } = 720;
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
                ScreenWidth = ScreenWidth,
                ScreenHeight = ScreenHeight,
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
