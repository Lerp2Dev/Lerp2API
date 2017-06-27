using System;
using UnityEngine;

namespace UnitySerializerNG
{
    /// <summary>
    /// Class ComponentDependencies.
    /// </summary>
    public static class ComponentDependencies
    {
        // Hard-coded, because Unity doesn't seem to make abstract classes really abstract.
        // Trying to add a Collider component tells you that it cannot be done, because Collider
        // is abstract, but reading the isAbstract property of the type returns false.
        // This method is there to resolve known problems like this by hand.
        /// <summary>
        /// Resolves the type requirement.
        /// </summary>
        /// <param name="requiredType">Type of the required.</param>
        /// <returns>Type.</returns>
        public static Type ResolveTypeRequirement(Type requiredType)
        {
            if (requiredType == typeof(Collider)) return typeof(SphereCollider);
            else if (requiredType == typeof(Collider2D)) return typeof(CircleCollider2D);
            else return requiredType;
        }
    }
}