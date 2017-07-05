using System;
using UnityEngine;

namespace Lerp2API.Utility
{
    /// <summary>
    /// Class NamedData.
    /// </summary>
    [Serializable]
    public class NamedData
    {
        /// <summary>
        /// The name
        /// </summary>
        public string Name;

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedData"/> class.
        /// </summary>
        /// <param name="n">The n.</param>
        public NamedData(string n)
        {
            Name = n;
        }
    }

    /// <summary>
    /// Class LayerData.
    /// </summary>
    /// <seealso cref="Lerp2API.Utility.NamedData" />
    [Serializable]
    public class LayerData : NamedData
    {
        /// <summary>
        /// The layer
        /// </summary>
        public int Layer;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayerData"/> class.
        /// </summary>
        /// <param name="n">The n.</param>
        /// <param name="l">The l.</param>
        public LayerData(string n, int l)
            : base(n)
        {
            Layer = l;
        }
    }

    /*/// <summary>
    /// Class ButtonData.
    /// </summary>
    /// <seealso cref="Lerp2API.Utility.NamedData" />
    [Serializable]
    public class ButtonData : NamedData
    {
        /// <summary>
        /// The primary key
        /// </summary>
        public KeyCode PrimaryKey,
                       /// <summary>
                       /// The second key
                       /// </summary>
                       SecondKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonData"/> class.
        /// </summary>
        /// <param name="n">The n.</param>
        /// <param name="pk">The pk.</param>
        /// <param name="sk">The sk.</param>
        public ButtonData(string n, KeyCode pk, KeyCode sk)
            : base(n)
        {
            PrimaryKey = pk;
            SecondKey = sk;
        }
    }

    /// <summary>
    /// Class AxisData.
    /// </summary>
    /// <seealso cref="Lerp2API.Utility.NamedData" />
    [Serializable]
    public class AxisData : NamedData
    {
        /// <summary>
        /// The positive
        /// </summary>
        public KeyCode Positive,
                       /// <summary>
                       /// The negative
                       /// </summary>
                       Negative,
                       /// <summary>
                       /// The alt positive
                       /// </summary>
                       AltPositive,
                       /// <summary>
                       /// The alt negative
                       /// </summary>
                       AltNegative;

        /// <summary>
        /// The sensibility
        /// </summary>
        public float Sensibility,
                     /// <summary>
                     /// The gravity
                     /// </summary>
                     Gravity;

        /// <summary>
        /// The invert
        /// </summary>
        public bool Invert;

        /// <summary>
        /// Initializes a new instance of the <see cref="AxisData"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="p">The p.</param>
        /// <param name="n">The n.</param>
        /// <param name="s">The s.</param>
        /// <param name="inv">if set to <c>true</c> [inv].</param>
        public AxisData(string name, KeyCode p, KeyCode n, float s, bool inv)
            : base(name)
        {
            Positive = p;
            Negative = n;
            Sensibility = s;
            Invert = inv;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AxisData"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="p">The p.</param>
        /// <param name="n">The n.</param>
        /// <param name="ap">The ap.</param>
        /// <param name="an">An.</param>
        /// <param name="s">The s.</param>
        /// <param name="g">The g.</param>
        /// <param name="inv">if set to <c>true</c> [inv].</param>
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
    }*/
}