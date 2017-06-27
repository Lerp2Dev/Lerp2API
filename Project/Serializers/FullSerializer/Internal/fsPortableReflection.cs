#if !UNITY_EDITOR && UNITY_METRO && !ENABLE_IL2CPP
#define USE_TYPEINFO
#if !UNITY_WINRT_10_0
#define USE_TYPEINFO_EXTENSIONS
#endif
#endif

using System;
using System.Collections.Generic;
using System.Reflection;

#if USE_TYPEINFO
namespace System {
/// <summary>
/// Class AssemblyExtensions.
/// </summary>
    public static class AssemblyExtensions {
#if USE_TYPEINFO_EXTENSIONS
        /// <summary>
        /// Gets the types.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>Type[].</returns>
        public static Type[] GetTypes(this Assembly assembly) {
            TypeInfo[] infos = assembly.DefinedTypes.ToArray();
            Type[] types = new Type[infos.Length];
            for (int i = 0; i < infos.Length; ++i) {
                types[i] = infos[i].AsType();
            }
            return types;
        }
#endif

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="name">The name.</param>
        /// <param name="throwOnError">if set to <c>true</c> [throw on error].</param>
        /// <returns>Type.</returns>
        /// <exception cref="Exception">Type " + name + " was not found</exception>
        public static Type GetType(this Assembly assembly, string name, bool throwOnError) {
            var types = assembly.GetTypes();
            for (int i = 0; i < types.Length; ++i) {
                if (types[i].Name == name) {
                    return types[i];
                }
            }

            if (throwOnError) throw new Exception("Type " + name + " was not found");
            return null;
        }
    }
}
#endif

namespace FullSerializer.Internal
{
    /// <summary>
    /// This wraps reflection types so that it is portable across different Unity runtimes.
    /// </summary>
    public static class fsPortableReflection
    {
        /// <summary>
        /// The empty types
        /// </summary>
        public static Type[] EmptyTypes = { };

        #region Attribute Queries

#if USE_TYPEINFO
        /// <summary>
        /// Gets the attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the t attribute.</typeparam>
        /// <param name="type">The type.</param>
        /// <returns>TAttribute.</returns>
        public static TAttribute GetAttribute<TAttribute>(Type type)
            where TAttribute : Attribute {
            return GetAttribute<TAttribute>(type.GetTypeInfo());
        }

        /// <summary>
        /// Gets the attribute.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="attributeType">Type of the attribute.</param>
        /// <returns>Attribute.</returns>
        public static Attribute GetAttribute(Type type, Type attributeType) {
            return GetAttribute(type.GetTypeInfo(), attributeType, /*shouldCache:*/false);
        }

        /// <summary>
        /// Determines whether the specified type has attribute.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="attributeType">Type of the attribute.</param>
        /// <returns><c>true</c> if the specified type has attribute; otherwise, <c>false</c>.</returns>
        public static bool HasAttribute(Type type, Type attributeType) {
            return GetAttribute(type, attributeType) != null;
        }
#endif

        /// <summary>
        /// Returns true if the given attribute is defined on the given element.
        /// </summary>
        public static bool HasAttribute<TAttribute>(MemberInfo element)
        {
            return HasAttribute(element, typeof(TAttribute));
        }

        /// <summary>
        /// Returns true if the given attribute is defined on the given element.
        /// </summary>
        public static bool HasAttribute<TAttribute>(MemberInfo element, bool shouldCache)
        {
            return HasAttribute(element, typeof(TAttribute), shouldCache);
        }

        /// <summary>
        /// Returns true if the given attribute is defined on the given element.
        /// </summary>
        public static bool HasAttribute(MemberInfo element, Type attributeType)
        {
            return HasAttribute(element, attributeType, true);
        }

        /// <summary>
        /// Returns true if the given attribute is defined on the given element.
        /// </summary>
        public static bool HasAttribute(MemberInfo element, Type attributeType, bool shouldCache)
        {
            return element.IsDefined(attributeType, true);
        }

