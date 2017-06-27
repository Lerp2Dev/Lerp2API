namespace Serialization
{
    using UnityEngine;

    /// <summary>
    /// Class StoreAnimator.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    [AddComponentMenu("Storage/Store Animator")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    [DoNotSerializePublic]
    public partial class StoreAnimator : MonoBehaviour
    {
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

        /// <summary>
        /// Gets or sets a value indicating whether [e store all layers].
        /// </summary>
        /// <value><c>true</c> if [e store all layers]; otherwise, <c>false</c>.</value>
        public bool e_storeAllLayers
        {
            get { return storeAllLayers; }
            set { storeAllLayers = value; }
        }

        /// <summary>
        /// Gets or sets the e layer mask.
        /// </summary>
        /// <value>The e layer mask.</value>
        public byte[] e_layerMask
        {
            get { return layerMask; }
            set { layerMask = value; }
        }

#endif

        #endregion Editor accessors

        /// <summary>
        /// Stores all relevant information for a mecanim layer
        /// </summary>
        public struct LayerInfo
        {
            /// <summary>
            /// The index
            /// </summary>
            public int index;
            /// <summary>
            /// The current hash
            /// </summary>
            public int currentHash;
            /// <summary>
            /// The next hash
            /// </summary>
            public int nextHash;
            /// <summary>
            /// The normalized time current
            /// </summary>
            public float normalizedTimeCurrent;
            /// <summary>
            /// The normalized time next
            /// </summary>
            public float normalizedTimeNext;
            /// <summary>
            /// The weight
            /// </summary>
            public float weight;
        }

        [SerializeThis]
        private LayerInfo[] layerData;

        /// <summary>
        /// Stores all relevant information for a mecanim parameter
        /// </summary>
        public struct ParameterInfo
        {
            /// <summary>
            /// The number
            /// </summary>
            public int number;
            /// <summary>
            /// The type
            /// </summary>
            public AnimatorControllerParameterType type;
            /// <summary>
            /// The value
            /// </summary>
            public object value;
        }

        [SerializeThis]
        private ParameterInfo[] parameterData;

        private void OnSerializing()
        {
            Animator animator = GetComponent<Animator>();

            if (storeAllLayers)
            {
                // Store the current state for each layer
                layerData = new LayerInfo[animator.layerCount];
                for (int i = 0; i < animator.layerCount; i++)
                {
                    layerData[i] = new LayerInfo
                    {
                        index = i,
                        currentHash = animator.GetCurrentAnimatorStateInfo(i).shortNameHash,
                        nextHash = animator.GetNextAnimatorStateInfo(i).shortNameHash,
                        normalizedTimeCurrent = animator.GetCurrentAnimatorStateInfo(i).normalizedTime,
                        normalizedTimeNext = animator.GetNextAnimatorStateInfo(i).normalizedTime,
                        weight = animator.GetLayerWeight(i)
                    };
                }
            }
            else
            {
                // Store the current state for each layer we want to store
                layerData = new LayerInfo[GetStoredLayerCount()];
                int li;
                for (int i = li = 0; i < animator.layerCount; i++)
                {
                    if ((layerMask[i / 8] >> (i % 8) & 1) == 0)
                    {
                        continue;
                    }

                    layerData[li] = new LayerInfo
                    {
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
            for (int i = 0; i < animator.parameterCount; i++)
            {
                parameterData[i] = new ParameterInfo
                {
                    number = animator.parameters[i].nameHash,
                    type = animator.parameters[i].type,
                    value = GetParameterValue(animator.parameters[i].nameHash, animator.parameters[i].type, animator)
                };
            }
        }

        private void OnDeserialized()
        {
            Animator animator = GetComponent<Animator>();

            // Restore the states of each layer
            foreach (LayerInfo layer in layerData)
            {
                if (loadingMode == LoadingMode.REVERT)
                {
                    animator.Play(layer.currentHash, layer.index, layer.normalizedTimeCurrent);
                }
                else
                {
                    animator.Play(layer.nextHash, layer.index, layer.normalizedTimeNext);
                }
                animator.SetLayerWeight(layer.index, layer.weight);
            }

            // Restore the parameters of the animator
            foreach (ParameterInfo parameter in parameterData)
            {
                switch (parameter.type)
                {
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
                        if ((bool)parameter.value)
                        {
                            animator.SetTrigger(parameter.number);
                        }
                        else
                        {
                            animator.ResetTrigger(parameter.number);
                        }
                        break;
                }
            }
        }

        private int GetStoredLayerCount()
        {
            int count = 0;
            for (int i = 0; i < layerMask.Length; i++)
            {
                for (int i2 = 0; i2 < 8; i2++)
                {
                    count += (layerMask[i] >> i2) & 1;
                }
            }
            return count;
        }

        private object GetParameterValue(int i, AnimatorControllerParameterType type, Animator animator)
        {
            switch (type)
            {
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
}