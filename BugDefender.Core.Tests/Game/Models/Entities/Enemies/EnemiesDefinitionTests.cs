using BugDefender.Core.Game.Models.Entities.Turrets;
using BugDefender.Core.Resources;

namespace BugDefender.Core.Tests.Game.Models.Entities.Enemies
{
    [TestClass]
    public class EnemiesDefinitionTests
    {
        [TestMethod]
        public void AreIDsUnique()
        {
            ResourceManager.ReloadResources();
            var ids = ResourceManager.Enemies.GetResources();

            Assert.AreEqual(ids.Count, ids.Distinct().Count());
        }
    }
}
