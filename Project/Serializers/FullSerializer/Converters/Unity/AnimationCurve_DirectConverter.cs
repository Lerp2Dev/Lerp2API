#if !NO_UNITY

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FullSerializer
{
    partial class fsConverterRegistrar
    {
        /// <summary>
        /// The register animation curve direct converter
        /// </summary>
        public static Internal.DirectConverters.AnimationCurve_DirectConverter Register_AnimationCurve_DirectConverter;
    }
}

namespace FullSerializer.Internal.DirectConverters
{
    /// <summary>
    /// Class AnimationCurve_DirectConverter.
    /// </summary>
    /// <seealso cref="FullSerializer.fsDirectConverter{UnityEngine.AnimationCurve}" />
    public class AnimationCurve_DirectConverter : fsDirectConverter<AnimationCurve>
    {
        /// <summary>
        /// Does the serialize.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="serialized">The serialized.</param>
        /// <returns>fsResult.</returns>
        protected override fsResult DoSerialize(AnimationCurve model, Dictionary<string, fsData> serialized)
        {
            var result = fsResult.Success;

            result += SerializeMember(serialized, null, "keys", model.keys);
            result += SerializeMember(serialized, null, "preWrapMode", model.preWrapMode);
            result += SerializeMember(serialized, null, "postWrapMode", model.postWrapMode);

            return result;
        }

        /// <summary>
        /// Does the deserialize.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="model">The model.</param>
        /// <returns>fsResult.</returns>
        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref AnimationCurve model)
        {
            var result = fsResult.Success;

            var t0 = model.keys;
            result += DeserializeMember(data, null, "keys", out t0);
            model.keys = t0;

            var t1 = model.preWrapMode;
            result += DeserializeMember(data, null, "preWrapMode", out t1);
            model.preWrapMode = t1;

            var t2 = model.postWrapMode;
            result += DeserializeMember(data, null, "postWrapMode", out t2);
            model.postWrapMode = t2;

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
            return new AnimationCurve();
        }
    }
}

#endif