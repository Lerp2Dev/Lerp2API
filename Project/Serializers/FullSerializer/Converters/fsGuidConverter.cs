using System;

namespace FullSerializer.Internal
{
    /// <summary>
    /// Serializes and deserializes guids.
    /// </summary>
    public class fsGuidConverter : fsConverter
    {
        /// <summary>
        /// Can this converter serialize and deserialize the given object type?
        /// </summary>
        /// <param name="type">The given object type.</param>
        /// <returns>True if the converter can serialize it, false otherwise.</returns>
        public override bool CanProcess(Type type)
        {
            return type == typeof(Guid);
        }

        /// <summary>
        /// If true, then the serializer will support cyclic references with the given converted
        /// type.
        /// </summary>
        /// <param name="storageType">The field/property type that is currently storing the object
        /// that is being serialized.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public override bool RequestCycleSupport(Type storageType)
        {
            return false;
        }

        /// <summary>
        /// If true, then the serializer will include inheritance data for the given converter.
        /// </summary>
        /// <param name="storageType">The field/property type that is currently storing the object
        /// that is being serialized.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public override bool RequestInheritanceSupport(Type storageType)
        {
            return false;
        }

        /// <summary>
        /// Serialize the actual object into the given data storage.
        /// </summary>
        /// <param name="instance">The object instance to serialize. This will never be null.</param>
        /// <param name="serialized">The serialized state.</param>
        /// <param name="storageType">The field/property type that is storing this instance.</param>
        /// <returns>If serialization was successful.</returns>
        public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
        {
            var guid = (Guid)instance;
            serialized = new fsData(guid.ToString());
            return fsResult.Success;
        }

        /// <summary>
        /// Deserialize data into the object instance.
        /// </summary>
        /// <param name="data">Serialization data to deserialize from.</param>
        /// <param name="instance">The object instance to deserialize into.</param>
        /// <param name="storageType">The field/property type that is storing the instance.</param>
        /// <returns>True if serialization was successful, false otherwise.</returns>
        public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
        {
            if (data.IsString)
            {
                instance = new Guid(data.AsString);
                return fsResult.Success;
            }

            return fsResult.Fail("fsGuidConverter encountered an unknown JSON data type");
        }

        /// <summary>
        /// Construct an object instance that will be passed to TryDeserialize. This should **not**
        /// deserialize the object.
        /// </summary>
        /// <param name="data">The data the object was serialized with.</param>
        /// <param name="storageType">The field/property type that is storing the instance.</param>
        /// <returns>An object instance</returns>
        public override object CreateInstance(fsData data, Type storageType)
        {
            return new Guid();
        }
    }
}