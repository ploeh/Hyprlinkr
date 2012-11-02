namespace Ploeh.Hyprlinkr
{
    /// <summary> Helper class to make it more visual that the parameters in the expression passed to <seealso cref="IActionVerifier.Verify{TController}"/> are
    /// irrelevant. </summary>
    /// <typeparam name="T">The type to return the default value for.</typeparam>
    public static class Arg<T>
    {
        /// <summary>Gets the default value of the specified type.</summary>
        /// <returns>default(T)</returns>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", 
            Justification = "All we do care about here is the type itself. There is no more concise way to write this.")]
        public static T Any
        {
            get { return default(T); }
        }
    }
}
