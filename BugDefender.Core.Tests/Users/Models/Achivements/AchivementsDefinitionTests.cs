using BugDefender.Core.Resources;

namespace BugDefender.Core.Tests.Users.Models.Achivements
{
    [TestClass]
    public class AchivementsDefinitionTests
    {
        [TestMethod]
        public void AreIDsUnique()
        {
            ResourceManager.ReloadResources();
            var ids = ResourceManager.Achivements.GetResources();

            Assert.AreEqual(ids.Count, ids.Distinct().Count());
        }
    }
}
