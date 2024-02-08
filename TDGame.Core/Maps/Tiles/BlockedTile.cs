using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDGame.Core.Maps.Tiles
{
    public class BlockedTile
    {
        public int X { get; set; }  
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public BlockedTile(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}
