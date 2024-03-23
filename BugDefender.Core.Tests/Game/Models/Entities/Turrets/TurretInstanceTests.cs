using BugDefender.Core.Game.Models.Entities.Turrets;
using BugDefender.Core.Game.Models.GameStyles;
using BugDefender.Core.Resources;

namespace BugDefender.Core.Tests.Game.Models.Entities.Turrets
{
    [TestClass]
    public class TurretInstanceTests
    {
        public static IEnumerable<object[]> UpgradeTurretIDs()
        {
            foreach (var gameStyleID in ResourceManager.GameStyles.GetResources())
            {
                var gameStyle = ResourceManager.GameStyles.GetResource(gameStyleID);
                foreach (var turretId in ResourceManager.Turrets.GetResources())
                {
                    var turret = ResourceManager.Turrets.GetResource(turretId);
                    foreach (var upgrades in turret.Upgrades)
                        yield return new object[] { turretId, upgrades.ID, gameStyle.TurretRefundPenalty };
                }
            }
        }

        [TestMethod]
        [DynamicData(nameof(UpgradeTurretIDs), DynamicDataSourceType.Method)]
        public void Can_GetCorrectTurretWorth(Guid turretID, Guid upgradeID, float refundPenalty)
        {
            // ARRANGE
            ResourceManager.ReloadResources();
            var turretDef = ResourceManager.Turrets.GetResource(turretID);
            var turretInstance = new TurretInstance(turretDef);
            var upgrade = turretDef.Upgrades.First(x => x.ID == upgradeID);
            var expect = (int)((turretDef.Cost + upgrade.Cost) * refundPenalty);
            upgrade.Apply(turretInstance);
            var gameStyle = new GameStyleDefinition(
                        Guid.NewGuid(),
                        "Empty",
                        "",
                        1,
                        1,
                        1,
                        1,
                        1,
                        1,
                        1,
                        1,
                        new List<Guid>(),
                        new List<Guid>(),
                        new List<Guid>(),
                        new List<Guid>(),
                        1,
                        refundPenalty,
                        false
                    );

            // ACT
            var result = turretInstance.GetTurretWorth(gameStyle);

            // ASSERT
            Assert.AreEqual(expect, result);
        }
    }
}
