using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDGame.Core.Maps.Tiles
{
    public static class TileBuilder
    {
        private static Dictionary<int, Func<ITile>> _tiles = new Dictionary<int, Func<ITile>>()
        {
            { 1, () => { return new PathTile(); } },
            { 2, () => { return new BuildTile(); } },
            { 3, () => { return new BlockedTile(); } },
        };

        public static ITile GetTileByID(int id)
        {
            if (_tiles.ContainsKey(id))
                return _tiles[id]();
            throw new Exception($"Tile id not found: {id}");
        }
    }
}
