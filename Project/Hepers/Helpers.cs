using Lerp2API.Optimizers;
using Lerp2API.SafeECalls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Assertions;
using Color = Lerp2API.Optimizers.Color;
using Random = System.Random;

namespace Lerp2API
{
    /// <summary>
    /// Class NativeHelpers.
    /// </summary>
    public static class NativeHelpers
    {
        #region "String Extensions"

        /// <summary>
        /// Firsts the character to upper.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        public static string FirstCharToUpper(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            return input[0].ToString().ToUpper() + input.Substring(1);
        }

        /// <summary>
        /// Replaces the last.
        /// </summary>
        /// <param name="Source">The source.</param>
        /// <param name="Find">The find.</param>
        /// <param name="Replace">The replace.</param>
        /// <returns>System.String.</returns>
        public static string ReplaceLast(this string Source, string Find, string Replace)
        {
            int place = Source.LastIndexOf(Find);

            if (place == -1)
                return Source;

            string result = Source.Remove(place, Find.Length).Insert(place, Replace);
            return result;
        }

        /// <summary>
        /// Determines whether [is empty or white space] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if [is empty or white space] [the specified value]; otherwise, <c>false</c>.</returns>
        public static bool IsEmptyOrWhiteSpace(this string value)
        {
            return value.All(char.IsWhiteSpace);
        }

        /// <summary>
        /// Replaces at.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="index">The index.</param>
        /// <param name="length">The length.</param>
        /// <param name="replace">The replace.</param>
        /// <returns>System.String.</returns>
        public static string ReplaceAt(this string str, int index, int length, string replace)
        {
            return str.Remove(index, Math.Min(length, str.Length - index))
                    .Insert(index, replace);
        }

        /// <summary>
        /// Odds the even replace.
        /// </summary>
        /// <param name="inpt">The inpt.</param>
        /// <param name="find">The find.</param>
        /// <param name="oddrpl">The oddrpl.</param>
        /// <param name="evenrpl">The evenrpl.</param>
        /// <returns>System.String.</returns>
        public static string OddEvenReplace(this string inpt, string find, string oddrpl, string evenrpl)
        {
            int i = 1;
            MatchCollection matches = Regex.Matches(inpt, find);
            foreach (Match m in matches)
            {
                string rpl = oddrpl;
                if (i % 2 == 0)
                    rpl = evenrpl;
                inpt = inpt.ReplaceAt(m.Index, m.Length, rpl);
                ++i;
            }
            return inpt;
        }

        /// <summary>
        /// Multis the replace.
        /// </summary>
        /// <param name="inpt">The inpt.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="find">The find.</param>
        /// <param name="rpl">The RPL.</param>
        /// <returns>System.String.</returns>
        public static string MultiReplace(this string inpt, string pattern, string find, string rpl)
        {
            MatchCollection col = Regex.Matches(inpt, pattern);
            int oldlen = inpt.Length,
                newlen = inpt.Length;
            foreach (Match m in col)
            {
                inpt = inpt.ReplaceAt(m.Index - (oldlen - newlen), m.Length, m.Value.Replace(find, rpl));
                newlen = inpt.Length;
            }
            return inpt;
        }

        /// <summary>
        /// Replaces the several chars.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="newVal">The new value.</param>
        /// <param name="seps">The seps.</param>
        /// <returns>System.String.</returns>
        public static string ReplaceSeveralChars(this string s, string newVal, params char[] seps)
        {
            return string.Join(newVal, s.Split(seps, StringSplitOptions.RemoveEmptyEntries));
        }

        #endregion "String Extensions"

        #region "Texture Extensions"

        /// <summary>
        /// To the texture.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <param name="w">The w.</param>
        /// <param name="h">The h.</param>
        /// <returns>Texture2D.</returns>
        /// <exception cref="Exception">This method doesn't avoid zero or negative dimensions.</exception>
        public static Texture2D ToTexture(this UnityEngine.Color c, int w = 1, int h = 1)
        {
            if (w < 1 || h < 1)
                throw new Exception("This method doesn't avoid zero or negative dimensions.");
            Texture2D t = new Texture2D(w, h);
            if (w > 1 && h > 1)
                for (int i = 0; i < w; ++i)
                    for (int j = 0; j < h; ++j)
                        t.SetPixel(i, j, c);
            else if (w > 1 && h == 1)
                for (int i = 0; i < w; ++i)
                    t.SetPixel(i, 0, c);
            else if (w == 1 && h > 1)
                for (int j = 0; j < h; ++j)
                    t.SetPixel(0, j, c);
            else
                t.SetPixel(0, 0, c);
            t.Apply();
            return t;
        }

