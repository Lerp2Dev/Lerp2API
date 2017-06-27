using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// Class WeakReference.
/// </summary>
/// <typeparam name="T"></typeparam>
public class WeakReference<T> where T : class
{
    private WeakReference r;

    /// <summary>
    /// Gets or sets the target.
    /// </summary>
    /// <value>The target.</value>
    public T Target
    {
        get
        {
            return r.IsAlive ? (T)r.Target : null;
        }
        set
        {
            r = new WeakReference(value);
        }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is alive.
    /// </summary>
    /// <value><c>true</c> if this instance is alive; otherwise, <c>false</c>.</value>
    public bool IsAlive
    {
        get
        {
            return r.IsAlive;
        }
    }

    /// <summary>
    /// Performs an implicit conversion from <see cref="WeakReference{T}"/> to <see cref="T"/>.
    /// </summary>
    /// <param name="re">The re.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator T(WeakReference<T> re)
    {
        return re.Target;
    }

    /// <summary>
    /// Performs an implicit conversion from <see cref="T"/> to <see cref="WeakReference{T}"/>.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator WeakReference<T>(T value)
    {
        return new WeakReference<T>() { Target = value };
    }
}

/// <summary>
/// Class ObservedList.
/// </summary>
/// <typeparam name="T"></typeparam>
[Serializable]
public class ObservedList<T> : List<T>
{
    /// <summary>
    /// Occurs when [changed].
    /// </summary>
    public event Action<int> Changed = delegate { };

    /// <summary>
    /// Gets or sets the <see cref="T"/> at the specified index.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns>T.</returns>
    public new T this[int index]
    {
        get
        {
            return base[index];
        }
        set
        {
            base[index] = value;
            Changed(index);
        }
    }
}

/// <summary>
/// Class Lookup.
/// </summary>
/// <typeparam name="TK">The type of the tk.</typeparam>
/// <typeparam name="TR">The type of the tr.</typeparam>
[Serializable]
public class Lookup<TK, TR> : Dictionary<TK, TR> where TR : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Lookup{TK, TR}"/> class.
    /// </summary>
    public Lookup()
        : base()
    { }

    /// <summary>
    /// Gets or sets the <see cref="TR"/> at the specified index.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns>TR.</returns>
    public new virtual TR this[TK index]
    {
        get
        {
            if (ContainsKey(index))
                return base[index];
            return null;
        }
        set
        {
            base[index] = value;
        }
    }

    /// <summary>
    /// Gets the specified index.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="index">The index.</param>
    /// <returns>T.</returns>
    public T Get<T>(TK index) where T : class
    {
        return this[index] as T;
    }
}

/// <summary>
/// Interface IChanged
/// </summary>
public interface IChanged
{
    /// <summary>
    /// Changeds the specified index.
    /// </summary>
    /// <param name="index">The index.</param>
    void Changed(object index);
}

/// <summary>
/// Interface INeedParent
/// </summary>
public interface INeedParent
{
    /// <summary>
    /// Sets the parent.
    /// </summary>
    /// <param name="parent">The parent.</param>
    /// <param name="index">The index.</param>
    void SetParent(IChanged parent, object index);
}

/// <summary>
/// Class Index.
/// </summary>
/// <typeparam name="TK">The type of the tk.</typeparam>
/// <typeparam name="TR">The type of the tr.</typeparam>
[Serializable]
public class Index<TK, TR> : Lookup<TK, TR>, IChanged where TR : class, new()
{
    /// <summary>
    /// Occurs when [setting].
    /// </summary>
    public event Action<TK, TR, TR> Setting = delegate { };

    /// <summary>
    /// Occurs when [getting].
    /// </summary>
    public event Action<TK, TR> Getting = delegate { };

    /// <summary>
    /// Changeds the specified index.
    /// </summary>
    /// <param name="index">The index.</param>
    public void Changed(object index)
    {
        if (Setting != null)
        {
            TR current = null;
            if (ContainsKey((TK)index))
                current = base[(TK)index];
            Setting((TK)index, current, current);
        }
    }

    /// <summary>
    /// Gets or sets the <see cref="TR"/> at the specified index.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns>TR.</returns>
    public override TR this[TK index]
    {
        get
        {
            if (ContainsKey(index))
            {
                return base[index];
            }
            var ret = new TR();
            if (ret is INeedParent)
            {
                (ret as INeedParent).SetParent(this, index);
            }
            base[index] = ret;
            Getting(index, ret);
            return ret;
        }
        set
        {
            if (Setting != null)
            {
                TR current = null;
                if (ContainsKey(index))
                    current = base[index];
                Setting(index, current, value);
            }
            base[index] = value;
        }
    }
}

/// <summary>
/// Class GUIBackgroundColor. This class cannot be inherited.
/// </summary>
public sealed class GUIBackgroundColor : IDisposable
{
    private Color old;

