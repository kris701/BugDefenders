using BugDefender.Core.Game.Models.Entities.Turrets;
using BugDefender.Core.Resources;

namespace BugDefender.Core.Tests.Game.Models.EnemyTypes
{
    [TestClass]
    public class EnemyTypesDefinitionTests
    {
        [TestMethod]
        public void AreIDsUnique()
        {
            ResourceManager.ReloadResources();
            var ids = ResourceManager.EnemyTypes.GetResources();

            Assert.AreEqual(ids.Count, ids.Distinct().Count());
        }
    }
}
