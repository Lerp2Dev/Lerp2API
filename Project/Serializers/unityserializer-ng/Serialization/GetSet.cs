using System;
using System.Reflection;

namespace Serialization
{
    /// <summary>
    /// Class GetSet.
    /// </summary>
    public abstract class GetSet
    {
        /// <summary>
        /// The priority
        /// </summary>
        public int Priority = 100;
        /// <summary>
        /// The information
        /// </summary>
        public PropertyInfo Info;
        /// <summary>
        /// The name
        /// </summary>
        public string Name;
        /// <summary>
        /// The field information
        /// </summary>
        public FieldInfo FieldInfo;
        /// <summary>
        /// The vanilla
        /// </summary>
        public object Vanilla;
        /// <summary>
        /// The collection type
        /// </summary>
        public bool CollectionType;
        /// <summary>
        /// The get
        /// </summary>
        public Func<object, object> Get;
        /// <summary>
        /// The set
        /// </summary>
        public Action<object, object> Set;
        /// <summary>
        /// The is static
        /// </summary>
        public bool IsStatic;

        /// <summary>
        /// Gets the member information.
        /// </summary>
        /// <value>The member information.</value>
        public MemberInfo MemberInfo
        {
            get
            {
                return Info != null ? (MemberInfo)Info : (MemberInfo)FieldInfo;
            }
        }
    }
}