        #endregion "Texture Extensions"

        #region "Array Extensions"

        /// <summary>
        /// Pushes the specified item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">The array.</param>
        /// <param name="item">The item.</param>
        /// <returns>T[].</returns>
        public static T[] Push<T>(this T[] array, T item)
        {
            List<T> l = new List<T>();
            if (array != null)
                l = array.ToList();
            l.Add(item);
            array = l.ToArray();
            return array;
        }

        /// <summary>
        /// Removes at.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">The array.</param>
        /// <param name="index">The index.</param>
        /// <returns>T[].</returns>
        public static T[] RemoveAt<T>(this T[] array, int index)
        {
            List<T> l = array.ToList();
            l.RemoveAt(index);
            array = l.ToArray();
            return array;
        }

        /// <summary>
        /// Merges the specified arrays.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arrays">The arrays.</param>
        /// <returns>T[].</returns>
        public static T[] Merge<T>(params object[] arrays)
        {
            List<T> arr = new List<T>();
            foreach (object t in arrays)
                if (t is Array)
                    foreach (object st in (Array)t)
                        arr.Add((T)st);
                else
                    arr.Add((T)t);
            return arr.ToArray();
        }

        /// <summary>
        /// Gets the WWW.
        /// </summary>
        /// <param name="a">a.</param>
        /// <returns>WWW[].</returns>
        public static WWW[] GetWWW(this string[] a)
        {
            List<WWW> w = new List<WWW>();
            foreach (string p in a)
                w.Add(new WWW(p));
            return w.ToArray();
        }

        #endregion "Array Extensions"

        #region "Dictionary Extensions"

        /// <summary>
        /// Renames the key.
        /// </summary>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <typeparam name="TValue">The type of the t value.</typeparam>
        /// <param name="dic">The dic.</param>
        /// <param name="fromKey">From key.</param>
        /// <param name="toKey">To key.</param>
        /// <returns>Dictionary&lt;TKey, TValue&gt;.</returns>
        public static Dictionary<TKey, TValue> RenameKey<TKey, TValue>(this IDictionary<TKey, TValue> dic,
                                          TKey fromKey, TKey toKey)
        {
            TValue value = dic[fromKey];
            dic.Remove(fromKey);
            if (!dic.ContainsKey(toKey))
                dic[toKey] = value;
            else
            {
                TValue v = dic[toKey];
                dic.Remove(toKey);
                dic[fromKey] = v;
                dic[toKey] = value;
            }
            return (Dictionary<TKey, TValue>)dic;
        }

        #endregion "Dictionary Extensions"

        #region "Iteration Extensions"

        /// <summary>
        /// Fors the each.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ie">The ie.</param>
        /// <param name="action">The action.</param>
        public static void ForEach<T>(this IEnumerable<T> ie, Action<T> action)
        {
            foreach (var i in ie)
                action(i);
        }

        #endregion "Iteration Extensions"

        #region "IO Extensions"

        /// <summary>
        /// Deletes the directory.
        /// </summary>
        /// <param name="target_dir">The target dir.</param>
        public static void DeleteDirectory(this string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
                DeleteDirectory(dir);

            Directory.Delete(target_dir, false);
        }

        /// <summary>
        /// Latests the modification.
        /// </summary>
        /// <param name="dir">The dir.</param>
        /// <returns>System.Int64.</returns>
        public static long LatestModification(string dir)
        {
            string[] files = Directory.GetFiles(dir, "*", SearchOption.AllDirectories);
            List<long> dates = new List<long>();
            foreach (string f in files)
                dates.Add(File.GetAttributes(f) == FileAttributes.Directory ? Directory.GetCreationTime(f).ToEpoch() : File.GetCreationTime(f).ToEpoch());
            if (files != null)
                return dates.Max();
            else
                return -1;
        }

        #endregion "IO Extensions"

        #region "GameObject Extensions"

