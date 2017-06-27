using UnityEngine;

namespace RadicalLibrary
{
    /// <summary>
    /// Class SmoothVector3.
    /// </summary>
    [SerializeAll]
    public class SmoothVector3
    {
        /// <summary>
        /// The ease
        /// </summary>
        public EasingType Ease = EasingType.Linear;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmoothVector3"/> class.
        /// </summary>
        public SmoothVector3()
        {
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("[SmoothVector3: IsComplete={0}, Target={1}, Current={2}, x={3}, y={4}, z={5}, Value={6}]", IsComplete, Target, Current, x, y, z, Value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SmoothVector3"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        public SmoothVector3(float x, float y, float z) : this(new Vector3(x, y, z))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SmoothVector3"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public SmoothVector3(Vector3 value)
        {
            _current = value;
            _target = value;
            _current = value;
            _start = value;
            _startTime = Time.time;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is complete.
        /// </summary>
        /// <value><c>true</c> if this instance is complete; otherwise, <c>false</c>.</value>
        public bool IsComplete
        {
            get
            {
                return ((Time.time - _startTime) / Duration) >= 1;
            }
        }

        /// <summary>
        /// The mode
        /// </summary>
        public SmoothingMode Mode = SmoothingMode.lerp;

        private Vector3 _target;

        /// <summary>
        /// Gets the target.
        /// </summary>
        /// <value>The target.</value>
        public Vector3 Target
        {
            get
            {
                return _target;
            }
        }

        private Vector3 _start;
        private Vector3 _velocity;
        private float _startTime;
        private Vector3 _current;

        /// <summary>
        /// The duration
        /// </summary>
        public float Duration = 0.1f;

        /// <summary>
        /// Gets or sets the current.
        /// </summary>
        /// <value>The current.</value>
        public Vector3 Current
        {
            get
            {
                return _current;
            }
            set
            {
                _current = value;
                _start = value;
            }
        }

        /// <summary>
        /// The lock
        /// </summary>
        public bool Lock;

        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        /// <value>The x.</value>
        public float x
        {
            get
            {
                return Current.x;
            }
            set
            {
                Value = new Vector3(value, _target.y, _target.z);
            }
        }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        /// <value>The y.</value>
        public float y
        {
            get
            {
                return Current.y;
            }
            set
            {
                Value = new Vector3(_target.x, value, _target.y);
            }
        }

        /// <summary>
        /// Gets or sets the z.
        /// </summary>
        /// <value>The z.</value>
        public float z
        {
            get
            {
                return Current.z;
            }
            set
            {
                Value = new Vector3(_target.x, _target.y, value);
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public Vector3 Value
        {
            get
            {
                if (Duration == 0)
                {
                    Duration = 0.1f;
                }
                if (_startTime == 0)
                {
                    _startTime = Time.time;
                }
                var t = Easing.EaseInOut(((double)(Time.time - _startTime) / (double)Duration), Ease, Ease);
                switch (Mode)
                {
                    case SmoothingMode.damp:
                        _current = Vector3.SmoothDamp(_current, _target, ref _velocity, Duration, float.PositiveInfinity, Time.time - _startTime);
                        _startTime = Time.time;
                        break;

                    case SmoothingMode.lerp:
                        _current = Vector3.Lerp(_start, _target, t);
                        break;

                    case SmoothingMode.slerp:
                        _current = Vector3.Slerp(_start, _target, t);
                        break;

                    case SmoothingMode.smooth:
                        _current = new Vector3(Mathf.SmoothStep(_start.x, _target.x, t), Mathf.SmoothStep(_start.y, _target.y, t),
                            Mathf.SmoothStep(_start.z, _target.z, t));
                        break;
                }
                if (Lock)
                {
                    if (_target.x == _start.x)
                    {
                        _current.x = _target.x;
                    }
                    if (_target.y == _start.y)
                    {
                        _current.y = _target.y;
                    }
                    if (_target.x == _start.z)
                    {
                        _current.z = _target.z;
                    }
                }
                return _current;
            }
            set
            {
                if (value.Equals(_target))
                    return;

                _start = Value;
                _startTime = Time.time;
                _target = value;
            }
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="SmoothVector3"/> to <see cref="Vector3"/>.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Vector3(SmoothVector3 obj)
        {
            return obj.Value;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Vector3"/> to <see cref="SmoothVector3"/>.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator SmoothVector3(Vector3 obj)
        {
            return new SmoothVector3(obj);
        }
    }

    /// <summary>
    /// Class SmoothFloat.
    /// </summary>
    [SerializeAll]
    public class SmoothFloat
    {
        /// <summary>
        /// The mode
        /// </summary>
        public SmoothingMode Mode = SmoothingMode.lerp;
        /// <summary>
        /// The ease
        /// </summary>
        public EasingType Ease = EasingType.Linear;

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("[SmoothFloat: Value={0}]", Current);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SmoothFloat"/> class.
        /// </summary>
        public SmoothFloat()
        {
        }

        private float _target;
        private float _start;
        private float _velocity;
        private float _startTime;

        /// <summary>
        /// The duration
        /// </summary>
        public float Duration = 0.5f;
        private float _current;

        /// <summary>
        /// Gets or sets the current.
        /// </summary>
        /// <value>The current.</value>
        [DoNotSerialize]
        public float Current
        {
            get
            {
                return _current;
            }
            set
            {
                _current = value;
                _target = value;
            }
        }

        /// <summary>
        /// Gets the target.
        /// </summary>
        /// <value>The target.</value>
        public float Target

        {
            get
            {
                return _target;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SmoothFloat"/> class.
        /// </summary>
        /// <param name="f">The f.</param>
        public SmoothFloat(float f)
        {
            Current = f;
            _start = f;
            _velocity = 0;
            _target = f;
            _startTime = Time.time;
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public float Value
        {
            get
            {
                if (Duration == 0)
                    Duration = 0.5f;
                if (_startTime == 0)
                {
                    _startTime = Time.time;
                }
                var t = Mathf.Clamp01(Easing.EaseInOut(Mathf.Clamp01((Time.time - _startTime) / Duration), Ease, Ease));

                switch (Mode)
                {
                    case SmoothingMode.damp:
                        _current = Mathf.SmoothDamp(Current, _target, ref _velocity, Duration, float.PositiveInfinity, Time.time - _startTime);
                        _startTime = Time.time;
                        break;

                    case SmoothingMode.lerp:
                        _current = Mathf.Lerp(_start, _target, t);
                        break;

                    case SmoothingMode.slerp:
                        _current = Mathf.Lerp(_start, _target, t);
                        break;

                    case SmoothingMode.smooth:
                        _current = Mathf.SmoothStep(_start, _target, t);
                        break;
                }

                return Current;
            }
            set
            {
                if (value.Equals(_target))
                    return;

                _start = Value;
                _startTime = Time.time;
                _target = value;
            }
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="SmoothFloat"/> to <see cref="System.Single"/>.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator float(SmoothFloat obj)
        {
            return obj.Value;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Single"/> to <see cref="SmoothFloat"/>.
        /// </summary>
        /// <param name="f">The f.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator SmoothFloat(float f)
        {
            return new SmoothFloat(f);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is complete.
        /// </summary>
        /// <value><c>true</c> if this instance is complete; otherwise, <c>false</c>.</value>
        public bool IsComplete
        {
            get
            {
                return ((Time.time - _startTime) / Duration) >= 1;
            }
        }
    }

    /// <summary>
    /// Class SmoothQuaternion.
    /// </summary>
    [SerializeAll]
    public class SmoothQuaternion
    {
        /// <summary>
        /// The mode
        /// </summary>
        public SmoothingMode Mode = SmoothingMode.slerp;
        /// <summary>
        /// The ease
        /// </summary>
        public EasingType Ease = EasingType.Linear;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmoothQuaternion"/> class.
        /// </summary>
        public SmoothQuaternion()
        {
        }

        private Quaternion _target;
        private Quaternion _start;
        private Vector3 _velocity;
        private float _startTime;

        /// <summary>
        /// The duration
        /// </summary>
        public float Duration = 0.2f;
        /// <summary>
        /// The current
        /// </summary>
        public Quaternion Current;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmoothQuaternion"/> class.
        /// </summary>
        /// <param name="q">The q.</param>
        public SmoothQuaternion(Quaternion q)
        {
            Current = q;
            _start = q;
            _target = q;
            _startTime = Time.time;
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public Quaternion Value
        {
            get
            {
                if (Duration == 0)
                {
                    Duration = 0.1f;
                }
                if (_startTime == 0)
                {
                    _startTime = Time.time;
                }
                var t = Easing.EaseInOut(((Time.time - _startTime) / Duration), Ease, Ease);

                switch (Mode)
                {
                    case SmoothingMode.damp:
                        Current = Quaternion.Euler(Vector3.SmoothDamp(Current.eulerAngles, _target.eulerAngles, ref _velocity, Duration, float.PositiveInfinity, Time.time - _startTime));
                        _startTime = Time.time;
                        break;

                    case SmoothingMode.lerp:

                        Current = Quaternion.Lerp(_start, _target, t);
                        break;

                    case SmoothingMode.slerp:
                        Current = Quaternion.Slerp(_start, _target, t);
                        break;

                    case SmoothingMode.smooth:
                        Current = Quaternion.Euler(new Vector3(Mathf.SmoothStep(_start.x, _target.x, t), Mathf.SmoothStep(_start.y, _target.y, t),
                            Mathf.SmoothStep(_start.z, _target.z, t)));
                        break;
                }

                return Current;
            }
            set
            {
                if (value.Equals(_target))
                {
                    return;
                }
                _start = Value;
                _startTime = Time.time;
                _target = value;
            }
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="SmoothQuaternion"/> to <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Quaternion(SmoothQuaternion obj)
        {
            return obj.Value;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Quaternion"/> to <see cref="SmoothQuaternion"/>.
        /// </summary>
        /// <param name="q">The q.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator SmoothQuaternion(Quaternion q)
        {
            return new SmoothQuaternion(q);
        }

        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        /// <value>The x.</value>
        public float x
        {
            get
            {
                return Current.x;
            }
            set
            {
                Value = new Quaternion(value, _target.y, _target.z, _target.w);
            }
        }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        /// <value>The y.</value>
        public float y
        {
            get
            {
                return Current.y;
            }
            set
            {
                Value = new Quaternion(_target.x, value, _target.y, _target.w);
            }
        }

        /// <summary>
        /// Gets or sets the z.
        /// </summary>
        /// <value>The z.</value>
        public float z
        {
            get
            {
                return Current.z;
            }
            set
            {
                Value = new Quaternion(_target.x, _target.y, value, _target.w);
            }
        }

        /// <summary>
        /// Gets or sets the w.
        /// </summary>
        /// <value>The w.</value>
        public float w
        {
            get
            {
                return Current.w;
            }
            set
            {
                Value = new Quaternion(_target.x, _target.y, _target.z, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is complete.
        /// </summary>
        /// <value><c>true</c> if this instance is complete; otherwise, <c>false</c>.</value>
        public bool IsComplete
        {
            get
            {
                return ((Time.time - _startTime) / Duration) >= 1;
            }
        }
    }
}