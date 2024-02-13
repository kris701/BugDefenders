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

namespace TDGame.Core.EnemyTypes
{
    public static class EnemyTypeBuilder
    {
        private static BaseBuilder<EnemyTypeDefinition> _builder = new BaseBuilder<EnemyTypeDefinition>("EnemyTypes.EnemyTypes", Assembly.GetExecutingAssembly());
        
        public static List<Guid> GetEnemyTypes() => _builder.GetResources();
        public static EnemyTypeDefinition GetEnemyType(Guid id) => _builder.GetResource(id);
    }
}
