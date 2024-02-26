using TDGame.Core.Game.Helpers;
using TDGame.Core.Game.Models.Entities.Turrets;
using TDGame.Core.Game.Models.Maps;
using TDGame.Core.Game.Modules.Turrets.SubModules;

namespace TDGame.Core.Game.Modules.Turrets
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

        public bool CanUpgradeTurret(TurretInstance turret, Guid id)
        {
            if (!Context.Turrets.Contains(turret))
                throw new Exception("Turret not in game!");
            var upgrade = turret.GetDefinition().Upgrades.FirstOrDefault(x => x.ID == id);
            if (upgrade == null)
                return false;
            if (Context.Money < upgrade.Cost)
                return false;
            if (upgrade.Requires != null && !turret.HasUpgrades.Contains((Guid)upgrade.Requires))
                return false;
            return true;
        }

        public bool UpgradeTurret(TurretInstance turret, Guid id)
        {
            if (!Context.Turrets.Contains(turret))
                throw new Exception("Turret not in game!");
            if (!CanUpgradeTurret(turret, id))
                return false;
            var upgrade = turret.GetDefinition().Upgrades.First(x => x.ID == id);
            if (upgrade == null)
                return false;
            OnBeforeTurretUpgraded?.Invoke(turret);

            upgrade.ApplyUpgrade(turret);
            Context.Money -= upgrade.Cost;
            Context.Outcome.TurretUpgraded(turret.DefinitionID);

            OnTurretUpgraded?.Invoke(turret);
            return true;
        }

        public void SellTurret(TurretInstance turret)
        {
            if (!Context.Turrets.Contains(turret))
                throw new Exception("Turret not in game!");
            Context.Money += turret.GetTurretWorth();
            Context.Turrets.Remove(turret);
            Context.Outcome.TurretSold(turret.DefinitionID);

            OnTurretSold?.Invoke(turret);
        }

        public bool AddTurret(TurretDefinition turretDef, FloatPoint at)
        {
            if (Context.Money < turretDef.Cost)
                return false;
            if (Context.Wave < turretDef.AvailableAtWave)
                return false;
            if (Context.GameStyle.TurretBlackList.Contains(turretDef.ID))
                return false;
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

            var newInstance = new TurretInstance(turretDef)
            {
                X = at.X,
                Y = at.Y,
                Angle = -(float)Math.PI / 2
            };
            Context.Money -= turretDef.Cost;
            Context.Turrets.Add(newInstance);
            OnTurretPurchased?.Invoke(newInstance);

            Context.Outcome.PlacedTurret(newInstance.DefinitionID);

            return true;
        }
    }
}
