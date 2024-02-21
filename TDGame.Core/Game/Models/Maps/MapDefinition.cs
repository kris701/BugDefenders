namespace TDGame.Core.Game.Models.Maps
{
    public class MapDefinition : IDefinition
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<List<FloatPoint>> Paths { get; set; }
        public List<BlockedTile> BlockingTiles { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
