namespace Lerp2API.Hepers.Unity_Extensions
{
    /// <summary>
    /// Class ComponentHelpers.
    /// </summary>
    public static class ComponentHelpers
    {
        /*public static T FindComponentInChildWithTag<T>(this GameObject parent, string tag) where T : Component
        {
            Transform t = parent.transform;
            foreach (Transform tr in t)
            {
                if (tr.tag == tag)
                {
                    return tr.GetComponent<T>();
                }
                else
                {
                    tr.GetComponent<T>().FindComponentInChildWithTag(tag);
                }
            }
        }*/

        /*public static T[] GetComponentsRecursively<T>(this GameObject go) where T : Component
        {
            return go.transform.GetComponentsRecursively<T>();
        }

        [BadOptimized] //StackOverFlowException
        public static T[] GetComponentsRecursively<T>(this Transform tr) where T : Component
        {
            List<T> objs = tr.GetComponentsInChildren<T>().ToList();
            if (tr.childCount > 0)
                foreach (Transform child in tr.GetComponentsInChildren<Transform>())
                    objs.AddRange(child.GetComponentsRecursively<T>());
            return objs.ToArray();
        }*/
    }
}