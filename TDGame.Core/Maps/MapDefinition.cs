using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TDGame.Core.Maps
{
    public class MapDefinition
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<WayPoint> WayPoints { get; set; }
        public List<BlockedTile> BlockingTiles { get; set; }

        public MapDefinition(Guid iD, string name, string description, List<WayPoint> wayPoints, List<BlockedTile> blockingTiles)
        {
            ID = iD;
            Name = name;
            Description = description;
            WayPoints = wayPoints;
            BlockingTiles = blockingTiles;
        }
    }
}
