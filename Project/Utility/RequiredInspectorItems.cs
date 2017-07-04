using System;
using UnityEngine;

namespace Lerp2API.Utility
{
    [Serializable]
    public class NamedData
    {
        public string Name;

        public NamedData(string n)
        {
            Name = n;
        }
    }

    public class LayerData : NamedData
    {
        public int Layer;

        public LayerData(string n, int l)
            : base(n)
        {
            Layer = l;
        }
    }

    [Serializable]
    public class ButtonData : NamedData
    {
        public KeyCode PrimaryKey,
                       SecondKey;

        public ButtonData(string n, KeyCode pk, KeyCode sk)
            : base(n)
        {
            PrimaryKey = pk;
            SecondKey = sk;
        }
    }

    [Serializable]
    public class AxisData : NamedData
    {
        public KeyCode Positive,
                       Negative,
                       AltPositive,
                       AltNegative;

        public float Sensibility,
                     Gravity;

        public bool Invert;

        public AxisData(string name, KeyCode p, KeyCode n, float s, bool inv)
            : base(name)
        {
            Positive = p;
            Negative = n;
            Sensibility = s;
            Invert = inv;
        }

        public AxisData(string name, KeyCode p, KeyCode n, KeyCode ap, KeyCode an, float s, float g, bool inv)
            : base(name)
        {
            Positive = p;
            Negative = n;
            AltPositive = ap;
            AltNegative = an;
            Sensibility = s;
            Gravity = g;
            Invert = inv;
        }
    }
}