using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TDGame.Core.Maps.Tiles;

namespace TDGame.Core.Maps
{
    public class MapDefinition
    {
        public string Name { get; set; }
        public List<WayPoint> WayPoints { get; set; }
        public List<BlockedTile> BlockingTiles { get; set; }

        public MapDefinition(string name, List<WayPoint> wayPoints, List<BlockedTile> blockingTiles)
        {
            Name = name;
            WayPoints = wayPoints;
            BlockingTiles = blockingTiles;
        }
    }
}