        /// <summary>
        /// Gets the root.
        /// </summary>
        /// <param name="go">The go.</param>
        /// <returns>GameObject.</returns>
        public static GameObject getRoot(this GameObject go)
        {
            Transform t = go.transform;
            while (t.parent != null)
                t = t.parent.transform;
            return t.gameObject;
        }

        #endregion "GameObject Extensions"

        #region "Object Extensions"

        /// <summary>
        /// To the u string.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>System.String.</returns>
        public static string ToUString(this object message) //To Universal String, converts an object to string by checking if it's a string...
        {
            return message.GetType().Equals(typeof(string)) ? (string)message : message.ToString();
        }

        #endregion "Object Extensions"

        #region "Color Extensions"

        /// <summary>
        /// Colors to hexadecimal.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>System.String.</returns>
        public static string ColorToHex(this UnityEngine.Color color)
        {
            return ColorToHex((Color32)color);
        }

        /// <summary>
        /// Colors to hexadecimal.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>System.String.</returns>
        public static string ColorToHex(this Color32 color)
        {
            string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
            return hex;
        }

        /// <summary>
        /// Hexadecimals to color.
        /// </summary>
        /// <param name="hex">The hexadecimal.</param>
        /// <returns>UnityEngine.Color.</returns>
        public static UnityEngine.Color HexToColor(this string hex)
        {
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                 g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                 b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            return new Color32(r, g, b, 255);
        }

        #endregion "Color Extensions"

        #region "Action Extensions"

        /// <summary>
        /// Converts the specified my action t.
        /// </summary>
        /// <typeparam name="T1">The type of the t1.</typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <param name="myActionT">My action t.</param>
        /// <returns>Action&lt;T2&gt;.</returns>
        public static Action<T2> Convert<T1, T2>(this Action<T1> myActionT)
        {
            if (myActionT == null) return null;
            else return new Action<T2>(o => myActionT((T1)(object)o)); //Doesn't convert correctly types
        }

        #endregion "Action Extensions"

        #region "IEnumerable Extensions"

        /// <summary>
        /// Clones the specified real object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="RealObject">The real object.</param>
        /// <returns>T.</returns>
        public static T Clone<T>(this T RealObject)
        {
            using (Stream objectStream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, RealObject);
                objectStream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(objectStream);
            }
        }

        /// <summary>
        /// Takes the last.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="N">The n.</param>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int N)
        {
            return source.Skip(Math.Max(0, source.Count() - N));
        }

        ///<summary>Finds the index of the first item matching an expression in an enumerable.</summary>
        ///<param name="items">The enumerable to search.</param>
        ///<param name="predicate">The expression to test the items against.</param>
        ///<returns>The index of the first matching item, or -1 if no items match.</returns>
        public static int FindIndex<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        {
            if (items == null) throw new ArgumentNullException("items");
            if (predicate == null) throw new ArgumentNullException("predicate");

            int retVal = 0;
            foreach (var item in items)
            {
                if (predicate(item)) return retVal;
                retVal++;
            }
            return -1;
        }

        ///<summary>Finds the index of the first occurrence of an item in an enumerable.</summary>
        ///<param name="items">The enumerable to search.</param>
        ///<param name="item">The item to find.</param>
        ///<returns>The index of the first matching item, or -1 if the item was not found.</returns>
        public static int IndexOf<T>(this IEnumerable<T> items, T item) { return items.FindIndex(i => EqualityComparer<T>.Default.Equals(item, i)); }

        #endregion "IEnumerable Extensions"

        #region "Misc Extensions"

        /// <summary>
        /// Unpacks the nl.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>System.String.</returns>
        public static string UnpackNl(string str)
        {
            return str.Replace("\\n", Environment.NewLine);
        }

        /// <summary>
        /// Packs the nl.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>System.String.</returns>
        public static string PackNl(string str)
        {
            return str.Replace(Environment.NewLine, "\\n");
        }

        /// <summary>
        /// Base64s the encode.
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <returns>System.String.</returns>
        public static string Base64Encode(this string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// Base64s the decode.
        /// </summary>
        /// <param name="base64EncodedData">The base64 encoded data.</param>
        /// <returns>System.String.</returns>
        public static string Base64Decode(this string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        /// <summary>
        /// Safes the arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>System.String.</returns>
        public static string SafeArguments(this string args)
        {
            return args.Base64Encode();
        }

        /// <summary>
        /// Unsafes the arguments.
        /// </summary>
        /// <param name="unargs">The unargs.</param>
        /// <returns>System.String[].</returns>
        public static string[] UnsafeArguments(this string unargs)
        {
            return unargs.Base64Decode().OddEvenReplace("'", "<", ">").MultiReplace(@"\<.+?\>", " ", "/*/").ReplaceSeveralChars("'", '<', '>').Split(' ').Select(x => x.Replace("/*/", " ")).ToArray();
        }

        #endregion "Misc Extensions"

        #region "UnityEngine Extensions"

        /// <summary>
        /// To the type of the logger.
        /// </summary>
        /// <param name="logtype">The logtype.</param>
        /// <returns>LoggerType.</returns>
        public static LoggerType ToLoggerType(this LogType logtype)
        {
            switch (logtype)
            {
                case LogType.Log:
                    return LoggerType.INFO;

                case LogType.Warning:
                    return LoggerType.WARN;

                case LogType.Error:
                    return LoggerType.ERROR;

                default:
                    return LoggerType.INFO;
            }
        }

        #endregion "UnityEngine Extensions"
    }
}

