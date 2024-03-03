using BugDefender.Core.Game.Models.Entities.Turrets;
using BugDefender.Core.Resources;

namespace BugDefender.Core.Tests.Game.Models.Entities.Projectiles
{
    [TestClass]
    public class ProjectilesDefinitionTests
    {
        [TestMethod]
        public void AreIDsUnique()
        {
            ResourceManager.ReloadResources();
            var ids = ResourceManager.Projectiles.GetResources();

            Assert.AreEqual(ids.Count, ids.Distinct().Count());
        }
    }
}
