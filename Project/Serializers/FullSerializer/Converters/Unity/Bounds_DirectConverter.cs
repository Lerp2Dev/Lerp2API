#if !NO_UNITY

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FullSerializer
{
    partial class fsConverterRegistrar
    {
        /// <summary>
        /// The register bounds direct converter
        /// </summary>
        public static Internal.DirectConverters.Bounds_DirectConverter Register_Bounds_DirectConverter;
    }
}

namespace FullSerializer.Internal.DirectConverters
{
    /// <summary>
    /// Class Bounds_DirectConverter.
    /// </summary>
    /// <seealso cref="FullSerializer.fsDirectConverter{UnityEngine.Bounds}" />
    public class Bounds_DirectConverter : fsDirectConverter<Bounds>
    {
        /// <summary>
        /// Does the serialize.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="serialized">The serialized.</param>
        /// <returns>fsResult.</returns>
        protected override fsResult DoSerialize(Bounds model, Dictionary<string, fsData> serialized)
        {
            var result = fsResult.Success;

            result += SerializeMember(serialized, null, "center", model.center);
            result += SerializeMember(serialized, null, "size", model.size);

            return result;
        }

        /// <summary>
        /// Does the deserialize.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="model">The model.</param>
        /// <returns>fsResult.</returns>
        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref Bounds model)
        {
            var result = fsResult.Success;

            var t0 = model.center;
            result += DeserializeMember(data, null, "center", out t0);
            model.center = t0;

            var t1 = model.size;
            result += DeserializeMember(data, null, "size", out t1);
            model.size = t1;

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
            return new Bounds();
        }
    }
}

#endif