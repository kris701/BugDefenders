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
    public static class TurretBuilder
    {
        private static BaseBuilder<TurretDefinition> _builder = new BaseBuilder<TurretDefinition>("Turrets.Turrets", Assembly.GetExecutingAssembly());

        public static List<string> GetTurrets() => _builder.GetResources();
        public static TurretDefinition GetTurret(string name) => _builder.GetResource(name);
    }
}
