using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TDGame.Core.Helpers;

namespace TDGame.Core.GameStyles
{
    public static class GameStyleBuilder
    {
        private static BaseBuilder<GameStyle> _builder = new BaseBuilder<GameStyle>("GameStyles.GameStyles");

        public static List<string> GetGameStyles() => _builder.GetResources();
        public static GameStyle GetGameStyle(string name) => _builder.GetResource(name);
    }
}
