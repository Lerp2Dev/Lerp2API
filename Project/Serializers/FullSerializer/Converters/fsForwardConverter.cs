using System;

namespace FullSerializer
{
    /// <summary>
    /// This allows you to forward serialization of an object to one of its members. For example,
    ///
    /// [fsForward("Values")]
    /// struct Wrapper {
    ///   public int[] Values;
    /// }
    ///
    /// Then `Wrapper` will be serialized into a JSON array of integers. It will be as if `Wrapper`
    /// doesn't exist.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public sealed class fsForwardAttribute : Attribute
    {
        /// <summary>
        /// The name of the member we should serialize as.
        /// </summary>
        public string MemberName;

        /// <summary>
        /// Forward object serialization to an instance member. See class comment.
        /// </summary>
        /// <param name="memberName">The name of the member that we should serialize this object as.</param>
        public fsForwardAttribute(string memberName)
        {
            MemberName = memberName;
        }
    }
}

namespace FullSerializer.Internal
{
    /// <summary>
    /// Class fsForwardConverter.
    /// </summary>
    /// <seealso cref="FullSerializer.fsConverter" />
    public class fsForwardConverter : fsConverter
    {
        private string _memberName;

        /// <summary>
        /// Initializes a new instance of the <see cref="fsForwardConverter"/> class.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        public fsForwardConverter(fsForwardAttribute attribute)
        {
            _memberName = attribute.MemberName;
        }

        /// <summary>
        /// Can this converter serialize and deserialize the given object type?
        /// </summary>
        /// <param name="type">The given object type.</param>
        /// <returns>True if the converter can serialize it, false otherwise.</returns>
        /// <exception cref="NotSupportedException">Please use the [fsForward(...)] attribute.</exception>
        public override bool CanProcess(Type type)
        {
            throw new NotSupportedException("Please use the [fsForward(...)] attribute.");
        }

        private fsResult GetProperty(object instance, out fsMetaProperty property)
        {
            var properties = fsMetaType.Get(Serializer.Config, instance.GetType()).Properties;
            for (int i = 0; i < properties.Length; ++i)
            {
                if (properties[i].MemberName == _memberName)
                {
                    property = properties[i];
                    return fsResult.Success;
                }
            }

            property = default(fsMetaProperty);
            return fsResult.Fail("No property named \"" + _memberName + "\" on " + instance.GetType().CSharpName());
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
            serialized = fsData.Null;
            var result = fsResult.Success;

            fsMetaProperty property;
            if ((result += GetProperty(instance, out property)).Failed) return result;

            var actualInstance = property.Read(instance);
            return Serializer.TrySerialize(property.StorageType, actualInstance, out serialized);
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
            var result = fsResult.Success;

            fsMetaProperty property;
            if ((result += GetProperty(instance, out property)).Failed) return result;

            object actualInstance = null;
            if ((result += Serializer.TryDeserialize(data, property.StorageType, ref actualInstance)).Failed)
                return result;

            property.Write(instance, actualInstance);
            return result;
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
            return fsMetaType.Get(Serializer.Config, storageType).CreateInstance();
        }
    }
}