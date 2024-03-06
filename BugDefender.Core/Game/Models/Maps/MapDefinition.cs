using System.Text.Json.Serialization;

namespace BugDefender.Core.Game.Models.Maps
{
    [JsonSerializable(typeof(MapDefinition))]
    public class MapDefinition : IDefinition
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<List<FloatPoint>> Paths { get; set; }
        public List<BlockedTile> BlockingTiles { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public MapDefinition(Guid iD, string name, string description, List<List<FloatPoint>> paths, List<BlockedTile> blockingTiles, int width, int height)
        {
            ID = iD;
            Name = name;
            Description = description;
            Paths = paths;
            BlockingTiles = blockingTiles;
            Width = width;
            Height = height;
        }
    }
}
