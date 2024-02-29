using BugDefender.Core.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugDefender.Core.Tests.Users.Models.Buffs
{
    [TestClass]
    public class BuffDefinitionTests
    {
        public static IEnumerable<object[]> BuffIDs()
        {
            foreach(var id in ResourceManager.Buffs.GetResources())
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
    }
}
