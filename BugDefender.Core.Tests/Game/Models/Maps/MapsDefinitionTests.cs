using BugDefender.Core.Resources;

namespace BugDefender.Core.Tests.Game.Models.Maps
{
    [TestClass]
    public class MapsDefinitionTests
    {
        [TestMethod]
        public void AreIDsUnique()
        {
            ResourceManager.ReloadResources();
            var ids = ResourceManager.Maps.GetResources();

            Assert.AreEqual(ids.Count, ids.Distinct().Count());
        }
    }
}
