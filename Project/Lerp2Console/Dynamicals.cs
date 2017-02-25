using System;
using System.Diagnostics.Contracts;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Lerp2Console
{
    /// <summary>
    /// A wrapper around a "ref" or "out" argument invoked dynamically.
    /// </summary>
    public sealed class RefOutArg
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RefOutArg"/> class.
        /// </summary>
        private RefOutArg()
        {
        }

        /// <summary>
        /// Gets or sets the wrapped value as an object.
        /// </summary>
        public object ValueAsObject { get; set; }

        /// <summary>
        /// Gets or sets the wrapped value.
        /// </summary>
        public dynamic Value
        {
            get
            {
                return this.ValueAsObject;
            }

            set
            {
                this.ValueAsObject = value;
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="RefOutArg"/> class wrapping the default value of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of value to wrap.</typeparam>
        /// <returns>A new instance of the <see cref="RefOutArg"/> class wrapping the default value of <typeparamref name="T"/>.</returns>
        public static RefOutArg Create<T>()
        {
            return new RefOutArg { ValueAsObject = default(T) };
        }

        /// <summary>
        /// Creates a new instance of the <see cref="RefOutArg"/> class wrapping the specified value.
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        /// <returns>A new instance of the <see cref="RefOutArg"/> class wrapping the specified value.</returns>
        public static RefOutArg Create(object value)
        {
            return new RefOutArg { ValueAsObject = value };
        }
    }

    /// <summary>
    /// A dynamic object that allows access to a type's static members, resolved dynamically at runtime.
    /// </summary>
    public sealed class DynamicStaticTypeMembers : DynamicObject
    {
        /// <summary>
        /// The underlying type.
        /// </summary>
        private readonly Type type;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicStaticTypeMembers"/> class wrapping the specified type.
        /// </summary>
        /// <param name="type">The underlying type to wrap.</param>
        private DynamicStaticTypeMembers(Type type)
        {
            this.type = type;
        }

        /// <summary>
        /// Gets a value for a static property defined by the wrapped type.
        /// </summary>
        /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
        /// <param name="result">The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result"/>.</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)
        /// </returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var prop = type.GetProperty(binder.Name, BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public);
            if (prop == null)
            {
                result = null;
                return false;
            }

            result = prop.GetValue(null, null);
            return true;
        }

        /// <summary>
        /// Sets a value for a static property defined by the wrapped type.
        /// </summary>
        /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member to which the value is being assigned. For example, for the statement sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
        /// <param name="value">The value to set to the member. For example, for sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, the <paramref name="value"/> is "Test".</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)
        /// </returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var prop = this.type.GetProperty(binder.Name, BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public);
            if (prop == null)
            {
                return false;
            }

            prop.SetValue(null, value, null);
            return true;
        }

        /// <summary>
        /// Calls a static method defined by the wrapped type.
        /// </summary>
        /// <param name="binder">Provides information about the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the statement sampleObject.SampleMethod(100), where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Name returns "SampleMethod". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
        /// <param name="args">The arguments that are passed to the object member during the invoke operation. For example, for the statement sampleObject.SampleMethod(100), where sampleObject is derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, <c>args[0]</c> is equal to 100.</param>
        /// <param name="result">The result of the member invocation.</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)
        /// </returns>
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            // Convert any RefOutArg arguments into ref/out arguments
            var refArguments = new RefOutArg[args.Length];
            for (int i = 0; i != args.Length; ++i)
            {
                refArguments[i] = args[i] as RefOutArg;
                if (refArguments[i] != null)
                    args[i] = refArguments[i].ValueAsObject;
            }

            // Resolve the method
            const BindingFlags flags = BindingFlags.InvokeMethod | BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public;
            object state;
            MethodBase method;

            var methods = type.GetMethods(flags).Where(x => x.Name == binder.Name);

            if (methods == null)
                throw new Exception(string.Format("Wrong typed {0} method!", binder.Name));

            method = Type.DefaultBinder.BindToMethod(flags, methods.ToArray(), ref args, null, null, null, out state);

            // Ensure that all ref/out arguments were properly wrapped
            if (method.GetParameters().Count(x => x.ParameterType.IsByRef) != refArguments.Count(x => x != null))
                throw new ArgumentException("ref/out parameters need a RefOutArg wrapper when invoking " + type.Name + "." + binder.Name + ".");

            // Invoke the method, allowing exceptions to propogate
            try
            {
                result = method.Invoke(null, args);
            }
            finally
            {
                if (state != null)
                    Type.DefaultBinder.ReorderArgumentArray(ref args, state);

                // Convert any ref/out arguments into RefOutArg results
                for (int i = 0; i != args.Length; ++i)
                    if (refArguments[i] != null)
                        refArguments[i].ValueAsObject = args[i];
            }

            return true;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DynamicStaticTypeMembers"/> class wrapping the specified type.
        /// </summary>
        /// <param name="type">The underlying type to wrap. May not be <c>null</c>.</param>
        /// <returns>An instance of <see cref="DynamicStaticTypeMembers"/>, as a dynamic type.</returns>
        public static dynamic Create(Type type)
        {
            //Contract.Requires<ArgumentNullException>(type != null);
            if(type == null) throw new ArgumentNullException();
            return new DynamicStaticTypeMembers(type);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DynamicStaticTypeMembers"/> class wrapping the specified type.
        /// </summary>
        /// <typeparam name="T">The underlying type to wrap.</typeparam>
        /// <returns>An instance of <see cref="DynamicStaticTypeMembers"/>, as a dynamic type.</returns>
        public static dynamic Create<T>()
        {
            return new DynamicStaticTypeMembers(typeof(T));
        }
    }
}