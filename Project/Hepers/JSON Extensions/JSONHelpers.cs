using FullSerializer;
using Lerp2API._Debug;
using System.IO;

namespace Lerp2API.Hepers.JSON_Extensions
{
    /// <summary>
    /// Class JSONHelpers.
    /// </summary>
    public static class JSONHelpers
    {
        private static readonly fsSerializer _serializer = new fsSerializer();

        /// <summary>
        /// Serializes the specified pretty.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="pretty">if set to <c>true</c> [pretty].</param>
        /// <returns>System.String.</returns>
        public static string Serialize<T>(this T value, bool pretty = true)
        {
            fsData data;
            fsResult res = _serializer.TrySerialize(typeof(T), value, out data).AssertSuccessWithoutWarnings();

            if (res.Failed)
                Debug.LogError(res.FormattedMessages);

            if (pretty)
                return fsJsonPrinter.PrettyJson(data);
            return fsJsonPrinter.CompressedJson(data);
        }

        /// <summary>
        /// Deserializes the specified serialized state.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializedState">State of the serialized.</param>
        /// <returns>T.</returns>
        public static T Deserialize<T>(this string serializedState)
        {
            fsData data = fsJsonParser.Parse(serializedState);

            object deserialized = null;
            _serializer.TryDeserialize(data, typeof(T), ref deserialized).AssertSuccessWithoutWarnings();

            return (T)deserialized;
        }

        /// <summary>
        /// Serializes to file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">The path.</param>
        /// <param name="value">The value.</param>
        /// <param name="pretty">if set to <c>true</c> [pretty].</param>
        public static void SerializeToFile<T>(string path, T value, bool pretty = true)
        {
            File.WriteAllText(path, Serialize(value, pretty));
        }

        /// <summary>
        /// Deserializes from file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">The path.</param>
        /// <returns>T.</returns>
        public static T DeserializeFromFile<T>(string path)
        {
            return Deserialize<T>(File.ReadAllText(path));
        }

        /// <summary>
        /// Determines whether the specified input is json.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns><c>true</c> if the specified input is json; otherwise, <c>false</c>.</returns>
        public static bool IsJson(this string input)
        {
            input = input.Trim();
            return input.StartsWith("{") && input.EndsWith("}")
                   || input.StartsWith("[") && input.EndsWith("]");
        }
    }
}