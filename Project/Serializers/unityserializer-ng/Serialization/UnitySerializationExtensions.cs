using Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

//using TreeEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Debug = Lerp2API._Debug.Debug;

/// <summary>
/// Class SerializeVector2.
/// </summary>
[Serializer(typeof(Vector2))]
public class SerializeVector2 : SerializerExtensionBase<Vector2>
{
    /// <summary>
    /// Saves the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public override IEnumerable<object> Save(Vector2 target)
    {
        return new object[] { target.x, target.y };
    }

    /// <summary>
    /// Loads the specified data.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="instance">The instance.</param>
    /// <returns>System.Object.</returns>
    public override object Load(object[] data, object instance)
    {
        Debug.LogFormat("Vector3: {0}, {1}, {2}", data[0], data[1], data[2]);
        return new Vector2((float)data[0], (float)data[1]);
    }
}

/// <summary>
/// Class SerializeVector3.
/// </summary>
[Serializer(typeof(Vector3))]
public class SerializeVector3 : SerializerExtensionBase<Vector3>
{
    /// <summary>
    /// Saves the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public override IEnumerable<object> Save(Vector3 target)
    {
        return new object[] { target.x, target.y, target.z };
    }

    /// <summary>
    /// Loads the specified data.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="instance">The instance.</param>
    /// <returns>System.Object.</returns>
    public override object Load(object[] data, object instance)
    {
        Debug.LogFormat("Vector3: {0}, {1}, {2}", data[0], data[1], data[2]);
        return new UnitySerializer.DeferredSetter(d =>
        {
            if (!float.IsNaN((float)data[0]) && !float.IsNaN((float)data[1]) && !float.IsNaN((float)data[2]))
            {
                return new Vector3((float)data[0], (float)data[1], (float)data[2]);
            }
            else
            {
                return Vector3.zero;
            }
        }
        );
    }
}

/// <summary>
/// Class SerializeVector4.
/// </summary>
[Serializer(typeof(Vector4))]
public class SerializeVector4 : SerializerExtensionBase<Vector4>
{
    /// <summary>
    /// Saves the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public override IEnumerable<object> Save(Vector4 target)
    {
        return new object[] { target.x, target.y, target.z, target.w };
    }

    /// <summary>
    /// Loads the specified data.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="instance">The instance.</param>
    /// <returns>System.Object.</returns>
    public override object Load(object[] data, object instance)
    {
        Debug.LogFormat("Vector3: {0}, {1}, {2}", data[0], data[1], data[2]);
        if (!float.IsNaN((float)data[0]) && !float.IsNaN((float)data[1]) && !float.IsNaN((float)data[2]) && !float.IsNaN((float)data[3]))
        {
            return new Vector4((float)data[0], (float)data[1], (float)data[2], (float)data[3]);
        }
        else
        {
            return Vector4.zero;
        }
    }
}

/// <summary>
/// Class SerializeQuaternion.
/// </summary>
[Serializer(typeof(Quaternion))]
public class SerializeQuaternion : SerializerExtensionBase<Quaternion>
{
    /// <summary>
    /// Saves the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public override IEnumerable<object> Save(Quaternion target)
    {
        return new object[] { target.x, target.y, target.z, target.w };
    }

    /// <summary>
    /// Loads the specified data.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="instance">The instance.</param>
    /// <returns>System.Object.</returns>
    public override object Load(object[] data, object instance)
    {
        return new UnitySerializer.DeferredSetter(d => new Quaternion((float)data[0], (float)data[1], (float)data[2], (float)data[3]));
    }
}

/// <summary>
/// Class SerializeColor.
/// </summary>
[Serializer(typeof(Color))]
public class SerializeColor : SerializerExtensionBase<Color>
{
    /// <summary>
    /// Saves the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public override IEnumerable<object> Save(Color target)
    {
        return new object[] { target.r, target.g, target.b, target.a };
    }

    /// <summary>
    /// Loads the specified data.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="instance">The instance.</param>
    /// <returns>System.Object.</returns>
    public override object Load(object[] data, object instance)
    {
        Debug.LogFormat("Vector3: {0}, {1}, {2}", data[0], data[1], data[2]);
        return new Color((float)data[0], (float)data[1], (float)data[2], (float)data[3]);
    }
}

/// <summary>
/// Class SerializeAnimationState.
/// </summary>
[Serializer(typeof(AnimationState))]
public class SerializeAnimationState : SerializerExtensionBase<AnimationState>
{
    /// <summary>
    /// Saves the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public override IEnumerable<object> Save(AnimationState target)
    {
        return new object[] { target.name };
    }

    /// <summary>
    /// Loads the specified data.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="instance">The instance.</param>
    /// <returns>System.Object.</returns>
    public override object Load(object[] data, object instance)
    {
        var uo = UnitySerializer.DeserializingObject;
        return new UnitySerializer.DeferredSetter(d =>
        {
            var p = uo.GetType().GetProperty("animation").GetGetMethod();
            if (p != null)
            {
                var animation = (Animation)p.Invoke(uo, null);
                if (animation != null)
                {
                    return animation[(string)data[0]];
                }
            }
            return null;
        });
    }
}

/// <summary>
/// Class SerializeWaitForSeconds.
/// </summary>
[Serializer(typeof(WaitForSeconds))]
public class SerializeWaitForSeconds : SerializerExtensionBase<WaitForSeconds>
{
    /// <summary>
    /// Saves the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public override IEnumerable<object> Save(WaitForSeconds target)
    {
        var tp = target.GetType();
        var f = tp.GetField("m_Seconds", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        return new object[] { f.GetValue(target) };
    }

    /// <summary>
    /// Loads the specified data.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="instance">The instance.</param>
    /// <returns>System.Object.</returns>
    public override object Load(object[] data, object instance)
    {
        return new WaitForSeconds((float)data[0]);
    }
}

/// <summary>
/// Class SerializeBounds.
/// </summary>
[Serializer(typeof(Bounds))]
public class SerializeBounds : SerializerExtensionBase<Bounds>
{
    /// <summary>
    /// Saves the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public override IEnumerable<object> Save(Bounds target)
    {
        return new object[] { target.center.x, target.center.y, target.center.z, target.size.x, target.size.y, target.size.z };
    }

    /// <summary>
    /// Loads the specified data.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="instance">The instance.</param>
    /// <returns>System.Object.</returns>
    public override object Load(object[] data, object instance)
    {
        return new Bounds(
                new Vector3((float)data[0], (float)data[1], (float)data[2]),
                new Vector3((float)data[3], (float)data[4], (float)data[5]));
    }
}

/// <summary>
/// Class ComponentSerializerExtensionBase.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class ComponentSerializerExtensionBase<T> : IComponentSerializer where T : Component
{
    /// <summary>
    /// Saves the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public abstract IEnumerable<object> Save(T target);

    /// <summary>
    /// Loads the into.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="instance">The instance.</param>
    public abstract void LoadInto(object[] data, T instance);

    #region IComponentSerializer implementation

    /// <summary>
    /// Serialize the specified component to a byte array
    /// </summary>
    /// <param name="component">Component to be serialized</param>
    /// <returns>System.Byte[].</returns>
    public byte[] Serialize(Component component)
    {
        return UnitySerializer.Serialize(Save((T)component).ToArray());
    }

    /// <summary>
    /// Deserialize the specified data into the instance.
    /// </summary>
    /// <param name="data">The data that represents the component, produced by Serialize</param>
    /// <param name="instance">The instance to target</param>
    public void Deserialize(byte[] data, Component instance)
    {
        object[] dataArray;
        dataArray = UnitySerializer.Deserialize<object[]>(data);
        LoadInto(dataArray, (T)instance);
    }

    #endregion IComponentSerializer implementation
}

/// <summary>
/// Class SerializerExtensionBase.
/// </summary>
/// <typeparam name="T"></typeparam>
public class SerializerExtensionBase<T> : ISerializeObjectEx
{
    #region ISerializeObject implementation

    /// <summary>
    /// Serializes the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>System.Object[].</returns>
    public object[] Serialize(object target)
    {
        return Save((T)target).ToArray();
    }

    /// <summary>
    /// Deserializes the specified data.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="instance">The instance.</param>
    /// <returns>System.Object.</returns>
    public object Deserialize(object[] data, object instance)
    {
        return Load(data, instance);
    }

    #endregion ISerializeObject implementation

    /// <summary>
    /// Saves the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public virtual IEnumerable<object> Save(T target)
    {
        return new object[0];
    }

    /// <summary>
    /// Loads the specified data.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="instance">The instance.</param>
    /// <returns>System.Object.</returns>
    public virtual object Load(object[] data, object instance)
    {
        return null;
    }

    #region ISerializeObjectEx implementation

    /// <summary>
    /// Determines whether this instance can serialize the specified target type.
    /// </summary>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="instance">The instance.</param>
    /// <returns><c>true</c> if this instance can serialize the specified target type; otherwise, <c>false</c>.</returns>
    public bool CanSerialize(Type targetType, object instance)
    {
        if (instance == null)
        {
            return true;
        }
        return CanBeSerialized(targetType, instance);
    }

    #endregion ISerializeObjectEx implementation

    /// <summary>
    /// Determines whether this instance [can be serialized] the specified target type.
    /// </summary>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="instance">The instance.</param>
    /// <returns><c>true</c> if this instance [can be serialized] the specified target type; otherwise, <c>false</c>.</returns>
    public virtual bool CanBeSerialized(Type targetType, object instance)
    {
        return true;
    }
}

/// <summary>
/// Class SerializeBoxCollider.
/// </summary>
[ComponentSerializerFor(typeof(BoxCollider))]
public class SerializeBoxCollider : ComponentSerializerExtensionBase<BoxCollider>
{
    /// <summary>
    /// Saves the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public override IEnumerable<object> Save(BoxCollider target)
    {
        return new object[] { target.isTrigger, target.size.x, target.size.y, target.size.z, target.center.x, target.center.y, target.center.z, target.enabled, target.sharedMaterial };
    }

    /// <summary>
    /// Loads the into.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="instance">The instance.</param>
    public override void LoadInto(object[] data, BoxCollider instance)
    {
        instance.isTrigger = (bool)data[0];
        instance.size = new Vector3((float)data[1], (float)data[2], (float)data[3]);
        instance.center = new Vector3((float)data[4], (float)data[5], (float)data[6]);
        instance.enabled = (bool)data[7];
        instance.sharedMaterial = (PhysicMaterial)data[8];
    }
}

/// <summary>
/// Class SerializeTerrain.
/// </summary>
[ComponentSerializerFor(typeof(Terrain))]
public class SerializeTerrain : ComponentSerializerExtensionBase<Terrain>
{
    /// <summary>
    /// Saves the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public override IEnumerable<object> Save(Terrain target)
    {
        return new object[] { target.enabled };
    }

    /// <summary>
    /// Loads the into.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="instance">The instance.</param>
    public override void LoadInto(object[] data, Terrain instance)
    {
        instance.enabled = (bool)data[0];
    }
}

/// <summary>
/// Class SerializeCollider.
/// </summary>
[ComponentSerializerFor(typeof(TerrainCollider))]
public class SerializeCollider : ComponentSerializerExtensionBase<TerrainCollider>
{
    /// <summary>
    /// Saves the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public override IEnumerable<object> Save(TerrainCollider target)
    {
        return new object[] { target.sharedMaterial, target.terrainData, target.enabled };
    }

    /// <summary>
    /// Loads the into.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="instance">The instance.</param>
    public override void LoadInto(object[] data, TerrainCollider instance)
    {
        instance.sharedMaterial = (PhysicMaterial)data[0];
        instance.terrainData = (TerrainData)data[1];
        instance.enabled = (bool)data[2];
    }
}

/// <summary>
/// Class SerializeMeshCollider.
/// </summary>
[ComponentSerializerFor(typeof(MeshCollider))]
public class SerializeMeshCollider : ComponentSerializerExtensionBase<MeshCollider>
{
    /// <summary>
    /// Saves the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public override IEnumerable<object> Save(MeshCollider target)
    {
        return new object[] { target.convex, target.isTrigger, target.sharedMaterial, target.sharedMesh, target.enabled };
    }

    /// <summary>
    /// Loads the into.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="instance">The instance.</param>
    public override void LoadInto(object[] data, MeshCollider instance)
    {
        instance.convex = (bool)data[0];
        instance.isTrigger = (bool)data[1];
        instance.sharedMaterial = (PhysicMaterial)data[2];
        instance.sharedMesh = (Mesh)data[3];
        instance.enabled = (bool)data[4];
    }
}

/// <summary>
/// Class SerializeWheelCollider.
/// </summary>
[ComponentSerializerFor(typeof(WheelCollider))]
public class SerializeWheelCollider : IComponentSerializer
{
    /// <summary>
    /// Class StoredInformation.
    /// </summary>
    public class StoredInformation
    {
        /// <summary>
        /// The enabled
        /// </summary>
        public bool Enabled;
        /// <summary>
        /// The brake torque
        /// </summary>
        public float brakeTorque;
        /// <summary>
        /// The center
        /// </summary>
        public Vector3 center;
        /// <summary>
        /// The force application point distance
        /// </summary>
        public float forceAppPointDistance;
        /// <summary>
        /// The forward friction
        /// </summary>
        public WheelFrictionCurve forwardFriction;
        /// <summary>
        /// The mass
        /// </summary>
        public float mass;
        /// <summary>
        /// The motor torque
        /// </summary>
        public float motorTorque;
        /// <summary>
        /// The radius
        /// </summary>
        public float radius;
        /// <summary>
        /// The sideways friction
        /// </summary>
        public WheelFrictionCurve sidewaysFriction;
        /// <summary>
        /// The steer angle
        /// </summary>
        public float steerAngle;
        /// <summary>
        /// The suspension distance
        /// </summary>
        public float suspensionDistance;
        /// <summary>
        /// The suspension spring
        /// </summary>
        public JointSpring suspensionSpring;
    }

    #region IComponentSerializer implementation

    /// <summary>
    /// Serialize the specified component to a byte array
    /// </summary>
    /// <param name="component">Component to be serialized</param>
    /// <returns>System.Byte[].</returns>
    public byte[] Serialize(Component component)
    {
        using (new UnitySerializer.SerializationSplitScope())
        {
            var collider = (WheelCollider)component;
            var si = new StoredInformation();
            si.Enabled = collider.enabled;
            si.brakeTorque = collider.brakeTorque;
            si.center = collider.center;
            si.forceAppPointDistance = collider.forceAppPointDistance;
            si.forwardFriction = collider.forwardFriction;
            si.mass = collider.mass;
            si.motorTorque = collider.motorTorque;
            si.radius = collider.radius;
            si.sidewaysFriction = collider.sidewaysFriction;
            si.steerAngle = collider.steerAngle;
            si.suspensionDistance = collider.suspensionDistance;
            si.suspensionSpring = collider.suspensionSpring;
            var data = UnitySerializer.Serialize(si);
            return data;
        }
    }

    /// <summary>
    /// Deserialize the specified data into the instance.
    /// </summary>
    /// <param name="data">The data that represents the component, produced by Serialize</param>
    /// <param name="instance">The instance to target</param>
    public void Deserialize(byte[] data, Component instance)
    {
        var collider = (WheelCollider)instance;
        collider.enabled = false;
        UnitySerializer.AddFinalAction(() =>
        {
            using (new UnitySerializer.SerializationSplitScope())
            {
                var si = UnitySerializer.Deserialize<StoredInformation>(data);
                if (si == null)
                {
                    Debug.LogError("An error occured when getting the stored information for a WheelCollider");
                    return;
                }
                collider.enabled = si.Enabled;
                collider.brakeTorque = si.brakeTorque;
                collider.center = si.center;
                collider.forceAppPointDistance = si.forceAppPointDistance;
                collider.forwardFriction = si.forwardFriction;
                collider.mass = si.mass;
                collider.motorTorque = si.motorTorque;
                collider.radius = si.radius;
                collider.sidewaysFriction = si.sidewaysFriction;
                collider.steerAngle = si.steerAngle;
                collider.suspensionDistance = si.suspensionDistance;
                collider.suspensionSpring = si.suspensionSpring;
            }
        }
        );
    }

    #endregion IComponentSerializer implementation
}

/// <summary>
/// Class SerializeCapsuleCollider.
/// </summary>
[ComponentSerializerFor(typeof(CapsuleCollider))]
public class SerializeCapsuleCollider : ComponentSerializerExtensionBase<CapsuleCollider>
{
    /// <summary>
    /// Saves the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public override IEnumerable<object> Save(CapsuleCollider target)
    {
        return new object[] { target.isTrigger, target.radius, target.center.x, target.center.y, target.center.z, target.height, target.enabled, target.sharedMaterial, target.direction };
    }

    /// <summary>
    /// Loads the into.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="instance">The instance.</param>
    public override void LoadInto(object[] data, CapsuleCollider instance)
    {
        instance.isTrigger = (bool)data[0];
        instance.radius = (float)data[1];
        instance.center = new Vector3((float)data[2], (float)data[3], (float)data[4]);
        instance.height = (float)data[5];
        instance.enabled = (bool)data[6];
        instance.sharedMaterial = (PhysicMaterial)data[7];
        instance.direction = (int)data[8];
    }
}

/// <summary>
/// Class SerializeSphereCollider.
/// </summary>
[ComponentSerializerFor(typeof(SphereCollider))]
public class SerializeSphereCollider : ComponentSerializerExtensionBase<SphereCollider>
{
    /// <summary>
    /// Saves the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public override IEnumerable<object> Save(SphereCollider target)
    {
        return new object[] { target.isTrigger, target.radius, target.center.x, target.center.y, target.center.z, target.enabled, target.sharedMaterial };
    }

