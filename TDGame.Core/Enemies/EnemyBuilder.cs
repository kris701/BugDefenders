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
        private static BaseBuilder<Enemy> _builder = new BaseBuilder<Enemy>("Enemies.Enemies");

        public static List<string> GetEnemies() => _builder.GetResources();
        public static Enemy GetEnemy(string name, double evolution)
        {
            var enemy = _builder.GetResource(name);

            enemy.Health = (int)(enemy.Health * evolution);

            return enemy;
        }
    }
}
