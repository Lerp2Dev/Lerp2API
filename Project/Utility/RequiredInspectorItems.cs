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
        public KeyCode Positive;

        public ButtonData(string n, KeyCode p)
            : base(n)
        {
            Positive = p;
        }
    }

    [Serializable]
    public class AxisData : ButtonData
    {
        public KeyCode Positive,
                       Negative;

        public float Sensibility;
        public bool Invert;

        public AxisData(string name, KeyCode p, KeyCode n, float s, bool inv)
            : base(name, p)
        {
            Positive = p;
            Negative = n;
            Sensibility = s;
            Invert = inv;
        }
    }
}