using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.Core.Entities.Enemies;
using TDGame.Core.Entities.Projectiles;
using TDGame.Core.Entities.Turrets;
using TDGame.Core.Helpers;
using TDGame.Core.Maps;

namespace TDGame.Core
{
    public partial class Game
    {
        private void UpdateTurrets(TimeSpan passed)
        {
            foreach (var turret in Turrets)
            {
                turret.CoolingFor -= passed;

                if (turret.CoolingFor <= TimeSpan.Zero)
                {
                    switch (turret.GetDefinition().Type)
                    {
                        case TurretType.Laser: UpdateLaserTurret(turret); break;
                        case TurretType.Projectile: UpdateProjectileTurret(turret); break;
                        case TurretType.AOE: UpdateAOETurret(turret); break;
                    }
                }
            }
        }

        private void UpdateLaserTurret(TurretInstance turret)
        {
            turret.Targeting = null;
            var best = GetNearestEnemy(turret);
            if (best != null)
            {
                var turretDef = turret.GetDefinition();
                if (!DamageEnemy(best, turret.Damage, turretDef.DamageModifiers, turretDef.SlowingFactor, turretDef.SlowingDuration))
                {
                    turret.Targeting = best;
                    turret.Angle = GetAngle(best, turret);
                }
                else
                    turret.Kills++;
                turret.CoolingFor = TimeSpan.FromMilliseconds(turret.Cooldown);
            }
        }

        private void UpdateAOETurret(TurretInstance turret)
        {
            turret.Targeting = null;
            var closest = float.MaxValue;
            EnemyInstance? best = null;
            var targeting = new List<EnemyInstance>();
            foreach (var enemy in CurrentEnemies)
            {
                var dist = MathHelpers.Distance(enemy, turret);
                if (dist <= turret.Range)
                {
                    targeting.Add(enemy);
                    if (dist < closest)
                    {
                        closest = dist;
                        best = enemy;
                    }
                }
            }

            if (best != null)
            {
                var turretDef = turret.GetDefinition();
                foreach (var enemy in targeting)
                    if (DamageEnemy(enemy, turret.Damage, turretDef.DamageModifiers, turretDef.SlowingFactor, turretDef.SlowingDuration))
                        turret.Kills++;
                turret.Targeting = best;
                turret.Angle = GetAngle(best, turret);
                turret.CoolingFor = TimeSpan.FromMilliseconds(turret.Cooldown);
            }
        }

        private void UpdateProjectileTurret(TurretInstance turret)
        {
            turret.Targeting = null;
            var best = GetNearestEnemy(turret);
            if (best != null && turret.ProjectileDefinition != null)
            {
                var projectile = new ProjectileInstance(turret.ProjectileDefinition);
                projectile.X = turret.X + turret.Size / 2;
                projectile.Y = turret.Y + turret.Size / 2;
                projectile.Source = turret;
                if (turret.GetDefinition().IsTrailing)
                    projectile.Angle = GetAngle(
                        GetTrailingPoint(best, projectile),
                        turret);
                else
                    projectile.Angle = GetAngle(best, turret);
                turret.Angle = projectile.Angle;
                Projectiles.Add(projectile);
                turret.CoolingFor = TimeSpan.FromMilliseconds(turret.Cooldown);
            }
        }

        private FloatPoint GetTrailingPoint(EnemyInstance enemy, ProjectileInstance projectile)
        {
            float x = enemy.X + enemy.Size / 2;
            float y = enemy.Y + enemy.Size / 2;
            var dist = MathHelpers.Distance(enemy, projectile);
            var steps = dist / projectile.GetDefinition().Speed;
            var change = GetEnemyLocationChange(enemy.Angle, enemy.GetSpeed());
            return new FloatPoint(x + change.X * (float)steps, y + change.Y * (float)steps);
        }

        public bool CanLevelUpTurret(TurretInstance turret, Guid id)
        {
            var upgrade = turret.GetDefinition().GetAllUpgrades().FirstOrDefault(x => x.ID == id);
            if (upgrade == null)
                return false;
            if (Money < upgrade.Cost)
                return false;
            if (upgrade.Requires != null && !turret.HasUpgrades.Contains((Guid)upgrade.Requires))
                return false;
            return true;
        }

        public bool LevelUpTurret(TurretInstance turret, Guid id)
        {
            if (!CanLevelUpTurret(turret, id))
                return false;
            var upgrade = turret.GetDefinition().GetAllUpgrades().First(x => x.ID == id);
            if (upgrade == null)
                return false;
            turret.ApplyUpgrade(id);
            Money -= upgrade.Cost;

            if (OnTurretUpgraded != null)
                OnTurretUpgraded.Invoke(turret);
            return true;
        }

        public void SellTurret(TurretInstance turret)
        {
            if (!Turrets.Contains(turret))
                throw new Exception("Turret not in game!");
            Money += turret.GetTurretWorth();
            Turrets.Remove(turret);

            if (OnTurretSold != null)
                OnTurretSold.Invoke(turret);
        }

        public bool AddTurret(TurretDefinition turretDef, FloatPoint at)
        {
            if (Money < turretDef.Cost)
                return false;
            if (at.X < 0)
                return false;
            if (at.X > Map.Width - turretDef.Size)
                return false;
            if (at.Y < 0)
                return false;
            if (at.Y > Map.Height - turretDef.Size)
                return false;

            foreach (var block in Map.BlockingTiles)
                if (MathHelpers.Intersects(turretDef, at, block))
                    return false;

            foreach (var otherTurret in Turrets)
                if (MathHelpers.Intersects(turretDef, at, otherTurret))
                    return false;

            var newInstance = new TurretInstance(turretDef);
            newInstance.X = at.X;
            newInstance.Y = at.Y;
            Money -= turretDef.Cost;
            Turrets.Add(newInstance);
            if (OnTurretPurchased != null)
                OnTurretPurchased.Invoke(newInstance);

            return true;
        }
    }
}
