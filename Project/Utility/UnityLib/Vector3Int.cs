using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lerp2API.Utility.UnityLib
{
    /// <summary>
    /// Class Vector3Int. This class cannot be inherited.
    /// </summary>
    public sealed class Vector3Int
    {
        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        /// <value>The x.</value>
        public int x { get; set; }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        /// <value>The y.</value>
        public int y { get; set; }

        /// <summary>
        /// Gets or sets the z.
        /// </summary>
        /// <value>The z.</value>
        public int z { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3Int"/> class.
        /// </summary>
        public Vector3Int()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3Int"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        public Vector3Int(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}