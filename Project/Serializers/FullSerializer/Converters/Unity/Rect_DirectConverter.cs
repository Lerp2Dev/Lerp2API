#if !NO_UNITY

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FullSerializer
{
    partial class fsConverterRegistrar
    {
        /// <summary>
        /// The register rect direct converter
        /// </summary>
        public static Internal.DirectConverters.Rect_DirectConverter Register_Rect_DirectConverter;
    }
}

namespace FullSerializer.Internal.DirectConverters
{
    /// <summary>
    /// Class Rect_DirectConverter.
    /// </summary>
    /// <seealso cref="FullSerializer.fsDirectConverter{UnityEngine.Rect}" />
    public class Rect_DirectConverter : fsDirectConverter<Rect>
    {
        /// <summary>
        /// Does the serialize.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="serialized">The serialized.</param>
        /// <returns>fsResult.</returns>
        protected override fsResult DoSerialize(Rect model, Dictionary<string, fsData> serialized)
        {
            var result = fsResult.Success;

            result += SerializeMember(serialized, null, "xMin", model.xMin);
            result += SerializeMember(serialized, null, "yMin", model.yMin);
            result += SerializeMember(serialized, null, "xMax", model.xMax);
            result += SerializeMember(serialized, null, "yMax", model.yMax);

            return result;
        }

        /// <summary>
        /// Does the deserialize.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="model">The model.</param>
        /// <returns>fsResult.</returns>
        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref Rect model)
        {
            var result = fsResult.Success;

            var t0 = model.xMin;
            result += DeserializeMember(data, null, "xMin", out t0);
            model.xMin = t0;

            var t1 = model.yMin;
            result += DeserializeMember(data, null, "yMin", out t1);
            model.yMin = t1;

            var t2 = model.xMax;
            result += DeserializeMember(data, null, "xMax", out t2);
            model.xMax = t2;

            var t3 = model.yMax;
            result += DeserializeMember(data, null, "yMax", out t3);
            model.yMax = t3;

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
            return new Rect();
        }
    }
}

#endif