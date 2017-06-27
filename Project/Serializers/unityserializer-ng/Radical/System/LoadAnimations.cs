using System.Linq;
using UnityEngine;

/// <summary>
/// Class LoadAnimations.
/// </summary>
[AddComponentMenu("System/Load Animations")]
public class LoadAnimations : MonoBehaviour
{
    /// <summary>
    /// The name
    /// </summary>
    public new string name;

    private void Awake()
    {
        var clips = Resources.LoadAll("Animations/" + name, typeof(AnimationClip)).Cast<AnimationClip>();
        foreach (var c in clips)
        {
            GetComponent<Animation>().AddClip(c, c.name.Contains("@") ? c.name.Substring(c.name.LastIndexOf("@") + 1) : c.name);
        }

        foreach (var a in GetComponent<Animation>().Cast<AnimationState>())
        {
            a.enabled = true;
        }
    }
}