    /// <summary>
    /// Loads the into.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="instance">The instance.</param>
    public override void LoadInto(object[] data, SphereCollider instance)
    {
        instance.isTrigger = (bool)data[0];
        instance.radius = (float)data[1];
        instance.center = new Vector3((float)data[2], (float)data[3], (float)data[4]);
        instance.enabled = (bool)data[5];
        instance.sharedMaterial = (PhysicMaterial)data[6];
    }
}

#region 2D

/// <summary>
/// Class SerializeRigidBody2D.
/// </summary>
[ComponentSerializerFor(typeof(Rigidbody2D))]
public class SerializeRigidBody2D : IComponentSerializer
{
    /// <summary>
    /// Class RigidBodyInfo.
    /// </summary>
    public class RigidBodyInfo
    {
        /// <summary>
        /// The angular drag
        /// </summary>
        public float angularDrag,
            /// <summary>
            /// The angular velocity
            /// </summary>
            angularVelocity,
            /// <summary>
            /// The drag
            /// </summary>
            drag,
            /// <summary>
            /// The gravity scale
            /// </summary>
            gravityScale,
            /// <summary>
            /// The inertia
            /// </summary>
            inertia,
            /// <summary>
            /// The mass
            /// </summary>
            mass,
            /// <summary>
            /// The rotation
            /// </summary>
            rotation;
        /// <summary>
        /// The center of mass
        /// </summary>
        public Vector2 centerOfMass,
            /// <summary>
            /// The position
            /// </summary>
            position,
            /// <summary>
            /// The velocity
            /// </summary>
            velocity;
        /// <summary>
        /// The collision detection mode
        /// </summary>
        public CollisionDetectionMode2D collisionDetectionMode;
        /// <summary>
        /// The constraints
        /// </summary>
        public RigidbodyConstraints2D constraints;
        /// <summary>
        /// The freeze rotation
        /// </summary>
        public bool freezeRotation,
            /// <summary>
            /// The is kinematic
            /// </summary>
            isKinematic,
            /// <summary>
            /// The simulated
            /// </summary>
            simulated,
            /// <summary>
            /// The use automatic mass
            /// </summary>
            useAutoMass;
        /// <summary>
        /// The interpolation
        /// </summary>
        public RigidbodyInterpolation2D interpolation;
        /// <summary>
        /// The sleep mode
        /// </summary>
        public RigidbodySleepMode2D sleepMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="RigidBodyInfo"/> class.
        /// </summary>
        public RigidBodyInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RigidBodyInfo"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public RigidBodyInfo(Rigidbody2D source)
        {
            angularDrag = source.angularDrag;
            angularVelocity = source.angularVelocity;
            drag = source.drag;
            gravityScale = source.gravityScale;
            inertia = source.inertia;
            mass = source.mass;
            rotation = source.rotation;

            centerOfMass = source.centerOfMass;
            position = source.position;
            velocity = source.velocity;

            collisionDetectionMode = source.collisionDetectionMode;
            constraints = source.constraints;

            freezeRotation = source.freezeRotation;
            isKinematic = source.isKinematic;
            simulated = source.simulated;
            useAutoMass = source.useAutoMass;

            interpolation = source.interpolation;
            sleepMode = source.sleepMode;
        }

        /// <summary>
        /// Configures the specified body.
        /// </summary>
        /// <param name="body">The body.</param>
        public void Configure(Rigidbody2D body)
        {
            body.isKinematic = true;

            body.angularDrag = angularDrag;
            body.drag = drag;
            body.gravityScale = gravityScale;
            body.rotation = rotation;
            body.position = position;

            if (centerOfMass != Vector2.zero)
            {
                body.centerOfMass = centerOfMass;
            }

            body.collisionDetectionMode = collisionDetectionMode;
            body.constraints = constraints;

            body.freezeRotation = freezeRotation;
            body.simulated = simulated;

            body.interpolation = interpolation;
            body.sleepMode = sleepMode;

            body.isKinematic = isKinematic;
            if (!isKinematic)
            {
                body.velocity = velocity;
                body.angularVelocity = angularVelocity;
                body.inertia = inertia;
            }

            body.useAutoMass = useAutoMass;
        }
    }

    #region IComponentSerializer implementation

    /// <summary>
    /// Serialize the specified component to a byte array
    /// </summary>
    /// <param name="component">Component to be serialized</param>
    /// <returns>System.Byte[].</returns>
    public byte[] Serialize(Component component)
    {
        return UnitySerializer.Serialize(new RigidBodyInfo((Rigidbody2D)component));
    }

    /// <summary>
    /// Deserialize the specified data into the instance.
    /// </summary>
    /// <param name="data">The data that represents the component, produced by Serialize</param>
    /// <param name="instance">The instance to target</param>
    public void Deserialize(byte[] data, Component instance)
    {
        var info = UnitySerializer.Deserialize<RigidBodyInfo>(data);
        info.Configure((Rigidbody2D)instance);
    }

    #endregion IComponentSerializer implementation
}

#region Colliders

/// <summary>
/// Class SerializeBoxCollider2D.
/// </summary>
[ComponentSerializerFor(typeof(BoxCollider2D))]
public class SerializeBoxCollider2D : IComponentSerializer
{
    /// <summary>
    /// Class StoredInformation.
    /// </summary>
    public class StoredInformation
    {
        // Meta-information
        /// <summary>
        /// The has rigidbody
        /// </summary>
        public bool hasRigidbody;

        // Properties
        /// <summary>
        /// The enabled
        /// </summary>
        public bool enabled,
                    /// <summary>
                    /// The is trigger
                    /// </summary>
                    isTrigger,
                    /// <summary>
                    /// The used by effector
                    /// </summary>
                    usedByEffector;

        /// <summary>
        /// The size
        /// </summary>
        public Vector2 size,
                       /// <summary>
                       /// The offset
                       /// </summary>
                       offset;
        /// <summary>
        /// The shared material
        /// </summary>
        public PhysicsMaterial2D sharedMaterial;
        /// <summary>
        /// The density
        /// </summary>
        public float density;
    }

    #region IComponentSerializer implementation

    /// <summary>
    /// Serialize the specified component to a byte array
    /// </summary>
    /// <param name="component">Component to be serialized</param>
    /// <returns>System.Byte[].</returns>
    public byte[] Serialize(Component component)
    {
        using (new UnitySerializer.SerializationSplitScope())
        {
            var col = (BoxCollider2D)component;
            var si = new StoredInformation();

            si.hasRigidbody = col.GetComponent<Rigidbody2D>();

            si.enabled = col.enabled;
            si.isTrigger = col.isTrigger;
            si.usedByEffector = col.usedByEffector;
            si.size = col.size;
            si.offset = col.offset;
            si.sharedMaterial = col.sharedMaterial;
            si.density = col.density;

            var data = UnitySerializer.Serialize(si);
            return data;
        }
    }

    /// <summary>
    /// Deserialize the specified data into the instance.
    /// </summary>
    /// <param name="data">The data that represents the component, produced by Serialize</param>
    /// <param name="instance">The instance to target</param>
    public void Deserialize(byte[] data, Component instance)
    {
        var col = (BoxCollider2D)instance;
        col.enabled = false;
        UnitySerializer.AddFinalAction(() =>
        {
            using (new UnitySerializer.SerializationSplitScope())
            {
                var si = UnitySerializer.Deserialize<StoredInformation>(data);
                if (si == null)
                {
                    Debug.LogError("An error occured when getting the stored information for a 2d joint");
                    return;
                }

                col.enabled = si.enabled;
                col.isTrigger = si.isTrigger;
                col.usedByEffector = si.usedByEffector;
                col.size = si.size;
                col.offset = si.offset;
                col.sharedMaterial = si.sharedMaterial;

                if (si.hasRigidbody)
                {
                    if (col.GetComponent<Rigidbody2D>().useAutoMass)
                    {
                        col.density = si.density;
                    }
                }
            }
        }
        );
    }

    #endregion IComponentSerializer implementation
}

/// <summary>
/// Class SerializeCircleCollider2D.
/// </summary>
[ComponentSerializerFor(typeof(CircleCollider2D))]
public class SerializeCircleCollider2D : IComponentSerializer
{
    /// <summary>
    /// Class StoredInformation.
    /// </summary>
    public class StoredInformation
    {
        // Meta-information
        /// <summary>
        /// The has rigidbody
        /// </summary>
        public bool hasRigidbody;

        // Properties
        /// <summary>
        /// The enabled
        /// </summary>
        public bool enabled,
                    /// <summary>
                    /// The is trigger
                    /// </summary>
                    isTrigger,
                    /// <summary>
                    /// The used by effector
                    /// </summary>
                    usedByEffector;

        /// <summary>
        /// The offset
        /// </summary>
        public Vector2 offset;
        /// <summary>
        /// The shared material
        /// </summary>
        public PhysicsMaterial2D sharedMaterial;
        /// <summary>
        /// The radius
        /// </summary>
        public float radius,
                     /// <summary>
                     /// The density
                     /// </summary>
                     density;
    }

    #region IComponentSerializer implementation

    /// <summary>
    /// Serialize the specified component to a byte array
    /// </summary>
    /// <param name="component">Component to be serialized</param>
    /// <returns>System.Byte[].</returns>
    public byte[] Serialize(Component component)
    {
        using (new UnitySerializer.SerializationSplitScope())
        {
            var col = (CircleCollider2D)component;
            var si = new StoredInformation();

            si.hasRigidbody = col.GetComponent<Rigidbody2D>();

            si.isTrigger = col.isTrigger;
            si.offset = col.offset;
            si.enabled = col.enabled;
            si.sharedMaterial = col.sharedMaterial;
            si.radius = col.radius;
            si.usedByEffector = col.usedByEffector;
            si.density = col.density;

            var data = UnitySerializer.Serialize(si);
            return data;
        }
    }

    /// <summary>
    /// Deserialize the specified data into the instance.
    /// </summary>
    /// <param name="data">The data that represents the component, produced by Serialize</param>
    /// <param name="instance">The instance to target</param>
    public void Deserialize(byte[] data, Component instance)
    {
        var col = (CircleCollider2D)instance;
        col.enabled = false;
        UnitySerializer.AddFinalAction(() =>
        {
            using (new UnitySerializer.SerializationSplitScope())
            {
                var si = UnitySerializer.Deserialize<StoredInformation>(data);
                if (si == null)
                {
                    Debug.LogError("An error occured when getting the stored information for a 2d joint");
                    return;
                }

                col.isTrigger = si.isTrigger;
                col.offset = si.offset;
                col.enabled = si.enabled;
                col.sharedMaterial = si.sharedMaterial;
                col.radius = si.radius;
                col.usedByEffector = si.usedByEffector;

                if (si.hasRigidbody)
                {
                    if (col.GetComponent<Rigidbody2D>().useAutoMass)
                    {
                        col.density = si.density;
                    }
                }
            }
        }
        );
    }

    #endregion IComponentSerializer implementation
}

/// <summary>
/// Class SerializeEdgeCollider2D.
/// </summary>
[ComponentSerializerFor(typeof(EdgeCollider2D))]
public class SerializeEdgeCollider2D : IComponentSerializer
{
    /// <summary>
    /// Class StoredInformation.
    /// </summary>
    public class StoredInformation
    {
        // Meta-information
        /// <summary>
        /// The has rigidbody
        /// </summary>
        public bool hasRigidbody;

        // Properties
        /// <summary>
        /// The enabled
        /// </summary>
        public bool enabled,
                    /// <summary>
                    /// The is trigger
                    /// </summary>
                    isTrigger,
                    /// <summary>
                    /// The used by effector
                    /// </summary>
                    usedByEffector;

        /// <summary>
        /// The offset
        /// </summary>
        public Vector2 offset;
        /// <summary>
        /// The points
        /// </summary>
        public Vector2[] points;
        /// <summary>
        /// The shared material
        /// </summary>
        public PhysicsMaterial2D sharedMaterial;
        /// <summary>
        /// The density
        /// </summary>
        public float density;
    }

    #region IComponentSerializer implementation

    /// <summary>
    /// Serialize the specified component to a byte array
    /// </summary>
    /// <param name="component">Component to be serialized</param>
    /// <returns>System.Byte[].</returns>
    public byte[] Serialize(Component component)
    {
        using (new UnitySerializer.SerializationSplitScope())
        {
            var col = (EdgeCollider2D)component;
            var si = new StoredInformation();

            si.hasRigidbody = col.GetComponent<Rigidbody2D>();

            si.isTrigger = col.isTrigger;
            si.offset = col.offset;
            si.enabled = col.enabled;
            si.sharedMaterial = col.sharedMaterial;
            si.points = col.points;
            si.usedByEffector = col.usedByEffector;
            si.density = col.density;

            var data = UnitySerializer.Serialize(si);
            return data;
        }
    }

    /// <summary>
    /// Deserialize the specified data into the instance.
    /// </summary>
    /// <param name="data">The data that represents the component, produced by Serialize</param>
    /// <param name="instance">The instance to target</param>
    public void Deserialize(byte[] data, Component instance)
    {
        var col = (EdgeCollider2D)instance;
        col.enabled = false;
        UnitySerializer.AddFinalAction(() =>
        {
            using (new UnitySerializer.SerializationSplitScope())
            {
                var si = UnitySerializer.Deserialize<StoredInformation>(data);
                if (si == null)
                {
                    Debug.LogError("An error occured when getting the stored information for a 2d joint");
                    return;
                }

                col.isTrigger = si.isTrigger;
                col.offset = si.offset;
                col.enabled = si.enabled;
                col.sharedMaterial = si.sharedMaterial;
                col.points = si.points;
                col.usedByEffector = si.usedByEffector;

                if (si.hasRigidbody)
                {
                    if (col.GetComponent<Rigidbody2D>().useAutoMass)
                    {
                        col.density = si.density;
                    }
                }
            }
        }
        );
    }

    #endregion IComponentSerializer implementation
}

/// <summary>
/// Class SerializePolygonCollider2D.
/// </summary>
[ComponentSerializerFor(typeof(PolygonCollider2D))]
public class SerializePolygonCollider2D : IComponentSerializer
{
    /// <summary>
    /// Class StoredInformation.
    /// </summary>
    public class StoredInformation
    {
        // Meta-information
        /// <summary>
        /// The has rigidbody
        /// </summary>
        public bool hasRigidbody;

        // Properties
        /// <summary>
        /// The enabled
        /// </summary>
        public bool enabled,
                    /// <summary>
                    /// The is trigger
                    /// </summary>
                    isTrigger,
                    /// <summary>
                    /// The used by effector
                    /// </summary>
                    usedByEffector;

        /// <summary>
        /// The offset
        /// </summary>
        public Vector2 offset;
        /// <summary>
        /// The paths
        /// </summary>
        public Vector2[][] paths;
        /// <summary>
        /// The shared material
        /// </summary>
        public PhysicsMaterial2D sharedMaterial;
        /// <summary>
        /// The density
        /// </summary>
        public float density;
        /// <summary>
        /// The path count
        /// </summary>
        public int pathCount;
    }

    #region IComponentSerializer implementation

    /// <summary>
    /// Serialize the specified component to a byte array
    /// </summary>
    /// <param name="component">Component to be serialized</param>
    /// <returns>System.Byte[].</returns>
    public byte[] Serialize(Component component)
    {
        using (new UnitySerializer.SerializationSplitScope())
        {
            var col = (PolygonCollider2D)component;
            var si = new StoredInformation();

            si.hasRigidbody = col.GetComponent<Rigidbody2D>();

            si.isTrigger = col.isTrigger;
            si.offset = col.offset;
            si.enabled = col.enabled;
            si.sharedMaterial = col.sharedMaterial;
            si.usedByEffector = col.usedByEffector;
            si.density = col.density;
            si.pathCount = col.pathCount;

            si.paths = new Vector2[si.pathCount][];
            for (int i = 0; i < si.pathCount; i++)
            {
                si.paths[i] = col.GetPath(i);
            }

            var data = UnitySerializer.Serialize(si);
            return data;
        }
    }

    /// <summary>
    /// Deserialize the specified data into the instance.
    /// </summary>
    /// <param name="data">The data that represents the component, produced by Serialize</param>
    /// <param name="instance">The instance to target</param>
    public void Deserialize(byte[] data, Component instance)
    {
        var col = (PolygonCollider2D)instance;
        col.enabled = false;
        UnitySerializer.AddFinalAction(() =>
        {
            using (new UnitySerializer.SerializationSplitScope())
            {
                var si = UnitySerializer.Deserialize<StoredInformation>(data);
                if (si == null)
                {
                    Debug.LogError("An error occured when getting the stored information for a 2d joint");
                    return;
                }

                col.isTrigger = si.isTrigger;
                col.offset = si.offset;
                col.enabled = si.enabled;
                col.sharedMaterial = si.sharedMaterial;
                col.usedByEffector = si.usedByEffector;

                Vector2[][] paths = si.paths;
                int pathCount = si.pathCount;
                for (int i = 0; i < pathCount; i++)
                {
                    col.SetPath(i, paths[i]);
                }

                if (si.hasRigidbody)
                {
                    if (col.GetComponent<Rigidbody2D>().useAutoMass)
                    {
                        col.density = si.density;
                    }
                }
            }
        }
        );
    }

