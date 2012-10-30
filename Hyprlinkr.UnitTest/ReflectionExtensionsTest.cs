using System;
using System.Linq;
using System.Reflection;
using Ploeh.Hyprlinkr.UnitTest.ReflectionHierarchy;
using Xunit;
using Xunit.Extensions;

namespace Ploeh.Hyprlinkr.UnitTest
{
    public class ReflectionExtensionsTest
    {
        [Theory]
        [AutoHypData]
        public void ComparingTwoMethodInfoObjects_ReturnsFalse_WhenBothAreDifferent(MethodInfo left, MethodInfo right)
        {
            Assert.True(left.RefersToTheSameMethodAs(right));
        }

        [Fact]
        public void ComparingTwoMethodInfoObjects_ReturnsFalse_WhenLeftIsAnOverloadOfRight()
        {
            var left = typeof(Base).GetMethod("Overloaded", new Type[0]);
            var right = typeof(Base).GetMethod("Overloaded", new[] { typeof(int) });
            Assert.False(left.RefersToTheSameMethodAs(right));
        }

        [Fact]
        public void ComparingTwoMethodInfoObjects_ReturnsFalse_WhenLeftIsFromABaseBaseClassOfRightAndTheMethodHasBeenOverridenOnlyInTheBaseClassOfRight()
        {
            var left = typeof(Base).GetMethod("Foo");
            var right = typeof(DerivedFromDerived).GetMethod("Foo");
            Assert.False(left.RefersToTheSameMethodAs(right));
        }

        [Fact]
        public void ComparingTwoMethodInfoObjects_ReturnsFalse_WhenLeftIsFromABaseClassOfRightAndTheMethodHasBeenOverridenInRight()
        {
            var left = typeof(Base).GetMethod("Foo");
            var right = typeof(Derived).GetMethod("Foo");
            Assert.False(left.RefersToTheSameMethodAs(right));
        }

        [Fact]
        public void ComparingTwoMethodInfoObjects_ReturnsFalse_WhenRightIsFromABaseBaseClassOfLeftAndTheMethodHasBeenOverridenOnlyInTheBaseClassOfLeft()
        {
            var left = typeof(DerivedFromDerived).GetMethod("Foo");
            var right = typeof(Base).GetMethod("Foo");
            Assert.False(left.RefersToTheSameMethodAs(right));
        }

        [Fact]
        public void ComparingTwoMethodInfoObjects_ReturnsFalse_WhenRightIsFromABaseClassOfLeftAndTheMethodHasBeenOverridenInLeft()
        {
            var left = typeof(Derived).GetMethod("Foo");
            var right = typeof(Base).GetMethod("Foo");
            Assert.False(left.RefersToTheSameMethodAs(right));
        }

        [Theory]
        [AutoHypData]
        public void ComparingTwoMethodInfoObjects_ReturnsTrue_WhenBothAreFromTheSameTypeAndForTheSameMethod(Type type)
        {
            var methods = type.GetMethods();
            var left = methods.First();
            var right = methods.First();
            Assert.True(left.RefersToTheSameMethodAs(right));
        }

        [Theory]
        [AutoHypData]
        public void ComparingTwoMethodInfoObjects_ReturnsTrue_WhenBothAreTheSameInstance(MethodInfo method)
        {
            Assert.True(method.RefersToTheSameMethodAs(method));
        }

        [Fact]
        public void ComparingTwoMethodInfoObjects_ReturnsTrue_WhenLeftIsFromABaseBaseClassOfRightAndTheMethodHasNotBeenOverridenInRightOrItsDirectBaseClass()
        {
            var left = typeof(Base).GetMethod("Bar");
            var right = typeof(DerivedFromDerived).GetMethod("Bar");
            Assert.True(left.RefersToTheSameMethodAs(right));
        }

        [Fact]
        public void ComparingTwoMethodInfoObjects_ReturnsTrue_WhenLeftIsFromABaseClassOfRightAndTheMethodHasNotBeenOverridenInRight()
        {
            var left = typeof(Base).GetMethod("Bar");
            var right = typeof(Derived).GetMethod("Bar");
            Assert.True(left.RefersToTheSameMethodAs(right));
        }

        [Fact]
        public void ComparingTwoMethodInfoObjects_ReturnsTrue_WhenLeftIsFromABaseClassOfRightAndTheMethodHasntBeenOverridenInRight()
        {
            var left = typeof(Base).GetMethod("Bar");
            var right = typeof(Derived).GetMethod("Bar");
            Assert.True(left.RefersToTheSameMethodAs(right));
        }

        [Fact]
        public void ComparingTwoMethodInfoObjects_ReturnsTrue_WhenRightIsFromABaseBaseClassOfLeftAndTheMethodHasNotBeenOverridenInLeftOrItsDirectBaseClass()
        {
            var left = typeof(DerivedFromDerived).GetMethod("Bar");
            var right = typeof(Base).GetMethod("Bar");
            Assert.True(left.RefersToTheSameMethodAs(right));
        }

        [Fact]
        public void ComparingTwoMethodInfoObjects_ReturnsTrue_WhenRightIsFromABaseClassOfLeftAndTheMethodHasNotBeenOverridenInLeft()
        {
            var left = typeof(Derived).GetMethod("Bar");
            var right = typeof(Base).GetMethod("Bar");
            Assert.True(left.RefersToTheSameMethodAs(right));
        }

        [Fact]
        public void ComparingTwoMethodInfoObjects_ReturnsTrue_WhenRightIsFromABaseClassOfLeftAndTheMethodHasntBeenOverridenInLeft()
        {
            var left = typeof(Derived).GetMethod("Bar");
            var right = typeof(Base).GetMethod("Bar");
            Assert.True(left.RefersToTheSameMethodAs(right));
        }

        [Theory]
        [AutoHypData]
        public void ComparingTwoMethodInfoObjects_ThrowsArgumentNullException_WhenLeftIsNull(MethodInfo right)
        {
            MethodInfo left = null;
            Assert.Throws<ArgumentNullException>(() => left.RefersToTheSameMethodAs(right));
        }

        [Theory]
        [AutoHypData]
        public void ComparingTwoMethodInfoObjects_ThrowsArgumentNullException_WhenRightIsNull(MethodInfo left)
        {
            MethodInfo right = null;
            Assert.Throws<ArgumentNullException>(() => left.RefersToTheSameMethodAs(right));
        }
    }
}
