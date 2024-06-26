﻿using BugDefender.Core.Game.Models.Entities.Turrets;
using BugDefender.Core.Resources;

namespace BugDefender.Core.Tests.Game.Models.Entities.Upgrades
{
    [TestClass]
    public class UpgradeDefinitionTests
    {
        public static IEnumerable<object[]> UpgradeTurretIDs()
        {
            foreach (var turretId in ResourceManager.Turrets.GetResources())
            {
                var turret = ResourceManager.Turrets.GetResource(turretId);
                foreach (var upgrades in turret.Upgrades)
                    yield return new object[] { turretId, upgrades.ID };
            }
        }

        [TestMethod]
        [DynamicData(nameof(UpgradeTurretIDs), DynamicDataSourceType.Method)]
        public void Can_Apply(Guid turretID, Guid upgradeID)
        {
            // ARRANGE
            ResourceManager.ReloadResources();
            var turretDef = ResourceManager.Turrets.GetResource(turretID);
            var turretInstance = new TurretInstance(turretDef);
            var upgrade = turretDef.Upgrades.First(x => x.ID == upgradeID);
            var before = turretInstance.ToString();

            // ACT
            upgrade.Apply(turretInstance);

            // ASSERT
            Assert.IsTrue(turretInstance.HasUpgrades.Contains(upgradeID));
            Assert.AreNotEqual(before, turretInstance.ToString());
        }
    }
}
