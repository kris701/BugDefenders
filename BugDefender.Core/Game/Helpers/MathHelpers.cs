using BugDefender.Core.Game.Models;
using BugDefender.Core.Game.Models.Entities.Turrets;
using BugDefender.Core.Game.Models.Maps;
using BugDefender.Tools;
using System.Drawing;

namespace BugDefender.Core.Game.Helpers
{
    public static class MathHelpers
    {
        public static bool Intersects(TurretDefinition turret, FloatPoint at, BlockedTile tile)
        {
            return new Rectangle((int)at.X, (int)at.Y, (int)turret.Size, (int)turret.Size).IntersectsWith(
                new Rectangle((int)tile.X, (int)tile.Y, (int)tile.Width, (int)tile.Height));
        }
        public static bool Intersects(TurretDefinition e1, FloatPoint at, TurretInstance e2)
        {
            return new Rectangle((int)at.X, (int)at.Y, (int)e1.Size, (int)e1.Size).IntersectsWith(
                new Rectangle((int)e2.X, (int)e2.Y, (int)e2.Size, (int)e2.Size));
        }

        public static float Distance(IPosition p1, IPosition p2) => Distance(p1.CenterX, p1.CenterY, p2.CenterX, p2.CenterY);
        public static float Distance(IPosition p1, FloatPoint p2) => Distance(p1.CenterX, p1.CenterY, p2.X, p2.Y);
        public static float Distance(float x1, float y1, float x2, float y2) => (float)Math.Sqrt(SqrDistance(x1, y1, x2, y2));

        public static float SqrDistance(IPosition p1, IPosition p2) => SqrDistance(p1.CenterX, p1.CenterY, p2.CenterX, p2.CenterY);
        public static float SqrDistance(IPosition p1, FloatPoint p2) => SqrDistance(p1.CenterX, p1.CenterY, p2.X, p2.Y);
        public static float SqrDistance(float x1, float y1, float x2, float y2) => (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2);

        public static float GetAngle(FloatPoint target, IPosition item) => GetAngle(target.X, target.Y, item.CenterX, item.CenterY);
        public static float GetAngle(IPosition item1, IPosition item2) => GetAngle(item1.CenterX, item1.CenterY, item2.CenterX, item2.CenterY);
        public static float GetAngle(float x1, float y1, float x2, float y2)
        {
            var a = y1 - y2;
            var b = x1 - x2;
            return (float)Math.Atan2(a, b);
        }

        public static FloatPoint GetPredictedLocation(float angle, float speed, float multiplier = 1)
        {
            var xMod = Math.Cos(angle);
            var yMod = Math.Sin(angle);
            return new FloatPoint(
                (float)xMod * speed * multiplier,
                (float)yMod * speed * multiplier);
        }
    }
}
