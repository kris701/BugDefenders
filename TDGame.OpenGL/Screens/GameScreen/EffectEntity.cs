using System;
using TDGame.Core.Game.Models;

namespace TDGame.OpenGL.Screens.GameScreen
{
    public class EffectEntity : BasePositionModel, IIdentifiable
    {
        public Guid ID { get; set; }
        public TimeSpan LifeTime { get; set; } = TimeSpan.FromSeconds(1);
    }
}
