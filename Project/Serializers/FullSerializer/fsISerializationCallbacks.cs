using System;

#if !NO_UNITY

using UnityEngine;

#endif

#if !UNITY_EDITOR && UNITY_WSA
// For System.Reflection.TypeExtensions
using System.Reflection;
#endif

namespace FullSerializer
{
    /// <summary>
    /// Extend this interface on your type to receive notifications about serialization/deserialization events. If you don't
    /// have access to the type itself, then you can write an fsObjectProcessor instead.
    /// </summary>
    public interface fsISerializationCallbacks
    {
        /// <summary>
        /// Called before serialization.
        /// </summary>
        void OnBeforeSerialize(Type storageType);

        /// <summary>
        /// Called after serialization.
        /// </summary>
        /// <param name="storageType">The field/property type that is storing the instance.</param>
        /// <param name="data">The data that was serialized.</param>
        void OnAfterSerialize(Type storageType, ref fsData data);

        /// <summary>
        /// Called before deserialization.
        /// </summary>
        /// <param name="storageType">The field/property type that is storing the instance.</param>
        /// <param name="data">The data that will be used for deserialization.</param>
        void OnBeforeDeserialize(Type storageType, ref fsData data);

        /// <summary>
        /// Called after deserialization.
        /// </summary>
        /// <param name="storageType">The field/property type that is storing the instance.</param>
        /// <param name="instance">The type of the instance.</param>
        void OnAfterDeserialize(Type storageType);
    }
}

namespace FullSerializer.Internal
{
    /// <summary>
    /// Class fsSerializationCallbackProcessor.
    /// </summary>
    /// <seealso cref="FullSerializer.fsObjectProcessor" />
    public class fsSerializationCallbackProcessor : fsObjectProcessor
    {
        /// <summary>
        /// Is the processor interested in objects of the given type?
        /// </summary>
        /// <param name="type">The given type.</param>
        /// <returns>True if the processor should be applied, false otherwise.</returns>
        public override bool CanProcess(Type type)
        {
            return typeof(fsISerializationCallbacks).IsAssignableFrom(type);
        }

        /// <summary>
        /// Called before serialization.
        /// </summary>
        /// <param name="storageType">The field/property type that is storing the instance.</param>
        /// <param name="instance">The type of the instance.</param>
        public override void OnBeforeSerialize(Type storageType, object instance)
        {
            // Don't call the callback on null instances.
            if (instance == null) return;
            ((fsISerializationCallbacks)instance).OnBeforeSerialize(storageType);
        }

        /// <summary>
        /// Called after serialization.
        /// </summary>
        /// <param name="storageType">The field/property type that is storing the instance.</param>
        /// <param name="instance">The type of the instance.</param>
        /// <param name="data">The data that was serialized.</param>
        public override void OnAfterSerialize(Type storageType, object instance, ref fsData data)
        {
            // Don't call the callback on null instances.
            if (instance == null) return;
            ((fsISerializationCallbacks)instance).OnAfterSerialize(storageType, ref data);
        }

        /// <summary>
        /// Called before deserialization has begun but *after* the object instance has been created. This will get
        /// invoked even if the user passed in an existing instance.
        /// </summary>
        /// <param name="storageType">The field/property type that is storing the instance.</param>
        /// <param name="instance">The created object instance. No deserialization has been applied to it.</param>
        /// <param name="data">The data that will be used for deserialization.</param>
        /// <exception cref="InvalidCastException">Please ensure the converter for " + storageType + " actually returns an instance of it, not an instance of " + instance.GetType()</exception>
        /// <remarks>**IMPORTANT**: The actual instance that gets passed here is *not* guaranteed to be an a subtype of storageType, since
        /// the value for instance is whatever the active converter returned for CreateInstance() - ie, some converters will return
        /// dummy types in CreateInstance() if instance creation cannot be separated from deserialization (ie, KeyValuePair).</remarks>
        public override void OnBeforeDeserializeAfterInstanceCreation(Type storageType, object instance, ref fsData data)
        {
            if (instance is fsISerializationCallbacks == false)
            {
                throw new InvalidCastException("Please ensure the converter for " + storageType + " actually returns an instance of it, not an instance of " + instance.GetType());
            }

            ((fsISerializationCallbacks)instance).OnBeforeDeserialize(storageType, ref data);
        }

        /// <summary>
        /// Called after deserialization.
        /// </summary>
        /// <param name="storageType">The field/property type that is storing the instance.</param>
        /// <param name="instance">The type of the instance.</param>
        public override void OnAfterDeserialize(Type storageType, object instance)
        {
            // Don't call the callback on null instances.
            if (instance == null) return;
            ((fsISerializationCallbacks)instance).OnAfterDeserialize(storageType);
        }
    }

#if !NO_UNITY

    /// <summary>
    /// Class fsSerializationCallbackReceiverProcessor.
    /// </summary>
    /// <seealso cref="FullSerializer.fsObjectProcessor" />
    public class fsSerializationCallbackReceiverProcessor : fsObjectProcessor
    {
        /// <summary>
        /// Is the processor interested in objects of the given type?
        /// </summary>
        /// <param name="type">The given type.</param>
        /// <returns>True if the processor should be applied, false otherwise.</returns>
        public override bool CanProcess(Type type)
        {
            return typeof(ISerializationCallbackReceiver).IsAssignableFrom(type);
        }

        /// <summary>
        /// Called before serialization.
        /// </summary>
        /// <param name="storageType">The field/property type that is storing the instance.</param>
        /// <param name="instance">The type of the instance.</param>
        public override void OnBeforeSerialize(Type storageType, object instance)
        {
            // Don't call the callback on null instances.
            if (instance == null) return;
            ((ISerializationCallbackReceiver)instance).OnBeforeSerialize();
        }

        /// <summary>
        /// Called after deserialization.
        /// </summary>
        /// <param name="storageType">The field/property type that is storing the instance.</param>
        /// <param name="instance">The type of the instance.</param>
        public override void OnAfterDeserialize(Type storageType, object instance)
        {
            // Don't call the callback on null instances.
            if (instance == null) return;
            ((ISerializationCallbackReceiver)instance).OnAfterDeserialize();
        }
    }

#endif
}