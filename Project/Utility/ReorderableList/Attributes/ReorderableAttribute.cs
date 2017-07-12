using UnityEngine;

namespace Malee
{
    /// <summary>
    /// Class ReorderableAttribute.
    /// </summary>
    /// <seealso cref="UnityEngine.PropertyAttribute" />
    public class ReorderableAttribute : PropertyAttribute
    {
        /// <summary>
        /// The add
        /// </summary>
        public bool add;

        /// <summary>
        /// The remove
        /// </summary>
        public bool remove;

        /// <summary>
        /// The draggable
        /// </summary>
        public bool draggable;

        /// <summary>
        /// The element name property
        /// </summary>
        public string elementNameProperty;

        /// <summary>
        /// The element name override
        /// </summary>
        public string elementNameOverride;

        /// <summary>
        /// The element icon path
        /// </summary>
        public string elementIconPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReorderableAttribute"/> class.
        /// </summary>
        public ReorderableAttribute()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReorderableAttribute"/> class.
        /// </summary>
        /// <param name="elementNameProperty">The element name property.</param>
        public ReorderableAttribute(string elementNameProperty)
            : this(true, true, true, elementNameProperty, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReorderableAttribute"/> class.
        /// </summary>
        /// <param name="elementNameProperty">The element name property.</param>
        /// <param name="elementIconPath">The element icon path.</param>
        public ReorderableAttribute(string elementNameProperty, string elementIconPath)
            : this(true, true, true, elementNameProperty, null, elementIconPath)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReorderableAttribute"/> class.
        /// </summary>
        /// <param name="elementNameProperty">The element name property.</param>
        /// <param name="elementNameOverride">The element name override.</param>
        /// <param name="elementIconPath">The element icon path.</param>
        public ReorderableAttribute(string elementNameProperty, string elementNameOverride, string elementIconPath)
            : this(true, true, true, elementNameProperty, elementNameOverride, elementIconPath)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReorderableAttribute"/> class.
        /// </summary>
        /// <param name="add">if set to <c>true</c> [add].</param>
        /// <param name="remove">if set to <c>true</c> [remove].</param>
        /// <param name="draggable">if set to <c>true</c> [draggable].</param>
        /// <param name="elementNameProperty">The element name property.</param>
        /// <param name="elementIconPath">The element icon path.</param>
        public ReorderableAttribute(bool add, bool remove, bool draggable, string elementNameProperty = null, string elementIconPath = null)
            : this(add, remove, draggable, elementNameProperty, null, elementIconPath)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReorderableAttribute"/> class.
        /// </summary>
        /// <param name="add">if set to <c>true</c> [add].</param>
        /// <param name="remove">if set to <c>true</c> [remove].</param>
        /// <param name="draggable">if set to <c>true</c> [draggable].</param>
        /// <param name="elementNameProperty">The element name property.</param>
        /// <param name="elementNameOverride">The element name override.</param>
        /// <param name="elementIconPath">The element icon path.</param>
        public ReorderableAttribute(bool add, bool remove, bool draggable, string elementNameProperty = null, string elementNameOverride = null, string elementIconPath = null)
        {
            this.add = add;
            this.remove = remove;
            this.draggable = draggable;
            this.elementNameProperty = elementNameProperty;
            this.elementNameOverride = elementNameOverride;
            this.elementIconPath = elementIconPath;
        }
    }
}