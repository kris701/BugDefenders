using BugDefender.Core.Game.Models.Maps;
using BugDefender.Core.Resources;
using static BugDefender.Core.Game.Modules.Turrets.TurretsModule;

namespace BugDefender.Core.Tests.Game.Modules.Turrets
{
    [TestClass]
    public class TurretsModuleTests : BaseModuleTests
    {
        public static IEnumerable<object[]> AllTurrets()
        {
            foreach (var id in ResourceManager.Turrets.GetResources())
                yield return new object[] { id };
        }

        public static IEnumerable<object[]> AllTurretsAndUpgrades()
        {
            foreach (var turretID in ResourceManager.Turrets.GetResources())
                foreach (var upgrade in ResourceManager.Turrets.GetResource(turretID).Upgrades)
                    yield return new object[] { turretID, upgrade.ID };
        }

        public static IEnumerable<object[]> AllInitialLegalUpgrades()
        {
            foreach (var turretID in ResourceManager.Turrets.GetResources())
                foreach (var upgrade in ResourceManager.Turrets.GetResource(turretID).Upgrades)
                    if (upgrade.Requires == null)
                        yield return new object[] { turretID, upgrade.ID };
        }

        public static IEnumerable<object[]> AllInitialIllegalUpgrades()
        {
            foreach (var turretID in ResourceManager.Turrets.GetResources())
                foreach (var upgrade in ResourceManager.Turrets.GetResource(turretID).Upgrades)
                    if (upgrade.Requires != null)
                        yield return new object[] { turretID, upgrade.ID };
        }

        #region CanUpgradeTurret

        [TestMethod]
        [DynamicData(nameof(AllInitialLegalUpgrades), DynamicDataSourceType.Method)]
        public void Can_CanUpgradeTurret_IfLegal(Guid turretID, Guid upgradeID)
        {
            // ARRANGE
            var game = GetBaseGame();
            var turretDef = ResourceManager.Turrets.GetResource(turretID);
            game.Context.Money = turretDef.Cost;
            game.Context.Wave = turretDef.AvailableAtWave;
            Assert.AreEqual(AddTurretResult.Success, game.TurretsModule.AddTurret(turretDef, new Tools.FloatPoint(50, 50)));
            var instance = game.Context.Turrets.ElementAt(0);
            Assert.IsNotNull(instance);
            var upgrade = turretDef.Upgrades.First(x => x.ID == upgradeID);
            game.Context.Money += upgrade.Cost;

            // ACT
            var result = game.TurretsModule.CanUpgradeTurret(instance, upgrade.ID);

            // ASSERT
            Assert.AreEqual(CanUpgradeResult.Success, result);
        }

        [TestMethod]
        [DynamicData(nameof(AllTurretsAndUpgrades), DynamicDataSourceType.Method)]
        public void Cant_CanUpgradeTurret_IfNotEnoughMoney(Guid turretID, Guid upgradeID)
        {
            // ARRANGE
            var game = GetBaseGame();
            var turretDef = ResourceManager.Turrets.GetResource(turretID);
            game.Context.Money = turretDef.Cost;
            game.Context.Wave = turretDef.AvailableAtWave;
            Assert.AreEqual(AddTurretResult.Success, game.TurretsModule.AddTurret(turretDef, new Tools.FloatPoint(50, 50)));
            var instance = game.Context.Turrets.ElementAt(0);
            Assert.IsNotNull(instance);
            var upgrade = turretDef.Upgrades.First(x => x.ID == upgradeID);
            game.Context.Money += upgrade.Cost - 1;

            // ACT
            var result = game.TurretsModule.CanUpgradeTurret(instance, upgrade.ID);

            // ASSERT
            Assert.AreEqual(CanUpgradeResult.NotEnoughMoney, result);
        }

        [TestMethod]
        [DynamicData(nameof(AllTurrets), DynamicDataSourceType.Method)]
        public void Cant_CanUpgradeTurret_IfUpgradeNotInGame(Guid turretID)
        {
            // ARRANGE
            var game = GetBaseGame();
            var turretDef = ResourceManager.Turrets.GetResource(turretID);
            game.Context.Money = turretDef.Cost;
            game.Context.Wave = turretDef.AvailableAtWave;
            Assert.AreEqual(AddTurretResult.Success, game.TurretsModule.AddTurret(turretDef, new Tools.FloatPoint(50, 50)));
            var instance = game.Context.Turrets.ElementAt(0);
            Assert.IsNotNull(instance);

            // ACT
            var result = game.TurretsModule.CanUpgradeTurret(instance, Guid.NewGuid());

            // ASSERT
            Assert.AreEqual(CanUpgradeResult.UpgradeNotInTurret, result);
        }

