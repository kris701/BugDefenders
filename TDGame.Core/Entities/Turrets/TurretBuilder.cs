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

namespace TDGame.Core.Entities.Turrets
{
    public static class TurretBuilder
    {
        private static BaseBuilder<TurretDefinition> _builder = new BaseBuilder<TurretDefinition>("Entities.Turrets.Turrets", Assembly.GetExecutingAssembly());

        public static List<Guid> GetTurrets() => _builder.GetResources();
        public static TurretDefinition GetTurret(Guid id) => _builder.GetResource(id);
    }
}
