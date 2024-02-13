using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TDGame.Core.Helpers;
using TDGame.Core.Maps;

namespace TDGame.Core.Enemies
{
    public static class EnemyBuilder
    {
        private static BaseBuilder<EnemyDefinition> _builder = new BaseBuilder<EnemyDefinition>("Enemies.Enemies", Assembly.GetExecutingAssembly());

        public static List<Guid> GetEnemies() => _builder.GetResources();
        public static EnemyDefinition GetEnemy(Guid id) => _builder.GetResource(id);
    }
}