        [TestMethod]
        [DynamicData(nameof(AllTurretsAndUpgrades), DynamicDataSourceType.Method)]
        public void Cant_CanUpgradeTurret_IfTurretAlreadyHasUpgrade(Guid turretID, Guid upgradeID)
        {
            // ARRANGE
            var game = GetBaseGame();
            var turretDef = ResourceManager.Turrets.GetResource(turretID);
            game.Context.Money = turretDef.Cost;
            game.Context.Wave = turretDef.AvailableAtWave;
            Assert.AreEqual(AddTurretResult.Success, game.TurretsModule.AddTurret(turretDef, new Tools.FloatPoint(50, 50)));
            var instance = game.Context.Turrets.ElementAt(0);
            Assert.IsNotNull(instance);
            var upgrade = turretDef.Upgrades.First(x => x.ID == upgradeID);
            game.Context.Money += upgrade.Cost;
            instance.HasUpgrades.Add(upgrade.ID);

            // ACT
            var result = game.TurretsModule.CanUpgradeTurret(instance, upgrade.ID);

            // ASSERT
            Assert.AreEqual(CanUpgradeResult.TurretAlreadyHasUpgrade, result);
        }

        [TestMethod]
        [DynamicData(nameof(AllInitialIllegalUpgrades), DynamicDataSourceType.Method)]
        public void Cant_CanUpgradeTurret_IfMissingRequired(Guid turretID, Guid upgradeID)
        {
            // ARRANGE
            var game = GetBaseGame();
            var turretDef = ResourceManager.Turrets.GetResource(turretID);
            game.Context.Money = turretDef.Cost;
            game.Context.Wave = turretDef.AvailableAtWave;
            Assert.AreEqual(AddTurretResult.Success, game.TurretsModule.AddTurret(turretDef, new Tools.FloatPoint(50, 50)));
            var instance = game.Context.Turrets.ElementAt(0);
            Assert.IsNotNull(instance);
            var upgrade = turretDef.Upgrades.First(x => x.ID == upgradeID);
            game.Context.Money += upgrade.Cost;

            // ACT
            var result = game.TurretsModule.CanUpgradeTurret(instance, upgrade.ID);

            // ASSERT
            Assert.AreEqual(CanUpgradeResult.MissingRequiredUpgrades, result);
        }

        #endregion

        #region UpgradeTurret

        [TestMethod]
        [DynamicData(nameof(AllInitialLegalUpgrades), DynamicDataSourceType.Method)]
        public void Can_UpgradeTurret_IfLegal(Guid turretID, Guid upgradeID)
        {
            // ARRANGE
            var game = GetBaseGame();
            var turretDef = ResourceManager.Turrets.GetResource(turretID);
            game.Context.Money = turretDef.Cost;
            game.Context.Wave = turretDef.AvailableAtWave;
            Assert.AreEqual(AddTurretResult.Success, game.TurretsModule.AddTurret(turretDef, new Tools.FloatPoint(50, 50)));
            var instance = game.Context.Turrets.ElementAt(0);
            Assert.IsNotNull(instance);
            var upgrade = turretDef.Upgrades.First(x => x.ID == upgradeID);
            game.Context.Money += upgrade.Cost;
            var preMoney = game.Context.Money;

            // ACT
            var result = game.TurretsModule.UpgradeTurret(instance, upgrade.ID);

            // ASSERT
            Assert.IsTrue(result);
            Assert.AreEqual(preMoney - upgrade.Cost, game.Context.Money);
        }