    #endregion IComponentSerializer implementation
}

#endregion Colliders

#region Joints

/// <summary>
/// Class SerializeDistanceJoint2D.
/// </summary>
[ComponentSerializerFor(typeof(DistanceJoint2D))]
public class SerializeDistanceJoint2D : IComponentSerializer
{
    /// <summary>
    /// Class StoredInformation.
    /// </summary>
    public class StoredInformation
    {
        /// <summary>
        /// The break force
        /// </summary>
        public float breakForce,
                     /// <summary>
                     /// The break torque
                     /// </summary>
                     breakTorque,
                     /// <summary>
                     /// The distance
                     /// </summary>
                     distance;
        /// <summary>
        /// The enable collision
        /// </summary>
        public bool enableCollision,
                    /// <summary>
                    /// The enabled
                    /// </summary>
                    enabled,
                    /// <summary>
                    /// The automatic configure distance
                    /// </summary>
                    autoConfigureDistance,
                    /// <summary>
                    /// The maximum distance only
                    /// </summary>
                    maxDistanceOnly,
                    /// <summary>
                    /// The automatic configure connected anchor
                    /// </summary>
                    autoConfigureConnectedAnchor;
        /// <summary>
        /// The anchor
        /// </summary>
        public Vector2 anchor,
                       /// <summary>
                       /// The connected anchor
                       /// </summary>
                       connectedAnchor;
        /// <summary>
        /// The connected body
        /// </summary>
        public Rigidbody2D connectedBody;
    }

    #region IComponentSerializer implementation

    /// <summary>
    /// Serialize the specified component to a byte array
    /// </summary>
    /// <param name="component">Component to be serialized</param>
    /// <returns>System.Byte[].</returns>
    public byte[] Serialize(Component component)
    {
        using (new UnitySerializer.SerializationSplitScope())
        {
            var joint = (DistanceJoint2D)component;
            var si = new StoredInformation();

            si.breakForce = joint.breakForce;
            si.breakTorque = joint.breakTorque;
            si.distance = joint.distance;
            si.enableCollision = joint.enableCollision;
            si.enabled = joint.enabled;
            si.autoConfigureDistance = joint.autoConfigureDistance;
            si.maxDistanceOnly = joint.maxDistanceOnly;
            si.autoConfigureConnectedAnchor = joint.autoConfigureConnectedAnchor;
            si.anchor = joint.anchor;
            si.connectedAnchor = joint.connectedAnchor;
            si.connectedBody = joint.connectedBody;

            var data = UnitySerializer.Serialize(si);
            return data;
        }
    }

    /// <summary>
    /// Deserialize the specified data into the instance.
    /// </summary>
    /// <param name="data">The data that represents the component, produced by Serialize</param>
    /// <param name="instance">The instance to target</param>
    public void Deserialize(byte[] data, Component instance)
    {
        var joint = (DistanceJoint2D)instance;
        joint.enabled = false;
        UnitySerializer.AddFinalAction(() =>
        {
            using (new UnitySerializer.SerializationSplitScope())
            {
                var si = UnitySerializer.Deserialize<StoredInformation>(data);
                if (si == null)
                {
                    Debug.LogError("An error occured when getting the stored information for a 2d joint");
                    return;
                }

                if (si.breakForce != float.PositiveInfinity)
                {
                    joint.breakForce = si.breakForce;
                }
                if (si.breakTorque != float.PositiveInfinity)
                {
                    joint.breakTorque = si.breakTorque;
                }

                joint.distance = si.distance;
                joint.enableCollision = si.enableCollision;
                joint.enabled = si.enabled;
                joint.autoConfigureDistance = si.autoConfigureDistance;
                joint.maxDistanceOnly = si.maxDistanceOnly;
                joint.autoConfigureConnectedAnchor = si.autoConfigureConnectedAnchor;
                joint.anchor = si.anchor;
                joint.connectedAnchor = si.connectedAnchor;
                joint.connectedBody = si.connectedBody;
            }
        }
        );
    }

    #endregion IComponentSerializer implementation
}

/// <summary>
/// Class SerializeFixedJoint2D.
/// </summary>
[ComponentSerializerFor(typeof(FixedJoint2D))]
public class SerializeFixedJoint2D : IComponentSerializer
{
    /// <summary>
    /// Class StoredInformation.
    /// </summary>
    public class StoredInformation
    {
        /// <summary>
        /// The break force
        /// </summary>
        public float breakForce,
                     /// <summary>
                     /// The break torque
                     /// </summary>
                     breakTorque;
        /// <summary>
        /// The enable collision
        /// </summary>
        public bool enableCollision,
                    /// <summary>
                    /// The enabled
                    /// </summary>
                    enabled,
                    /// <summary>
                    /// The automatic configure connected anchor
                    /// </summary>
                    autoConfigureConnectedAnchor;
        /// <summary>
        /// The anchor
        /// </summary>
        public Vector2 anchor,
                       /// <summary>
                       /// The connected anchor
                       /// </summary>
                       connectedAnchor;
        /// <summary>
        /// The connected body
        /// </summary>
        public Rigidbody2D connectedBody;

        /// <summary>
        /// The damping ratio
        /// </summary>
        public float dampingRatio,
                     /// <summary>
                     /// The frequency
                     /// </summary>
                     frequency;
    }

    #region IComponentSerializer implementation

    /// <summary>
    /// Serialize the specified component to a byte array
    /// </summary>
    /// <param name="component">Component to be serialized</param>
    /// <returns>System.Byte[].</returns>
    public byte[] Serialize(Component component)
    {
        using (new UnitySerializer.SerializationSplitScope())
        {
            var joint = (FixedJoint2D)component;
            var si = new StoredInformation();

            si.breakForce = joint.breakForce;
            si.breakTorque = joint.breakTorque;
            si.enableCollision = joint.enableCollision;
            si.enabled = joint.enabled;
            si.autoConfigureConnectedAnchor = joint.autoConfigureConnectedAnchor;
            si.anchor = joint.anchor;
            si.connectedAnchor = joint.connectedAnchor;
            si.connectedBody = joint.connectedBody;

            si.dampingRatio = joint.dampingRatio;
            si.frequency = joint.frequency;

            var data = UnitySerializer.Serialize(si);
            return data;
        }
    }

    /// <summary>
    /// Deserialize the specified data into the instance.
    /// </summary>
    /// <param name="data">The data that represents the component, produced by Serialize</param>
    /// <param name="instance">The instance to target</param>
    public void Deserialize(byte[] data, Component instance)
    {
        var joint = (FixedJoint2D)instance;
        joint.enabled = false;
        UnitySerializer.AddFinalAction(() =>
        {
            using (new UnitySerializer.SerializationSplitScope())
            {
                var si = UnitySerializer.Deserialize<StoredInformation>(data);
                if (si == null)
                {
                    Debug.LogError("An error occured when getting the stored information for a 2d joint");

                    return;
                }

                if (si.breakForce != float.PositiveInfinity)
                {
                    joint.breakForce = si.breakForce;
                }
                if (si.breakTorque != float.PositiveInfinity)
                {
                    joint.breakTorque = si.breakTorque;
                }

                joint.enableCollision = si.enableCollision;
                joint.enabled = si.enabled;
                joint.autoConfigureConnectedAnchor = si.autoConfigureConnectedAnchor;
                joint.anchor = si.anchor;
                joint.connectedAnchor = si.connectedAnchor;
                joint.connectedBody = si.connectedBody;

                joint.dampingRatio = si.dampingRatio;
                joint.frequency = si.frequency;
            }
        }
        );
    }

    #endregion IComponentSerializer implementation
}

/// <summary>
/// Class SerializeFrictionJoint2D.
/// </summary>
[ComponentSerializerFor(typeof(FrictionJoint2D))]
public class SerializeFrictionJoint2D : IComponentSerializer
{
    /// <summary>
    /// Class StoredInformation.
    /// </summary>
    public class StoredInformation
    {
        /// <summary>
        /// The break force
        /// </summary>
        public float breakForce,
                     /// <summary>
                     /// The break torque
                     /// </summary>
                     breakTorque;
        /// <summary>
        /// The enable collision
        /// </summary>
        public bool enableCollision,
                    /// <summary>
                    /// The enabled
                    /// </summary>
                    enabled,
                    /// <summary>
                    /// The automatic configure connected anchor
                    /// </summary>
                    autoConfigureConnectedAnchor;
        /// <summary>
        /// The anchor
        /// </summary>
        public Vector2 anchor,
                       /// <summary>
                       /// The connected anchor
                       /// </summary>
                       connectedAnchor;
        /// <summary>
        /// The connected body
        /// </summary>
        public Rigidbody2D connectedBody;

        /// <summary>
        /// The maximum force
        /// </summary>
        public float maxForce,
                     /// <summary>
                     /// The maximum torque
                     /// </summary>
                     maxTorque;
    }

    #region IComponentSerializer implementation

    /// <summary>
    /// Serialize the specified component to a byte array
    /// </summary>
    /// <param name="component">Component to be serialized</param>
    /// <returns>System.Byte[].</returns>
    public byte[] Serialize(Component component)
    {
        using (new UnitySerializer.SerializationSplitScope())
        {
            var joint = (FrictionJoint2D)component;
            var si = new StoredInformation();

            si.breakForce = joint.breakForce;
            si.breakTorque = joint.breakTorque;
            si.enableCollision = joint.enableCollision;
            si.enabled = joint.enabled;
            si.autoConfigureConnectedAnchor = joint.autoConfigureConnectedAnchor;
            si.anchor = joint.anchor;
            si.connectedAnchor = joint.connectedAnchor;
            si.connectedBody = joint.connectedBody;

            si.maxForce = joint.maxForce;
            si.maxTorque = joint.maxTorque;

            var data = UnitySerializer.Serialize(si);
            return data;
        }
    }

    /// <summary>
    /// Deserialize the specified data into the instance.
    /// </summary>
    /// <param name="data">The data that represents the component, produced by Serialize</param>
    /// <param name="instance">The instance to target</param>
    public void Deserialize(byte[] data, Component instance)
    {
        var joint = (FrictionJoint2D)instance;
        joint.enabled = false;
        UnitySerializer.AddFinalAction(() =>
        {
            using (new UnitySerializer.SerializationSplitScope())
            {
                var si = UnitySerializer.Deserialize<StoredInformation>(data);
                if (si == null)
                {
                    Debug.LogError("An error occured when getting the stored information for a 2d joint");

                    return;
                }

                if (si.breakForce != float.PositiveInfinity)
                {
                    joint.breakForce = si.breakForce;
                }
                if (si.breakTorque != float.PositiveInfinity)
                {
                    joint.breakTorque = si.breakTorque;
                }

                joint.enableCollision = si.enableCollision;
                joint.enabled = si.enabled;
                joint.autoConfigureConnectedAnchor = si.autoConfigureConnectedAnchor;
                joint.anchor = si.anchor;
                joint.connectedAnchor = si.connectedAnchor;
                joint.connectedBody = si.connectedBody;

                joint.maxForce = si.maxForce;
                joint.maxTorque = si.maxTorque;
            }
        }
        );
    }

    #endregion IComponentSerializer implementation
}

/// <summary>
/// Class SerializeHingeJoint2D.
/// </summary>
[ComponentSerializerFor(typeof(HingeJoint2D))]
public class SerializeHingeJoint2D : IComponentSerializer
{
    /// <summary>
    /// Class StoredInformation.
    /// </summary>
    public class StoredInformation
    {
        /// <summary>
        /// The break force
        /// </summary>
        public float breakForce,
                     /// <summary>
                     /// The break torque
                     /// </summary>
                     breakTorque;
        /// <summary>
        /// The enable collision
        /// </summary>
        public bool enableCollision,
                    /// <summary>
                    /// The enabled
                    /// </summary>
                    enabled,
                    /// <summary>
                    /// The automatic configure connected anchor
                    /// </summary>
                    autoConfigureConnectedAnchor;
        /// <summary>
        /// The anchor
        /// </summary>
        public Vector2 anchor,
                       /// <summary>
                       /// The connected anchor
                       /// </summary>
                       connectedAnchor;
        /// <summary>
        /// The connected body
        /// </summary>
        public Rigidbody2D connectedBody;

        /// <summary>
        /// The limits
        /// </summary>
        public JointAngleLimits2D limits;
        /// <summary>
        /// The motor
        /// </summary>
        public JointMotor2D motor;
        /// <summary>
        /// The use limits
        /// </summary>
        public bool useLimits,
                    /// <summary>
                    /// The use motor
                    /// </summary>
                    useMotor;
    }

    #region IComponentSerializer implementation

    /// <summary>
    /// Serialize the specified component to a byte array
    /// </summary>
    /// <param name="component">Component to be serialized</param>
    /// <returns>System.Byte[].</returns>
    public byte[] Serialize(Component component)
    {
        using (new UnitySerializer.SerializationSplitScope())
        {
            var joint = (HingeJoint2D)component;
            var si = new StoredInformation();

            si.breakForce = joint.breakForce;
            si.breakTorque = joint.breakTorque;
            si.enableCollision = joint.enableCollision;
            si.enabled = joint.enabled;
            si.autoConfigureConnectedAnchor = joint.autoConfigureConnectedAnchor;
            si.anchor = joint.anchor;
            si.connectedAnchor = joint.connectedAnchor;
            si.connectedBody = joint.connectedBody;

            si.limits = joint.limits;
            si.motor = joint.motor;
            si.useLimits = joint.useLimits;
            si.useMotor = joint.useMotor;

            var data = UnitySerializer.Serialize(si);
            return data;
        }
    }

    /// <summary>
    /// Deserialize the specified data into the instance.
    /// </summary>
    /// <param name="data">The data that represents the component, produced by Serialize</param>
    /// <param name="instance">The instance to target</param>
    public void Deserialize(byte[] data, Component instance)
    {
        var joint = (HingeJoint2D)instance;
        joint.enabled = false;
        UnitySerializer.AddFinalAction(() =>
        {
            using (new UnitySerializer.SerializationSplitScope())
            {
                var si = UnitySerializer.Deserialize<StoredInformation>(data);
                if (si == null)
                {
                    Debug.LogError("An error occured when getting the stored information for a 2d joint");

                    return;
                }

                if (si.breakForce != float.PositiveInfinity)
                {
                    joint.breakForce = si.breakForce;
                }
                if (si.breakTorque != float.PositiveInfinity)
                {
                    joint.breakTorque = si.breakTorque;
                }

                joint.enableCollision = si.enableCollision;
                joint.enabled = si.enabled;
                joint.autoConfigureConnectedAnchor = si.autoConfigureConnectedAnchor;
                joint.anchor = si.anchor;
                joint.connectedAnchor = si.connectedAnchor;
                joint.connectedBody = si.connectedBody;

                joint.limits = si.limits;
                joint.motor = si.motor;
                joint.useLimits = si.useLimits;
                joint.useMotor = si.useMotor;
            }
        }
        );
    }

    #endregion IComponentSerializer implementation
}

/// <summary>
/// Class SerializeRelativeJoint2D.
/// </summary>
[ComponentSerializerFor(typeof(RelativeJoint2D))]
public class SerializeRelativeJoint2D : IComponentSerializer
{
    /// <summary>
    /// Class StoredInformation.
    /// </summary>
    public class StoredInformation
    {
        /// <summary>
        /// The enabled
        /// </summary>
        public bool enabled,
            /// <summary>
            /// The enable collision
            /// </summary>
            enableCollision;
        /// <summary>
        /// The connected body
        /// </summary>
        public Rigidbody2D connectedBody;

        /// <summary>
        /// The angular offset
        /// </summary>
        public float angularOffset,
            /// <summary>
            /// The correction scale
            /// </summary>
            correctionScale,
            /// <summary>
            /// The maximum force
            /// </summary>
            maxForce,
            /// <summary>
            /// The maximum torque
            /// </summary>
            maxTorque;
        /// <summary>
        /// The automatic configure offset
        /// </summary>
        public bool autoConfigureOffset;
        /// <summary>
        /// The linear offset
        /// </summary>
        public Vector2 linearOffset;
    }

    #region IComponentSerializer implementation

    /// <summary>
    /// Serialize the specified component to a byte array
    /// </summary>
    /// <param name="component">Component to be serialized</param>
    /// <returns>System.Byte[].</returns>
    public byte[] Serialize(Component component)
    {
        using (new UnitySerializer.SerializationSplitScope())
        {
            var joint = (RelativeJoint2D)component;
            var si = new StoredInformation();

            si.connectedBody = joint.connectedBody;
            si.enableCollision = joint.enableCollision;
            si.enabled = joint.enabled;

            si.angularOffset = joint.angularOffset;
            si.autoConfigureOffset = joint.autoConfigureOffset;
            si.correctionScale = joint.correctionScale;
            si.linearOffset = joint.linearOffset;
            si.maxForce = joint.maxForce;
            si.maxTorque = joint.maxTorque;

            var data = UnitySerializer.Serialize(si);
            return data;
        }
    }

