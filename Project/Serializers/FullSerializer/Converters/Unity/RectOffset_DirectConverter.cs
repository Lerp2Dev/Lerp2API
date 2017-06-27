#if !NO_UNITY

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FullSerializer
{
    partial class fsConverterRegistrar
    {
        /// <summary>
        /// The register rect offset direct converter
        /// </summary>
        public static Internal.DirectConverters.RectOffset_DirectConverter Register_RectOffset_DirectConverter;
    }
}

namespace FullSerializer.Internal.DirectConverters
{
    /// <summary>
    /// Class RectOffset_DirectConverter.
    /// </summary>
    /// <seealso cref="FullSerializer.fsDirectConverter{UnityEngine.RectOffset}" />
    public class RectOffset_DirectConverter : fsDirectConverter<RectOffset>
    {
        /// <summary>
        /// Does the serialize.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="serialized">The serialized.</param>
        /// <returns>fsResult.</returns>
        protected override fsResult DoSerialize(RectOffset model, Dictionary<string, fsData> serialized)
        {
            var result = fsResult.Success;

            result += SerializeMember(serialized, null, "bottom", model.bottom);
            result += SerializeMember(serialized, null, "left", model.left);
            result += SerializeMember(serialized, null, "right", model.right);
            result += SerializeMember(serialized, null, "top", model.top);

            return result;
        }

        /// <summary>
        /// Does the deserialize.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="model">The model.</param>
        /// <returns>fsResult.</returns>
        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref RectOffset model)
        {
            var result = fsResult.Success;

            var t0 = model.bottom;
            result += DeserializeMember(data, null, "bottom", out t0);
            model.bottom = t0;

            var t2 = model.left;
            result += DeserializeMember(data, null, "left", out t2);
            model.left = t2;

            var t3 = model.right;
            result += DeserializeMember(data, null, "right", out t3);
            model.right = t3;

            var t4 = model.top;
            result += DeserializeMember(data, null, "top", out t4);
            model.top = t4;

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
            return new RectOffset();
        }
    }
}

#endif