#region "GUI Utils"

/// <summary>
/// Class ShadowAndOutline.
/// </summary>
public static class ShadowAndOutline
{
    /// <summary>
    /// Draws the outline.
    /// </summary>
    /// <param name="rect">The rect.</param>
    /// <param name="text">The text.</param>
    /// <param name="style">The style.</param>
    /// <param name="outColor">Color of the out.</param>
    /// <param name="inColor">Color of the in.</param>
    /// <param name="size">The size.</param>
    public static void DrawOutline(Rect rect, string text, GUIStyle style, UnityEngine.Color outColor, UnityEngine.Color inColor, float size)
    {
        float halfSize = size * 0.5F;
        GUIStyle backupStyle = new GUIStyle(style);
        UnityEngine.Color backupColor = GUI.color;

        style.normal.textColor = outColor;
        GUI.color = outColor;

        rect.x -= halfSize;
        GUI.Label(rect, text, style);

        rect.x += size;
        GUI.Label(rect, text, style);

        rect.x -= halfSize;
        rect.y -= halfSize;
        GUI.Label(rect, text, style);

        rect.y += size;
        GUI.Label(rect, text, style);

        rect.y -= halfSize;
        style.normal.textColor = inColor;
        GUI.color = backupColor;
        GUI.Label(rect, text, style);

        style = backupStyle;
    }

    /// <summary>
    /// Draws the shadow.
    /// </summary>
    /// <param name="rect">The rect.</param>
    /// <param name="content">The content.</param>
    /// <param name="style">The style.</param>
    /// <param name="txtColor">Color of the text.</param>
    /// <param name="shadowColor">Color of the shadow.</param>
    /// <param name="direction">The direction.</param>
    public static void DrawShadow(Rect rect, GUIContent content, GUIStyle style, UnityEngine.Color txtColor, UnityEngine.Color shadowColor,
                                        Vector2 direction)
    {
        rect.x += direction.x;
        rect.y += direction.y;
        GUI.Label(rect, content, new GUIStyle(style) { normal = new GUIStyleState() { textColor = shadowColor } });

        rect.x -= direction.x;
        rect.y -= direction.y;
        GUI.Label(rect, content, new GUIStyle(style) { normal = new GUIStyleState() { textColor = txtColor } });
    }

    /// <summary>
    /// Draws the layout outline.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="style">The style.</param>
    /// <param name="outColor">Color of the out.</param>
    /// <param name="inColor">Color of the in.</param>
    /// <param name="size">The size.</param>
    /// <param name="options">The options.</param>
    public static void DrawLayoutOutline(string text, GUIStyle style, UnityEngine.Color outColor, UnityEngine.Color inColor, float size, params GUILayoutOption[] options)
    {
        DrawOutline(GUILayoutUtility.GetRect(new GUIContent(text), style, options), text, style, outColor, inColor, size);
    }

