using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.Core.Enemies;
using TDGame.Core.Maps;
using TDGame.Core.Turrets;

namespace TDGame.Core.Helpers
{
    public static class MathHelpers
    {

        public static bool Intersects(TurretDefinition turret, WayPoint turretLocation, BlockedTile tile)
        {
            if (turretLocation.X >= tile.X && turretLocation.X <= tile.X + tile.Width &&
                turretLocation.Y >= tile.Y && turretLocation.Y <= tile.Y + tile.Height)
                return true;

            float closestX = (turretLocation.X < tile.X ? tile.X : (turretLocation.X > tile.Width ? tile.Width : turretLocation.X));
            float closestY = (turretLocation.Y < tile.Y ? tile.Y : (turretLocation.Y > tile.Height ? tile.Height : turretLocation.Y));
            float dx = closestX - turretLocation.X;
            float dy = closestY - turretLocation.Y;

            return (dx * dx + dy * dy) <= turret.Size * turret.Size;
        }
        public static bool Intersects(TurretDefinition e1, TurretDefinition e2, WayPoint turretLocation) => Distance(e1.X, e1.Y, turretLocation.X, turretLocation.Y) < (e1.Size / 2) + (e2.Size / 2);

        public static double Distance(TurretDefinition e1, TurretDefinition e2) => Distance(e1.X, e1.Y, e2.X, e2.Y);
        public static double Distance(TurretDefinition e1, EnemyDefinition e2) => Distance(e1.X, e1.Y, e2.X, e2.Y);
        public static double Distance(EnemyDefinition e1, EnemyDefinition e2) => Distance(e1.X, e1.Y, e2.X, e2.Y);
        public static double Distance(EnemyDefinition e1, WayPoint w2) => Distance(e1.X, e1.Y, w2.X, w2.Y);
        public static double Distance(int x1, double y1, double x2, double y2) => Math.Sqrt(((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)));
    }
}