    /// <summary>
    /// Initializes a new instance of the <see cref="GUIBackgroundColor"/> class.
    /// </summary>
    /// <param name="color">The color.</param>
    public GUIBackgroundColor(Color color)
    {
        old = GUI.backgroundColor;
        GUI.backgroundColor = color;
    }

    /// <summary>
    /// Disposes this instance.
    /// </summary>
    public void Dispose()
    {
        GUI.backgroundColor = old;
    }
}

/// <summary>
/// Class GUIArea. This class cannot be inherited.
/// </summary>
public sealed class GUIArea : IDisposable
{
    private static int rotated;

    /// <summary>
    /// Class Rotated. This class cannot be inherited.
    /// </summary>
    public sealed class Rotated : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Rotated"/> class.
        /// </summary>
        public Rotated()
        {
            rotated++;
        }

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        public void Dispose()
        {
            rotated--;
        }
    }

    /// <summary>
    /// Gets the standard area.
    /// </summary>
    /// <returns>Rect.</returns>
    public static Rect GetStandardArea()
    {
        return new Rect(10, 10, 940, 620);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GUIArea"/> class.
    /// </summary>
    public GUIArea()
        : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GUIArea"/> class.
    /// </summary>
    /// <param name="area">The area.</param>
    public GUIArea(Rect? area)
    {
        var a = area ?? GUIArea.GetStandardArea();
        if (rotated > 0)
        {
            a.y += a.height;
            var w = a.width;
            a.width = a.height;
            a.height = w;
        }

        GUILayout.BeginArea(a);
        if (rotated > 0)
        {
            GUIUtility.RotateAroundPivot(-90f, Vector2.zero);
        }
    }

    /// <summary>
    /// Disposes this instance.
    /// </summary>
    public void Dispose()
    {
        GUILayout.EndArea();
    }
}

/// <summary>
/// Class GUIScale. This class cannot be inherited.
/// </summary>
public sealed class GUIScale : IDisposable
{
    private static int count = 0;
    private static Matrix4x4 cached;

    /// <summary>
    /// Initializes a new instance of the <see cref="GUIScale"/> class.
    /// </summary>
    public GUIScale()
    {
        if (count++ == 0)
        {
            cached = GUI.matrix;
            if (Screen.width < 500)
                GUIUtility.ScaleAroundPivot(new Vector2(0.5f, 0.5f), Vector2.zero);
            if (Screen.width > 1100)
                GUIUtility.ScaleAroundPivot(new Vector2(2f, 2f), Vector2.zero);
        }
    }

    /// <summary>
    /// Disposes this instance.
    /// </summary>
    public void Dispose()
    {
        if (--count == 0)
            GUI.matrix = cached;
    }
}

/// <summary>
/// Class Horizontal. This class cannot be inherited.
/// </summary>
public sealed class Horizontal : IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Horizontal"/> class.
    /// </summary>
    public Horizontal()
    {
        GUILayout.BeginHorizontal();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Horizontal"/> class.
    /// </summary>
    /// <param name="style">The style.</param>
    public Horizontal(GUIStyle style)
    {
        GUILayout.BeginHorizontal(style);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Horizontal"/> class.
    /// </summary>
    /// <param name="options">The options.</param>
    public Horizontal(params GUILayoutOption[] options)
    {
        GUILayout.BeginHorizontal(options);
    }

    /// <summary>
    /// Disposes this instance.
    /// </summary>
    public void Dispose()
    {
        GUILayout.EndHorizontal();
    }
}

/// <summary>
/// Class Vertical. This class cannot be inherited.
/// </summary>
public sealed class Vertical : IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Vertical"/> class.
    /// </summary>
    /// <param name="options">The options.</param>
    public Vertical(params GUILayoutOption[] options)
    {
        GUILayout.BeginVertical(options);
    }

    /// <summary>
    /// Disposes this instance.
    /// </summary>
    public void Dispose()
    {
        GUILayout.EndVertical();
    }
}

/// <summary>
/// Class ScrollView. This class cannot be inherited.
/// </summary>
public sealed class ScrollView : IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ScrollView"/> class.
    /// </summary>
    /// <param name="scroll">The scroll.</param>
    public ScrollView(ref Vector2 scroll)
    {
        scroll = GUILayout.BeginScrollView(scroll);
    }

    /// <summary>
    /// Disposes this instance.
    /// </summary>
    public void Dispose()
    {
        GUILayout.EndScrollView();
    }
}

/// <summary>
/// Class Box. This class cannot be inherited.
/// </summary>
public sealed class Box : IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Box"/> class.
    /// </summary>
    /// <param name="style">The style.</param>
    public Box(GUIStyle style)
    {
        GUILayout.BeginVertical(style);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Box"/> class.
    /// </summary>
    /// <param name="options">The options.</param>
    public Box(params GUILayoutOption[] options)
    {
        GUILayout.BeginVertical("box", options);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Box"/> class.
    /// </summary>
    public Box()
    {
        GUILayout.BeginVertical("box");
    }

    /// <summary>
    /// Disposes this instance.
    /// </summary>
    public void Dispose()
    {
        GUILayout.EndVertical();
    }
}

/// <summary>
/// Class HorizontalCentered. This class cannot be inherited.
/// </summary>
public sealed class HorizontalCentered : IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HorizontalCentered"/> class.
    /// </summary>
    public HorizontalCentered()
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HorizontalCentered"/> class.
    /// </summary>
    /// <param name="style">The style.</param>
    public HorizontalCentered(GUIStyle style)
    {
        GUILayout.BeginHorizontal(style);
        GUILayout.FlexibleSpace();
    }

    /// <summary>
    /// Disposes this instance.
    /// </summary>
    public void Dispose()
    {
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }
}

