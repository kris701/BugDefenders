using BugDefender.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
    }
}
