using BugDefender.Core.Helpers;
using BugDefender.Core.Models;
using System.Buffers;
using System.Text.Json;

namespace BugDefender.Core.Tests.Models
{
    [TestClass]
    public class EffectTargetTests
    {
        [TestMethod]
        public void Can_SetModifier()
        {
            // ARRANGE
            var target = new EffectTarget("prop");
            var modifier = 2;

            // ACT
            target.Modifier = modifier;

            // ASSERT
            Assert.AreEqual(modifier, target.Modifier);
            Assert.IsNull(target.Value);
        }

        [TestMethod]
        public void Can_SetValue()
        {
            // ARRANGE
            var target = new EffectTarget("prop");
            var value = new JsonElement();

            // ACT
            target.Value = value;

            // ASSERT
            Assert.AreEqual(value, target.Value);
            Assert.AreEqual(1, target.Modifier);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Cannot have a modifier and value at the same time!")]
        public void Cant_SetValueAndModifier_1()
        {
            // ARRANGE
            var target = new EffectTarget("prop");

            // ACT
            target.Value = new JsonElement();
            target.Modifier = 2;
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Cannot have a modifier and value at the same time!")]
        public void Cant_SetValueAndModifier_2()
        {
            // ARRANGE
            var target = new EffectTarget("prop");

            // ACT
            target.Modifier = 2;
            target.Value = new JsonElement();
        }

        [TestMethod]
        public void Can_Copy_1()
        {
            // ARRANGE
            var target = new EffectTarget("prop");
            target.Value = new JsonElement();

            // ACT
            var copy = target.Copy();

            // ASSERT
            Assert.IsTrue(target.Equals(copy));
        }

        [TestMethod]
        public void Can_Copy_2()
        {
            // ARRANGE
            var target = new EffectTarget("prop");
            target.Modifier = 2;

            // ACT
            var copy = target.Copy();

            // ASSERT
            Assert.IsTrue(target.Equals(copy));
        }

        #region Apply and Unapply

        private class TestClass
        {
            public int Value1 { get; set; } = 10;
            public float Value2 { get; set; } = 5f;
            public string Value3 { get; set; } = "abc";
        }

        private static JsonElement GetElement(string text)
        {
            byte[] arr = new byte[text.Length];
            for (int i = 0; i < arr.Length; i++)
                arr[i] = ((byte)text[i]);
            var reader = new Utf8JsonReader(new ReadOnlySequence<byte>(arr));
            var element = JsonElement.ParseValue(ref reader);
            return element;
        }

        [TestMethod]
        [DataRow("Value1", 1.5f, null, 15)]
        [DataRow("Value1", 0.5f, null, 5)]
        [DataRow("Value2", 0.5f, null, 5f * 0.5f)]
        [DataRow("Value3", 1f, "\"123\"", "123")]
        [DataRow("Value1", 1f, "200", 200)]
        [DataRow("Value2", 1f, "5.73", 5.73f)]
        public void Can_ApplyOnObject(string target, float modifier, string? value, dynamic expected)
        {
            // ARRANGE
            var effect = new EffectTarget(target) 
            {
                Modifier = modifier
            };
            if (value != null)
                effect.Value = GetElement(value);
            var item = new TestClass();

            // ACT
            effect.ApplyOnObject(item);

            // ASSERT
            var prop = ReflectionHelper.GetPropertyInstance(item, target);
            Assert.IsNotNull(prop);
            var postValue = prop.Item2.GetValue(prop.Item1);
            Assert.IsNotNull(postValue);
            Assert.AreEqual(expected, postValue);
        }

        [TestMethod]
        [DataRow("Value645")]
        [DataRow("asdafs")]
        public void Can_TryApplyOnObject(string target)
        {
            // ARRANGE
            var effect = new EffectTarget(target)
            {
                Modifier = 5
            };
            var item = new TestClass();

            // ACT
            effect.TryApplyOnObject(item);

            // ASSERT
            var prop = ReflectionHelper.GetPropertyInstance(item, target);
            Assert.IsNull(prop);
        }

        [TestMethod]
        [DataRow("Value1", 1.5f, 15)]
        [DataRow("Value1", 0.5f, 5)]
        [DataRow("Value2", 0.5f, 5f * 0.5f)]
        public void Can_UnApplyOnObject(string target, float modifier, dynamic expected)
        {
            // ARRANGE
            var effect = new EffectTarget(target)
            {
                Modifier = modifier
            };
            var item = new TestClass();
            var prop = ReflectionHelper.GetPropertyInstance(item, target);
            Assert.IsNotNull(prop);
            var preValue = prop.Item2.GetValue(prop.Item1);
            effect.ApplyOnObject(item);
            var postValue = prop.Item2.GetValue(prop.Item1);
            Assert.IsNotNull(postValue);
            Assert.AreEqual(expected, postValue);

            // ACT
            effect.UnApplyOnObject(item);

            // ASSERT
            var postValue2 = prop.Item2.GetValue(prop.Item1);
            Assert.AreEqual(preValue, postValue2);
        }

        [TestMethod]
        [DataRow("Value645")]
        [DataRow("asdafs")]
        public void Can_TryUnApplyOnObject(string target)
        {
            // ARRANGE
            var effect = new EffectTarget(target)
            {
                Modifier = 5
            };
            var item = new TestClass();

            // ACT
            effect.TryUnApplyOnObject(item);

            // ASSERT
            var prop = ReflectionHelper.GetPropertyInstance(item, target);
            Assert.IsNull(prop);
        }

        #endregion
    }
}
