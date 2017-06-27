// note: This file contains exceptions used by FullSerializer. Exceptions are never used at runtime
//       in FullSerializer; they are only used when validating annotations and code-based models.

using System;

namespace FullSerializer
{
    /// <summary>
    /// Class fsMissingVersionConstructorException. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public sealed class fsMissingVersionConstructorException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="fsMissingVersionConstructorException"/> class.
        /// </summary>
        /// <param name="versionedType">Type of the versioned.</param>
        /// <param name="constructorType">Type of the constructor.</param>
        public fsMissingVersionConstructorException(Type versionedType, Type constructorType) :
            base(versionedType + " is missing a constructor for previous model type " + constructorType)
        { }
    }

    /// <summary>
    /// Class fsDuplicateVersionNameException. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public sealed class fsDuplicateVersionNameException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="fsDuplicateVersionNameException"/> class.
        /// </summary>
        /// <param name="typeA">The type a.</param>
        /// <param name="typeB">The type b.</param>
        /// <param name="version">The version.</param>
        public fsDuplicateVersionNameException(Type typeA, Type typeB, string version) :
            base(typeA + " and " + typeB + " have the same version string (" + version + "); please change one of them.")
        { }
    }
}