    /// <summary>
    /// Draws the layout shadow.
    /// </summary>
    /// <param name="content">The content.</param>
    /// <param name="style">The style.</param>
    /// <param name="txtColor">Color of the text.</param>
    /// <param name="shadowColor">Color of the shadow.</param>
    /// <param name="direction">The direction.</param>
    /// <param name="options">The options.</param>
    public static void DrawLayoutShadow(GUIContent content, GUIStyle style, UnityEngine.Color txtColor, UnityEngine.Color shadowColor,
                                        Vector2 direction, params GUILayoutOption[] options)
    {
        DrawShadow(GUILayoutUtility.GetRect(content, style, options), content, style, txtColor, shadowColor, direction);
    }

    /// <summary>
    /// Draws the button with shadow.
    /// </summary>
    /// <param name="r">The r.</param>
    /// <param name="content">The content.</param>
    /// <param name="style">The style.</param>
    /// <param name="shadowAlpha">The shadow alpha.</param>
    /// <param name="direction">The direction.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public static bool DrawButtonWithShadow(Rect r, GUIContent content, GUIStyle style, float shadowAlpha, Vector2 direction)
    {
        GUIStyle letters = new GUIStyle(style);
        letters.normal.background = null;
        letters.hover.background = null;
        letters.active.background = null;

        bool result = GUI.Button(r, content, style);

        UnityEngine.Color color = r.Contains(Event.current.mousePosition) ? letters.hover.textColor : letters.normal.textColor;

        DrawShadow(r, content, letters, color, new UnityEngine.Color(0f, 0f, 0f, shadowAlpha), direction);

        return result;
    }

    /// <summary>
    /// Draws the layout button with shadow.
    /// </summary>
    /// <param name="content">The content.</param>
    /// <param name="style">The style.</param>
    /// <param name="shadowAlpha">The shadow alpha.</param>
    /// <param name="direction">The direction.</param>
    /// <param name="options">The options.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public static bool DrawLayoutButtonWithShadow(GUIContent content, GUIStyle style, float shadowAlpha,
                                                       Vector2 direction, params GUILayoutOption[] options)
    {
        return DrawButtonWithShadow(GUILayoutUtility.GetRect(content, style, options), content, style, shadowAlpha, direction);
    }
}

#endregion "GUI Utils"

#region "Color Utils"

/// <summary>
/// Class ColorHelpers.
/// </summary>
public static class ColorHelpers
{
    /// <summary>
    /// Gets the bixels.
    /// </summary>
    /// <param name="t">The t.</param>
    /// <returns>Color[].</returns>
    public static Color[,] GetBixels(this Texture2D t)
    {
        int w = t.width, h = t.height;
        Color[,] cs = new Color[w, h];
        for (int i = 0; i < w; ++i)
            for (int j = 0; j < h; ++j)
                cs[i, j] = (Color)t.GetPixel(i, h - j - 1); //c[i + j * w];
        return cs;
    }

    /// <summary>
    /// Gets the color.
    /// </summary>
    /// <param name="c">The c.</param>
    /// <param name="w">The w.</param>
    /// <param name="h">The h.</param>
    /// <returns>UnityEngine.Color[].</returns>
    public static UnityEngine.Color[] GetColor(this Color[,] c, int w, int h)
    {
        UnityEngine.Color[] cs = new UnityEngine.Color[w * h];
        for (int i = 0; i < w; ++i)
            for (int j = 0; j < h; ++j)
                cs[i + j * w] = c[i, h - j - 1];
        return cs;
    }

    /// <summary>
    /// Fills the specified w.
    /// </summary>
    /// <param name="c">The c.</param>
    /// <param name="w">The w.</param>
    /// <param name="h">The h.</param>
    /// <returns>Color[].</returns>
    public static Color[,] Fill(this Color c, int w, int h)
    {
        Color[,] cs = new Color[w, h];
        for (int i = 0; i < w; ++i)
            for (int j = 0; j < h; ++j)
                cs[i, j] = c;
        return cs;
    }

    internal static Dictionary<string, Texture2D> assocColor = new Dictionary<string, Texture2D>();

    /// <summary>
    /// To the texture.
    /// </summary>
    /// <param name="c">The c.</param>
    /// <returns>Texture2D.</returns>
    public static Texture2D ToTexture(this Color c)
    {
        Texture2D t = new Texture2D(1, 1);
        t.SetPixel(0, 0, c);
        t.Apply();
        if (assocColor.ContainsKey(c.ToString()))
            return assocColor[c.ToString()];
        else
        {
            assocColor.Add(c.ToString(), t);
            return t;
        }
    }