/// <summary>
/// Class VerticalCentered. This class cannot be inherited.
/// </summary>
public sealed class VerticalCentered : IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VerticalCentered"/> class.
    /// </summary>
    public VerticalCentered()
    {
        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VerticalCentered"/> class.
    /// </summary>
    /// <param name="options">The options.</param>
    public VerticalCentered(params GUILayoutOption[] options)
    {
        GUILayout.BeginVertical(options);
        GUILayout.FlexibleSpace();
    }

    /// <summary>
    /// Disposes this instance.
    /// </summary>
    public void Dispose()
    {
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
    }
}

/// <summary>
/// Class RightAligned. This class cannot be inherited.
/// </summary>
public sealed class RightAligned : IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RightAligned"/> class.
    /// </summary>
    public RightAligned()
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
    }

    /// <summary>
    /// Disposes this instance.
    /// </summary>
    public void Dispose()
    {
        GUILayout.EndHorizontal();
    }
}

/// <summary>
/// Class LeftAligned. This class cannot be inherited.
/// </summary>
public sealed class LeftAligned : IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LeftAligned"/> class.
    /// </summary>
    public LeftAligned()
    {
        GUILayout.BeginHorizontal();
    }

    /// <summary>
    /// Disposes this instance.
    /// </summary>
    public void Dispose()
    {
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }
}

/// <summary>
/// Class BottomAligned. This class cannot be inherited.
/// </summary>
public sealed class BottomAligned : IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BottomAligned"/> class.
    /// </summary>
    public BottomAligned()
    {
        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();
    }

    /// <summary>
    /// Disposes this instance.
    /// </summary>
    public void Dispose()
    {
        GUILayout.EndVertical();
    }
}

/// <summary>
/// Class SceneIndex.
/// </summary>
public class SceneIndex : MonoBehaviour
{
}

/// <summary>
/// Class Radical.
/// </summary>
public static class Radical
{
    /// <summary>
    /// Activates the children.
    /// </summary>
    /// <param name="co">The co.</param>
    public static void ActivateChildren(this Component co)
    {
        co.gameObject.SetActive(true);
    }

