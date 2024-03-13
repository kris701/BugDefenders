using BugDefender.Core.Game;
using BugDefender.Core.Game.Models.GameStyles;
using BugDefender.Core.Game.Models.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugDefender.Core.Tests.Game.Modules
{
    public abstract class BaseModuleTests
    {
        internal static GameEngine GetBaseGame()
        {
            return new GameEngine(
                new GameContext(
                    new MapDefinition(
                        Guid.NewGuid(),
                        "Empty",
                        "",
                        new List<List<Tools.FloatPoint>>(),
                        new List<BlockedTile>(),
                        950,
                        950,
                        new List<string>()
                    ),
                    new GameStyleDefinition(
                        Guid.NewGuid(),
                        "Empty",
                        "",
                        1,
                        1,
                        1,
                        1,
                        1,
                        1,
                        1,
                        1,
                        new List<Guid>(),
                        new List<Guid>(),
                        new List<Guid>(),
                        new List<Guid>(),
                        1,
                        1
                    )
                )
            );
        }
    }
}