    /// <summary>
    /// Deserialize the specified data into the instance.
    /// </summary>
    /// <param name="data">The data that represents the component, produced by Serialize</param>
    /// <param name="instance">The instance to target</param>
    public void Deserialize(byte[] data, Component instance)
    {
        var joint = (RelativeJoint2D)instance;
        joint.enabled = false;
        UnitySerializer.AddFinalAction(() =>
        {
            using (new UnitySerializer.SerializationSplitScope())
            {
                var si = UnitySerializer.Deserialize<StoredInformation>(data);
                if (si == null)
                {
                    Debug.LogError("An error occured when getting the stored information for a 2d joint");

                    return;
                }

                joint.connectedBody = si.connectedBody;
                joint.enableCollision = si.enableCollision;
                joint.enabled = si.enabled;

                joint.angularOffset = si.angularOffset;
                joint.autoConfigureOffset = si.autoConfigureOffset;
                joint.correctionScale = si.correctionScale;
                joint.linearOffset = si.linearOffset;
                joint.maxForce = si.maxForce;
                joint.maxTorque = si.maxTorque;
            }
        }
        );
    }

    #endregion IComponentSerializer implementation
}

/// <summary>
/// Class SerializeSliderJoint2D.
/// </summary>
[ComponentSerializerFor(typeof(SliderJoint2D))]
public class SerializeSliderJoint2D : IComponentSerializer
{
    /// <summary>
    /// Class StoredInformation.
    /// </summary>
    public class StoredInformation
    {
        /// <summary>
        /// The break force
        /// </summary>
        public float breakForce,
            /// <summary>
            /// The break torque
            /// </summary>
            breakTorque;
        /// <summary>
        /// The enable collision
        /// </summary>
        public bool enableCollision,
            /// <summary>
            /// The enabled
            /// </summary>
            enabled,
            /// <summary>
            /// The automatic configure connected anchor
            /// </summary>
            autoConfigureConnectedAnchor;
        /// <summary>
        /// The anchor
        /// </summary>
        public Vector2 anchor,
            /// <summary>
            /// The connected anchor
            /// </summary>
            connectedAnchor;
        /// <summary>
        /// The connected body
        /// </summary>
        public Rigidbody2D connectedBody;

        /// <summary>
        /// The angle
        /// </summary>
        public float angle;
        /// <summary>
        /// The automatic configure angle
        /// </summary>
        public bool autoConfigureAngle,
            /// <summary>
            /// The use limits
            /// </summary>
            useLimits,
            /// <summary>
            /// The use motor
            /// </summary>
            useMotor;
        /// <summary>
        /// The limits
        /// </summary>
        public JointTranslationLimits2D limits;
        /// <summary>
        /// The motor
        /// </summary>
        public JointMotor2D motor;
    }

    #region IComponentSerializer implementation

    /// <summary>
    /// Serialize the specified component to a byte array
    /// </summary>
    /// <param name="component">Component to be serialized</param>
    /// <returns>System.Byte[].</returns>
    public byte[] Serialize(Component component)
    {
        using (new UnitySerializer.SerializationSplitScope())
        {
            var joint = (SliderJoint2D)component;
            var si = new StoredInformation();

            si.breakForce = joint.breakForce;
            si.breakTorque = joint.breakTorque;
            si.enableCollision = joint.enableCollision;
            si.enabled = joint.enabled;
            si.autoConfigureConnectedAnchor = joint.autoConfigureConnectedAnchor;
            si.anchor = joint.anchor;
            si.connectedAnchor = joint.connectedAnchor;
            si.connectedBody = joint.connectedBody;

            si.angle = joint.angle;
            si.autoConfigureAngle = joint.autoConfigureAngle;
            si.limits = joint.limits;
            si.motor = joint.motor;
            si.useLimits = joint.useLimits;
            si.useMotor = joint.useMotor;

            var data = UnitySerializer.Serialize(si);
            return data;
        }
    }

    /// <summary>
    /// Deserialize the specified data into the instance.
    /// </summary>
    /// <param name="data">The data that represents the component, produced by Serialize</param>
    /// <param name="instance">The instance to target</param>
    public void Deserialize(byte[] data, Component instance)
    {
        var joint = (SliderJoint2D)instance;
        joint.enabled = false;
        UnitySerializer.AddFinalAction(() =>
        {
            using (new UnitySerializer.SerializationSplitScope())
            {
                var si = UnitySerializer.Deserialize<StoredInformation>(data);
                if (si == null)
                {
                    Debug.LogError("An error occured when getting the stored information for a 2d joint");

                    return;
                }

                if (si.breakForce != float.PositiveInfinity)
                {
                    joint.breakForce = si.breakForce;
                }
                if (si.breakTorque != float.PositiveInfinity)
                {
                    joint.breakTorque = si.breakTorque;
                }

                joint.enableCollision = si.enableCollision;
                joint.enabled = si.enabled;
                joint.autoConfigureConnectedAnchor = si.autoConfigureConnectedAnchor;
                joint.anchor = si.anchor;
                joint.connectedAnchor = si.connectedAnchor;
                joint.connectedBody = si.connectedBody;

                joint.angle = si.angle;
                joint.autoConfigureAngle = si.autoConfigureAngle;
                joint.limits = si.limits;
                joint.motor = si.motor;
                joint.useLimits = si.useLimits;
                joint.useMotor = si.useMotor;
            }
        }
        );
    }

    #endregion IComponentSerializer implementation
}

/// <summary>
/// Class SerializeSpringJoint2D.
/// </summary>
[ComponentSerializerFor(typeof(SpringJoint2D))]
public class SerializeSpringJoint2D : IComponentSerializer
{
    /// <summary>
    /// Class StoredInformation.
    /// </summary>
    public class StoredInformation
    {
        /// <summary>
        /// The break force
        /// </summary>
        public float breakForce,
            /// <summary>
            /// The break torque
            /// </summary>
            breakTorque;
        /// <summary>
        /// The enable collision
        /// </summary>
        public bool enableCollision,
            /// <summary>
            /// The enabled
            /// </summary>
            enabled,
            /// <summary>
            /// The automatic configure connected anchor
            /// </summary>
            autoConfigureConnectedAnchor;
        /// <summary>
        /// The anchor
        /// </summary>
        public Vector2 anchor,
            /// <summary>
            /// The connected anchor
            /// </summary>
            connectedAnchor;
        /// <summary>
        /// The connected body
        /// </summary>
        public Rigidbody2D connectedBody;

        /// <summary>
        /// The damping ratio
        /// </summary>
        public float dampingRatio,
            /// <summary>
            /// The distance
            /// </summary>
            distance,
            /// <summary>
            /// The frequency
            /// </summary>
            frequency;
        /// <summary>
        /// The automatic configure distance
        /// </summary>
        public bool autoConfigureDistance;
    }

    #region IComponentSerializer implementation

    /// <summary>
    /// Serialize the specified component to a byte array
    /// </summary>
    /// <param name="component">Component to be serialized</param>
    /// <returns>System.Byte[].</returns>
    public byte[] Serialize(Component component)
    {
        using (new UnitySerializer.SerializationSplitScope())
        {
            var joint = (SpringJoint2D)component;
            var si = new StoredInformation();

            si.breakForce = joint.breakForce;
            si.breakTorque = joint.breakTorque;
            si.enableCollision = joint.enableCollision;
            si.enabled = joint.enabled;
            si.autoConfigureConnectedAnchor = joint.autoConfigureConnectedAnchor;
            si.anchor = joint.anchor;
            si.connectedAnchor = joint.connectedAnchor;
            si.connectedBody = joint.connectedBody;

            si.autoConfigureDistance = joint.autoConfigureDistance;
            si.dampingRatio = joint.dampingRatio;
            si.distance = joint.distance;
            si.frequency = joint.frequency;

            var data = UnitySerializer.Serialize(si);
            return data;
        }
    }

    /// <summary>
    /// Deserialize the specified data into the instance.
    /// </summary>
    /// <param name="data">The data that represents the component, produced by Serialize</param>
    /// <param name="instance">The instance to target</param>
    public void Deserialize(byte[] data, Component instance)
    {
        var joint = (SpringJoint2D)instance;
        joint.enabled = false;
        UnitySerializer.AddFinalAction(() =>
        {
            using (new UnitySerializer.SerializationSplitScope())
            {
                var si = UnitySerializer.Deserialize<StoredInformation>(data);
                if (si == null)
                {
                    Debug.LogError("An error occured when getting the stored information for a 2d joint");

                    return;
                }

                if (si.breakForce != float.PositiveInfinity)
                {
                    joint.breakForce = si.breakForce;
                }
                if (si.breakTorque != float.PositiveInfinity)
                {
                    joint.breakTorque = si.breakTorque;
                }

                joint.enableCollision = si.enableCollision;
                joint.enabled = si.enabled;
                joint.autoConfigureConnectedAnchor = si.autoConfigureConnectedAnchor;
                joint.anchor = si.anchor;
                joint.connectedAnchor = si.connectedAnchor;
                joint.connectedBody = si.connectedBody;

                joint.autoConfigureDistance = si.autoConfigureDistance;
                joint.dampingRatio = si.dampingRatio;
                joint.distance = si.distance;
                joint.frequency = si.frequency;
            }
        }
        );
    }

    #endregion IComponentSerializer implementation
}

/// <summary>
/// Class SerializeWheelJoint2D.
/// </summary>
[ComponentSerializerFor(typeof(WheelJoint2D))]
public class SerializeWheelJoint2D : IComponentSerializer
{
    /// <summary>
    /// Class StoredInformation.
    /// </summary>
    public class StoredInformation
    {
        /// <summary>
        /// The break force
        /// </summary>
        public float breakForce,
            /// <summary>
            /// The break torque
            /// </summary>
            breakTorque;
        /// <summary>
        /// The enable collision
        /// </summary>
        public bool enableCollision,
            /// <summary>
            /// The enabled
            /// </summary>
            enabled,
            /// <summary>
            /// The automatic configure connected anchor
            /// </summary>
            autoConfigureConnectedAnchor;
        /// <summary>
        /// The anchor
        /// </summary>
        public Vector2 anchor,
            /// <summary>
            /// The connected anchor
            /// </summary>
            connectedAnchor;
        /// <summary>
        /// The connected body
        /// </summary>
        public Rigidbody2D connectedBody;

        /// <summary>
        /// The motor
        /// </summary>
        public JointMotor2D motor;
        /// <summary>
        /// The suspension
        /// </summary>
        public JointSuspension2D suspension;
        /// <summary>
        /// The use motor
        /// </summary>
        public bool useMotor;
    }

    #region IComponentSerializer implementation

    /// <summary>
    /// Serialize the specified component to a byte array
    /// </summary>
    /// <param name="component">Component to be serialized</param>
    /// <returns>System.Byte[].</returns>
    public byte[] Serialize(Component component)
    {
        using (new UnitySerializer.SerializationSplitScope())
        {
            var joint = (WheelJoint2D)component;
            var si = new StoredInformation();

            si.breakForce = joint.breakForce;
            si.breakTorque = joint.breakTorque;
            si.enableCollision = joint.enableCollision;
            si.enabled = joint.enabled;
            si.autoConfigureConnectedAnchor = joint.autoConfigureConnectedAnchor;
            si.anchor = joint.anchor;
            si.connectedAnchor = joint.connectedAnchor;
            si.connectedBody = joint.connectedBody;

            si.motor = joint.motor;
            si.suspension = joint.suspension;
            si.useMotor = joint.useMotor;

            var data = UnitySerializer.Serialize(si);
            return data;
        }
    }

    /// <summary>
    /// Deserialize the specified data into the instance.
    /// </summary>
    /// <param name="data">The data that represents the component, produced by Serialize</param>
    /// <param name="instance">The instance to target</param>
    public void Deserialize(byte[] data, Component instance)
    {
        var joint = (WheelJoint2D)instance;
        joint.enabled = false;
        UnitySerializer.AddFinalAction(() =>
        {
            using (new UnitySerializer.SerializationSplitScope())
            {
                var si = UnitySerializer.Deserialize<StoredInformation>(data);
                if (si == null)
                {
                    Debug.LogError("An error occured when getting the stored information for a 2d joint");

                    return;
                }

                if (si.breakForce != float.PositiveInfinity)
                {
                    joint.breakForce = si.breakForce;
                }
                if (si.breakTorque != float.PositiveInfinity)
                {
                    joint.breakTorque = si.breakTorque;
                }

                joint.enableCollision = si.enableCollision;
                joint.enabled = si.enabled;
                joint.autoConfigureConnectedAnchor = si.autoConfigureConnectedAnchor;
                joint.anchor = si.anchor;
                joint.connectedAnchor = si.connectedAnchor;
                joint.connectedBody = si.connectedBody;

                joint.motor = si.motor;
                joint.suspension = si.suspension;
                joint.useMotor = si.useMotor;
            }
        }
        );
    }

    #endregion IComponentSerializer implementation
}

#endregion Joints

#region Effectors

/// <summary>
/// Class SerializeAreaEffector2D.
/// </summary>
[ComponentSerializerFor(typeof(AreaEffector2D))]
public class SerializeAreaEffector2D : ComponentSerializerExtensionBase<AreaEffector2D>
{
    /// <summary>
    /// Saves the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public override IEnumerable<object> Save(AreaEffector2D target)
    {
        return new object[] {
            target.colliderMask, target.useColliderMask, target.enabled,
            target.angularDrag, target.drag, target.forceAngle, target.forceMagnitude,
            target.forceVariation, target.useGlobalAngle
        };
    }

    /// <summary>
    /// Loads the into.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="instance">The instance.</param>
    public override void LoadInto(object[] data, AreaEffector2D instance)
    {
        instance.colliderMask = (int)data[0];
        instance.useColliderMask = (bool)data[1];
        instance.enabled = (bool)data[2];

        instance.angularDrag = (float)data[3];
        instance.drag = (float)data[4];
        instance.forceAngle = (float)data[5];
        instance.forceMagnitude = (float)data[6];
        instance.forceVariation = (float)data[7];
        instance.useGlobalAngle = (bool)data[8];
    }
}

/// <summary>
/// Class SerializeBuoyancyEffector2D.
/// </summary>
[ComponentSerializerFor(typeof(BuoyancyEffector2D))]
public class SerializeBuoyancyEffector2D : ComponentSerializerExtensionBase<BuoyancyEffector2D>
{
    /// <summary>
    /// Saves the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public override IEnumerable<object> Save(BuoyancyEffector2D target)
    {
        return new object[] {
            target.colliderMask, target.useColliderMask, target.enabled,
            target.angularDrag, target.density, target.flowAngle, target.flowMagnitude,
            target.flowVariation, target.linearDrag, target.surfaceLevel
        };
    }

    /// <summary>
    /// Loads the into.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="instance">The instance.</param>
    public override void LoadInto(object[] data, BuoyancyEffector2D instance)
    {
        instance.colliderMask = (int)data[0];
        instance.useColliderMask = (bool)data[1];
        instance.enabled = (bool)data[2];

        instance.angularDrag = (float)data[3];
        instance.density = (float)data[4];
        instance.flowAngle = (float)data[5];
        instance.flowMagnitude = (float)data[6];
        instance.flowVariation = (float)data[7];
        instance.linearDrag = (float)data[8];
        instance.surfaceLevel = (float)data[9];
    }
}

/// <summary>
/// Class SerializePointEffector2D.
/// </summary>
[ComponentSerializerFor(typeof(PointEffector2D))]
public class SerializePointEffector2D : ComponentSerializerExtensionBase<PointEffector2D>
{
    /// <summary>
    /// Saves the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public override IEnumerable<object> Save(PointEffector2D target)
    {
        return new object[] {
            target.colliderMask, target.useColliderMask, target.enabled,
            target.angularDrag, target.distanceScale, target.drag, target.forceMagnitude,
            target.forceMode, target.forceSource, target.forceTarget, target.forceVariation
        };
    }

    /// <summary>
    /// Loads the into.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="instance">The instance.</param>
    public override void LoadInto(object[] data, PointEffector2D instance)
    {
        instance.colliderMask = (int)data[0];
        instance.useColliderMask = (bool)data[1];
        instance.enabled = (bool)data[2];

        instance.angularDrag = (float)data[3];
        instance.distanceScale = (float)data[4];
        instance.drag = (float)data[5];
        instance.forceMagnitude = (float)data[6];
        instance.forceMode = (EffectorForceMode2D)data[7];
        instance.forceSource = (EffectorSelection2D)data[8];
        instance.forceTarget = (EffectorSelection2D)data[9];
        instance.forceVariation = (float)data[10];
    }
}

/// <summary>
/// Class SerializePlatformEffector2D.
/// </summary>
[ComponentSerializerFor(typeof(PlatformEffector2D))]
public class SerializePlatformEffector2D : ComponentSerializerExtensionBase<PlatformEffector2D>
{
    /// <summary>
    /// Saves the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public override IEnumerable<object> Save(PlatformEffector2D target)
    {
        return new object[] {
            target.colliderMask, target.useColliderMask, target.enabled,
            target.sideArc, target.surfaceArc, target.useOneWay, target.useOneWayGrouping,
            target.useSideBounce, target.useSideFriction
        };
    }

