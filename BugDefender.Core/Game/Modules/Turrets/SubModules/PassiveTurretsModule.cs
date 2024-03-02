using BugDefender.Core.Game.Helpers;
using BugDefender.Core.Game.Models.Entities.Projectiles.Modules;
using BugDefender.Core.Game.Models.Entities.Turrets;
using BugDefender.Core.Game.Models.Entities.Turrets.Modules;

namespace BugDefender.Core.Game.Modules.Turrets.SubModules
{
    public class PassiveTurretsModule : BaseTurretModule<PassiveTurretDefinition>
    {
        public PassiveTurretsModule(GameContext context, GameEngine game) : base(context, game)
        {
        }

        public override void Initialize()
        {
            Game.TurretsModule.OnTurretPurchased += TurretAdded;
            Game.TurretsModule.OnTurretSold += TurretRemoved;
            Game.TurretsModule.OnBeforeTurretUpgraded += TurretBeforeUpgraded;
            Game.TurretsModule.OnTurretUpgraded += TurretUpgraded;
            if (Context.Turrets.Count > 0)
                UpdateEffectedList();
        }

        private void UpdateEffectedList()
        {
            foreach (var turret in Context.Turrets)
            {
                if (turret.TurretInfo is PassiveTurretDefinition def)
                {
                    def.Affected.Clear();
                    var range = (float)Math.Pow(def.Range, 2);
                    foreach (var other in Context.Turrets)
                    {
                        if (other == turret || other.TurretInfo is PassiveTurretDefinition)
                            continue;
                        var dist = MathHelpers.SqrDistance(turret, other);
                        if (dist < range)
                            def.Affected.Add(other);
                    }
                }
            }
        }

        private void UpdateEffectedListAndApply(PassiveTurretDefinition? unApply = null, PassiveTurretDefinition? apply = null)
        {
            foreach (var other in Context.Turrets)
                if (other.TurretInfo is PassiveTurretDefinition def && unApply != def)
                    foreach (var effected in def.Affected)
                        def.TryUnApplyUpgradeEffectOnObject(effected.TurretInfo);

            UpdateEffectedList();

            foreach (var other in Context.Turrets)
                if (other.TurretInfo is PassiveTurretDefinition def && apply != def)
                    foreach (var effected in def.Affected)
                        def.TryApplyUpgradeEffectOnObject(effected.TurretInfo);
        }

        private void TurretAdded(TurretInstance turret)
        {
            UpdateEffectedListAndApply();
        }

        private void TurretRemoved(TurretInstance turret)
        {
            if (turret.TurretInfo is PassiveTurretDefinition def)
            {
                foreach (var effected in def.Affected)
                    def.TryUnApplyUpgradeEffectOnObject(effected.TurretInfo);
                UpdateEffectedListAndApply();
            }
            else
            {
                foreach (var other in Context.Turrets)
                {
                    if (other.TurretInfo is PassiveTurretDefinition oDef)
                    {
                        if (oDef.Affected.Contains(turret))
                            oDef.Affected.Remove(turret);
                    }
                }
            }
        }

        private void TurretBeforeUpgraded(TurretInstance turret)
        {
            if (turret.TurretInfo is PassiveTurretDefinition def)
            {
                foreach (var effected in def.Affected)
                    def.TryUnApplyUpgradeEffectOnObject(effected.TurretInfo);
                UpdateEffectedListAndApply();
            }
        }

        private void TurretUpgraded(TurretInstance turret)
        {
            if (turret.TurretInfo is PassiveTurretDefinition def)
                UpdateEffectedListAndApply(def);
        }

        public override void UpdateTurret(TimeSpan passed, TurretInstance turret, PassiveTurretDefinition def)
        {
        }
    }
}
