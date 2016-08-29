// /* ------------------
//       ${Name}
//       (c)3Radical 2012
//           by Mike Talbot
//     ------------------- */
//
using UnityEngine;

[AddComponentMenu("Storage/Advanced/In Range Item")]
public class InRangeItem : MonoBehaviour
{
    private void Start()
    {
        if (OnlyInRangeManager.Instance != null)
        {
            OnlyInRangeManager.Instance.AddRangedItem(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (OnlyInRangeManager.Instance != null)
        {
            OnlyInRangeManager.Instance.DestroyRangedItem(gameObject);
        }
    }
}