    /// <summary>
    /// Loads the into.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="instance">The instance.</param>
    public override void LoadInto(object[] data, PlatformEffector2D instance)
    {
        instance.colliderMask = (int)data[0];
        instance.useColliderMask = (bool)data[1];
        instance.enabled = (bool)data[2];

        instance.sideArc = (float)data[3];
        instance.surfaceArc = (float)data[4];
        instance.useOneWay = (bool)data[5];
        instance.useOneWayGrouping = (bool)data[6];
        instance.useSideBounce = (bool)data[7];
        instance.useSideFriction = (bool)data[8];
    }
}

/// <summary>
/// Class SerializeSurfaceEffector2D.
/// </summary>
[ComponentSerializerFor(typeof(SurfaceEffector2D))]
public class SerializeSurfaceEffector2D : ComponentSerializerExtensionBase<SurfaceEffector2D>
{
    /// <summary>
    /// Saves the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public override IEnumerable<object> Save(SurfaceEffector2D target)
    {
        return new object[] {
            target.colliderMask, target.useColliderMask, target.enabled,
            target.forceScale, target.speed, target.speedVariation, target.useBounce,
            target.useContactForce, target.useFriction
        };
    }

    /// <summary>
    /// Loads the into.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="instance">The instance.</param>
    public override void LoadInto(object[] data, SurfaceEffector2D instance)
    {
        instance.colliderMask = (int)data[0];
        instance.useColliderMask = (bool)data[1];
        instance.enabled = (bool)data[2];

        instance.forceScale = (float)data[3];
        instance.speed = (float)data[4];
        instance.speedVariation = (float)data[5];
        instance.useBounce = (bool)data[6];
        instance.useContactForce = (bool)data[7];
        instance.useFriction = (bool)data[8];
    }
}

#endregion Effectors

#endregion 2D

/// <summary>
/// Class SerializeTexture2D.
/// </summary>
[Serializer(typeof(Texture2D))]
public class SerializeTexture2D : SerializerExtensionBase<Texture2D>
{
    /// <summary>
    /// Saves the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public override IEnumerable<object> Save(Texture2D target)
    {
        if (target.GetInstanceID() >= 0)
        {
            return new object[] { true, SaveGameManager.Instance.GetAssetId(target) };
        }
        else
        {
            return new object[] {
                false,
                target.anisoLevel,
                target.filterMode,
                target.format,
                target.mipMapBias,
                target.mipmapCount,
                target.name,
                target.texelSize,
                target.wrapMode,
                target.EncodeToPNG()
            };
        }
    }

    /// <summary>
    /// Loads the specified data.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="instance">The instance.</param>
    /// <returns>System.Object.</returns>
    public override object Load(object[] data, object instance)
    {
        if ((bool)data[0])
        {
            return SaveGameManager.Instance.GetAsset((SaveGameManager.AssetReference)data[1]);
        }
        Texture2D t;
        if (data.Length == 10)
        {
            t = new Texture2D(1, 1, (TextureFormat)data[3], (int)data[5] > 0 ? true : false);
            t.LoadImage((byte[])data[9]);
            t.anisoLevel = (int)data[1];
            t.filterMode = (FilterMode)data[2];
            t.mipMapBias = (float)data[4];
            t.name = (string)data[6];
            t.wrapMode = (TextureWrapMode)data[8];
            t.Apply();

            return t;
        }

        t = new Texture2D((int)data[9], (int)data[4], (TextureFormat)data[3], (int)data[6] > 0 ? true : false);
        t.anisoLevel = (int)data[1];
        t.filterMode = (FilterMode)data[2];
        t.mipMapBias = (float)data[5];
        t.name = (string)data[7];
        t.wrapMode = (TextureWrapMode)data[10];
        t.SetPixels32((Color32[])data[11]);
        t.Apply();

        return t;
    }

    /// <summary>
    /// Determines whether this instance [can be serialized] the specified target type.
    /// </summary>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="instance">The instance.</param>
    /// <returns><c>true</c> if this instance [can be serialized] the specified target type; otherwise, <c>false</c>.</returns>
    public override bool CanBeSerialized(Type targetType, object instance)
    {
        var obj = instance as Texture2D;
        if (!obj)
        {
            return false;
        }
        if (obj.GetInstanceID() < 0 || SaveGameManager.Instance.GetAssetId(instance as UnityEngine.Object).index != -1)
        {
            return true;
        }
        return false;
    }
}

/// <summary>
/// Class SerializeMaterial.
/// </summary>
[Serializer(typeof(Material))]
public class SerializeMaterial : SerializerExtensionBase<Material>
{
    /// <summary>
    /// Saves the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public override IEnumerable<object> Save(Material target)
    {
        var store = GetStore();
        if (target.GetInstanceID() >= 0)
        {
            return new object[] { true, SaveGameManager.Instance.GetAssetId(target) };
        }
        else
        {
            return new object[] { false, target.shader.name, target.name, target.renderQueue, store != null ? store.GetValues(target) : null };
        }
    }

    private StoreMaterials GetStore()
    {
        if (SerializeRenderer.Store != null)
        {
            return SerializeRenderer.Store;
        }
        if (UnitySerializer.currentlySerializingObject is Component)
        {
            var comp = UnitySerializer.currentlySerializingObject as Component;
            return comp.GetComponent<StoreMaterials>();
        }
        return null;
    }

    /// <summary>
    /// Loads the specified data.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="instance">The instance.</param>
    /// <returns>System.Object.</returns>
    public override object Load(object[] data, object instance)
    {
        if ((bool)data[0])
        {
            return SaveGameManager.Instance.GetAsset((SaveGameManager.AssetReference)data[1]);
        }
        Material m;
        var shdr = Shader.Find((string)data[1]);
        m = new Material(shdr);
        m.name = (string)data[2];
        var store = GetStore();
        m.renderQueue = (int)data[3];
        if (data[4] != null && store != null)
        {
            store.SetValues(m, (List<StoreMaterials.StoredValue>)data[4]);
        }
        return m;
    }

    /// <summary>
    /// Determines whether this instance [can be serialized] the specified target type.
    /// </summary>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="instance">The instance.</param>
    /// <returns><c>true</c> if this instance [can be serialized] the specified target type; otherwise, <c>false</c>.</returns>
    public override bool CanBeSerialized(Type targetType, object instance)
    {
        var obj = instance as Material;
        if (!obj)
        {
            return false;
        }
        if ((Shader.Find(obj.shader.name) != null && obj.GetInstanceID() < 0 && SerializeRenderer.Store != null) || SaveGameManager.Instance.GetAssetId(instance as UnityEngine.Object).index != -1)
        {
            return true;
        }
        return false;
    }
}

/// <summary>
/// Class SerializeAnimator.
/// </summary>
[ComponentSerializerFor(typeof(Animator))]
public class SerializeAnimator : ComponentSerializerExtensionBase<Animator>
{
    /// <summary>
    /// Loads the into.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="instance">The instance.</param>
    public override void LoadInto(object[] data, Animator instance)
    {
        Debug.LogWarningFormat(instance, "usng: Warning! The \"gameobject\" {0} is trying to load an Animator component without using a StoreAnimator component!", instance.gameObject);
    }

    /// <summary>
    /// Saves the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public override IEnumerable<object> Save(Animator target)
    {
        Debug.LogWarningFormat(target, "usng: Warning! The \"gameobject\" {0} is trying to store an Animator component without using a StoreAnimator component!", target.gameObject);

        return new object[] { null };
    }
}

/// <summary>
/// Class SerializeAssetReference.
/// </summary>
[SubTypeSerializer(typeof(Texture))]
[Serializer(typeof(Font))]
[Serializer(typeof(AudioClip))]
[Serializer(typeof(TextAsset))]
[SubTypeSerializer(typeof(Mesh))]
[Serializer(typeof(AnimationClip))]
[Serializer(typeof(Sprite))]
[Serializer(typeof(Button.ButtonClickedEvent))]
[Serializer(typeof(Slider.SliderEvent))]
[Serializer(typeof(Scrollbar.ScrollEvent))]
[Serializer(typeof(Toggle.ToggleEvent))]
[Serializer(typeof(InputField.OnChangeEvent))]
[Serializer(typeof(InputField.SubmitEvent))]
public class SerializeAssetReference : SerializerExtensionBase<object>
{
    /// <summary>
    /// The instance
    /// </summary>
    public static SerializeAssetReference instance = new SerializeAssetReference();

    /// <summary>
    /// Saves the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public override IEnumerable<object> Save(object target)
    {
        return new object[] { SaveGameManager.Instance.GetAssetId(target as UnityEngine.Object) };
    }

    /// <summary>
    /// Determines whether this instance [can be serialized] the specified target type.
    /// </summary>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="instance">The instance.</param>
    /// <returns><c>true</c> if this instance [can be serialized] the specified target type; otherwise, <c>false</c>.</returns>
    public override bool CanBeSerialized(Type targetType, object instance)
    {
        return instance == null || typeof(UnityEngine.Object).IsAssignableFrom(targetType);
    }

    /// <summary>
    /// Loads the specified data.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="instance">The instance.</param>
    /// <returns>System.Object.</returns>
    public override object Load(object[] data, object instance)
    {
        return SaveGameManager.Instance.GetAsset((SaveGameManager.AssetReference)data[0]);
    }
}

/// <summary>
/// Class SerializeScriptableObjectReference.
/// </summary>
[SubTypeSerializer(typeof(ScriptableObject))]
public class SerializeScriptableObjectReference : SerializerExtensionBase<object>
{
    /// <summary>
    /// Saves the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public override IEnumerable<object> Save(object target)
    {
        var id = SaveGameManager.Instance.GetAssetId(target as UnityEngine.Object);
        if (id.index == -1)
        {
            byte[] data = null;
            data = UnitySerializer.SerializeForDeserializeInto(target);
            var result = new object[] { true, target.GetType().FullName, data };
            return result;
        }
        else
        {
            return new object[] { false, id };
        }
    }

    /// <summary>
    /// Determines whether this instance [can be serialized] the specified target type.
    /// </summary>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="instance">The instance.</param>
    /// <returns><c>true</c> if this instance [can be serialized] the specified target type; otherwise, <c>false</c>.</returns>
    public override bool CanBeSerialized(Type targetType, object instance)
    {
        return instance != null;
    }

    /// <summary>
    /// Loads the specified data.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="instance">The instance.</param>
    /// <returns>System.Object.</returns>
    public override object Load(object[] data, object instance)
    {
        if ((bool)data[0])
        {
            var newInstance = ScriptableObject.CreateInstance(UnitySerializer.GetTypeEx(data[1]));
            UnitySerializer.DeserializeInto((byte[])data[2], newInstance);
            return newInstance;
        }
        else
        {
            return SaveGameManager.Instance.GetAsset((SaveGameManager.AssetReference)data[1]);
        }
    }
}

/// <summary>
/// Store a reference to a game object, first checking whether it is really another game
/// object and not a prefab
/// </summary>
[Serializer(typeof(GameObject))]
public class SerializeGameObjectReference : SerializerExtensionBase<GameObject>
{
    static SerializeGameObjectReference()
    {
        UnitySerializer.CanSerialize += (tp) =>
        {
            return !typeof(MeshFilter).IsAssignableFrom(tp);
        };
    }

    /// <summary>
    /// Saves the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public override IEnumerable<object> Save(GameObject target)
    {
        //Is this a reference to a prefab
        var assetId = SaveGameManager.Instance.GetAssetId(target);
        if (assetId.index != -1)
        {
            return new object[] { 0, true, null, assetId };
        }
        return new object[] { target.GetId(), UniqueIdentifier.GetByName(target.gameObject.GetId()) != null /* Identify a prefab */ };
    }

    /// <summary>
    /// Loads the specified data.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="instance">The instance.</param>
    /// <returns>System.Object.</returns>
    public override object Load(object[] data, object instance)
    {
        if (instance != null)
            return instance;

        if (!(bool)data[1])
            Debug.LogError("[[Disabled, will not be set]]");
        Debug.LogFormat("GameObject: {0}", data[0]);

        if (data.Length > 3)
        {
            var asset = SaveGameManager.Instance.GetAsset((SaveGameManager.AssetReference)data[3]) as GameObject;
            return asset;
        }
        return instance ?? new UnitySerializer.DeferredSetter((d) =>
        {
            return UniqueIdentifier.GetByName((string)data[0]);
        })
        {
            enabled = (bool)data[1]
        };
    }
}

/// <summary>
/// Class SerializeNavMeshAgent.
/// </summary>
[ComponentSerializerFor(typeof(NavMeshAgent))]
public class SerializeNavMeshAgent : IComponentSerializer
{
    /// <summary>
    /// Class StoredInfo.
    /// </summary>
    public class StoredInfo
    {
        /// <summary>
        /// The has path
        /// </summary>
        public bool hasPath,
            /// <summary>
            /// The off mesh
            /// </summary>
            offMesh,
            /// <summary>
            /// The automatic braking
            /// </summary>
            autoBraking,
            /// <summary>
            /// The automatic traverse off mesh link
            /// </summary>
            autoTraverseOffMeshLink,
            /// <summary>
            /// The automatic repath
            /// </summary>
            autoRepath;
        /// <summary>
        /// The x
        /// </summary>
        public float x,
            /// <summary>
            /// The y
            /// </summary>
            y,
            /// <summary>
            /// The z
            /// </summary>
            z,
            /// <summary>
            /// The speed
            /// </summary>
            speed,
            /// <summary>
            /// The angular speed
            /// </summary>
            angularSpeed,
            /// <summary>
            /// The height
            /// </summary>
            height,
            /// <summary>
            /// The offset
            /// </summary>
            offset,
            /// <summary>
            /// The acceleration
            /// </summary>
            acceleration,
            /// <summary>
            /// The radius
            /// </summary>
            radius,
            /// <summary>
            /// The stopping distance
            /// </summary>
            stoppingDistance;
        /// <summary>
        /// The passable
        /// </summary>
        public int passable = -1,
            /// <summary>
            /// The avoidance priority
            /// </summary>
            avoidancePriority;
        /// <summary>
        /// The obstacle avoidance type
        /// </summary>
        public ObstacleAvoidanceType obstacleAvoidanceType;
    }

    #region IComponentSerializer implementation

    /// <summary>
    /// Serialize the specified component to a byte array
    /// </summary>
    /// <param name="component">Component to be serialized</param>
    /// <returns>System.Byte[].</returns>
    public byte[] Serialize(Component component)
    {
        var agent = (NavMeshAgent)component;
        return UnitySerializer.Serialize(new StoredInfo
        {
            x = agent.destination.x,
            y = agent.destination.y,
            z = agent.destination.z,
            speed = agent.speed,
            acceleration = agent.acceleration,
            angularSpeed = agent.angularSpeed,
            height = agent.height,
            offset = agent.baseOffset,
            hasPath = agent.hasPath,
            offMesh = agent.isOnOffMeshLink,
            passable = agent.areaMask,
            radius = agent.radius,
            stoppingDistance = agent.stoppingDistance,
            autoBraking = agent.autoBraking,
            obstacleAvoidanceType = agent.obstacleAvoidanceType,
            avoidancePriority = agent.avoidancePriority,
            autoTraverseOffMeshLink = agent.autoTraverseOffMeshLink,
            autoRepath = agent.autoRepath
        });
    }

    /// <summary>
    /// Deserialize the specified data into the instance.
    /// </summary>
    /// <param name="data">The data that represents the component, produced by Serialize</param>
    /// <param name="instance">The instance to target</param>
    public void Deserialize(byte[] data, Component instance)
    {
        var path = new NavMeshPath();
        var agent = (NavMeshAgent)instance;
        agent.enabled = false;

        Loom.QueueOnMainThread(() =>
        {
            var si = UnitySerializer.Deserialize<StoredInfo>(data);
            agent.speed = si.speed;
            agent.acceleration = si.acceleration;
            agent.angularSpeed = si.angularSpeed;
            agent.height = si.height;
            agent.baseOffset = si.offset;
            agent.areaMask = si.passable;
            agent.radius = si.radius;
            agent.stoppingDistance = si.stoppingDistance;
            agent.autoBraking = si.autoBraking;
            agent.obstacleAvoidanceType = si.obstacleAvoidanceType;
            agent.avoidancePriority = si.avoidancePriority;
            agent.autoTraverseOffMeshLink = si.autoTraverseOffMeshLink;
            agent.autoRepath = si.autoRepath;

            if (si.hasPath && !agent.isOnOffMeshLink)
            {
                agent.enabled = true;
                //if(agent.CalculatePath( new Vector3 (si.x, si.y, si.z), path))
                //{
                //		agent.SetPath(path);
                //	}

                if (NavMesh.CalculatePath(agent.transform.position, new Vector3(si.x, si.y, si.z), si.passable, path))
                {
                    agent.SetPath(path);
                }
            }
        }, 0.1f);
    }