    /// <summary>
    /// Maximums the by.
    /// </summary>
    /// <typeparam name="TSource">The type of the t source.</typeparam>
    /// <typeparam name="TKey">The type of the t key.</typeparam>
    /// <param name="source">The source.</param>
    /// <param name="selector">The selector.</param>
    /// <returns>TSource.</returns>
    public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source,
Func<TSource, TKey> selector) where TSource : class
    {
        return source.MaxBy(selector, Comparer<TKey>.Default);
    }

    /// <summary>
    /// Maximums the by.
    /// </summary>
    /// <typeparam name="TSource">The type of the t source.</typeparam>
    /// <typeparam name="TKey">The type of the t key.</typeparam>
    /// <param name="source">The source.</param>
    /// <param name="selector">The selector.</param>
    /// <param name="comparer">The comparer.</param>
    /// <returns>TSource.</returns>
    public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source,
    Func<TSource, TKey> selector, IComparer<TKey> comparer) where TSource : class
    {
        using (IEnumerator<TSource> sourceIterator = source.GetEnumerator())
        {
            if (!sourceIterator.MoveNext())
            {
                return null;
            }
            TSource max = sourceIterator.Current;
            TKey maxKey = selector(max);
            while (sourceIterator.MoveNext())
            {
                TSource candidate = sourceIterator.Current;
                TKey candidateProjected = selector(candidate);
                if (comparer.Compare(candidateProjected, maxKey) > 0)
                {
                    max = candidate;
                    maxKey = candidateProjected;
                }
            }
            return max;
        }
    }

    /// <summary>
    /// Zips the specified seq2.
    /// </summary>
    /// <typeparam name="T1">The type of the t1.</typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <typeparam name="TResult">The type of the t result.</typeparam>
    /// <param name="seq1">The seq1.</param>
    /// <param name="seq2">The seq2.</param>
    /// <param name="resultSelector">The result selector.</param>
    /// <returns>IEnumerable&lt;TResult&gt;.</returns>
    public static IEnumerable<TResult> Zip<T1, T2, TResult>(this IEnumerable<T1> seq1, IEnumerable<T2> seq2, Func<T1, T2, TResult> resultSelector)
    {
        var results = new List<TResult>();
        var enm1 = seq1.GetEnumerator();
        var enm2 = seq2.GetEnumerator();
        while (enm1.MoveNext() && enm2.MoveNext())
        {
            results.Add(resultSelector(enm1.Current, enm2.Current));
        }
        return results;
    }

    /// <summary>
    /// Calleds from.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public static bool CalledFrom(string name)
    {
        var st = new StackTrace();
        foreach (var frame in st.GetFrames())
        {
            if (frame.GetMethod().Name.Contains(name))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Masks the layers.
    /// </summary>
    /// <param name="layers">The layers.</param>
    /// <returns>System.Int32.</returns>
    public static int MaskLayers(params int[] layers)
    {
        var result = 0;
        foreach (var i in layers)
        {
            result |= 1 << i;
        }
        return result;
    }

    /// <summary>
    /// Masks the layers.
    /// </summary>
    /// <param name="layers">The layers.</param>
    /// <returns>System.Int32.</returns>
    public static int MaskLayers(params string[] layers)
    {
        var result = 0;
        foreach (var l in layers)
        {
            result |= 1 << LayerMask.NameToLayer(l);
        }
        return result;
    }

    /// <summary>
    /// Plays the one shot.
    /// </summary>
    /// <param name="gameObject">The game object.</param>
    /// <param name="clip">The clip.</param>
    public static void PlayOneShot(this GameObject gameObject, AudioClip clip)
    {
        if (clip == null)
            return;
        if (!gameObject.GetComponent<AudioSource>())
        {
            gameObject.AddComponent<AudioSource>();
            gameObject.GetComponent<AudioSource>().playOnAwake = false;
        }
        gameObject.GetComponent<AudioSource>().PlayOneShot(clip);
    }

    /// <summary>
    /// Plays the audio.
    /// </summary>
    /// <param name="gameObject">The game object.</param>
    /// <param name="clip">The clip.</param>
    public static void PlayAudio(this GameObject gameObject, AudioClip clip)
    {
        if (clip == null)
            return;
        if (!gameObject.GetComponent<AudioSource>())
        {
            gameObject.AddComponent<AudioSource>();
            gameObject.GetComponent<AudioSource>().playOnAwake = false;
        }
        gameObject.GetComponent<AudioSource>().clip = clip;
        gameObject.GetComponent<AudioSource>().loop = true;
        gameObject.GetComponent<AudioSource>().Play();
    }

    /// <summary>
    /// Fades the volume.
    /// </summary>
    /// <param name="component">The component.</param>
    /// <param name="toLevel">To level.</param>
    /// <param name="time">The time.</param>
    /// <param name="fromLevel">From level.</param>
    public static void FadeVolume(this GameObject component, float toLevel = 1, float time = 1f, float? fromLevel = null)
    {
        component.gameObject.StartExtendedCoroutine(VolumeFader(component.GetComponent<AudioSource>(), toLevel, time, fromLevel));
    }

    private static IEnumerator VolumeFader(AudioSource source, float level, float time, float? fromLevel)
    {
        var currentVolume = fromLevel ?? source.volume;
        var t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / time;
            source.volume = Mathf.Lerp(currentVolume, level, t);
            yield return null;
        }
    }

    /// <summary>
    /// Deactivates the children.
    /// </summary>
    /// <param name="co">The co.</param>
    public static void DeactivateChildren(this Component co)
    {
        foreach (var c in co.transform.GetComponentsInChildren<Transform>().Except(new[] { co.transform }))
            c.gameObject.SetActive(false);
    }

    /// <summary>
    /// Destroys the children.
    /// </summary>
    /// <param name="t">The t.</param>
    public static void DestroyChildren(this Transform t)
    {
        foreach (var c in t.Cast<Transform>())
        {
            GameObject.Destroy(c.gameObject);
        }
    }

    /// <summary>
    /// Class PreferenceAccess.
    /// </summary>
    public class PreferenceAccess
    {
        /// <summary>
        /// Gets or sets the <see cref="System.Boolean"/> with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool this[string name]
        {
            get
            {
                name = "Pref " + name;
                return FilePrefs.HasKey(name) ? (FilePrefs.GetInt(name) == 1) : false;
            }
            set
            {
                name = "Pref " + name;
                FilePrefs.SetInt(name, value ? 1 : 0);
            }
        }
    }

    /// <summary>
    /// The preferences
    /// </summary>
    public static PreferenceAccess Preferences = new PreferenceAccess();

    private static Lookup<string, GameObject> _gameObjects;
    private static Lookup<string, GameObject> _fullPaths;

    /// <summary>
    /// The allow deferred logging
    /// </summary>
    public static bool AllowDeferredLogging = false;

    /// <summary>
    /// Finds the child including deactivated.
    /// </summary>
    /// <param name="t">The t.</param>
    /// <param name="name">The name.</param>
    /// <returns>Transform.</returns>
    public static Transform FindChildIncludingDeactivated(this Transform t, string name)
    {
        var all = t.GetComponentsInChildren<Transform>(true);
        return all.FirstOrDefault(c => c.name == name);
    }

    /// <summary>
    /// Gets the identifier.
    /// </summary>
    /// <param name="go">The go.</param>
    /// <returns>System.String.</returns>
    public static string GetId(this GameObject go)
    {
        var ui = go.GetComponent<UniqueIdentifier>();
        return ui == null ? go.GetFullName() : ui.Id;
    }

    /// <summary>
    /// Finds the game object.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>GameObject.</returns>
    public static GameObject FindGameObject(string name)
    {
        IndexScene();
        return name.Contains("/") ? _fullPaths[name] : _gameObjects[name];
    }

    /// <summary>
    /// Gets the full name.
    /// </summary>
    /// <param name="gameObject">The game object.</param>
    /// <returns>System.String.</returns>
    public static string GetFullName(this GameObject gameObject)
    {
        var list = new Stack<string>();
        var t = gameObject.transform;
        while (t != null)
        {
            list.Push(t.name);
            t = t.parent;
        }
        var sb = new StringBuilder();
        while (list.Count > 0)
        {
            sb.AppendFormat("/{0}", list.Pop());
        }
        return sb.ToString();
    }

    private static void IndexScene()
    {
        if (GameObject.Find("_SceneIndex") != null)
            return;
        _gameObjects = new Lookup<string, GameObject>();
        _fullPaths = new Lookup<string, GameObject>();
        foreach (var g in
            //Resources.FindObjectsOfTypeAll(typeof(Transform)).Cast<Transform>().Select(t=>t.gameObject)
            GameObject.FindObjectsOfType(typeof(GameObject))
            .Cast<GameObject>()
            .Where(g => g.transform.parent == null)
            .SelectMany(g => g.GetComponentsInChildren(typeof(Transform), true)
                .Cast<Transform>()
                .Select(t => t.gameObject))
            )
        {
            _gameObjects[g.name] = g;
            _fullPaths[g.GetFullName().Substring(1)] = g;
        }
        new GameObject("_SceneIndex");
    }

    /// <summary>
    /// Res the index scene.
    /// </summary>
    public static void ReIndexScene()
    {
        var go = GameObject.Find("_SceneIndex");
        if (go != null)
        {
            GameObject.Destroy(go);
        }
    }

    /// <summary>
    /// Finds the specified name.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name">The name.</param>
    /// <returns>T.</returns>
    public static T Find<T>(string name) where T : Component
    {
        IndexScene();
        var go = name.Contains("/") ? _fullPaths[name] : _gameObjects[name];
        if (go == null)
            return null;
        return go.GetComponent<T>();
    }

    /// <summary>
    /// Finds the specified name.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="go">The go.</param>
    /// <param name="name">The name.</param>
    /// <returns>T.</returns>
    public static T Find<T>(this GameObject go, string name) where T : Component
    {
        go = go.transform.FindChild(name).gameObject;
        return go.GetComponentInChildren<T>();
    }

    /// <summary>
    /// To the index.
    /// </summary>
    /// <typeparam name="TSource">The type of the t source.</typeparam>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TR">The type of the tr.</typeparam>
    /// <param name="source">The source.</param>
    /// <param name="keySelector">The key selector.</param>
    /// <param name="elementSelector">The element selector.</param>
    /// <returns>Index&lt;T, List&lt;TR&gt;&gt;.</returns>
    public static Index<T, List<TR>> ToIndex<TSource, T, TR>(this IEnumerable<TSource> source, Func<TSource, T> keySelector, Func<TSource, TR> elementSelector) where T : class where TR : class
    {
        var x = new Index<T, List<TR>>();
        foreach (var v in source)
        {
            x[keySelector(v)].Add(elementSelector == null ? v as TR : elementSelector(v));
        }
        return x;
    }

    /// <summary>
    /// Minimums the by.
    /// </summary>
    /// <typeparam name="TSource">The type of the t source.</typeparam>
    /// <typeparam name="TKey">The type of the t key.</typeparam>
    /// <param name="source">The source.</param>
    /// <param name="selector">The selector.</param>
    /// <returns>TSource.</returns>
    public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source,
    Func<TSource, TKey> selector)
    {
        return source.MinBy(selector, Comparer<TKey>.Default);
    }

    /// <summary>
    /// Minimums the by.
    /// </summary>
    /// <typeparam name="TSource">The type of the t source.</typeparam>
    /// <typeparam name="TKey">The type of the t key.</typeparam>
    /// <param name="source">The source.</param>
    /// <param name="selector">The selector.</param>
    /// <param name="comparer">The comparer.</param>
    /// <returns>TSource.</returns>
    /// <exception cref="InvalidOperationException">Sequence was empty</exception>
    public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source,
    Func<TSource, TKey> selector, IComparer<TKey> comparer)
    {
        using (IEnumerator<TSource> sourceIterator = source.GetEnumerator())
        {
            if (!sourceIterator.MoveNext())
            {
                throw new InvalidOperationException("Sequence was empty");
            }
            TSource min = sourceIterator.Current;
            TKey minKey = selector(min);
            while (sourceIterator.MoveNext())
            {
                TSource candidate = sourceIterator.Current;
                TKey candidateProjected = selector(candidate);
                if (comparer.Compare(candidateProjected, minKey) < 0)
                {
                    min = candidate;
                    minKey = candidateProjected;
                }
            }
            return min;
        }
    }

    /// <summary>
    /// Discretes the specified function.
    /// </summary>
    /// <typeparam name="TResult">The type of the t result.</typeparam>
    /// <typeparam name="T1">The type of the t1.</typeparam>
    /// <param name="seq">The seq.</param>
    /// <param name="func">The function.</param>
    /// <returns>IEnumerable&lt;TResult&gt;.</returns>
    public static IEnumerable<TResult> Discrete<TResult, T1>(this IEnumerable<TResult> seq, Func<TResult, T1> func)
    {
        return seq.GroupBy(func).Select(g => g.First());
    }

    /// <summary>
    /// To the index.
    /// </summary>
    /// <typeparam name="TSource">The type of the t source.</typeparam>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">The source.</param>
    /// <param name="keySelector">The key selector.</param>
    /// <returns>Index&lt;T, List&lt;TSource&gt;&gt;.</returns>
    public static Index<T, List<TSource>> ToIndex<TSource, T>(this IEnumerable<TSource> source, Func<TSource, T> keySelector) where T : class
    {
        var x = new Index<T, List<TSource>>();
        foreach (var v in source)
        {
            x[keySelector(v)].Add(v);
        }
        return x;
    }

    //private static int _indent = 0;
    //public static readonly bool DebugBuild;
    //public static int _deferredLoggingEnabled = 0;

    /*public class Logging : IDisposable
	{
		public Logging()
		{
			_deferredLoggingEnabled++;
		}

		public void Dispose()
		{
			_deferredLoggingEnabled--;
			if (_deferredLoggingEnabled == 0)
			{
				Radical.CommitLog();
			}
		}
	}

	public static bool DeferredLoggingEnabled
	{
		get
		{
			return _deferredLoggingEnabled > 0;
		}
	}*/

    /// <summary>
    /// Finds the interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="go">The go.</param>
    /// <returns>T.</returns>
    public static T FindInterface<T>(this GameObject go) where T : class
    {
        return go.GetComponents<Component>().OfType<T>().FirstOrDefault();
    }

    /// <summary>
    /// Finds the implementor.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="go">The go.</param>
    /// <returns>T.</returns>
    public static T FindImplementor<T>(this GameObject go) where T : class
    {
        return RecurseFind<T>(go);
    }

    /// <summary>
    /// Finds the implementors.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="go">The go.</param>
    /// <returns>T[].</returns>
    public static T[] FindImplementors<T>(this GameObject go) where T : class
    {
        return go.GetComponentsInChildren<Component>().OfType<T>().ToArray();
    }

    private static T RecurseFind<T>(GameObject go) where T : class
    {
        var component = go.GetComponents<Component>().FirstOrDefault(c => c is T);
        if (component != null)
        {
            return component as T;
        }
        if (go.transform.parent != null)
        {
            return RecurseFind<T>(go.transform.parent.gameObject);
        }
        return null;
    }

    ///<summary>Finds the index of the first occurence of an item in an enumerable.</summary>
    ///<param name="items">The enumerable to search.</param>
    ///<param name="item">The item to find.</param>
    ///<returns>The index of the first matching item, or -1 if the item was not found.</returns>
    public static int IndexOf<T>(this IEnumerable<T> items, T item)
    {
        return items.FindIndex(i => EqualityComparer<T>.Default.Equals(item, i));
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

    /// <summary>
    /// Rgbas the specified r.
    /// </summary>
    /// <param name="r">The r.</param>
    /// <param name="g">The g.</param>
    /// <param name="b">The b.</param>
    /// <param name="a">a.</param>
    /// <returns>Color.</returns>
    public static Color RGBA(int r, int g, int b, int a)
    {
        return new Color((float)(r / 255f), (float)(g / 255f), (float)(b / 255f), (float)(a / 255f));
    }

    /// <summary>
    /// The merge mix
    /// </summary>
    public static Vector3 mergeMix = new Vector3(0, 1, 0);

    /// <summary>
    /// Merges the specified second.
    /// </summary>
    /// <param name="first">The first.</param>
    /// <param name="second">The second.</param>
    /// <returns>Quaternion.</returns>
    public static Quaternion Merge(this Quaternion first, Vector3 second)
    {
        return Quaternion.Euler(Merge(first.eulerAngles, second));
    }

    /// <summary>
    /// Merges the specified second.
    /// </summary>
    /// <param name="first">The first.</param>
    /// <param name="second">The second.</param>
    /// <returns>Vector3.</returns>
    public static Vector3 Merge(this Vector3 first, Vector3 second)
    {
        return new Vector3((first.x * (1 - mergeMix.x)) + (second.x * mergeMix.x), (first.y * (1 - mergeMix.y)) + (second.y * mergeMix.y), (first.z * (1 - mergeMix.z)) + (second.z * mergeMix.z));
    }

    static Radical()
    {
        //DebugBuild = UnityEngine.Debug.isDebugBuild;
    }

    /// <summary>
    /// Gets the interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="tra">The tra.</param>
    /// <returns>T.</returns>
    public static T GetInterface<T>(this Transform tra) where T : class
    {
        return tra.gameObject.GetInterface<T>();
    }

    /// <summary>
    /// Gets the interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="go">The go.</param>
    /// <returns>T.</returns>
    public static T GetInterface<T>(this GameObject go) where T : class
    {
        foreach (var c in go.GetComponents<MonoBehaviour>())
        {
            if (c is T)
            {
                return c as T;
            }
        }

        return null;
    }

    /// <summary>
    /// Gets the interfaces.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="go">The go.</param>
    /// <returns>IList&lt;T&gt;.</returns>
    public static IList<T> GetInterfaces<T>(this GameObject go) where T : class
    {
        var l = new List<T>();
        foreach (var c in go.GetComponents<MonoBehaviour>())
        {
            if (c is T)
            {
                l.Add(c as T);
            }
        }

        return l;
    }

    //private static List<string> logEntries = new List<string>();

    /*public static void IndentLog()
	{
		_indent++;
	}

	public static void OutdentLog()
	{
		_indent--;
	}

	public static void LogNode(object message)
	{
		LogNow(message.ToString());
	}
	public static void LogNow(string message, params object[] parms)
	{
		if (!DebugBuild)
			return;
		UnityEngine.Debug.Log(string.Format(message, parms));
	}

	public static void LogWarning(string message)
	{
		LogWarning ( message, null);
	}
	public static void LogWarning(string message, UnityEngine.Object context)
	{
		if (!DebugBuild)
			return;
		if (context != null)
		{
			UnityEngine.Debug.LogWarning(message, context);
		}
		else
		{
			UnityEngine.Debug.LogWarning(message);
		}
	}

	public static void LogError(string message)
	{
		LogError( message, null);
	}
	public static void LogError(string message, UnityEngine.Object context)
	{
		if (!DebugBuild)
		{
			return;
		}
		if (context != null)
			UnityEngine.Debug.LogError(message, context);
		else
			UnityEngine.Debug.LogError(message);
	}

	public static bool IsLogging()
	{
		if (DebugBuild == false || ! DeferredLoggingEnabled)
		{
			return false;
		}
		return true;
	}

	public static void Log(string message, params object[] parms)
	{
		if (DebugBuild == false || ! DeferredLoggingEnabled || !AllowDeferredLogging)
		{
			return;
		}
		logEntries.Add((new string(' ', 4 * _indent)) + string.Format(message, parms));
		if (logEntries.Count > 50000)
		{
			logEntries.RemoveAt(0);
		}
	}

	public static void ClearLog()
	{
		logEntries.Clear();
	}

    public static void CommitLog()
	{
		if (logEntries.Count == 0)
		{
			return;
		}
		var sb = logEntries.Aggregate((current, next) => current + "\n" + next);
		UnityEngine.Debug.Log(sb);
		logEntries.Clear();
	}*/

    /// <summary>
    /// Instantiates the specified template.
    /// </summary>
    /// <param name="template">The template.</param>
    /// <returns>GameObject.</returns>
    public static GameObject Instantiate(Transform template)
    {
        return Instantiate(template, null);
    }

    /// <summary>
    /// Instantiates the specified template.
    /// </summary>
    /// <param name="template">The template.</param>
    /// <param name="parent">The parent.</param>
    /// <returns>GameObject.</returns>
    public static GameObject Instantiate(Transform template, GameObject parent)
    {
        var go = (GameObject.Instantiate(template) as Transform).gameObject;
        if (parent != null)
        {
            go.transform.SetParent(parent.transform);
        }
        return go;
    }

    /// <summary>
    /// Sets the parent.
    /// </summary>
    /// <param name="child">The child.</param>
    /// <param name="parent">The parent.</param>
    /// <returns>GameObject.</returns>
    public static GameObject SetParent(this GameObject child, GameObject parent)
    {
        return SetParent(child, parent, false);
    }

    /// <summary>
    /// Sets the parent.
    /// </summary>
    /// <param name="child">The child.</param>
    /// <param name="parent">The parent.</param>
    /// <param name="setScale">if set to <c>true</c> [set scale].</param>
    /// <returns>GameObject.</returns>
    public static GameObject SetParent(this GameObject child, GameObject parent, bool setScale)
    {
        child.transform.SetParent(parent.transform, setScale);
        return child;
    }

    /// <summary>
    /// Sets the parent.
    /// </summary>
    /// <param name="child">The child.</param>
    /// <param name="parent">The parent.</param>
    /// <returns>Transform.</returns>
    public static Transform SetParent(this Transform child, GameObject parent)
    {
        return SetParent(child, parent, false);
    }

    /// <summary>
    /// Sets the parent.
    /// </summary>
    /// <param name="child">The child.</param>
    /// <param name="parent">The parent.</param>
    /// <param name="setScale">if set to <c>true</c> [set scale].</param>
    /// <returns>Transform.</returns>
    public static Transform SetParent(this Transform child, GameObject parent, bool setScale)
    {
        child.SetParent(parent.transform, setScale);
        return child;
    }

    /// <summary>
    /// Sets the parent.
    /// </summary>
    /// <param name="child">The child.</param>
    /// <param name="parent">The parent.</param>
    /// <returns>Transform.</returns>
    public static Transform SetParent(this Transform child, Transform parent)
    {
        return SetParent(child, parent, false);
    }

    /// <summary>
    /// Sets the parent.
    /// </summary>
    /// <param name="child">The child.</param>
    /// <param name="parent">The parent.</param>
    /// <param name="setScale">if set to <c>true</c> [set scale].</param>
    /// <returns>Transform.</returns>
    public static Transform SetParent(this Transform child, Transform parent, bool setScale)
    {
        try
        {
            Vector3 pos = child.localPosition;
            Quaternion rot = child.localRotation;
            Vector3 scale = child.localScale;

            child.parent = parent;
            child.localPosition = pos;
            child.localRotation = rot;
            if (setScale)
            {
                child.localScale = scale;
            }
        }
        catch
        {
        }
        return child;
    }

    /// <summary>
    /// Smoothes the damp.
    /// </summary>
    /// <param name="current">The current.</param>
    /// <param name="target">The target.</param>
    /// <param name="velocity">The velocity.</param>
    /// <param name="time">The time.</param>
    /// <returns>Quaternion.</returns>
    public static Quaternion SmoothDamp(this Vector3 current, Vector3 target, ref Vector3 velocity, float time)
    {
        Vector3 result = Vector3.zero;
        result.x = Mathf.SmoothDampAngle(current.x, target.x, ref velocity.x, time);
        result.y = Mathf.SmoothDampAngle(current.y, target.y, ref velocity.y, time);
        result.z = Mathf.SmoothDampAngle(current.z, target.z, ref velocity.z, time);
        return Quaternion.Euler(result);
    }

    /// <summary>
    /// Adds the child.
    /// </summary>
    /// <param name="parent">The parent.</param>
    /// <param name="template">The template.</param>
    /// <returns>GameObject.</returns>
    public static GameObject AddChild(this GameObject parent, Transform template)
    {
        return Instantiate(template, parent);
    }

    /// <summary>
    /// Ensures the component.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="t">The t.</param>
    public static void EnsureComponent(this GameObject obj, Type t)
    {
        if (obj.GetComponent(t) == null)
        {
            obj.AddComponent(t);
        }
    }

    /// <summary>
    /// Removes the component.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="t">The t.</param>
    public static void RemoveComponent(this GameObject obj, Type t)
    {
        foreach (var c in obj.GetComponents(t))
            Object.DestroyImmediate(c);
    }
}