        /// <summary>
        /// Fetches the given attribute from the given MemberInfo. This method applies caching
        /// and is allocation free (after caching has been performed).
        /// </summary>
        /// <param name="element">The MemberInfo the get the attribute from.</param>
        /// <param name="attributeType">The type of attribute to fetch.</param>
        /// <returns>The attribute or null.</returns>
        public static Attribute GetAttribute(MemberInfo element, Type attributeType, bool shouldCache)
        {
            var query = new AttributeQuery
            {
                MemberInfo = element,
                AttributeType = attributeType
            };

            Attribute attribute;
            if (_cachedAttributeQueries.TryGetValue(query, out attribute) == false)
            {
                var attributes = element.GetCustomAttributes(attributeType, /*inherit:*/ true);
                if (attributes.Length > 0)
                    attribute = (Attribute)attributes[0];
                if (shouldCache)
                    _cachedAttributeQueries[query] = attribute;
            }

            return attribute;
        }

        /// <summary>
        /// Fetches the given attribute from the given MemberInfo.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute to fetch.</typeparam>
        /// <param name="element">The MemberInfo to get the attribute from.</param>
        /// <param name="shouldCache">Should this computation be cached? If this is the only time it will ever be done, don't bother caching.</param>
        /// <returns>The attribute or null.</returns>
        public static TAttribute GetAttribute<TAttribute>(MemberInfo element, bool shouldCache)
            where TAttribute : Attribute
        {
            return (TAttribute)GetAttribute(element, typeof(TAttribute), shouldCache);
        }

        /// <summary>
        /// Gets the attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the t attribute.</typeparam>
        /// <param name="element">The element.</param>
        /// <returns>TAttribute.</returns>
        public static TAttribute GetAttribute<TAttribute>(MemberInfo element)
            where TAttribute : Attribute
        {
            return GetAttribute<TAttribute>(element, /*shouldCache:*/true);
        }

        private struct AttributeQuery
        {
            /// <summary>
            /// The member information
            /// </summary>
            public MemberInfo MemberInfo;
            /// <summary>
            /// The attribute type
            /// </summary>
            public Type AttributeType;
        }

        private static IDictionary<AttributeQuery, Attribute> _cachedAttributeQueries =
            new Dictionary<AttributeQuery, Attribute>(new AttributeQueryComparator());

        private class AttributeQueryComparator : IEqualityComparer<AttributeQuery>
        {
            /// <summary>
            /// Equalses the specified x.
            /// </summary>
            /// <param name="x">The x.</param>
            /// <param name="y">The y.</param>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
            public bool Equals(AttributeQuery x, AttributeQuery y)
            {
                return
                    x.MemberInfo == y.MemberInfo &&
                    x.AttributeType == y.AttributeType;
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <param name="obj">The object.</param>
            /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
            public int GetHashCode(AttributeQuery obj)
            {
                return
                    obj.MemberInfo.GetHashCode() +
                    (17 * obj.AttributeType.GetHashCode());
            }
        }

        #endregion Attribute Queries

#if !USE_TYPEINFO

        private static BindingFlags DeclaredFlags =
            BindingFlags.NonPublic |
            BindingFlags.Public |
            BindingFlags.Instance |
            BindingFlags.Static |
            BindingFlags.DeclaredOnly;

#endif

