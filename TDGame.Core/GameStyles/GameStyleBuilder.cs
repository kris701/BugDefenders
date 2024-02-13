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
        private static BaseBuilder<GameStyleDefinition> _builder = new BaseBuilder<GameStyleDefinition>("GameStyles.GameStyles", Assembly.GetExecutingAssembly());

        public static List<Guid> GetGameStyles() => _builder.GetResources();
        public static GameStyleDefinition GetGameStyle(Guid id) => _builder.GetResource(id);
    }
}