/// <summary>
/// Class TextHelper.
/// </summary>
public static class TextHelper
{
    /// <summary>
    /// Fixes to.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <param name="width">The width.</param>
    /// <returns>System.String.</returns>
    public static string FixTo(this string str, float width)
    {
        return FixTo(str, width, "label");
    }

    /// <summary>
    /// Fixes to.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <param name="width">The width.</param>
    /// <param name="type">The type.</param>
    /// <returns>System.String.</returns>
    public static string FixTo(this string str, float width, string type)
    {
        var widthOfTab = GUI.skin.GetStyle(type).CalcSize(new GUIContent("\t")).x;
        var widthOfDot = GUI.skin.GetStyle(type).CalcSize(new GUIContent(".")).x;
        var widthOfSpace = Mathf.Max(1, GUI.skin.GetStyle(type).CalcSize(new GUIContent(". .")).x - (2 * widthOfDot));
        var widthOfString = GUI.skin.GetStyle(type).CalcSize(new GUIContent(str)).x;
        return str + new String(' ', (int)((width - widthOfTab - widthOfString) / widthOfSpace) + 1) + "\t";
    }
}

/// <summary>
/// Class ValueLookup.
/// </summary>
/// <typeparam name="TK">The type of the tk.</typeparam>
/// <typeparam name="TR">The type of the tr.</typeparam>
[Serializable]
public class ValueLookup<TK, TR> : Dictionary<TK, TR> where TR : struct
{
    /// <summary>
    /// Gets or sets the <see cref="TR"/> at the specified index.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns>TR.</returns>
    public new virtual TR this[TK index]
    {
        get
        {
            if (ContainsKey(index))
                return base[index];
            return default(TR);
        }
        set
        {
            base[index] = value;
        }
    }

    /// <summary>
    /// Gets the specified index.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="index">The index.</param>
    /// <returns>T.</returns>
    public T Get<T>(TK index) where T : class
    {
        return this[index] as T;
    }
}