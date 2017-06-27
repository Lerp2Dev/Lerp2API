namespace Lerp2API.Hepers.Unity_Extensions
{
    /// <summary>
    /// Class GenericsHelpers.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GenericsHelpers<T>
    {
        /// <summary>
        /// Arrays the parameters.
        /// </summary>
        /// <param name="pars">The pars.</param>
        /// <returns>T[].</returns>
        public static T[] ArrayParams(params T[] pars)
        {
            return pars;
        }
    }
}