    /// <summary>
    /// Clones the specified w.
    /// </summary>
    /// <param name="c">The c.</param>
    /// <param name="w">The w.</param>
    /// <param name="h">The h.</param>
    /// <param name="step">The step.</param>
    /// <param name="upt">The upt.</param>
    /// <param name="f">The f.</param>
    /// <returns>IEnumerator.</returns>
    public static IEnumerator Clone(this Color[,] c, int w, int h, int step, Action upt, Action<Color[,]> f)
    {
        Color[,] nc = new Color[w, h];
        int k = 0;
        for (int i = 0; i < w; ++i)
            for (int j = 0; j < h; ++j)
            {
                //Color cc = ((Color[])new Color[] { c[i, j] }.Clone())[0];
                nc[i, j] = (Color)c[i, j].Clone(); //new Color(cc.r, cc.g, cc.b);
                upt();
                ++k;
                if (k % step == 0)
                    yield return null;
            }
        f(nc);
    }

    /// <summary>
    /// Upts the pixel.
    /// </summary>
    /// <param name="t">The t.</param>
    /// <param name="p">The p.</param>
    /// <param name="c">The c.</param>
    public static void UptPixel(this Texture2D t, Point p, Color c)
    {
        t.SetPixel(p.x, t.height - p.y - 1, c);
        t.Apply();
    }
}

#endregion "Color Utils"

#region "Color Extensions"

/// <summary>
/// Class PointHelpers.
/// </summary>
public static class PointHelpers
{
    /// <summary>
    /// Gets the vec arr.
    /// </summary>
    /// <param name="ps">The ps.</param>
    /// <returns>Vector2[].</returns>
    public static Vector2[] GetVecArr(this Point[] ps)
    {
        return Array.ConvertAll(ps, (Point item) => (Vector2)item);
    }

    /// <summary>
    /// Gets the point arr.
    /// </summary>
    /// <param name="ps">The ps.</param>
    /// <returns>Point[].</returns>
    public static Point[] GetPointArr(this Vector2[] ps)
    {
        return Array.ConvertAll(ps, (Vector2 item) => (Point)item);
    }
}

#endregion "Color Extensions"

#region "Math Extensions"

/// <summary>
/// Class MathHelpers.
/// </summary>
public static class MathHelpers
{
    /// <summary>
    /// Determines whether the specified vertices is clockwise.
    /// </summary>
    /// <param name="vertices">The vertices.</param>
    /// <returns><c>true</c> if the specified vertices is clockwise; otherwise, <c>false</c>.</returns>
    public static bool IsClockwise(this IEnumerable<Point> vertices)
    {
        double sum = 0.0;
        for (int i = 0; i < vertices.Count(); i++)
        {
            Point v1 = vertices.ElementAt(i),
                  v2 = vertices.ElementAt((i + 1) % vertices.Count()); // % is the modulo operator
            sum += (v2.x - v1.x) * (v2.y + v1.y);
        }
        return sum > 0.0;
    }

    /// <summary>
    /// Sorts the corners clockwise.
    /// </summary>
    /// <param name="A">a.</param>
    /// <param name="B">The b.</param>
    /// <param name="center">The center.</param>
    /// <returns>System.Int32.</returns>
    public static int SortCornersClockwise(Point A, Point B, Point center)
    {
        //  Variables to Store the atans
        double aTanA, aTanB;

        //  Fetch the atans
        aTanA = Math.Atan2(A.y - center.y, A.x - center.x);
        aTanB = Math.Atan2(B.y - center.y, B.x - center.x);

        //  Determine next point in Clockwise rotation
        if (aTanA < aTanB) return -1;
        else if (aTanA > aTanB) return 1;
        return 0;
    }

    /// <summary>
    /// Ins the range.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="max">The maximum.</param>
    /// <returns>System.Int32.</returns>
    public static int InRange(this int value, int max) //Zero-index bases
    {
        if (value >= max) return value % max;
        else if (value < 0) return max - Mathf.Abs(value);
        else return value;
    }

    /*public static ulong RandomUInt64(ulong min, ulong max)
    {
        return ((max - min) + min) * Random.value;
    }*/

