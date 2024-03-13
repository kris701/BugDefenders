using BugDefender.Core.Game.Models;
using BugDefender.Core.Game.Models.Entities.Enemies;
using BugDefender.Core.Game.Models.Entities.Turrets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BugDefender.Core.Game.Models.Entities.Enemies.EnemyDefinition;

namespace BugDefender.Core.Tests.Game.Modules.Enemies
{
    [TestClass]
    public class EnemiesModuleTests : BaseModuleTests
    {
        #region GetBestEnemy

        private static List<EnemyInstance> BestEnemyTestData(Guid enemyDefinitionID)
        {
            return new List<EnemyInstance>()
            {
                new EnemyInstance(enemyDefinitionID, 1)
                {
                    X = 100,
                    Y = 100,
                    Health = 100,
                },
                new EnemyInstance(enemyDefinitionID, 1)
                {
                    X = 400,
                    Y = 400,
                    Health = 50
                },
                new EnemyInstance(enemyDefinitionID, 1)
                {
                    X = 800,
                    Y = 800,
                    Health = 10
                }
            };
        }

        private class SimplePosModel : BasePositionModel
        {

        }

        [TestMethod]
        [DataRow("709a9b8d-7a8b-488c-827f-bc7ed266226f", 50, 50, 100, TurretInstance.TargetingTypes.Closest, 0)]
        [DataRow("709a9b8d-7a8b-488c-827f-bc7ed266226f", 50, 50, 1000, TurretInstance.TargetingTypes.Closest, 0)]
        [DataRow("709a9b8d-7a8b-488c-827f-bc7ed266226f", 1000, 1000, 10000, TurretInstance.TargetingTypes.Closest, 2)]
        [DataRow("709a9b8d-7a8b-488c-827f-bc7ed266226f", 500, 500, 1000, TurretInstance.TargetingTypes.Closest, 1)]
        [DataRow("709a9b8d-7a8b-488c-827f-bc7ed266226f", 0, 0, 1, TurretInstance.TargetingTypes.Closest, -1)]

        [DataRow("709a9b8d-7a8b-488c-827f-bc7ed266226f", 50, 50, 100, TurretInstance.TargetingTypes.Weakest, 0)]
        [DataRow("709a9b8d-7a8b-488c-827f-bc7ed266226f", 50, 50, 1000, TurretInstance.TargetingTypes.Weakest, 1)]
        [DataRow("709a9b8d-7a8b-488c-827f-bc7ed266226f", 1000, 1000, 10000, TurretInstance.TargetingTypes.Weakest, 2)]
        [DataRow("709a9b8d-7a8b-488c-827f-bc7ed266226f", 500, 500, 1000, TurretInstance.TargetingTypes.Weakest, 2)]
        [DataRow("709a9b8d-7a8b-488c-827f-bc7ed266226f", 0, 0, 1, TurretInstance.TargetingTypes.Weakest, -1)]

        [DataRow("709a9b8d-7a8b-488c-827f-bc7ed266226f", 50, 50, 100, TurretInstance.TargetingTypes.Strongest, 0)]
        [DataRow("709a9b8d-7a8b-488c-827f-bc7ed266226f", 50, 50, 1000, TurretInstance.TargetingTypes.Strongest, 0)]
        [DataRow("709a9b8d-7a8b-488c-827f-bc7ed266226f", 1000, 1000, 10000, TurretInstance.TargetingTypes.Strongest, 0)]
        [DataRow("709a9b8d-7a8b-488c-827f-bc7ed266226f", 500, 500, 1000, TurretInstance.TargetingTypes.Strongest, 0)]
        [DataRow("709a9b8d-7a8b-488c-827f-bc7ed266226f", 0, 0, 1, TurretInstance.TargetingTypes.Strongest, -1)]
        public void Can_GetBestEnemy(string enemyDefinitionID, float x, float y, float range, TurretInstance.TargetingTypes targetType, int expectedIndex)
        {
            // ARRANGE
            var game = GetBaseGame();
            var enemies = BestEnemyTestData(new Guid(enemyDefinitionID));
            foreach (var enemy in enemies)
                game.Context.CurrentEnemies.Add(enemy);
            var location = new SimplePosModel()
            {
                X = x,
                Y = y
            };
            var canTarget = new HashSet<EnemyDefinition.EnemyTerrrainTypes>();
            foreach (var option in Enum.GetValues(typeof(EnemyTerrrainTypes)))
                if (option is EnemyTerrrainTypes op)
                    canTarget.Add(op);

            // ACT
            var best = game.EnemiesModule.GetBestEnemy(location, range, targetType, canTarget);

            // ASSERT
            if (expectedIndex == -1)
            {
                Assert.IsNull(best);
            }
            else
            {
                Assert.IsNotNull(best);
                int index = 0;
                foreach (var enemy in game.Context.CurrentEnemies.Enemies)
                {
                    if (index == expectedIndex)
                        Assert.AreEqual(enemy, best);
                    index++;
                }
            }
        }

        #endregion
    }
}
