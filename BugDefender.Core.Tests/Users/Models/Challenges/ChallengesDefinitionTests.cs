using BugDefender.Core.Resources;

namespace BugDefender.Core.Tests.Users.Models.Challenges
{
    [TestClass]
    public class ChallengesDefinitionTests
    {
        [TestMethod]
        public void AreIDsUnique()
        {
            ResourceManager.ReloadResources();
            var ids = ResourceManager.Challenges.GetResources();

            Assert.AreEqual(ids.Count, ids.Distinct().Count());
        }
    }
}