    /// <summary>
    /// Nexts the u int64.
    /// </summary>
    /// <param name="rnd">The random.</param>
    /// <param name="max">The maximum.</param>
    /// <returns>System.UInt64.</returns>
    public static ulong NextUInt64(this Random rnd, ulong max)
    {
        int rawsize = System.Runtime.InteropServices.Marshal.SizeOf(max);
        var buffer = new byte[rawsize];
        rnd.NextBytes(buffer);
        return BitConverter.ToUInt64(buffer, 0);
    }

    /*public static bool Orientation(this IEnumerable<Point> polygon, Point up)
    {
        var sum = polygon
            .Buffer(2, 1) // from Interactive Extensions Nuget Pkg
            .Where(b => b.Count == 2)
            .Aggregate
              (Vector3.Zero
              , (p, b) => p + Vector3.Cross(b[0], b[1])
                              / b[0].Length() / b[1].Length());

        return Vector3.Dot(up, sum) > 0;
    }*/
}

#endregion "Math Extensions"

#region "DateTime Extensions"

/// <summary>
/// Class DateTimeHelpers.
/// </summary>
public static class DateTimeHelpers
{
    /// <summary>
    /// Converts a DateTime to the long representation which is the number of seconds since the unix epoch.
    /// </summary>
    /// <param name="dateTime">A DateTime to convert to epoch time.</param>
    /// <returns>The long number of seconds since the unix epoch.</returns>
    public static long ToEpoch(this DateTime dateTime) => (long)(dateTime - new DateTime(1970, 1, 1)).TotalSeconds;

    /// <summary>
    /// Converts a long representation of time since the unix epoch to a DateTime.
    /// </summary>
    /// <param name="epoch">The number of seconds since Jan 1, 1970.</param>
    /// <returns>A DateTime representing the time since the epoch.</returns>
    public static DateTime FromEpoch(this long epoch) => new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified).AddSeconds(epoch);

    /// <summary>
    /// Converts a DateTime? to the long? representation which is the number of seconds since the unix epoch.
    /// </summary>
    /// <param name="dateTime">A DateTime? to convert to epoch time.</param>
    /// <returns>The long? number of seconds since the unix epoch.</returns>
    public static long? ToEpoch(this DateTime? dateTime) => dateTime.HasValue ? (long?)ToEpoch(dateTime.Value) : null;

    /// <summary>
    /// Converts a long? representation of time since the unix epoch to a DateTime?.
    /// </summary>
    /// <param name="epoch">The number of seconds since Jan 1, 1970.</param>
    /// <returns>A DateTime? representing the time since the epoch.</returns>
    public static DateTime? FromEpoch(this long? epoch) => epoch.HasValue ? (DateTime?)FromEpoch(epoch.Value) : null;
}

#endregion "DateTime Extensions"

#region "Reflection Extensions"

/// <summary>
/// Class ReflectionHelpers.
/// </summary>
public class ReflectionHelpers
{
    // All error checking omitted. In particular, check the results
    // of Type.GetType, and make sure you call it with a fully qualified
    // type name, including the assembly if it's not in mscorlib or
    // the current assembly. The method has to be a public instance
    // method with no parameters. (Use BindingFlags with GetMethod
    // to change this.)
    /// <summary>
    /// Invokes the specified type name.
    /// </summary>
    /// <param name="typeName">Name of the type.</param>
    /// <param name="methodName">Name of the method.</param>
    public static void Invoke(string typeName, string methodName)
    {
        Type type = Type.GetType(typeName);
        object instance = Activator.CreateInstance(type);
        MethodInfo method = type.GetMethod(methodName);
        method.Invoke(instance, null);
    }

    /// <summary>
    /// Invokes the specified method name.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="methodName">Name of the method.</param>
    public static void Invoke<T>(string methodName) where T : new()
    {
        T instance = new T();
        MethodInfo method = typeof(T).GetMethod(methodName);
        method.Invoke(instance, null);
    }
}

#endregion "Reflection Extensions"

#region "Assertions Extensions"

/// <summary>
/// Class AssertExt.
/// </summary>
public class AssertExt
{
    /// <summary>
    /// Ares the same.
    /// </summary>
    /// <param name="a">a.</param>
    /// <param name="b">The b.</param>
    public static void AreSame(object a, object b)
    {
        Assert.IsTrue(ReferenceEquals(a, b));
    }
}

#endregion "Assertions Extensions"