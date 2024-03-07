using BugDefender.Core.Game.Models.Maps;
using BugDefender.Tools;

namespace MapPathBlockingTileGen.Models
{
    public class PathsModel
    {
        public List<List<FloatPoint>> Paths { get; set; } = new List<List<FloatPoint>>();
    }
}
