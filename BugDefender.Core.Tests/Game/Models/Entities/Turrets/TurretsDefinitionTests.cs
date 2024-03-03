using BugDefender.Core.Resources;

namespace BugDefender.Core.Tests.Game.Models.Entities.Turrets
{
    [TestClass]
    public class TurretsDefinitionTests
    {
        [TestMethod]
        public void AreIDsUnique()
        {
            ResourceManager.ReloadResources();
            var ids = ResourceManager.Turrets.GetResources();

            Assert.AreEqual(ids.Count, ids.Distinct().Count());
        }
    }
}
