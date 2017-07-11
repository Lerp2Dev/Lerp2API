using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lerp2API.Utility.UnityLib
{
    public sealed class Vector3Int
    {
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }

        public Vector3Int()
        {
        }

        public Vector3Int(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}