    #endregion IComponentSerializer implementation
}

/// <summary>
/// Class SerializeCamera.
/// </summary>
[ComponentSerializerFor(typeof(Camera))]
public class SerializeCamera : IComponentSerializer
{
    /// <summary>
    /// Class CameraData.
    /// </summary>
    public class CameraData
    {
        /// <summary>
        /// The rendering path
        /// </summary>
        public RenderingPath renderingPath;
        /// <summary>
        /// The field of view
        /// </summary>
        public float fieldOfView;
        /// <summary>
        /// The near clip plane
        /// </summary>
        public float nearClipPlane;
        /// <summary>
        /// The far clip plane
        /// </summary>
        public float farClipPlane;
        /// <summary>
        /// The depth
        /// </summary>
        public float depth;
        /// <summary>
        /// The rect
        /// </summary>
        public Rect rect;
        /// <summary>
        /// The use occlusion culling
        /// </summary>
        public bool useOcclusionCulling;
        /// <summary>
        /// The HDR
        /// </summary>
        public bool hdr;
        /// <summary>
        /// The target texture
        /// </summary>
        public RenderTexture targetTexture;
        /// <summary>
        /// The orthographic
        /// </summary>
        public bool orthographic;
        /// <summary>
        /// The orthographic size
        /// </summary>
        public float orthographicSize;
        /// <summary>
        /// The background color
        /// </summary>
        public Color backgroundColor;
    }

    #region IComponentSerializer implementation

    /// <summary>
    /// Serialize the specified component to a byte array
    /// </summary>
    /// <param name="component">Component to be serialized</param>
    /// <returns>System.Byte[].</returns>
    public byte[] Serialize(Component component)
    {
        var camera = (Camera)component;
        var cd = new CameraData
        {
            renderingPath = camera.actualRenderingPath,
            fieldOfView = camera.fieldOfView,
            depth = camera.depth,
            nearClipPlane = camera.nearClipPlane,
            farClipPlane = camera.farClipPlane,
            rect = camera.rect,
            useOcclusionCulling = camera.useOcclusionCulling,
            hdr = camera.hdr,
            targetTexture = camera.targetTexture,
            orthographic = camera.orthographic,
            orthographicSize = camera.orthographicSize,
            backgroundColor = camera.backgroundColor
        };
        return UnitySerializer.Serialize(cd);
    }

    /// <summary>
    /// Deserialize the specified data into the instance.
    /// </summary>
    /// <param name="data">The data that represents the component, produced by Serialize</param>
    /// <param name="instance">The instance to target</param>
    public void Deserialize(byte[] data, Component instance)
    {
        var cd = UnitySerializer.Deserialize<CameraData>(data);
        var camera = (Camera)instance;
        camera.renderingPath = cd.renderingPath;
        camera.fieldOfView = cd.fieldOfView;
        camera.nearClipPlane = cd.nearClipPlane;
        camera.farClipPlane = cd.farClipPlane;
        camera.depth = cd.depth;
        camera.rect = cd.rect;
        camera.useOcclusionCulling = cd.useOcclusionCulling;
        camera.hdr = cd.hdr;
        camera.targetTexture = cd.targetTexture;
        camera.orthographic = cd.orthographic;
        camera.orthographicSize = cd.orthographicSize;
        camera.backgroundColor = cd.backgroundColor;
    }

    #endregion IComponentSerializer implementation
}

/// <summary>
/// Class SerializeRigidBody.
/// </summary>
[ComponentSerializerFor(typeof(Rigidbody))]
public class SerializeRigidBody : IComponentSerializer
{
    /// <summary>
    /// Class RigidBodyInfo.
    /// </summary>
    public class RigidBodyInfo
    {
        /// <summary>
        /// The is kinematic
        /// </summary>
        public bool isKinematic;
        /// <summary>
        /// The use gravity
        /// </summary>
        public bool useGravity,
            /// <summary>
            /// The freeze rotation
            /// </summary>
            freezeRotation,
            /// <summary>
            /// The detect collisions
            /// </summary>
            detectCollisions;
        /// <summary>
        /// The velocity
        /// </summary>
        public Vector3 velocity,
            /// <summary>
            /// The position
            /// </summary>
            position,
            /// <summary>
            /// The angular velocity
            /// </summary>
            angularVelocity,
            /// <summary>
            /// The center of mass
            /// </summary>
            centerOfMass,
            /// <summary>
            /// The inertia tensor
            /// </summary>
            inertiaTensor;
        /// <summary>
        /// The rotation
        /// </summary>
        public Quaternion rotation,
            /// <summary>
            /// The inertia tensor rotation
            /// </summary>
            inertiaTensorRotation;
        /// <summary>
        /// The drag
        /// </summary>
        public float drag,
            /// <summary>
            /// The angular drag
            /// </summary>
            angularDrag,
            /// <summary>
            /// The mass
            /// </summary>
            mass,
            /// <summary>
            /// The sleep threshold
            /// </summary>
            sleepThreshold,
            /// <summary>
            /// The maximum angular velocity
            /// </summary>
            maxAngularVelocity;
        /// <summary>
        /// The constraints
        /// </summary>
        public RigidbodyConstraints constraints;
        /// <summary>
        /// The collision detection mode
        /// </summary>
        public CollisionDetectionMode collisionDetectionMode;
        /// <summary>
        /// The interpolation
        /// </summary>
        public RigidbodyInterpolation interpolation;
        /// <summary>
        /// The solver iteration count
        /// </summary>
        public int solverIterationCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="RigidBodyInfo"/> class.
        /// </summary>
        public RigidBodyInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RigidBodyInfo"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public RigidBodyInfo(Rigidbody source)
        {
            isKinematic = source.isKinematic;
            useGravity = source.useGravity;
            freezeRotation = source.freezeRotation;
            detectCollisions = source.detectCollisions;
            velocity = source.velocity;
            position = source.position;
            rotation = source.rotation;
            angularVelocity = source.angularVelocity;
            centerOfMass = source.centerOfMass;
            inertiaTensor = source.inertiaTensor;
            inertiaTensorRotation = source.inertiaTensorRotation;
            drag = source.drag;
            angularDrag = source.angularDrag;
            mass = source.mass;
            sleepThreshold = source.sleepThreshold;
            maxAngularVelocity = source.maxAngularVelocity;
            constraints = source.constraints;
            collisionDetectionMode = source.collisionDetectionMode;
            interpolation = source.interpolation;
            solverIterationCount = source.solverIterations;
        }

        /// <summary>
        /// Configures the specified body.
        /// </summary>
        /// <param name="body">The body.</param>
        public void Configure(Rigidbody body)
        {
            body.isKinematic = true;
            body.freezeRotation = freezeRotation;
            body.useGravity = useGravity;
            body.detectCollisions = detectCollisions;
            if (centerOfMass != Vector3.zero)
            {
                body.centerOfMass = centerOfMass;
            }

            body.drag = drag;
            body.angularDrag = angularDrag;
            body.mass = mass;
            if (!rotation.Equals(zero))
            {
                body.rotation = rotation;
            }

            body.sleepThreshold = sleepThreshold;
            body.maxAngularVelocity = maxAngularVelocity;
            body.constraints = constraints;
            body.collisionDetectionMode = collisionDetectionMode;
            body.interpolation = interpolation;
            body.solverIterations = solverIterationCount;
            body.isKinematic = isKinematic;

            if (!isKinematic)
            {
                body.velocity = velocity;
                body.useGravity = useGravity;
                body.angularVelocity = angularVelocity;
                if (inertiaTensor != Vector3.zero)
                {
                    body.inertiaTensor = inertiaTensor;
                }
                if (!inertiaTensorRotation.Equals(zero))
                {
                    body.inertiaTensorRotation = inertiaTensorRotation;
                }
            }
        }
    }

    private static Quaternion zero = new Quaternion(0, 0, 0, 0);

    #region IComponentSerializer implementation

    /// <summary>
    /// Serialize the specified component to a byte array
    /// </summary>
    /// <param name="component">Component to be serialized</param>
    /// <returns>System.Byte[].</returns>
    public byte[] Serialize(Component component)
    {
        return UnitySerializer.Serialize(new RigidBodyInfo((Rigidbody)component));
    }

    /// <summary>
    /// Deserialize the specified data into the instance.
    /// </summary>
    /// <param name="data">The data that represents the component, produced by Serialize</param>
    /// <param name="instance">The instance to target</param>
    public void Deserialize(byte[] data, Component instance)
    {
        var info = UnitySerializer.Deserialize<RigidBodyInfo>(data);
        info.Configure((Rigidbody)instance);
        UnitySerializer.AddFinalAction(() =>
        {
            info.Configure((Rigidbody)instance);
        });
    }

    #endregion IComponentSerializer implementation
}

/// <summary>
/// Class RagePixelSupport.
/// </summary>
[SerializerPlugIn]
public class RagePixelSupport
{
    static RagePixelSupport()
    {
        new SerializePrivateFieldOfType("RagePixelSprite", "animationPingPongDirection");
        new SerializePrivateFieldOfType("RagePixelSprite", "myTime");
    }
}

//[ComponentSerializerFor(typeof(Renderer))]
/// <summary>
/// Class SerializeRenderer.
/// </summary>
[ComponentSerializerFor(typeof(MeshRenderer))]
public class SerializeRenderer : IComponentSerializer
{
    /// <summary>
    /// The store
    /// </summary>
    public static StoreMaterials Store;

    /// <summary>
    /// Class StoredInformation.
    /// </summary>
    public class StoredInformation
    {
        /// <summary>
        /// The enabled
        /// </summary>
        public bool Enabled;
        /// <summary>
        /// The materials
        /// </summary>
        public List<Material> materials = new List<Material>();
        /// <summary>
        /// The shadow casting mode
        /// </summary>
        public ShadowCastingMode shadowCastingMode;
        /// <summary>
        /// The receive shadows
        /// </summary>
        public bool receiveShadows;
        /// <summary>
        /// The light probe usage
        /// </summary>
        public LightProbeUsage lightProbeUsage;
        /// <summary>
        /// The reflection probe usage
        /// </summary>
        public ReflectionProbeUsage reflectionProbeUsage;
        /// <summary>
        /// The light probe proxy volume override
        /// </summary>
        public GameObject lightProbeProxyVolumeOverride;
        /// <summary>
        /// The probe anchor
        /// </summary>
        public Transform probeAnchor;
        /// <summary>
        /// The lightmap index
        /// </summary>
        public int lightmapIndex;
        /// <summary>
        /// The realtime lightmap index
        /// </summary>
        public int realtimeLightmapIndex;
        /// <summary>
        /// The lightmap scale offset
        /// </summary>
        public Vector4 lightmapScaleOffset;
        /// <summary>
        /// The realtime lightmap scale offset
        /// </summary>
        public Vector4 realtimeLightmapScaleOffset;
    }

    #region IComponentSerializer implementation

    /// <summary>
    /// Serialize the specified component to a byte array
    /// </summary>
    /// <param name="component">Component to be serialized</param>
    /// <returns>System.Byte[].</returns>
    public byte[] Serialize(Component component)
    {
        using (new UnitySerializer.SerializationSplitScope())
        {
            var renderer = (Renderer)component;
            var si = new StoredInformation();
            si.Enabled = renderer.enabled;
            if ((Store = renderer.GetComponent<StoreMaterials>()) != null)
                if (Application.isPlaying)
                    si.materials = renderer.materials.ToList();
                else
                    si.materials = renderer.sharedMaterials.ToList();
            si.shadowCastingMode = renderer.shadowCastingMode;
            si.receiveShadows = renderer.receiveShadows;
            si.lightProbeUsage = renderer.lightProbeUsage;
            si.reflectionProbeUsage = renderer.reflectionProbeUsage;
            si.lightProbeProxyVolumeOverride = renderer.lightProbeProxyVolumeOverride;
            si.probeAnchor = renderer.probeAnchor;
            si.lightmapIndex = renderer.lightmapIndex;
            si.realtimeLightmapIndex = renderer.realtimeLightmapIndex;
            si.lightmapScaleOffset = renderer.lightmapScaleOffset;
            si.realtimeLightmapScaleOffset = renderer.realtimeLightmapScaleOffset;
            var data = UnitySerializer.Serialize(si);
            Store = null;
            return data;
        }
    }

    /// <summary>
    /// Deserialize the specified data into the instance.
    /// </summary>
    /// <param name="data">The data that represents the component, produced by Serialize</param>
    /// <param name="instance">The instance to target</param>
    public void Deserialize(byte[] data, Component instance)
    {
        var renderer = (Renderer)instance;
        renderer.enabled = false;
        UnitySerializer.AddFinalAction(() =>
        {
            Store = renderer.GetComponent<StoreMaterials>();
            using (new UnitySerializer.SerializationSplitScope())
            {
                var si = UnitySerializer.Deserialize<StoredInformation>(data);
                if (si == null)
                {
                    Debug.LogError("An error occured when getting the stored information for a renderer");

                    return;
                }
                renderer.enabled = si.Enabled;
                if (Application.isPlaying)
                {
                    if (si.materials.Count > 0)
                        if (Store != null)
                            renderer.materials = si.materials.ToArray();
                }
                else
                {
                    if (si.materials.Count > 0)
                        if (Store != null)
                            renderer.sharedMaterials = si.materials.ToArray();
                }

                renderer.shadowCastingMode = si.shadowCastingMode;
                renderer.receiveShadows = si.receiveShadows;
                renderer.lightProbeUsage = si.lightProbeUsage;
                renderer.reflectionProbeUsage = si.reflectionProbeUsage;
                renderer.lightProbeProxyVolumeOverride = si.lightProbeProxyVolumeOverride;
                renderer.probeAnchor = si.probeAnchor;
                renderer.lightmapIndex = si.lightmapIndex;
                renderer.realtimeLightmapIndex = si.realtimeLightmapIndex;
                if (!renderer.isPartOfStaticBatch)
                    renderer.lightmapScaleOffset = si.lightmapScaleOffset;
                renderer.realtimeLightmapScaleOffset = si.realtimeLightmapScaleOffset;
            }
            Store = null;
        }
        );
    }

    #endregion IComponentSerializer implementation
}

/// <summary>
/// Class SerializeLineRenderer.
/// </summary>
[ComponentSerializerFor(typeof(LineRenderer))]
public class SerializeLineRenderer : IComponentSerializer
{
    /// <summary>
    /// The store
    /// </summary>
    public static StoreMaterials Store;

    /// <summary>
    /// Class StoredInformation.
    /// </summary>
    public class StoredInformation : SerializeRenderer.StoredInformation
    {
        /// <summary>
        /// The use world space
        /// </summary>
        public bool useWorldSpace;
    }

    #region IComponentSerializer implementation

    /// <summary>
    /// Serialize the specified component to a byte array
    /// </summary>
    /// <param name="component">Component to be serialized</param>
    /// <returns>System.Byte[].</returns>
    public byte[] Serialize(Component component)
    {
        using (new UnitySerializer.SerializationSplitScope())
        {
            var renderer = (LineRenderer)component;
            var si = new StoredInformation();
            si.Enabled = renderer.enabled;
            if ((Store = renderer.GetComponent<StoreMaterials>()) != null)
            {
                si.materials = renderer.materials.ToList();
            }
            si.shadowCastingMode = renderer.shadowCastingMode;
            si.receiveShadows = renderer.receiveShadows;
            si.lightProbeUsage = renderer.lightProbeUsage;
            si.reflectionProbeUsage = renderer.reflectionProbeUsage;
            si.lightProbeProxyVolumeOverride = renderer.lightProbeProxyVolumeOverride;
            si.probeAnchor = renderer.probeAnchor;
            si.lightmapIndex = renderer.lightmapIndex;
            si.realtimeLightmapIndex = renderer.realtimeLightmapIndex;
            si.lightmapScaleOffset = renderer.lightmapScaleOffset;
            si.realtimeLightmapScaleOffset = renderer.realtimeLightmapScaleOffset;
            si.useWorldSpace = renderer.useWorldSpace;
            var data = UnitySerializer.Serialize(si);
            Store = null;
            return data;
        }
    }

    /// <summary>
    /// Deserialize the specified data into the instance.
    /// </summary>
    /// <param name="data">The data that represents the component, produced by Serialize</param>
    /// <param name="instance">The instance to target</param>
    public void Deserialize(byte[] data, Component instance)
    {
        var renderer = (LineRenderer)instance;
        renderer.enabled = false;
        UnitySerializer.AddFinalAction(() =>
        {
            Store = renderer.GetComponent<StoreMaterials>();

            using (new UnitySerializer.SerializationSplitScope())
            {
                var si = UnitySerializer.Deserialize<StoredInformation>(data);
                if (si == null)
                {
                    Debug.LogError("An error occured when getting the stored information for a LineRenderer");

                    return;
                }
                renderer.enabled = si.Enabled;
                if (si.materials.Count > 0)
                {
                    if (Store != null)
                    {
                        renderer.materials = si.materials.ToArray();
                    }
                }
                renderer.shadowCastingMode = si.shadowCastingMode;
                renderer.receiveShadows = si.receiveShadows;
                renderer.lightProbeUsage = si.lightProbeUsage;
                renderer.reflectionProbeUsage = si.reflectionProbeUsage;
                renderer.lightProbeProxyVolumeOverride = si.lightProbeProxyVolumeOverride;
                renderer.probeAnchor = si.probeAnchor;
                renderer.lightmapIndex = si.lightmapIndex;
                renderer.realtimeLightmapIndex = si.realtimeLightmapIndex;
                if (!renderer.isPartOfStaticBatch)
                {
                    renderer.lightmapScaleOffset = si.lightmapScaleOffset;
                }
                renderer.realtimeLightmapScaleOffset = si.realtimeLightmapScaleOffset;
            }
            Store = null;
        }
        );
    }

    #endregion IComponentSerializer implementation
}

/// <summary>
/// Class SerializeTrailRenderer.
/// </summary>
[ComponentSerializerFor(typeof(TrailRenderer))]
public class SerializeTrailRenderer : IComponentSerializer
{
    /// <summary>
    /// The store
    /// </summary>
    public static StoreMaterials Store;

