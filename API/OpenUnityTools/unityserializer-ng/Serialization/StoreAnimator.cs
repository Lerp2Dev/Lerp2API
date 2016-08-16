using UnityEngine;

[AddComponentMenu("Storage/Store Animator")]
[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
[DoNotSerializePublic]
public class StoreAnimator : MonoBehaviour {
    /// <summary>
    /// Unity's API doesn't allow to restore mecanim transitions. When a save occurs during one,
    /// you can either choose to revert back to the starting point of the transition or skip it.
    /// </summary>
    public enum LoadingMode {
        REVERT,
        SKIP
    }
    [SerializeField]
    private LoadingMode loadingMode = LoadingMode.REVERT;

    [SerializeField]
    private bool storeAllLayers = true;

    [SerializeField]
    [HideInInspector]
    /// <summary>
    /// Stores which layers to save and which to ignore.
    /// </summary>
    private byte[] layerMask = new byte[0];

    #region Editor accessors
#if UNITY_EDITOR
    public bool e_storeAllLayers {
        get { return storeAllLayers; }
        set { storeAllLayers = value; }
    }
    public byte[] e_layerMask {
        get { return layerMask; }
        set { layerMask = value; }
    }
#endif
    #endregion

    /// <summary>
    /// Stores all relevant information for a mecanim layer
    /// </summary>
    public struct LayerInfo {
        public int index;
        public int currentHash;
        public int nextHash;
        public float normalizedTimeCurrent;
        public float normalizedTimeNext;
        public float weight;
    }
    [SerializeThis]
    private LayerInfo[] layerData;

    /// <summary>
    /// Stores all relevant information for a mecanim parameter
    /// </summary>
    public struct ParameterInfo {
        public int number;
        public AnimatorControllerParameterType type;
        public object value;
    }
    [SerializeThis]
    private ParameterInfo[] parameterData;


    private void OnSerializing() {
        Animator animator = GetComponent<Animator>();

        if (storeAllLayers) {
            // Store the current state for each layer
            layerData = new LayerInfo[animator.layerCount];
            for (int i = 0; i < animator.layerCount; i++) {
                layerData[i] = new LayerInfo {
                    index = i,
                    currentHash = animator.GetCurrentAnimatorStateInfo(i).shortNameHash,
                    nextHash = animator.GetNextAnimatorStateInfo(i).shortNameHash,
                    normalizedTimeCurrent = animator.GetCurrentAnimatorStateInfo(i).normalizedTime,
                    normalizedTimeNext = animator.GetNextAnimatorStateInfo(i).normalizedTime,
                    weight = animator.GetLayerWeight(i)
                };
            }
        }
        else {
            // Store the current state for each layer we want to store
            layerData = new LayerInfo[GetStoredLayerCount()];
            int li;
            for (int i = li = 0; i < animator.layerCount; i++) {
                if ((layerMask[i / 8] >> (i % 8) & 1) == 0) {
                    continue;
                }

                layerData[li] = new LayerInfo {
                    index = i,
                    currentHash = animator.GetCurrentAnimatorStateInfo(i).shortNameHash,
                    nextHash = animator.GetNextAnimatorStateInfo(i).shortNameHash,
                    normalizedTimeCurrent = animator.GetCurrentAnimatorStateInfo(i).normalizedTime,
                    normalizedTimeNext = animator.GetNextAnimatorStateInfo(i).normalizedTime,
                    weight = animator.GetLayerWeight(i)
                };
                li++;
            }
        }

        // Store every parameter
        parameterData = new ParameterInfo[animator.parameterCount];
        for (int i = 0; i < animator.parameterCount; i++) {
            parameterData[i] = new ParameterInfo {
                number = animator.parameters[i].nameHash,
                type = animator.parameters[i].type,
                value = GetParameterValue(animator.parameters[i].nameHash, animator.parameters[i].type, animator)
            };
        }
    }

    private void OnDeserialized() {
        Animator animator = GetComponent<Animator>();

        // Restore the states of each layer
        foreach (LayerInfo layer in layerData) {
            if (loadingMode == LoadingMode.REVERT) {
                animator.Play(layer.currentHash, layer.index, layer.normalizedTimeCurrent);
            }
            else {
                animator.Play(layer.nextHash, layer.index, layer.normalizedTimeNext);
            }
            animator.SetLayerWeight(layer.index, layer.weight);
        }

        // Restore the parameters of the animator
        foreach (ParameterInfo parameter in parameterData) {
            switch (parameter.type) {
                case AnimatorControllerParameterType.Float:
                    animator.SetFloat(parameter.number, (float)parameter.value);
                    break;
                case AnimatorControllerParameterType.Int:
                    animator.SetInteger(parameter.number, (int)parameter.value);
                    break;
                case AnimatorControllerParameterType.Bool:
                    animator.SetBool(parameter.number, (bool)parameter.value);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    if ((bool)parameter.value) {
                        animator.SetTrigger(parameter.number);
                    }
                    else {
                        animator.ResetTrigger(parameter.number);
                    }
                    break;
            }
        }
    }

    private int GetStoredLayerCount() {
        int count = 0;
        for (int i = 0; i < layerMask.Length; i++) {
            for (int i2 = 0; i2 < 8; i2++) {
                count += (layerMask[i] >> i2) & 1;
            }
        }
        return count;
    }

    private object GetParameterValue(int i, AnimatorControllerParameterType type, Animator animator) {
        switch (type) {
            case AnimatorControllerParameterType.Float:
                return animator.GetFloat(i);
            case AnimatorControllerParameterType.Int:
                return animator.GetInteger(i);
            case AnimatorControllerParameterType.Bool:
                return animator.GetBool(i);
            case AnimatorControllerParameterType.Trigger:
                return animator.GetBool(i);
            default:
                return null;
        }
    }
}