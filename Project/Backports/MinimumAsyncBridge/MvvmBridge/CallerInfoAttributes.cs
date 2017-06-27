namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Class CallerFilePathAttribute. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    public sealed class CallerFilePathAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CallerFilePathAttribute"/> class.
        /// </summary>
        public CallerFilePathAttribute() { }
    }

    /// <summary>
    /// Class CallerLineNumberAttribute. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    public sealed class CallerLineNumberAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CallerLineNumberAttribute"/> class.
        /// </summary>
        public CallerLineNumberAttribute() { }
    }

    /// <summary>
    /// Class CallerMemberNameAttribute. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    public sealed class CallerMemberNameAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CallerMemberNameAttribute"/> class.
        /// </summary>
        public CallerMemberNameAttribute() { }
    }
}