using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Lerp2API.Hepers.Unity_Extensions
{
    /// <summary>
    /// Class UnityHelpers.
    /// </summary>
    public static class UnityHelpers
    {
        //d = delay, lt = loop time
        /// <summary>
        /// Cools the repeating.
        /// </summary>
        /// <param name="mono">The mono.</param>
        /// <param name="act">The act.</param>
        /// <param name="d">The d.</param>
        /// <param name="lt">The lt.</param>
        /// <returns>AdvancedCoroutine.</returns>
        public static AdvancedCoroutine CoolRepeating(this MonoBehaviour mono, Action<AdvancedCoroutine> act, float d, float lt)
        {
            AdvancedCoroutine adv = new AdvancedCoroutine(mono, act);
            adv.cor = mono.StartCoroutine(CoolCoroutine(act, adv, d, lt));
            return adv;
        }

        private static IEnumerator CoolCoroutine(Action<AdvancedCoroutine> a, AdvancedCoroutine adv, float d, float lt)
        {
            yield return new WaitForSeconds(d);
            while (true)
            {
                a(adv);
                yield return new WaitForSeconds(lt);
            }
        }

        /// <summary>
        /// Fades the position.
        /// </summary>
        /// <param name="tr">The tr.</param>
        /// <param name="pos1">The pos1.</param>
        /// <param name="pos2">The pos2.</param>
        /// <param name="time">The time.</param>
        /// <param name="local">if set to <c>true</c> [local].</param>
        /// <param name="fin">The fin.</param>
        /// <returns>IEnumerator.</returns>
        public static IEnumerator FadePosition(Transform tr, Vector3 pos1, Vector3 pos2, float time, bool local = false, Action fin = null)
        {
            float frames = time / Time.fixedDeltaTime;

            for (int i = 0; i < frames; ++i)
            {
                Vector3 pos = Vector3.Lerp(pos1, pos2, i / frames);
                if (local)
                    tr.localPosition = pos;
                else
                    tr.position = pos;
                yield return new WaitForFixedUpdate();
            }
            if (fin != null)
                fin();
        }

        /// <summary>
        /// Delayeds the start.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="t">The t.</param>
        /// <returns>IEnumerator.</returns>
        public static IEnumerator DelayedStart(this Action a, float t)
        {
            yield return new WaitForSeconds(t);
            a();
        }

        /// <summary>
        /// Loads the clip.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="clip">The clip.</param>
        public static void LoadClip(this AudioSource source, AudioClip clip)
        {
            if (source == null)
            {
                Debug.LogWarning("The AudioSource you passed is null.");
                return;
            }

            source.clip = clip;
        }

        /// <summary>
        /// Determines whether the specified transform is grounded.
        /// </summary>
        /// <param name="transform">The transform.</param>
        /// <returns><c>true</c> if the specified transform is grounded; otherwise, <c>false</c>.</returns>
        public static bool IsGrounded(this Transform transform)
        {
            Collider col = transform.gameObject.GetComponent<Collider>();

            if (col == null)
            {
                Debug.LogWarningFormat("'{0}' doesn't have a valid Collider.", transform.gameObject.name);
                return false;
            }

            float dist = col.bounds.extents.y;
            return Physics.Raycast(transform.position, -Vector3.up, dist + .1f);
        }

        /// <summary>
        /// Determines whether [is touching something] [the specified transform].
        /// </summary>
        /// <param name="transform">The transform.</param>
        /// <returns><c>true</c> if [is touching something] [the specified transform]; otherwise, <c>false</c>.</returns>
        public static bool IsTouchingSomething(this Transform transform)
        {
            Collider[] cols = null;
            return transform.IsTouchingSomething(out cols);
        }

        /// <summary>
        /// Determines whether [is touching something] [the specified cols].
        /// </summary>
        /// <param name="transform">The transform.</param>
        /// <param name="cols">The cols.</param>
        /// <returns><c>true</c> if [is touching something] [the specified cols]; otherwise, <c>false</c>.</returns>
        public static bool IsTouchingSomething(this Transform transform, out Collider[] cols)
        {
            SphereCollider col = transform.gameObject.GetComponent<SphereCollider>();

            if (col == null)
            {
                Debug.LogWarningFormat("'{0}' doesn't have a valid SphereCollider.", transform.gameObject.name);
                cols = null;
                return false;
            }
            cols = Physics.OverlapSphere(transform.position, col.radius + .1f);
            return cols != null;
        }

        /// <summary>
        /// Gets the radius.
        /// </summary>
        /// <param name="tr">The tr.</param>
        /// <returns>System.Single.</returns>
        public static float GetRadius(this Transform tr)
        {
            return (tr.localScale.x + tr.localScale.y + tr.localScale.z) / 3;
        }

        //I have to make all the overloads...
        /// <summary>
        /// Verticals the look at.
        /// </summary>
        /// <param name="transform">The transform.</param>
        /// <param name="worldPos">The world position.</param>
        /// <param name="worldUp">The world up.</param>
        public static void VerticalLookAt(this Transform transform, Vector3 worldPos, Vector3 worldUp)
        {
            transform.LookAt(worldPos, worldUp);
            Vector3 rot = transform.eulerAngles;
            transform.eulerAngles = new Vector3(0, rot.y, 0); //Fix lookat inclination, I will need also in my waypoint asset
        }

        /// <summary>
        /// Clamps the rotation.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="angle">The angle.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <param name="local">if set to <c>true</c> [local].</param>
        public static void ClampRotation(this Transform t, Vector3 angle, Vector3 min, Vector3 max, bool local = true)
        {
            Vector3 aang = t.eulerAngles + angle,
                    nang = new Vector3(ClampAngle(aang.x, min.x, max.x), ClampAngle(aang.y, min.y, max.y), ClampAngle(aang.z, min.z, max.z));
            if (local)
                t.localRotation = Quaternion.Euler(nang);
            else
                t.rotation = Quaternion.Euler(nang);
        }

        /// <summary>
        /// Clamps the rotation.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="angle">The angle.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <param name="dir">The dir.</param>
        /// <param name="local">if set to <c>true</c> [local].</param>
        public static void ClampRotation(this Transform t, float angle, float min, float max, Vector3 dir, bool local = true)
        {
            Vector3 adir = new Vector3(Mathf.Abs(dir.x), Mathf.Abs(dir.y), Mathf.Abs(dir.z));
            float a = 0;
            if (adir == Vector3.zero || adir == Vector3.one)
                return;
            a = MultiplyVectors(t.eulerAngles, adir).magnitude;
            float aang = a + angle,
                    nang = ClampAngle(aang, min, max);
            if (local)
                t.localRotation = Quaternion.Euler(nang * adir);
            else
                t.rotation = Quaternion.Euler(nang * adir);
        }

        /// <summary>
        /// Multiplies the vectors.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>Vector3.</returns>
        public static Vector3 MultiplyVectors(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        /** Normalize angles to a range from -180 to 180 an then clamp the angle
          * with min and max.
          */

        /// <summary>
        /// Clamps the angle.
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns>System.Single.</returns>
        public static float ClampAngle(float angle, float min, float max)
        {
            angle = NormalizeAngle(angle);
            if (angle > 180)
            {
                angle -= 360;
            }
            else if (angle < -180)
            {
                angle += 360;
            }

            min = NormalizeAngle(min);
            if (min > 180)
            {
                min -= 360;
            }
            else if (min < -180)
            {
                min += 360;
            }

            max = NormalizeAngle(max);
            if (max > 180)
            {
                max -= 360;
            }
            else if (max < -180)
            {
                max += 360;
            }

            // Aim is, convert angles to -180 until 180.
            return Mathf.Clamp(angle, min, max);
        }

        /** If angles over 360 or under 360 degree, then normalize them.
         */

        /// <summary>
        /// Normalizes the angle.
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <returns>System.Single.</returns>
        public static float NormalizeAngle(float angle)
        {
            while (angle > 360)
                angle -= 360;
            while (angle < 0)
                angle += 360;
            return angle;
        }

        /// <summary>
        /// Copies the component.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="destination">The destination.</param>
        /// <param name="original">The original.</param>
        /// <returns>T.</returns>
        public static T CopyComponent<T>(this GameObject destination, T original) where T : Component
        {
            Type type = original.GetType();
            Component copy = destination.AddComponent(type);
            FieldInfo[] fields = type.GetFields();
            foreach (FieldInfo field in fields)
            {
                field.SetValue(copy, field.GetValue(original));
            }
            return copy as T;
        }

        /// <summary>
        /// Determines whether the specified this is prefab.
        /// </summary>
        /// <param name="This">The this.</param>
        /// <returns><c>true</c> if the specified this is prefab; otherwise, <c>false</c>.</returns>
        public static bool IsPrefab(this Transform This)
        {
            var TempObject = new GameObject();
            try
            {
                TempObject.transform.parent = This.parent;
                var OriginalIndex = This.GetSiblingIndex();
                This.SetSiblingIndex(int.MaxValue);
                if (This.GetSiblingIndex() == 0) return true;
                This.SetSiblingIndex(OriginalIndex);
                return false;
            }
            finally
            {
                Object.DestroyImmediate(TempObject);
            }
        }

        /// <summary>
        /// Checks if a GameObject has been destroyed.
        /// </summary>
        /// <param name="gameObject">GameObject reference to check for destructedness</param>
        /// <returns>If the game object has been marked as destroyed by UnityEngine</returns>
        public static bool IsDestroyed(this GameObject gameObject)
        {
            // UnityEngine overloads the == opeator for the GameObject type
            // and returns null when the object has been destroyed, but
            // actually the object is still there but has not been cleaned up yet
            // if we test both we can determine if the object has been destroyed.
            return gameObject == null && !ReferenceEquals(gameObject, null);
        }
    }
}