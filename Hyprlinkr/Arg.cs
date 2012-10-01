namespace Ploeh.Hyprlinkr
{
    /// <summary> Helper class to make it more visual that the parameters in the expression passed to <seealso cref="IActionVerifier.Verify{TController}"/> are
    /// irrelevant. </summary>
    public static class Arg
    {
        /// <summary> Returns a default value of the specified type. </summary>
        /// <typeparam name="T"> The type to return the default value for. </typeparam>
        /// <returns> default(T) </returns>
        public static T OfType<T>()
        {
            return default(T);
        }
    }
}