        [TestMethod]
        [DynamicData(nameof(AllInitialIllegalUpgrades), DynamicDataSourceType.Method)]
        public void Cant_UpgradeTurret_IfNotLegal(Guid turretID, Guid upgradeID)
        {
            // ARRANGE
            var game = GetBaseGame();
            var turretDef = ResourceManager.Turrets.GetResource(turretID);
            game.Context.Money = turretDef.Cost;
            game.Context.Wave = turretDef.AvailableAtWave;
            Assert.AreEqual(AddTurretResult.Success, game.TurretsModule.AddTurret(turretDef, new Tools.FloatPoint(50, 50)));
            var instance = game.Context.Turrets.ElementAt(0);
            Assert.IsNotNull(instance);
            var upgrade = turretDef.Upgrades.First(x => x.ID == upgradeID);
            game.Context.Money += upgrade.Cost;

            // ACT
            var result = game.TurretsModule.UpgradeTurret(instance, upgrade.ID);

            // ASSERT
            Assert.IsFalse(result);
        }

        #endregion

        #region SellTurret


        [TestMethod]
        [DynamicData(nameof(AllTurrets), DynamicDataSourceType.Method)]
        public void Can_SellTurret(Guid turretID)
        {
            // ARRANGE
            var game = GetBaseGame();
            var turretDef = ResourceManager.Turrets.GetResource(turretID);
            game.Context.Money = turretDef.Cost;
            game.Context.Wave = turretDef.AvailableAtWave;
            Assert.AreEqual(AddTurretResult.Success, game.TurretsModule.AddTurret(turretDef, new Tools.FloatPoint(50, 50)));
            var instance = game.Context.Turrets.ElementAt(0);
            Assert.IsNotNull(instance);
            Assert.IsTrue(game.Context.Turrets.Contains(instance));
            var preMoney = game.Context.Money;

            // ACT
            game.TurretsModule.SellTurret(instance);

            // ASSERT
            Assert.IsFalse(game.Context.Turrets.Contains(instance));
            Assert.AreEqual((turretDef.Cost * game.Context.GameStyle.TurretRefundPenalty) - preMoney, game.Context.Money);
        }

        #endregion

        #region IsTurretPlacementOk

        [TestMethod]
        [DynamicData(nameof(AllTurrets), DynamicDataSourceType.Method)]
        public void Can_IsTurretPlacementOk_IfValid(Guid turretID)
        {
            // ARRANGE
            var game = GetBaseGame();
            var turretDef = ResourceManager.Turrets.GetResource(turretID);

            // ACT
            var result = game.TurretsModule.IsTurretPlacementOk(turretDef, new Tools.FloatPoint(300, 300));

            // ASSERT
            Assert.IsTrue(result);
        }

        [TestMethod]
        [DynamicData(nameof(AllTurrets), DynamicDataSourceType.Method)]
        public void Cant_IsTurretPlacementOk_IfBlockingTile(Guid turretID)
        {
            // ARRANGE
            var game = GetBaseGame();
            var turretDef = ResourceManager.Turrets.GetResource(turretID);
            game.Context.Map.BlockingTiles.Add(new BlockedTile(
                200, 200,
                200, 200));

            // ACT
            var result = game.TurretsModule.IsTurretPlacementOk(turretDef, new Tools.FloatPoint(300, 300));

            // ASSERT
            Assert.IsFalse(result);
        }

        [TestMethod]
        [DynamicData(nameof(AllTurrets), DynamicDataSourceType.Method)]
        public void Cant_IsTurretPlacementOk_IfOutsideOfmap(Guid turretID)
        {
            // ARRANGE
            var game = GetBaseGame();
            var turretDef = ResourceManager.Turrets.GetResource(turretID);

            // ACT
            var result = game.TurretsModule.IsTurretPlacementOk(turretDef, new Tools.FloatPoint(-50, 300));

            // ASSERT
            Assert.IsFalse(result);
        }

        [TestMethod]
        [DynamicData(nameof(AllTurrets), DynamicDataSourceType.Method)]
        public void Cant_IsTurretPlacementOk_IfAnotherTurret(Guid turretID)
        {
            // ARRANGE
            var game = GetBaseGame();
            var turretDef = ResourceManager.Turrets.GetResource(turretID);
            game.Context.Money = turretDef.Cost;
            game.Context.Wave = turretDef.AvailableAtWave;
            Assert.AreEqual(AddTurretResult.Success, game.TurretsModule.AddTurret(turretDef, new Tools.FloatPoint(300, 300)));

            // ACT
            var result = game.TurretsModule.IsTurretPlacementOk(turretDef, new Tools.FloatPoint(300, 300));

            // ASSERT
            Assert.IsFalse(result);
        }

        #endregion