        /// <summary>
        /// Gets the declared property.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>PropertyInfo.</returns>
        public static PropertyInfo GetDeclaredProperty(this Type type, string propertyName)
        {
            var props = GetDeclaredProperties(type);

            for (int i = 0; i < props.Length; ++i)
            {
                if (props[i].Name == propertyName)
                {
                    return props[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the declared method.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <returns>MethodInfo.</returns>
        public static MethodInfo GetDeclaredMethod(this Type type, string methodName)
        {
            var methods = GetDeclaredMethods(type);

            for (int i = 0; i < methods.Length; ++i)
            {
                if (methods[i].Name == methodName)
                {
                    return methods[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the declared constructor.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>ConstructorInfo.</returns>
        public static ConstructorInfo GetDeclaredConstructor(this Type type, Type[] parameters)
        {
            var ctors = GetDeclaredConstructors(type);

            for (int i = 0; i < ctors.Length; ++i)
            {
                var ctor = ctors[i];
                var ctorParams = ctor.GetParameters();

                if (parameters.Length != ctorParams.Length) continue;

                for (int j = 0; j < ctorParams.Length; ++j)
                {
                    // require an exact match
                    if (ctorParams[j].ParameterType != parameters[j]) continue;
                }

                return ctor;
            }

            return null;
        }

        /// <summary>
        /// Gets the declared constructors.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>ConstructorInfo[].</returns>
        public static ConstructorInfo[] GetDeclaredConstructors(this Type type)
        {
#if USE_TYPEINFO
            return type.GetTypeInfo().DeclaredConstructors.ToArray();
#else
            return type.GetConstructors(DeclaredFlags);
#endif
        }

        /// <summary>
        /// Gets the flattened member.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <returns>MemberInfo[].</returns>
        public static MemberInfo[] GetFlattenedMember(this Type type, string memberName)
        {
            var result = new List<MemberInfo>();

            while (type != null)
            {
                var members = GetDeclaredMembers(type);

                for (int i = 0; i < members.Length; ++i)
                {
                    if (members[i].Name == memberName)
                    {
                        result.Add(members[i]);
                    }
                }

                type = type.Resolve().BaseType;
            }

            return result.ToArray();
        }

        /// <summary>
        /// Gets the flattened method.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <returns>MethodInfo.</returns>
        public static MethodInfo GetFlattenedMethod(this Type type, string methodName)
        {
            while (type != null)
            {
                var methods = GetDeclaredMethods(type);

                for (int i = 0; i < methods.Length; ++i)
                {
                    if (methods[i].Name == methodName)
                    {
                        return methods[i];
                    }
                }

                type = type.Resolve().BaseType;
            }

            return null;
        }

        /// <summary>
        /// Gets the flattened methods.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <returns>IEnumerable&lt;MethodInfo&gt;.</returns>
        public static IEnumerable<MethodInfo> GetFlattenedMethods(this Type type, string methodName)
        {
            while (type != null)
            {
                var methods = GetDeclaredMethods(type);

                for (int i = 0; i < methods.Length; ++i)
                {
                    if (methods[i].Name == methodName)
                    {
                        yield return methods[i];
                    }
                }

                type = type.Resolve().BaseType;
            }
        }

        /// <summary>
        /// Gets the flattened property.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>PropertyInfo.</returns>
        public static PropertyInfo GetFlattenedProperty(this Type type, string propertyName)
        {
            while (type != null)
            {
                var properties = GetDeclaredProperties(type);

                for (int i = 0; i < properties.Length; ++i)
                {
                    if (properties[i].Name == propertyName)
                    {
                        return properties[i];
                    }
                }

                type = type.Resolve().BaseType;
            }

            return null;
        }

        /// <summary>
        /// Gets the declared member.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <returns>MemberInfo.</returns>
        public static MemberInfo GetDeclaredMember(this Type type, string memberName)
        {
            var members = GetDeclaredMembers(type);

            for (int i = 0; i < members.Length; ++i)
            {
                if (members[i].Name == memberName)
                {
                    return members[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the declared methods.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>MethodInfo[].</returns>
        public static MethodInfo[] GetDeclaredMethods(this Type type)
        {
#if USE_TYPEINFO
            return type.GetTypeInfo().DeclaredMethods.ToArray();
#else
            return type.GetMethods(DeclaredFlags);
#endif
        }

        /// <summary>
        /// Gets the declared properties.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>PropertyInfo[].</returns>
        public static PropertyInfo[] GetDeclaredProperties(this Type type)
        {
#if USE_TYPEINFO
            return type.GetTypeInfo().DeclaredProperties.ToArray();
#else
            return type.GetProperties(DeclaredFlags);
#endif
        }

        /// <summary>
        /// Gets the declared fields.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>FieldInfo[].</returns>
        public static FieldInfo[] GetDeclaredFields(this Type type)
        {
#if USE_TYPEINFO
            return type.GetTypeInfo().DeclaredFields.ToArray();
#else
            return type.GetFields(DeclaredFlags);
#endif
        }

        /// <summary>
        /// Gets the declared members.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>MemberInfo[].</returns>
        public static MemberInfo[] GetDeclaredMembers(this Type type)
        {
#if USE_TYPEINFO
            return type.GetTypeInfo().DeclaredMembers.ToArray();
#else
            return type.GetMembers(DeclaredFlags);
#endif
        }

        /// <summary>
        /// Ases the member information.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>MemberInfo.</returns>
        public static MemberInfo AsMemberInfo(Type type)
        {
#if USE_TYPEINFO
            return type.GetTypeInfo();
#else
            return type;
#endif
        }

        /// <summary>
        /// Determines whether the specified member is type.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns><c>true</c> if the specified member is type; otherwise, <c>false</c>.</returns>
        public static bool IsType(MemberInfo member)
        {
#if USE_TYPEINFO
            return member is TypeInfo;
#else
            return member is Type;
#endif
        }

        /// <summary>
        /// Ases the type.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns>Type.</returns>
        public static Type AsType(MemberInfo member)
        {
#if USE_TYPEINFO
            return ((TypeInfo)member).AsType();
#else
            return (Type)member;
#endif
        }

#if USE_TYPEINFO
        /// <summary>
        /// Resolves the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>TypeInfo.</returns>
        public static TypeInfo Resolve(this Type type) {
            return type.GetTypeInfo();
        }
#else

        public static Type Resolve(this Type type)
        {
            return type;
        }

#endif

        #region Extensions

#if USE_TYPEINFO_EXTENSIONS
        /// <summary>
        /// Determines whether [is assignable from] [the specified child].
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="child">The child.</param>
        /// <returns><c>true</c> if [is assignable from] [the specified child]; otherwise, <c>false</c>.</returns>
        public static bool IsAssignableFrom(this Type parent, Type child) {
            return parent.GetTypeInfo().IsAssignableFrom(child.GetTypeInfo());
        }

        /// <summary>
        /// Gets the type of the element.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Type.</returns>
        public static Type GetElementType(this Type type) {
            return type.GetTypeInfo().GetElementType();
        }

        /// <summary>
        /// Gets the set method.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <param name="nonPublic">if set to <c>true</c> [non public].</param>
        /// <returns>MethodInfo.</returns>
        public static MethodInfo GetSetMethod(this PropertyInfo member, bool nonPublic = false) {
            // only public requested but the set method is not public
            if (nonPublic == false && member.SetMethod != null && member.SetMethod.IsPublic == false) return null;

            return member.SetMethod;
        }

        /// <summary>
        /// Gets the get method.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <param name="nonPublic">if set to <c>true</c> [non public].</param>
        /// <returns>MethodInfo.</returns>
        public static MethodInfo GetGetMethod(this PropertyInfo member, bool nonPublic = false) {
            // only public requested but the set method is not public
            if (nonPublic == false && member.GetMethod != null && member.GetMethod.IsPublic == false) return null;

            return member.GetMethod;
        }

        /// <summary>
        /// Gets the base definition.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>MethodInfo.</returns>
        public static MethodInfo GetBaseDefinition(this MethodInfo method) {
            return method.GetRuntimeBaseDefinition();
        }

        /// <summary>
        /// Gets the interfaces.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Type[].</returns>
        public static Type[] GetInterfaces(this Type type) {
            return type.GetTypeInfo().ImplementedInterfaces.ToArray();
        }

        /// <summary>
        /// Gets the generic arguments.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Type[].</returns>
        public static Type[] GetGenericArguments(this Type type) {
            return type.GetTypeInfo().GenericTypeArguments.ToArray();
        }
#endif

        #endregion Extensions
    }
}