    /// <summary>
    /// Class StoredInformation.
    /// </summary>
    public class StoredInformation : SerializeRenderer.StoredInformation
    {
        /// <summary>
        /// The autodestruct
        /// </summary>
        public bool autodestruct;
        /// <summary>
        /// The start width
        /// </summary>
        public float startWidth;
        /// <summary>
        /// The end width
        /// </summary>
        public float endWidth;
        /// <summary>
        /// The time
        /// </summary>
        public float time;
    }

    #region IComponentSerializer implementation

    /// <summary>
    /// Serialize the specified component to a byte array
    /// </summary>
    /// <param name="component">Component to be serialized</param>
    /// <returns>System.Byte[].</returns>
    public byte[] Serialize(Component component)
    {
        using (new UnitySerializer.SerializationSplitScope())
        {
            var renderer = (TrailRenderer)component;
            var si = new StoredInformation();
            si.Enabled = renderer.enabled;
            if ((Store = renderer.GetComponent<StoreMaterials>()) != null)
            {
                si.materials = renderer.materials.ToList();
            }
            si.shadowCastingMode = renderer.shadowCastingMode;
            si.receiveShadows = renderer.receiveShadows;
            si.lightProbeUsage = renderer.lightProbeUsage;
            si.reflectionProbeUsage = renderer.reflectionProbeUsage;
            si.lightProbeProxyVolumeOverride = renderer.lightProbeProxyVolumeOverride;
            si.probeAnchor = renderer.probeAnchor;
            si.lightmapIndex = renderer.lightmapIndex;
            si.realtimeLightmapIndex = renderer.realtimeLightmapIndex;
            si.lightmapScaleOffset = renderer.lightmapScaleOffset;
            si.realtimeLightmapScaleOffset = renderer.realtimeLightmapScaleOffset;
            si.autodestruct = renderer.autodestruct;
            si.startWidth = renderer.startWidth;
            si.endWidth = renderer.endWidth;
            si.time = renderer.time;
            var data = UnitySerializer.Serialize(si);
            Store = null;
            return data;
        }
    }

    /// <summary>
    /// Deserialize the specified data into the instance.
    /// </summary>
    /// <param name="data">The data that represents the component, produced by Serialize</param>
    /// <param name="instance">The instance to target</param>
    public void Deserialize(byte[] data, Component instance)
    {
        var renderer = (TrailRenderer)instance;
        renderer.enabled = false;
        UnitySerializer.AddFinalAction(() =>
        {
            Store = renderer.GetComponent<StoreMaterials>();

            using (new UnitySerializer.SerializationSplitScope())
            {
                var si = UnitySerializer.Deserialize<StoredInformation>(data);
                if (si == null)
                {
                    Debug.LogError("An error occured when getting the stored information for a TrailRenderer");

                    return;
                }
                renderer.enabled = si.Enabled;
                if (si.materials.Count > 0)
                {
                    if (Store != null)
                    {
                        renderer.materials = si.materials.ToArray();
                    }
                }
                renderer.shadowCastingMode = si.shadowCastingMode;
                renderer.receiveShadows = si.receiveShadows;
                renderer.lightProbeUsage = si.lightProbeUsage;
                renderer.reflectionProbeUsage = si.reflectionProbeUsage;
                renderer.lightProbeProxyVolumeOverride = si.lightProbeProxyVolumeOverride;
                renderer.probeAnchor = si.probeAnchor;
                renderer.lightmapIndex = si.lightmapIndex;
                renderer.realtimeLightmapIndex = si.realtimeLightmapIndex;
                if (!renderer.isPartOfStaticBatch)
                {
                    renderer.lightmapScaleOffset = si.lightmapScaleOffset;
                }
                renderer.realtimeLightmapScaleOffset = si.realtimeLightmapScaleOffset;
                renderer.autodestruct = si.autodestruct;
                renderer.startWidth = si.startWidth;
                renderer.endWidth = si.endWidth;
                renderer.time = si.time;
            }
            Store = null;
        }
        );
    }

    #endregion IComponentSerializer implementation
}

/// <summary>
/// Class SerializeSkinnedMeshRenderer.
/// </summary>
[ComponentSerializerFor(typeof(SkinnedMeshRenderer))]
public class SerializeSkinnedMeshRenderer : IComponentSerializer
{
    /// <summary>
    /// The store
    /// </summary>
    public static StoreMaterials Store;

    /// <summary>
    /// Class StoredInformation.
    /// </summary>
    public class StoredInformation : SerializeRenderer.StoredInformation
    {
        /// <summary>
        /// The local bounds
        /// </summary>
        public Bounds localBounds;
        /// <summary>
        /// The quality
        /// </summary>
        public SkinQuality quality;
        /// <summary>
        /// The update when offscreen
        /// </summary>
        public bool updateWhenOffscreen;
    }

    #region IComponentSerializer implementation

    /// <summary>
    /// Serialize the specified component to a byte array
    /// </summary>
    /// <param name="component">Component to be serialized</param>
    /// <returns>System.Byte[].</returns>
    public byte[] Serialize(Component component)
    {
        using (new UnitySerializer.SerializationSplitScope())
        {
            var renderer = (SkinnedMeshRenderer)component;
            var si = new StoredInformation();
            si.Enabled = renderer.enabled;
            if ((Store = renderer.GetComponent<StoreMaterials>()) != null)
            {
                si.materials = renderer.materials.ToList();
            }
            si.shadowCastingMode = renderer.shadowCastingMode;
            si.receiveShadows = renderer.receiveShadows;
            si.lightProbeUsage = renderer.lightProbeUsage;
            si.reflectionProbeUsage = renderer.reflectionProbeUsage;
            si.lightProbeProxyVolumeOverride = renderer.lightProbeProxyVolumeOverride;
            si.probeAnchor = renderer.probeAnchor;
            si.lightmapIndex = renderer.lightmapIndex;
            si.realtimeLightmapIndex = renderer.realtimeLightmapIndex;
            si.lightmapScaleOffset = renderer.lightmapScaleOffset;
            si.realtimeLightmapScaleOffset = renderer.realtimeLightmapScaleOffset;
            si.localBounds = renderer.localBounds;
            si.quality = renderer.quality;
            si.updateWhenOffscreen = renderer.updateWhenOffscreen;
            var data = UnitySerializer.Serialize(si);
            Store = null;
            return data;
        }
    }

    /// <summary>
    /// Deserialize the specified data into the instance.
    /// </summary>
    /// <param name="data">The data that represents the component, produced by Serialize</param>
    /// <param name="instance">The instance to target</param>
    public void Deserialize(byte[] data, Component instance)
    {
        var renderer = (SkinnedMeshRenderer)instance;
        renderer.enabled = false;
        UnitySerializer.AddFinalAction(() =>
        {
            Store = renderer.GetComponent<StoreMaterials>();

            using (new UnitySerializer.SerializationSplitScope())
            {
                var si = UnitySerializer.Deserialize<StoredInformation>(data);
                if (si == null)
                {
                    Debug.LogError("An error occured when getting the stored information for a SkinnedMeshRenderer");

                    return;
                }
                renderer.enabled = si.Enabled;
                if (si.materials.Count > 0)
                {
                    if (Store != null)
                    {
                        renderer.materials = si.materials.ToArray();
                    }
                }
                renderer.shadowCastingMode = si.shadowCastingMode;
                renderer.receiveShadows = si.receiveShadows;
                renderer.lightProbeUsage = si.lightProbeUsage;
                renderer.reflectionProbeUsage = si.reflectionProbeUsage;
                renderer.lightProbeProxyVolumeOverride = si.lightProbeProxyVolumeOverride;
                renderer.probeAnchor = si.probeAnchor;
                renderer.lightmapIndex = si.lightmapIndex;
                renderer.realtimeLightmapIndex = si.realtimeLightmapIndex;
                if (!renderer.isPartOfStaticBatch)
                {
                    renderer.lightmapScaleOffset = si.lightmapScaleOffset;
                }
                renderer.realtimeLightmapScaleOffset = si.realtimeLightmapScaleOffset;
                renderer.localBounds = si.localBounds;
                renderer.quality = si.quality;
                renderer.updateWhenOffscreen = si.updateWhenOffscreen;
            }
            Store = null;
        }
        );
    }

    #endregion IComponentSerializer implementation
}

/// <summary>
/// Class SerializeAudioChorusFilter.
/// </summary>
[ComponentSerializerFor(typeof(AudioChorusFilter))]
public class SerializeAudioChorusFilter : IComponentSerializer
{
    /// <summary>
    /// Class StoredInformation.
    /// </summary>
    public class StoredInformation
    {
        /// <summary>
        /// The enabled
        /// </summary>
        public bool enabled;
        /// <summary>
        /// The delay
        /// </summary>
        public float delay;
        /// <summary>
        /// The depth
        /// </summary>
        public float depth;
        /// <summary>
        /// The dry mix
        /// </summary>
        public float dryMix;
        /// <summary>
        /// The rate
        /// </summary>
        public float rate;
        /// <summary>
        /// The wet mix1
        /// </summary>
        public float wetMix1;
        /// <summary>
        /// The wet mix2
        /// </summary>
        public float wetMix2;
        /// <summary>
        /// The wet mix3
        /// </summary>
        public float wetMix3;
    }

    #region IComponentSerializer implementation

    /// <summary>
    /// Serialize the specified component to a byte array
    /// </summary>
    /// <param name="component">Component to be serialized</param>
    /// <returns>System.Byte[].</returns>
    public byte[] Serialize(Component component)
    {
        using (new UnitySerializer.SerializationSplitScope())
        {
            var filter = (AudioChorusFilter)component;
            var si = new StoredInformation();
            si.enabled = filter.enabled;
            si.delay = filter.delay;
            si.depth = filter.depth;
            si.dryMix = filter.dryMix;
            si.rate = filter.rate;
            si.wetMix1 = filter.wetMix1;
            si.wetMix2 = filter.wetMix2;
            si.wetMix3 = filter.wetMix3;
            var data = UnitySerializer.Serialize(si);
            return data;
        }
    }

    /// <summary>
    /// Deserialize the specified data into the instance.
    /// </summary>
    /// <param name="data">The data that represents the component, produced by Serialize</param>
    /// <param name="instance">The instance to target</param>
    public void Deserialize(byte[] data, Component instance)
    {
        var filter = (AudioChorusFilter)instance;
        filter.enabled = false;
        UnitySerializer.AddFinalAction(() =>
        {
            using (new UnitySerializer.SerializationSplitScope())
            {
                var si = UnitySerializer.Deserialize<StoredInformation>(data);
                if (si == null)
                {
                    Debug.LogError("An error occured when getting the stored information for a AudioChorusFilter");

                    return;
                }
                filter.delay = si.delay;
                filter.depth = si.depth;
                filter.dryMix = si.dryMix;
                filter.rate = si.rate;
                filter.wetMix1 = si.wetMix1;
                filter.wetMix2 = si.wetMix2;
                filter.wetMix3 = si.wetMix3;
                filter.enabled = si.enabled;
            }
        }
        );
    }

    #endregion IComponentSerializer implementation
}

/// <summary>
/// Class SerializeAudioDistortionFilter.
/// </summary>
[ComponentSerializerFor(typeof(AudioDistortionFilter))]
public class SerializeAudioDistortionFilter : IComponentSerializer
{
    /// <summary>
    /// Class StoredInformation.
    /// </summary>
    public class StoredInformation
    {
        /// <summary>
        /// The enabled
        /// </summary>
        public bool enabled;
        /// <summary>
        /// The distortion level
        /// </summary>
        public float distortionLevel;
    }

    #region IComponentSerializer implementation

    /// <summary>
    /// Serialize the specified component to a byte array
    /// </summary>
    /// <param name="component">Component to be serialized</param>
    /// <returns>System.Byte[].</returns>
    public byte[] Serialize(Component component)
    {
        using (new UnitySerializer.SerializationSplitScope())
        {
            var filter = (AudioDistortionFilter)component;
            var si = new StoredInformation();
            si.enabled = filter.enabled;
            si.distortionLevel = filter.distortionLevel;
            var data = UnitySerializer.Serialize(si);
            return data;
        }
    }

    /// <summary>
    /// Deserialize the specified data into the instance.
    /// </summary>
    /// <param name="data">The data that represents the component, produced by Serialize</param>
    /// <param name="instance">The instance to target</param>
    public void Deserialize(byte[] data, Component instance)
    {
        var filter = (AudioDistortionFilter)instance;
        filter.enabled = false;
        UnitySerializer.AddFinalAction(() =>
        {
            using (new UnitySerializer.SerializationSplitScope())
            {
                var si = UnitySerializer.Deserialize<StoredInformation>(data);
                if (si == null)
                {
                    Debug.LogError("An error occured when getting the stored information for a AudioDistortionFilter");

                    return;
                }
                filter.distortionLevel = si.distortionLevel;
                filter.enabled = si.enabled;
            }
        }
        );
    }

    #endregion IComponentSerializer implementation
}

/// <summary>
/// Class SerializeAudioEchoFilter.
/// </summary>
[ComponentSerializerFor(typeof(AudioEchoFilter))]
public class SerializeAudioEchoFilter : IComponentSerializer
{
    /// <summary>
    /// Class StoredInformation.
    /// </summary>
    public class StoredInformation
    {
        /// <summary>
        /// The enabled
        /// </summary>
        public bool enabled;
        /// <summary>
        /// The decay ratio
        /// </summary>
        public float decayRatio;
        /// <summary>
        /// The delay
        /// </summary>
        public float delay;
        /// <summary>
        /// The dry mix
        /// </summary>
        public float dryMix;
        /// <summary>
        /// The wet mix
        /// </summary>
        public float wetMix;
    }

    #region IComponentSerializer implementation

    /// <summary>
    /// Serialize the specified component to a byte array
    /// </summary>
    /// <param name="component">Component to be serialized</param>
    /// <returns>System.Byte[].</returns>
    public byte[] Serialize(Component component)
    {
        using (new UnitySerializer.SerializationSplitScope())
        {
            var filter = (AudioEchoFilter)component;
            var si = new StoredInformation();
            si.enabled = filter.enabled;
            si.decayRatio = filter.decayRatio;
            si.delay = filter.delay;
            si.wetMix = filter.wetMix;
            var data = UnitySerializer.Serialize(si);
            return data;
        }
    }

    /// <summary>
    /// Deserialize the specified data into the instance.
    /// </summary>
    /// <param name="data">The data that represents the component, produced by Serialize</param>
    /// <param name="instance">The instance to target</param>
    public void Deserialize(byte[] data, Component instance)
    {
        var filter = (AudioEchoFilter)instance;
        filter.enabled = false;
        UnitySerializer.AddFinalAction(() =>
        {
            using (new UnitySerializer.SerializationSplitScope())
            {
                var si = UnitySerializer.Deserialize<StoredInformation>(data);
                if (si == null)
                {
                    Debug.LogError("An error occured when getting the stored information for a AudioEchoFilter");

                    return;
                }
                filter.decayRatio = si.decayRatio;
                filter.delay = si.delay;
                filter.wetMix = si.wetMix;
                filter.enabled = si.enabled;
            }
        }
        );
    }

    #endregion IComponentSerializer implementation
}

/// <summary>
/// Class SerializeAudioLowPassFilter.
/// </summary>
[ComponentSerializerFor(typeof(AudioLowPassFilter))]
public class SerializeAudioLowPassFilter : IComponentSerializer
{
    /// <summary>
    /// Class StoredInformation.
    /// </summary>
    public class StoredInformation
    {
        /// <summary>
        /// The enabled
        /// </summary>
        public bool enabled;
        /// <summary>
        /// The cutoff frequency
        /// </summary>
        public float cutoffFrequency;
        /// <summary>
        /// The lowpass resonance q
        /// </summary>
        public float lowpassResonanceQ;
    }

    #region IComponentSerializer implementation

    /// <summary>
    /// Serialize the specified component to a byte array
    /// </summary>
    /// <param name="component">Component to be serialized</param>
    /// <returns>System.Byte[].</returns>
    public byte[] Serialize(Component component)
    {
        using (new UnitySerializer.SerializationSplitScope())
        {
            var filter = (AudioLowPassFilter)component;
            var si = new StoredInformation();
            si.enabled = filter.enabled;
            si.cutoffFrequency = filter.cutoffFrequency;
            si.lowpassResonanceQ = filter.lowpassResonanceQ;
            var data = UnitySerializer.Serialize(si);
            return data;
        }
    }

    /// <summary>
    /// Deserialize the specified data into the instance.
    /// </summary>
    /// <param name="data">The data that represents the component, produced by Serialize</param>
    /// <param name="instance">The instance to target</param>
    public void Deserialize(byte[] data, Component instance)
    {
        var filter = (AudioLowPassFilter)instance;
        filter.enabled = false;
        UnitySerializer.AddFinalAction(() =>
        {
            using (new UnitySerializer.SerializationSplitScope())
            {
                var si = UnitySerializer.Deserialize<StoredInformation>(data);
                if (si == null)
                {
                    Debug.LogError("An error occured when getting the stored information for a AudioLowPassFilter");

                    return;
                }
                filter.cutoffFrequency = si.cutoffFrequency;
                filter.lowpassResonanceQ = si.lowpassResonanceQ;
                filter.enabled = si.enabled;
            }
        }
        );
    }

