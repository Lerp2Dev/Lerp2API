using UnityEngine;
using System.Collections;

// usage: attach this script into camera, call Shake() method to start
// source: http://answers.unity3d.com/answers/992509/view.html

/// <summary>
/// Class CameraShake.
/// </summary>
public class CameraShake : MonoBehaviour
{
    /// <summary>
    /// The shake position
    /// </summary>
    public bool shakePosition;

    /// <summary>
    /// The shake rotation
    /// </summary>
    public bool shakeRotation;

    /// <summary>
    /// The shake intensity minimum
    /// </summary>
    public float shakeIntensityMin = 0.1f;

    /// <summary>
    /// The shake intensity maximum
    /// </summary>
    public float shakeIntensityMax = 0.5f;

    /// <summary>
    /// The shake decay
    /// </summary>
    public float shakeDecay = 0.02f;

    private Vector3 OriginalPos;
    private Quaternion OriginalRot;

    private bool isShakeRunning = false;

    // call this function to start shaking
    /// <summary>
    /// Shakes this instance.
    /// </summary>
    public void Shake()
    {
        OriginalPos = transform.position;
        OriginalRot = transform.rotation;
        StartCoroutine("ProcessShake");
    }

    private IEnumerator ProcessShake()
    {
        if (!isShakeRunning)
        {
            isShakeRunning = true;
            float currentShakeIntensity = Random.Range(shakeIntensityMin, shakeIntensityMax);

            while (currentShakeIntensity > 0)
            {
                if (shakePosition)
                {
                    transform.position = OriginalPos + Random.insideUnitSphere * currentShakeIntensity;
                }
                if (shakeRotation)
                {
                    transform.rotation = new Quaternion(OriginalRot.x + Random.Range(-currentShakeIntensity, currentShakeIntensity) * .2f,
                                                         OriginalRot.y + Random.Range(-currentShakeIntensity, currentShakeIntensity) * .2f,
                                                         OriginalRot.z + Random.Range(-currentShakeIntensity, currentShakeIntensity) * .2f,
                                                         OriginalRot.w + Random.Range(-currentShakeIntensity, currentShakeIntensity) * .2f);
                }
                currentShakeIntensity -= shakeDecay;
                yield return null;
            }

            isShakeRunning = false;
        }
    }
}