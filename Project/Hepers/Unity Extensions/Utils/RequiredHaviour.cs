using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Debug = Lerp2API._Debug.Debug;

namespace Lerp2API.Hepers.Unity_Extensions.Utils
{
    //This uses LateUpdate(), so, don't forget to call base.LateUpdate()
    /// <summary>
    /// Class RequiredHaviour.
    /// </summary>
    public class RequiredHaviour : MonoBehaviour
    {
        /// <summary>
        /// Me
        /// </summary>
        public static RequiredHaviour me;

        //You will need to check if this is true before you use, any of the required members.
        /// <summary>
        /// Gets or sets a value indicating whether [allow continue].
        /// </summary>
        /// <value><c>true</c> if [allow continue]; otherwise, <c>false</c>.</value>
        public bool allowContinue
        {
            get
            {
                if (!_allow && _checked)
                    Debug.LogWarningFormat("You cannot continue using this script, because, you don't have declared the required attributes in the class '{0}'.", _par.GetType().Name);
                return _allow && _par != null;
            }
            set
            {
                _allow = value;
            }
        }

        private bool _allow, _checked;
        private float calledTime;

        private object _par;

        /// <summary>
        /// Gets or sets the parent class.
        /// </summary>
        /// <value>The parent class.</value>
        public object parentClass
        {
            get
            {
                return _par;
            }
            set
            {
                _par = value;

                if (!_checked)
                {
                    //Nada mas arrancar vamos a comprobar el atributo requiredattrs, y hacer lo que sea necesario.
                    //https://stackoverflow.com/questions/1168535/when-is-a-custom-attributes-constructor-run
                    //Aqui se puede observar que comprobando el tipo podemos invocar al constructor del atributo y exigirle que continue.

                    //Tengo que ver si se llama a los metodos de la clasde que hereda esto, con esto me refiero al Awake, Start, porque si, entonces tengo control sobre ellos.
                    calledTime = Time.fixedDeltaTime;
                    var attr = GetType().GetCustomAttributes(typeof(RequiredAttrs), true);
                    //var childClass = attachedObj.GetComponent(GetType()) as MonoBehaviour;

                    if (attr == null)
                    {
                        Debug.LogError("If you use 'RequiredHaviour' class you must declare a 'RequiredAttrs' attribute on the top of the desired class.");
                        return;
                    }

                    //Entonces...
                    var attrs = attr[0] as RequiredAttrs;

                    string[] nd = null;
                    if (!CheckMemebers(_par, out nd, attrs.Attributes)) //Obtener el tipo de clase que requirió de BallisticController, en este caso, ha sido ThrowableController
                        Debug.LogWarningFormat("There are some fields/props that the class '{0}' doesn't have declared (the component has been disabled to avoid major issues). See the manual.{1}", _par.GetType().Name, string.Format("{0}", nd != null ? string.Format("\nThe following are NOT defined: {0}", string.Join(", ", nd)) : ""));
                    else
                        allowContinue = true;

                    _checked = true;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredHaviour"/> class.
        /// </summary>
        public RequiredHaviour()
        {
            //Si queremos usar esto para otra cosa...
            me = this;
        }

        private bool CheckMemebers(object obj, out string[] ndef, params string[] attrs)
        {
            if (obj == null)
            {
                ndef = null;
                return false;
            }
            IEnumerable<string> def = attrs.Where(x => obj.GetField(x) != null); //It doesn't matter because property is subchecked. //&& obj.GetProp(x) != null);
            ndef = attrs.Except(def).ToArray();
            return def != null && def.Count() == attrs.Length;
        }
    }
}