using BugDefender.Core.Resources;

namespace BugDefender.Core.Tests.Users.Models.Buffs
{
    [TestClass]
    public class BuffDefinitionTests
    {
        public static IEnumerable<object[]> BuffIDs()
        {
            foreach (var id in ResourceManager.Buffs.GetResources())
                yield return new object[] { id };
        }

        [TestMethod]
        [DynamicData(nameof(BuffIDs), DynamicDataSourceType.Method)]
        public void Can_ApplyBuff(Guid buffID)
        {
            // ARRANGE
            ResourceManager.ReloadResources();
            var buff = ResourceManager.Buffs.GetResource(buffID);

            // ACT
            buff.Apply();

            // ASSERT

        }

        [TestMethod]
        public void AreIDsUnique()
        {
            ResourceManager.ReloadResources();
            var ids = ResourceManager.Buffs.GetResources();

            Assert.AreEqual(ids.Count, ids.Distinct().Count());
        }
    }
}
