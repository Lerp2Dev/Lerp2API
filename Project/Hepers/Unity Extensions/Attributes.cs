using System;

namespace Lerp2API.Hepers.Unity_Extensions
{
    /// <summary>
    /// Class BadOptimized.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class BadOptimized : Attribute
    {
    }

    /// <summary>
    /// Class RequiredAttrs.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class RequiredAttrs : Attribute
    {
        /// <summary>
        /// The attributes
        /// </summary>
        public string[] Attributes;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredAttrs"/> class.
        /// </summary>
        /// <param name="attrs">The attrs.</param>
        public RequiredAttrs(params string[] attrs)
        {
            Attributes = attrs;
        }
    }
}