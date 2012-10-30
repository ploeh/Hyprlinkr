using System;
using System.Reflection;

namespace Ploeh.Hyprlinkr
{
    /// <summary>A class with some extension methods with regards to reflection.</summary>
    public static class ReflectionExtensions
    {
        /// <summary>Checks whether <paramref name="left"/> refers to the same method as <paramref name="right"/>.</summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns><c>true</c> if <paramref name="left"/> refers to the same method as <paramref name="right"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        ///     <para>
        /// Two <see cref="MethodInfo"/> instances can be un-equal although they refer to the same physical method. This happens when <c>Base</c> has a virtual method
        ///         <c>Foo</c> and <c>Derived</c>derives from <c>Base</c> but doesn't override <c>Foo</c>.
        ///     </para>
        ///     <para>
        /// Example:
        ///         <code><![CDATA[
        /// var baseMethod = typeof(Base).GetMethod("Foo");
        /// var derivedMethod = typeof(Derived).GetMethod("Foo");
        /// Assert.False(baseMethod.Equals(derivedMethod));]]></code>
        ///     </para>
        ///     <para>
        /// The difference comes from the fact that for both <c>baseMethod</c> and <c>derivedMethod</c> the property <see cref="MemberInfo.DeclaringType"/> is <c>Base</c>,
        /// but the <see cref="MemberInfo.ReflectedType"/> is the class specified in the <see langword="typeof"/>.
        ///     </para>
        ///     <para>Especially when mixing reflection and expressions, this becomes a problem.</para>
        ///     <para>The following assert will succeed in both cases:</para>
        ///     <para>
        ///         <code><![CDATA[
        /// void Test<T>(Expression<Action<T>> expression)
        /// {
        ///     Assert.Equal(typeof(Base), ((MethodCallExpression)expression.Body).Method.ReflectedType);
        /// }
        /// 
        /// Test<Base>(x => x.Foo());
        /// Test<Derived>(x => x.Foo());
        /// ]]></code>
        ///     </para>
        ///     <para>
        /// When trying to compare the <see cref="MethodInfo"/> instance from the expression with that created by reflection, the result will be <c>false</c> as soon as
        /// the reflection instance has been created from a derived type
        ///     </para>
        /// </remarks>
        public static bool RefersToTheSameMethodAs(this MethodInfo left, MethodInfo right)
        {
            if (left == null)
                throw new ArgumentNullException("left");
            if (right == null)
                throw new ArgumentNullException("right");

            return left.MethodHandle.Equals(right.MethodHandle);
        }
    }
}