        #region AddTurret

        [TestMethod]
        [DynamicData(nameof(AllTurrets), DynamicDataSourceType.Method)]
        public void Can_AddTurret_IfValid(Guid turretID)
        {
            // ARRANGE
            var game = GetBaseGame();
            var turretDef = ResourceManager.Turrets.GetResource(turretID);
            game.Context.Money = turretDef.Cost;
            game.Context.Wave = turretDef.AvailableAtWave;
            var preMoney = game.Context.Money;

            // ACT
            var result = game.TurretsModule.AddTurret(turretDef, new Tools.FloatPoint(300, 300));

            // ASSERT
            Assert.AreEqual(AddTurretResult.Success, result);
            Assert.AreEqual(preMoney - turretDef.Cost, game.Context.Money);
        }

        [TestMethod]
        [DynamicData(nameof(AllTurrets), DynamicDataSourceType.Method)]
        public void Cant_AddTurret_IfNotEnoughMoney(Guid turretID)
        {
            // ARRANGE
            var game = GetBaseGame();
            var turretDef = ResourceManager.Turrets.GetResource(turretID);
            game.Context.Money = turretDef.Cost - 1;
            game.Context.Wave = turretDef.AvailableAtWave;

            // ACT
            var result = game.TurretsModule.AddTurret(turretDef, new Tools.FloatPoint(300, 300));

            // ASSERT
            Assert.AreEqual(AddTurretResult.NotEnoughMoney, result);
        }

        [TestMethod]
        [DynamicData(nameof(AllTurrets), DynamicDataSourceType.Method)]
        public void Cant_AddTurret_IfWaveTooLow(Guid turretID)
        {
            // ARRANGE
            var game = GetBaseGame();
            var turretDef = ResourceManager.Turrets.GetResource(turretID);
            game.Context.Money = turretDef.Cost;
            game.Context.Wave = turretDef.AvailableAtWave - 1;

            // ACT
            var result = game.TurretsModule.AddTurret(turretDef, new Tools.FloatPoint(300, 300));

            // ASSERT
            Assert.AreEqual(AddTurretResult.NotAvailableForWave, result);
        }

        [TestMethod]
        [DynamicData(nameof(AllTurrets), DynamicDataSourceType.Method)]
        public void Cant_AddTurret_IfBlacklisted(Guid turretID)
        {
            // ARRANGE
            var game = GetBaseGame();
            var turretDef = ResourceManager.Turrets.GetResource(turretID);
            game.Context.Money = turretDef.Cost;
            game.Context.Wave = turretDef.AvailableAtWave;
            game.Context.GameStyle.TurretBlackList.Add(turretID);

            // ACT
            var result = game.TurretsModule.AddTurret(turretDef, new Tools.FloatPoint(300, 300));

            // ASSERT
            Assert.AreEqual(AddTurretResult.Blacklisted, result);
        }

        [TestMethod]
        [DynamicData(nameof(AllTurrets), DynamicDataSourceType.Method)]
        public void Cant_AddTurret_IfNotWhitelisted(Guid turretID)
        {
            // ARRANGE
            var game = GetBaseGame();
            var turretDef = ResourceManager.Turrets.GetResource(turretID);
            game.Context.Money = turretDef.Cost;
            game.Context.Wave = turretDef.AvailableAtWave;
            game.Context.GameStyle.TurretWhiteList.Add(Guid.NewGuid());

            // ACT
            var result = game.TurretsModule.AddTurret(turretDef, new Tools.FloatPoint(300, 300));

            // ASSERT
            Assert.AreEqual(AddTurretResult.NotWhiteListed, result);
        }

        [TestMethod]
        [DynamicData(nameof(AllTurrets), DynamicDataSourceType.Method)]
        public void Cant_AddTurret_IfPlacementInvalid(Guid turretID)
        {
            // ARRANGE
            var game = GetBaseGame();
            var turretDef = ResourceManager.Turrets.GetResource(turretID);
            game.Context.Money = turretDef.Cost;
            game.Context.Wave = turretDef.AvailableAtWave;

            // ACT
            var result = game.TurretsModule.AddTurret(turretDef, new Tools.FloatPoint(-50, 300));

            // ASSERT
            Assert.AreEqual(AddTurretResult.PlacementInvalid, result);
        }

        #endregion
    }
}
