using BugDefender.OpenGL.Engine.Audio;
using BugDefender.OpenGL.Engine.BackgroundWorkers;
using BugDefender.OpenGL.Engine.Textures;
using BugDefender.OpenGL.Engine.Views;
using Microsoft.Xna.Framework;

namespace BugDefender.OpenGL.Engine
{
    public interface IWindow
    {
        public static readonly Point BaseScreenSize = new Point(1920, 1080);
        public float XScale { get; }
        public float YScale { get; }

        public IView CurrentScreen { get; set; }
        public List<IBackgroundWorker> BackroundWorkers { get; set; }
        public AudioController AudioController { get; }
        public TextureController TextureController { get; }
        public bool IsActive { get; }
    }
}
