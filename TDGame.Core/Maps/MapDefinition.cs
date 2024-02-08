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
        public string MapName { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int TileSize { get; set; }
        public WayPoint Start { get; set; }
        public WayPoint End { get; set; }
        public List<WayPoint> PathWayPoints { get; set; }
        public List<int> TileSetIDs { get; }
        private ITile[,] _tileSet { get; }
        public ITile[,] GetTileSet() => _tileSet;

        [JsonConstructor]
        public MapDefinition(string mapName, int width, int height, int tileSize, WayPoint start, WayPoint end, List<WayPoint> pathWayPoints, List<int> tileSetIDs)
        {
            MapName = mapName;
            Width = width;
            Height = height;
            TileSize = tileSize;
            Start = start;
            End = end;
            PathWayPoints = pathWayPoints;
            TileSetIDs = tileSetIDs;
            _tileSet = new ITile[Width, Height];
            int id = 0;
            for(int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    _tileSet[x, y] = TileBuilder.GetTileByID(TileSetIDs[id++]);
        }
    }
}
