using System;

namespace FullSerializer.Internal
{
    /// <summary>
    /// Struct fsVersionedType
    /// </summary>
    public struct fsVersionedType
    {
        /// <summary>
        /// The direct ancestors that this type can import.
        /// </summary>
        public fsVersionedType[] Ancestors;

        /// <summary>
        /// The identifying string that is unique among all ancestors.
        /// </summary>
        public string VersionString;

        /// <summary>
        /// The modeling type that this versioned type maps back to.
        /// </summary>
        public Type ModelType;

        /// <summary>
        /// Migrate from an instance of an ancestor.
        /// </summary>
        public object Migrate(object ancestorInstance)
        {
            return Activator.CreateInstance(ModelType, ancestorInstance);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return "fsVersionedType [ModelType=" + ModelType + ", VersionString=" + VersionString + ", Ancestors.Length=" + Ancestors.Length + "]";
        }

        /// <summary>
        /// Implements the ==.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(fsVersionedType a, fsVersionedType b)
        {
            return a.ModelType == b.ModelType;
        }

        /// <summary>
        /// Implements the !=.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(fsVersionedType a, fsVersionedType b)
        {
            return a.ModelType != b.ModelType;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            return
                obj is fsVersionedType &&
                ModelType == ((fsVersionedType)obj).ModelType;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return ModelType.GetHashCode();
        }
    }
}