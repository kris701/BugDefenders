using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TDGame.Core.Maps.Tiles;
using TDGame.Core.Models;

namespace TDGame.Core.Maps
{
    public class MapDefinition : BaseGameModel
    {
        public List<WayPoint> WayPoints { get; set; }
        public List<BlockedTile> BlockingTiles { get; set; }

        public MapDefinition(Guid iD, string name, string description, List<WayPoint> wayPoints, List<BlockedTile> blockingTiles) : base(iD, name, description)
        {
            WayPoints = wayPoints;
            BlockingTiles = blockingTiles;
        }
    }
}
