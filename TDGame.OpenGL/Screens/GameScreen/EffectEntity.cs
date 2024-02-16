using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.Core.Models;
using TDGame.Core.Models.Entities.Projectiles;

namespace TDGame.OpenGL.Screens.GameScreen
{
    public class EffectEntity : BasePositionModel, IIdentifiable
    {
        public Guid ID { get; set; }
        public TimeSpan LifeTime { get; set; } = TimeSpan.FromSeconds(1);

        public EffectEntity(ProjectileInstance projectile)
        {
            Size = projectile.GetDefinition().SplashRange;
            X = projectile.CenterX - Size / 2;
            Y = projectile.CenterY - Size / 2;
        }
    }
}
