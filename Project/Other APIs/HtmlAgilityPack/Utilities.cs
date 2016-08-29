namespace HtmlAgilityPack
{
    using System.Collections.Generic;

    internal static class Utilities
    {
        public static TValue GetDictionaryValueOrNull<TKey, TValue>(Dictionary<TKey, TValue> dict, TKey key) where TKey : class
        {
            if (!dict.ContainsKey(key))
            {
                return default(TValue);
            }
            return dict[key];
        }
    }
}