namespace HtmlAgilityPack
{
    using System;
    using System.Collections.Generic;

    internal class NameValuePairList
    {
        private List<KeyValuePair<string, string>> _allPairs;
        private Dictionary<string, List<KeyValuePair<string, string>>> _pairsWithName;
        internal readonly string Text;

        internal NameValuePairList() : this(null)
        {
        }

        internal NameValuePairList(string text)
        {
            Text = text;
            _allPairs = new List<KeyValuePair<string, string>>();
            _pairsWithName = new Dictionary<string, List<KeyValuePair<string, string>>>();
            Parse(text);
        }

        internal List<KeyValuePair<string, string>> GetNameValuePairs(string name)
        {
            if (name == null)
            {
                return _allPairs;
            }
            if (!_pairsWithName.ContainsKey(name))
            {
                return new List<KeyValuePair<string, string>>();
            }
            return _pairsWithName[name];
        }

        internal static string GetNameValuePairsValue(string text, string name)
        {
            NameValuePairList list = new NameValuePairList(text);
            return list.GetNameValuePairValue(name);
        }

        internal string GetNameValuePairValue(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException();
            }
            List<KeyValuePair<string, string>> nameValuePairs = GetNameValuePairs(name);
            if (nameValuePairs.Count == 0)
            {
                return string.Empty;
            }
            KeyValuePair<string, string> pair = nameValuePairs[0];
            return pair.Value.Trim();
        }

        private void Parse(string text)
        {
            _allPairs.Clear();
            _pairsWithName.Clear();
            if (text != null)
            {
                foreach (string str in text.Split(new char[] { ';' }))
                {
                    if (str.Length != 0)
                    {
                        string[] strArray2 = str.Split(new char[] { '=' }, 2);
                        if (strArray2.Length != 0)
                        {
                            List<KeyValuePair<string, string>> list;
                            KeyValuePair<string, string> item = new KeyValuePair<string, string>(strArray2[0].Trim().ToLower(), (strArray2.Length < 2) ? "" : strArray2[1]);
                            _allPairs.Add(item);
                            if (!_pairsWithName.ContainsKey(item.Key))
                            {
                                list = new List<KeyValuePair<string, string>>();
                                _pairsWithName[item.Key] = list;
                            }
                            else
                            {
                                list = _pairsWithName[item.Key];
                            }
                            list.Add(item);
                        }
                    }
                }
            }
        }
    }
}