using TDGame.Core.Helpers;
using TDGame.Core.Models.Entities.Turrets;
using TDGame.Core.Models.Maps;

namespace TDGame.Core
{
    public partial class Game
    {
        public bool CanLevelUpTurret(TurretInstance turret, Guid id)
        {
            var upgrade = turret.GetDefinition().Upgrades.FirstOrDefault(x => x.ID == id);
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
            var upgrade = turret.GetDefinition().Upgrades.First(x => x.ID == id);
            if (upgrade == null)
                return false;
            upgrade.ApplyUpgrade(turret);
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
            if (GameStyle.TurretBlackList.Contains(turretDef.ID))
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
