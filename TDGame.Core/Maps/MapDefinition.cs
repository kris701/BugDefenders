using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TDGame.Core.Models;

namespace TDGame.Core.Maps
{
    public class MapDefinition : IDefinition
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<FloatPoint> WayPoints { get; set; }
        public List<BlockedTile> BlockingTiles { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
