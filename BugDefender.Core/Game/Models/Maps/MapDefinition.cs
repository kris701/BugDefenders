using BugDefender.Core.Game.Helpers;
using BugDefender.Tools;

namespace BugDefender.Core.Game.Models.Maps
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
        public List<string> Tags { get; set; }

        public MapDefinition(Guid iD, string name, string description, List<List<FloatPoint>> paths, List<BlockedTile> blockingTiles, int width, int height, List<string> tags)
        {
            ID = iD;
            Name = name;
            Description = description;
            Paths = paths;
            BlockingTiles = blockingTiles;
            Width = width;
            Height = height;
            Tags = tags;
        }

        public float GetDifficultyRating()
        {
            float difficulty = 1;

            foreach(var path in Paths)
            {
                float totalLength = 0;
                var from = path[0];
                foreach (var point in path.Skip(1))
                    totalLength += MathHelpers.Distance(from, point);
                difficulty += (1 / totalLength) * 100;
                difficulty *= 1.25f;
            }
            float totalSize = 0;
            foreach(var block in BlockingTiles)
                totalSize += block.Width * block.Height;
            difficulty *= 1 + (totalSize / (Width * Height));

            return difficulty;
        }
    }
}
