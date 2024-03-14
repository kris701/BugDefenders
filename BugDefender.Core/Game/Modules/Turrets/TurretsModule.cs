using BugDefender.Core.Game.Helpers;
using BugDefender.Core.Game.Models.Entities.Turrets;
using BugDefender.Core.Game.Modules.Turrets.SubModules;
using BugDefender.Tools;

namespace BugDefender.Core.Game.Modules.Turrets
{
    public delegate void TurretEventHandler(TurretInstance turret);
    public class TurretsModule : BaseSuperGameModule
    {
        public TurretEventHandler? OnTurretPurchased;
        public TurretEventHandler? OnTurretSold;
        public TurretEventHandler? OnBeforeTurretUpgraded;
        public TurretEventHandler? OnTurretUpgraded;
        public TurretEventHandler? OnTurretShooting;
        public TurretEventHandler? OnTurretIdle;

        public TurretsModule(GameContext context, GameEngine game) : base(context, game)
        {
            Modules = new List<IGameModule>()
            {
                new AOETurretsModule(Context, Game),
                new LaserTurretsModule(Context, Game),
                new ProjectileTurretsModule(Context, Game),
                new InvestmentTurretsModule(Context, Game),
                new PassiveTurretsModule(Context, Game),
            };
        }

        public enum CanUpgradeResult { Success, UpgradeNotInTurret, TurretAlreadyHasUpgrade, NotEnoughMoney, MissingRequiredUpgrades }
        public CanUpgradeResult CanUpgradeTurret(TurretInstance turret, Guid id)
        {
            if (!Context.Turrets.Contains(turret))
                throw new Exception("Turret not in game!");
            var upgrade = turret.GetDefinition().Upgrades.FirstOrDefault(x => x.ID == id);
            if (upgrade == null)
                return CanUpgradeResult.UpgradeNotInTurret;
            if (turret.HasUpgrades.Contains(id))
                return CanUpgradeResult.TurretAlreadyHasUpgrade;
            if (Context.Money < upgrade.Cost)
                return CanUpgradeResult.NotEnoughMoney;
            if (upgrade.Requires != null && !turret.HasUpgrades.Contains((Guid)upgrade.Requires))
                return CanUpgradeResult.MissingRequiredUpgrades;
            return CanUpgradeResult.Success;
        }

        public bool UpgradeTurret(TurretInstance turret, Guid id)
        {
            if (!Context.Turrets.Contains(turret))
                throw new Exception("Turret not in game!");
            if (CanUpgradeTurret(turret, id) != CanUpgradeResult.Success)
                return false;
            var upgrade = turret.GetDefinition().Upgrades.First(x => x.ID == id);
            if (upgrade == null)
                return false;
            OnBeforeTurretUpgraded?.Invoke(turret);

            upgrade.Apply(turret);
            Context.Money -= upgrade.Cost;
            Context.Stats.TurretUpgraded(turret.DefinitionID);

            OnTurretUpgraded?.Invoke(turret);
            return true;
        }

        public void SellTurret(TurretInstance turret)
        {
            if (!Context.Turrets.Contains(turret))
                throw new Exception("Turret not in game!");
            Context.Money += turret.GetTurretWorth(Context.GameStyle);
            Context.Turrets.Remove(turret);
            Context.Stats.TurretSold(turret.DefinitionID);

            OnTurretSold?.Invoke(turret);
        }

        public bool IsTurretPlacementOk(TurretDefinition turretDef, FloatPoint at)
        {
            if (at.X < 0)
                return false;
            if (at.X > Context.Map.Width - turretDef.Size)
                return false;
            if (at.Y < 0)
                return false;
            if (at.Y > Context.Map.Height - turretDef.Size)
                return false;
            foreach (var block in Context.Map.BlockingTiles)
                if (MathHelpers.Intersects(turretDef, at, block))
                    return false;
            foreach (var otherTurret in Context.Turrets)
                if (MathHelpers.Intersects(turretDef, at, otherTurret))
                    return false;
            return true;
        }

        public enum AddTurretResult { Success, NotEnoughMoney, NotAvailableForWave, Blacklisted, NotWhiteListed, PlacementInvalid }
        public AddTurretResult AddTurret(TurretDefinition turretDef, FloatPoint at)
        {
            if (Context.Money < turretDef.Cost)
                return AddTurretResult.NotEnoughMoney;
            if (Context.Wave < turretDef.AvailableAtWave)
                return AddTurretResult.NotAvailableForWave;
            if (Context.GameStyle.TurretBlackList.Contains(turretDef.ID))
                return AddTurretResult.Blacklisted;
            if (Context.GameStyle.TurretWhiteList.Count > 0 && !Context.GameStyle.TurretWhiteList.Contains(turretDef.ID))
                return AddTurretResult.NotWhiteListed;
            if (!IsTurretPlacementOk(turretDef, at))
                return AddTurretResult.PlacementInvalid;

            var newInstance = new TurretInstance(turretDef)
            {
                X = at.X,
                Y = at.Y,
                Angle = -(float)Math.PI / 2
            };
            Context.Money -= turretDef.Cost;
            Context.Turrets.Add(newInstance);
            OnTurretPurchased?.Invoke(newInstance);

            Context.Stats.PlacedTurret(newInstance.DefinitionID);

            return AddTurretResult.Success;
        }
    }
}
