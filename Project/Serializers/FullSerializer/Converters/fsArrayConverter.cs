using System;
using System.Collections;

namespace FullSerializer.Internal
{
    /// <summary>
    /// Class fsArrayConverter.
    /// </summary>
    /// <seealso cref="FullSerializer.fsConverter" />
    public class fsArrayConverter : fsConverter
    {
        /// <summary>
        /// Can this converter serialize and deserialize the given object type?
        /// </summary>
        /// <param name="type">The given object type.</param>
        /// <returns>True if the converter can serialize it, false otherwise.</returns>
        public override bool CanProcess(Type type)
        {
            return type.IsArray;
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
            // note: IList[index] is **significantly** faster than Array.Get, so make sure we use
            //       that instead.

            IList arr = (Array)instance;
            Type elementType = storageType.GetElementType();

            var result = fsResult.Success;

            serialized = fsData.CreateList(arr.Count);
            var serializedList = serialized.AsList;

            for (int i = 0; i < arr.Count; ++i)
            {
                object item = arr[i];

                fsData serializedItem;

                var itemResult = Serializer.TrySerialize(elementType, item, out serializedItem);
                result.AddMessages(itemResult);
                if (itemResult.Failed) continue;

                serializedList.Add(serializedItem);
            }

            return result;
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

            // Verify that we actually have an List
            if ((result += CheckType(data, fsDataType.Array)).Failed)
            {
                return result;
            }

            Type elementType = storageType.GetElementType();

            var serializedList = data.AsList;
            var list = new ArrayList(serializedList.Count);
            int existingCount = list.Count;

            for (int i = 0; i < serializedList.Count; ++i)
            {
                var serializedItem = serializedList[i];
                object deserialized = null;
                if (i < existingCount) deserialized = list[i];

                var itemResult = Serializer.TryDeserialize(serializedItem, elementType, ref deserialized);
                result.AddMessages(itemResult);
                if (itemResult.Failed) continue;

                if (i < existingCount) list[i] = deserialized;
                else list.Add(deserialized);
            }

            instance = list.ToArray(elementType);
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