using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.Core.Enemies;
using TDGame.Core.Maps;
using TDGame.Core.Models;
using TDGame.Core.Turrets;

namespace TDGame.Core.Helpers
{
    public static class MathHelpers
    {
        public static bool Intersects(TurretDefinition turret, FloatPoint at, BlockedTile tile)
        {
            return new Rectangle((int)at.X, (int)at.Y, (int)turret.Size, (int)turret.Size).IntersectsWith(
                new Rectangle((int)tile.X, (int)tile.Y, (int)tile.Width, (int)tile.Height));
        }
        public static bool Intersects(TurretInstance turret, BlockedTile tile)
        {
            return new Rectangle((int)turret.X, (int)turret.Y, (int)turret.Size, (int)turret.Size).IntersectsWith(
                new Rectangle((int)tile.X, (int)tile.Y, (int)tile.Width, (int)tile.Height));
        }
        public static bool Intersects(TurretDefinition e1, FloatPoint at, TurretInstance e2)
        {
            return new Rectangle((int)at.X, (int)at.Y, (int)e1.Size, (int)e1.Size).IntersectsWith(
                new Rectangle((int)e2.X, (int)e2.Y, (int)e2.Size, (int)e2.Size));
        }

        public static float Distance(IPosition e1, IPosition e2) => Distance(e1.X + e1.Size / 2, e1.Y + e1.Size / 2, e2.X + e2.Size / 2, e2.Y + e2.Size / 2);
        public static float Distance(EnemyInstance e1, FloatPoint w2) => Distance(e1.X + e1.Size / 2, e1.Y + e1.Size / 2, w2.X, w2.Y);
        public static float Distance(double x1, double y1, double x2, double y2) => (float)Math.Sqrt(((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)));

        public static float GetAngle(float x1, float y1, float x2, float y2)
        {
            var a = y1 - y2;
            var b = x1 - x2;
            return (float)Math.Atan2(a, b);
        }
    }
}
