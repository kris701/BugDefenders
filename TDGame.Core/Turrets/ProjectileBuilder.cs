using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using TDGame.Core.GameStyles;
using TDGame.Core.Helpers;
using TDGame.Core.Turrets;

namespace TDGame.Core.Turret
{
    public static class ProjectileBuilder
    {
        private static BaseBuilder<ProjectileDefinition> _builder = new BaseBuilder<ProjectileDefinition>("Turrets.Projectiles", Assembly.GetExecutingAssembly());

        public static List<string> GetProjectiles() => _builder.GetResources();
        public static ProjectileDefinition GetProjectile(string name) => _builder.GetResource(name);
    }
}
