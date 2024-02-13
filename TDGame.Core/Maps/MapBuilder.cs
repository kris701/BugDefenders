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

namespace TDGame.Core.Maps
{
    public static class MapBuilder
    {
        private static BaseBuilder<MapDefinition> _builder = new BaseBuilder<MapDefinition>("Maps.Maps", Assembly.GetExecutingAssembly());

        public static List<Guid> GetMaps() => _builder.GetResources();
        public static MapDefinition GetMap(Guid id) => _builder.GetResource(id);
    }
}
