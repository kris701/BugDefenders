using BugDefender.OpenGL.Engine.Audio;
using BugDefender.OpenGL.Engine.BackgroundWorkers;
using BugDefender.OpenGL.Engine.Textures;
using BugDefender.OpenGL.Engine.Views;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugDefender.OpenGL.Engine
{
    public interface IGameWindow
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
