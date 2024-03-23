using BugDefender.Core.Helpers;

namespace BugDefender.Core.Tests.Helpers
{
    [TestClass]
    public class ReflectionHelperTests
    {
        private class Simple1
        {
            public int Value1 { get; set; } = 10;
            public float Value2 { get; set; } = 5.5f;
            public string Value3 { get; set; } = "abc";

            public Simple1()
            {
            }

            public Simple1(int value1, float value2, string value3)
            {
                Value1 = value1;
                Value2 = value2;
                Value3 = value3;
            }
        }

        private class Simple2
        {
            public Simple1 Subitem { get; set; } = new Simple1();
            public double Value1 { get; set; } = 5f;

            public Simple2()
            {
            }

            public Simple2(Simple1 subitem, double value1)
            {
                Subitem = subitem;
                Value1 = value1;
            }
        }

        private class Simple3
        {
            public List<Simple1> Items { get; set; } = new List<Simple1>()
            {
                new Simple1(5, 10f, "abc"),
                new Simple1(10, 20f, "cba")
            };
            public int Value1 { get; set; } = 5;
        }

        private class Simple4
        {
            public List<Simple3> Items { get; set; } = new List<Simple3>()
            {
                new Simple3()
                {
                    Value1 = 10,
                    Items = new List<Simple1>()
                    {
                        new Simple1(50, 1f, "aaa")
                    }
                },
                new Simple3()
            };
        }
        [TestMethod]
        [DataRow("Value1", typeof(int))]
        [DataRow("Value2", typeof(float))]
        [DataRow("Value3", typeof(string))]
        public void Can_GetSimpleProperty(string target, Type expected)
        {
            // ARRANGE
            var item = new Simple1();

            // ACT
            var result = ReflectionHelper.GetPropertyInstance(item, target);

            // ASSERT
            Assert.IsNotNull(result);
            var value = result.Item2.GetValue(result.Item1);
            Assert.IsInstanceOfType(value, expected);
        }

        [TestMethod]
        [DataRow("Subitem.Value1", typeof(int))]
        [DataRow("Subitem.Value2", typeof(float))]
        [DataRow("Subitem.Value3", typeof(string))]
        [DataRow("Value1", typeof(double))]
        public void Can_GetLayeredProperty(string target, Type expected)
        {
            // ARRANGE
            var item = new Simple2();

            // ACT
            var result = ReflectionHelper.GetPropertyInstance(item, target);

            // ASSERT
            Assert.IsNotNull(result);
            var value = result.Item2.GetValue(result.Item1);
            Assert.IsInstanceOfType(value, expected);
        }

        [TestMethod]
        [DataRow("Items[Value1=5].Value2", typeof(float), 10f)]
        [DataRow("Items[Value1=10].Value1", typeof(int), 10)]
        [DataRow("Items[Value1=5].Value3", typeof(string), "abc")]
        [DataRow("Items[Value1=10].Value3", typeof(string), "cba")]
        public void Can_GetListedProperty_1(string target, Type expected, dynamic expectedValue)
        {
            // ARRANGE
            var item = new Simple3();

            // ACT
            var result = ReflectionHelper.GetPropertyInstance(item, target);

            // ASSERT
            Assert.IsNotNull(result);
            var value = result.Item2.GetValue(result.Item1);
            Assert.IsInstanceOfType(value, expected);
            Assert.AreEqual(expectedValue, value);
        }

        [TestMethod]
        [DataRow("Items[Value1=10].Items[Value1=50].Value3", typeof(string), "aaa")]
        [DataRow("Items[Value1=5].Items[Value1=10].Value3", typeof(string), "cba")]
        public void Can_GetListedProperty_2(string target, Type expected, dynamic expectedValue)
        {
            // ARRANGE
            var item = new Simple4();

            // ACT
            var result = ReflectionHelper.GetPropertyInstance(item, target);

            // ASSERT
            Assert.IsNotNull(result);
            var value = result.Item2.GetValue(result.Item1);
            Assert.IsInstanceOfType(value, expected);
            Assert.AreEqual(expectedValue, value);
        }

        [TestMethod]
        [DataRow("Value69")]
        [DataRow("NoValue")]
        public void Cant_GetPropertyIfNotThere(string target)
        {
            // ARRANGE
            var item = new Simple1();

            // ACT
            var result = ReflectionHelper.GetPropertyInstance(item, target);

            // ASSERT
            Assert.IsNull(result);
        }
    }
}
