using BugDefender.Core.Resources;

namespace BugDefender.Core.Tests.Game.Models.GameStyles
{
    [TestClass]
    public class GameStylesDefinitionTests
    {
        [TestMethod]
        public void AreIDsUnique()
        {
            ResourceManager.ReloadResources();
            var ids = ResourceManager.GameStyles.GetResources();

            Assert.AreEqual(ids.Count, ids.Distinct().Count());
        }
    }
}