    #endregion IComponentSerializer implementation
}

/// <summary>
/// Class SerializeAudioHighPassFilter.
/// </summary>
[ComponentSerializerFor(typeof(AudioHighPassFilter))]
public class SerializeAudioHighPassFilter : IComponentSerializer
{
    /// <summary>
    /// Class StoredInformation.
    /// </summary>
    public class StoredInformation
    {
        /// <summary>
        /// The enabled
        /// </summary>
        public bool enabled;
        /// <summary>
        /// The cutoff frequency
        /// </summary>
        public float cutoffFrequency;
        /// <summary>
        /// The highpass resonance q
        /// </summary>
        public float highpassResonanceQ;
    }

    #region IComponentSerializer implementation

    /// <summary>
    /// Serialize the specified component to a byte array
    /// </summary>
    /// <param name="component">Component to be serialized</param>
    /// <returns>System.Byte[].</returns>
    public byte[] Serialize(Component component)
    {
        using (new UnitySerializer.SerializationSplitScope())
        {
            var filter = (AudioHighPassFilter)component;
            var si = new StoredInformation();
            si.enabled = filter.enabled;
            si.cutoffFrequency = filter.cutoffFrequency;
            si.highpassResonanceQ = filter.highpassResonanceQ;
            var data = UnitySerializer.Serialize(si);
            return data;
        }
    }

    /// <summary>
    /// Deserialize the specified data into the instance.
    /// </summary>
    /// <param name="data">The data that represents the component, produced by Serialize</param>
    /// <param name="instance">The instance to target</param>
    public void Deserialize(byte[] data, Component instance)
    {
        var filter = (AudioHighPassFilter)instance;
        filter.enabled = false;
        UnitySerializer.AddFinalAction(() =>
        {
            using (new UnitySerializer.SerializationSplitScope())
            {
                var si = UnitySerializer.Deserialize<StoredInformation>(data);
                if (si == null)
                {
                    Debug.LogError("An error occured when getting the stored information for a AudioHighPassFilter");

                    return;
                }
                filter.cutoffFrequency = si.cutoffFrequency;
                filter.highpassResonanceQ = si.highpassResonanceQ;
                filter.enabled = si.enabled;
            }
        }
        );
    }

    #endregion IComponentSerializer implementation
}

/// <summary>
/// Class SerializeEventSystem.
/// </summary>
[ComponentSerializerFor(typeof(EventSystem))]
public class SerializeEventSystem : IComponentSerializer
{
    /// <summary>
    /// Class StoredInformation.
    /// </summary>
    public class StoredInformation
    {
        /// <summary>
        /// The enabled
        /// </summary>
        public bool enabled;
        /// <summary>
        /// The first selected game object
        /// </summary>
        public GameObject firstSelectedGameObject;
        /// <summary>
        /// The pixel drag threshold
        /// </summary>
        public int pixelDragThreshold;
        /// <summary>
        /// The send navigation events
        /// </summary>
        public bool sendNavigationEvents;
    }

    #region IComponentSerializer implementation

    /// <summary>
    /// Serialize the specified component to a byte array
    /// </summary>
    /// <param name="component">Component to be serialized</param>
    /// <returns>System.Byte[].</returns>
    public byte[] Serialize(Component component)
    {
        using (new UnitySerializer.SerializationSplitScope())
        {
            var system = (EventSystem)component;
            var si = new StoredInformation();
            si.enabled = system.enabled;
            si.firstSelectedGameObject = system.firstSelectedGameObject;
            si.pixelDragThreshold = system.pixelDragThreshold;
            si.sendNavigationEvents = system.sendNavigationEvents;
            var data = UnitySerializer.Serialize(si);
            return data;
        }
    }

    /// <summary>
    /// Deserialize the specified data into the instance.
    /// </summary>
    /// <param name="data">The data that represents the component, produced by Serialize</param>
    /// <param name="instance">The instance to target</param>
    public void Deserialize(byte[] data, Component instance)
    {
        var system = (EventSystem)instance;
        system.enabled = false;
        UnitySerializer.AddFinalAction(() =>
        {
            using (new UnitySerializer.SerializationSplitScope())
            {
                var si = UnitySerializer.Deserialize<StoredInformation>(data);
                if (si == null)
                {
                    Debug.LogError("An error occured when getting the stored information for a EventSystem");

                    return;
                }
                system.firstSelectedGameObject = si.firstSelectedGameObject;
                system.pixelDragThreshold = si.pixelDragThreshold;
                system.sendNavigationEvents = si.sendNavigationEvents;
                system.enabled = si.enabled;
            }
        }
        );
    }

    #endregion IComponentSerializer implementation
}

/*[ComponentSerializerFor(typeof(SplineNode))]
public class SerializeSplineNode : IComponentSerializer
{
    public class StoredInformation
    {
        public Vector3 point = Vector3.zero;
        public Quaternion rot = Quaternion.identity;
        public Vector3 normal = Vector3.zero;
        public Vector3 tangent = Vector3.zero;
        public float time;
    }

    #region IComponentSerializer implementation

    public byte[] Serialize(Component component)
    {
        using (new UnitySerializer.SerializationSplitScope())
        {
            var system = (SplineNode)(object)component;
            var si = new StoredInformation();
            si.point = system.point;
            si.rot = system.rot;
            si.normal = system.normal;
            si.tangent = system.tangent;
            si.time = system.time;
            var data = UnitySerializer.Serialize(si);
            return data;
        }
    }

    public void Deserialize(byte[] data, Component instance)
    {
        var system = (SplineNode)(object)instance;
        UnitySerializer.AddFinalAction(() =>
        {
            using (new UnitySerializer.SerializationSplitScope())
            {
                var si = UnitySerializer.Deserialize<StoredInformation>(data);
                if (si == null)
                {
                    Debug.LogError("An error occured when getting the stored information for a EventSystem");

                    return;
                }
                system.point = si.point;
                system.rot = si.rot;
                system.normal = si.normal;
                system.tangent = si.tangent;
                system.time = si.time;
            }
        }
        );
    }

    #endregion IComponentSerializer implementation
}*/

/// <summary>
/// Class SerializeComponentReference.
/// </summary>
[SubTypeSerializer(typeof(Component))]
public class SerializeComponentReference : SerializerExtensionBase<Component>
{
    /// <summary>
    /// Saves the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public override IEnumerable<object> Save(Component target)
    {
        //Is this a reference to a prefab
        var assetId = SaveGameManager.Instance.GetAssetId(target);
        if (assetId.index != -1)
        {
            return new object[] { null, true, target.GetType().FullName, assetId };
        }

        var index = target.gameObject.GetComponents(target.GetType()).FindIndex(c => c == target);

        if (UniqueIdentifier.GetByName(target.gameObject.GetId()) != null)
        {
            return new object[] { target.gameObject.GetId(), true, target.GetType().FullName, "", index /* Identify a prefab */ };
        }
        else
        {
            return new object[] { target.gameObject.GetId(), false, target.GetType().FullName, "", index /* Identify a prefab */ };
        }
    }

    /// <summary>
    /// Loads the specified data.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="instance">The instance.</param>
    /// <returns>System.Object.</returns>
    public override object Load(object[] data, object instance)
    {
        if (!(bool)data[1])
            Debug.LogError("[[Disabled, will not be set]]");
        Debug.LogFormat("Component: {0}.{1}", data[0], data[2]);

        if (data[3] != null && data[3].GetType() == typeof(SaveGameManager.AssetReference))
        {
            return SaveGameManager.Instance.GetAsset((SaveGameManager.AssetReference)data[3]);
        }
        if (data.Length == 5)
        {
            return new UnitySerializer.DeferredSetter((d) =>
            {
                var item = UniqueIdentifier.GetByName((string)data[0]);
                if (item == null)
                {
                    Debug.LogError("Could not find reference to " + data[0] + " a " + (string)data[2]);

                    return null;
                }
                var allComponentsOfType = item.GetComponents(UnitySerializer.GetTypeEx(data[2]));
                if (allComponentsOfType.Length == 0)
                {
                    return null;
                }
                if (allComponentsOfType.Length <= (int)data[4])
                {
                    data[4] = 0;
                }

                return item != null ? allComponentsOfType[(int)data[4]] : null;
            })
            {
                enabled = (bool)data[1]
            };
        }
        return new UnitySerializer.DeferredSetter((d) =>
        {
            var item = UniqueIdentifier.GetByName((string)data[0]);
            return item != null ? item.GetComponent(UnitySerializer.GetTypeEx(data[2])) : null;
        })
        {
            enabled = (bool)data[1]
        };
    }
}

/// <summary>
/// Class ProvideAttributes.
/// </summary>
public class ProvideAttributes : IProvideAttributeList
{
    private string[] _attributes;
    /// <summary>
    /// All simple
    /// </summary>
    protected bool AllSimple = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProvideAttributes"/> class.
    /// </summary>
    /// <param name="attributes">The attributes.</param>
    public ProvideAttributes(string[] attributes)
        : this(attributes, true)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProvideAttributes"/> class.
    /// </summary>
    /// <param name="attributes">The attributes.</param>
    /// <param name="allSimple">if set to <c>true</c> [all simple].</param>
    public ProvideAttributes(string[] attributes, bool allSimple)
    {
        _attributes = attributes;
        AllSimple = allSimple;
    }

    #region IProvideAttributeList implementation

    /// <summary>
    /// Gets the attribute list.
    /// </summary>
    /// <param name="tp">The tp.</param>
    /// <returns>IEnumerable&lt;System.String&gt;.</returns>
    public IEnumerable<string> GetAttributeList(Type tp)
    {
        return _attributes;
    }

    #endregion IProvideAttributeList implementation

    #region IProvideAttributeList implementation

    /// <summary>
    /// Allows all simple.
    /// </summary>
    /// <param name="tp">The tp.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public virtual bool AllowAllSimple(Type tp)
    {
        return AllSimple;
    }

    #endregion IProvideAttributeList implementation
}

/// <summary>
/// Class ProvideCameraAttributes.
/// </summary>
[AttributeListProvider(typeof(Camera))]
public class ProvideCameraAttributes : ProvideAttributes
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProvideCameraAttributes"/> class.
    /// </summary>
    public ProvideCameraAttributes()
        : base(new string[0])
    {
    }
}

/// <summary>
/// Class ProviderTransformAttributes.
/// </summary>
[AttributeListProvider(typeof(Transform))]
public class ProviderTransformAttributes : ProvideAttributes
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProviderTransformAttributes"/> class.
    /// </summary>
    public ProviderTransformAttributes()
        : base(new string[] {
        "localPosition",
        "localRotation",
        "localScale"
    }, false)
    {
    }
}

/// <summary>
/// Class ProviderRectTransformAttributes.
/// </summary>
[AttributeListProvider(typeof(RectTransform))]
public class ProviderRectTransformAttributes : ProvideAttributes
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProviderRectTransformAttributes"/> class.
    /// </summary>
    public ProviderRectTransformAttributes()
        : base(new string[] {
        "anchoredPosition",
        "anchoredPosition3D",
        "anchorMax",
        "anchorMin",
        "offsetMax",
        "offsetMin",
        "pivot",
        "rect",
        "sizeDelta"
    }, false)
    {
    }
}

/// <summary>
/// Class ProvideColliderAttributes.
/// </summary>
[AttributeListProvider(typeof(Collider))]
public class ProvideColliderAttributes : ProvideAttributes
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProvideColliderAttributes"/> class.
    /// </summary>
    public ProvideColliderAttributes()
        : base(new string[] {
        "material",
        "active",
        "text",
        "mesh",
        "anchor",
        "targetPosition",
        "alignment",
        "lineSpacing",
        "spring",
        "useSpring",
        "motor",
        "useMotor",
        "limits",
        "useLimits",
        "axis",
        "breakForce",
        "breakTorque",
        "connectedBody",
        "offsetZ",
        "playAutomatically",
        "animatePhysics",
        "tabSize",
        "enabled",
        "isTrigger",
        "emit",
        "minSize",
        "maxSize",
        "clip",
        "loop",
        "playOnAwake",
        "bypassEffects",
        "volume",
        "priority",
        "pitch",
        "mute",
        "dopplerLevel",
        "spread",
        "panLevel",
        "volumeRolloff",
        "minDistance",
        "maxDistance",
        "pan2D",
        "castShadows",
        "receiveShadows",
        "slopeLimit",
        "stepOffset",
        "skinWidth",
        "minMoveDistance",
        "center",
        "radius",
        "height",
        "canControl",
        "damper",
        "useFixedUpdate",
        "movement",
        "jumping",
        "movingPlatform",
        "sliding",
        "autoRotate",
        "maxRotationSpeed",
        "range",
        "angle",
        "velocity",
        "intensity",
        "secondaryAxis",
        "xMotion",
        "yMotion",
        "zMotion",
        "angularXMotion",
        "angularYMotion",
        "angularZMotion",
        "linearLimit",
        "lowAngularXLimit",
        "highAngularXLimit",
        "angularYLimit",
        "angularZLimit",
        "targetVelocity",
        "xDrive",
        "yDrive",
        "zDrive",
        "targetAngularVelocity",
        "rotationDriveMode",
        "angularXDrive",
        "angularYZDrive",
        "slerpDrive",
        "projectionMode",
        "projectionDistance",
        "projectionAngle",
        "configuredInWorldSpace",
        "swapBodies",
        "cookie",
        "color",
        "drawHalo",
        "shadowType",
        "renderMode",
        "cullingMask",
        "lightmapping",
        "type",
        "lineSpacing",
        "text",
        "anchor",
        "alignment",
        "tabSize",
        "fontSize",
        "fontStyle",
        "font",
        "characterSize",
        "minEnergy",
        "maxEnergy",
        "minEmission",
        "maxEmission",
        "rndRotation",
        "rndVelocity",
        "rndAngularVelocity",
        "angularVelocity",
        "emitterVelocityScale",
        "localVelocity",
        "worldVelocity",
        "useWorldVelocity"
    }, false)
    {
    }
}

/// <summary>
/// Class ProviderRendererAttributes.
/// </summary>
[AttributeListProvider(typeof(Renderer))]
[AttributeListProvider(typeof(AudioListener))]
#pragma warning disable
[AttributeListProvider(typeof(ParticleEmitter))]
#pragma warning restore
[AttributeListProvider(typeof(Cloth))]
[AttributeListProvider(typeof(Light))]
[AttributeListProvider(typeof(Joint))]
[AttributeListProvider(typeof(MeshFilter))]
[AttributeListProvider(typeof(TextMesh))]
public class ProviderRendererAttributes : ProvideAttributes
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProviderRendererAttributes"/> class.
    /// </summary>
    public ProviderRendererAttributes()
        : base(new string[] {
        "active",
        "text",
        "anchor",
        "sharedMesh",
        "targetPosition",
        "alignment",
        "lineSpacing",
        "spring",
        "useSpring",
        "motor",
        "useMotor",
        "limits",
        "useLimits",
        "axis",
        "breakForce",
        "breakTorque",
        "connectedBody",
        "offsetZ",
        "playAutomatically",
        "animatePhysics",
        "tabSize",
        "enabled",
        "isTrigger",
        "emit",
        "minSize",
        "maxSize",
        "clip",
        "loop",
        "playOnAwake",
        "bypassEffects",
        "volume",
        "priority",
        "pitch",
        "mute",
        "dopplerLevel",
        "spread",
        "panLevel",
        "volumeRolloff",
        "minDistance",
        "maxDistance",
        "pan2D",
        "castShadows",
        "receiveShadows",
        "slopeLimit",
        "stepOffset",
        "skinWidth",
        "minMoveDistance",
        "center",
        "radius",
        "height",
        "canControl",
        "damper",
        "useFixedUpdate",
        "movement",
        "jumping",
        "movingPlatform",
        "sliding",
        "autoRotate",
        "maxRotationSpeed",
        "range",
        "angle",
        "velocity",
        "intensity",
        "secondaryAxis",
        "xMotion",
        "yMotion",
        "zMotion",
        "angularXMotion",
        "angularYMotion",
        "angularZMotion",
        "linearLimit",
        "lowAngularXLimit",
        "highAngularXLimit",
        "angularYLimit",
        "angularZLimit",
        "targetVelocity",
        "xDrive",
        "yDrive",
        "zDrive",
        "targetAngularVelocity",
        "rotationDriveMode",
        "angularXDrive",
        "angularYZDrive",
        "slerpDrive",
        "projectionMode",
        "projectionDistance",
        "projectionAngle",
        "configuredInWorldSpace",
        "swapBodies",
        "cookie",
        "color",
        "drawHalo",
        "shadowType",
        "renderMode",
        "cullingMask",
        "lightmapping",
        "type",
        "lineSpacing",
        "text",
        "anchor",
        "alignment",
        "tabSize",
        "fontSize",
        "fontStyle",
        "font",
        "characterSize",
        "minEnergy",
        "maxEnergy",
        "minEmission",
        "maxEmission",
        "rndRotation",
        "rndVelocity",
        "rndAngularVelocity",
        "angularVelocity",
        "emitterVelocityScale",
        "localVelocity",
        "worldVelocity",
        "useWorldVelocity"
    }, false)
    { }
}