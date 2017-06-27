using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Class ObjectPool.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ObjectPool<T> where T : new()
{
    private readonly Stack<T> m_Stack = new Stack<T>();
    private readonly UnityAction<T> m_ActionOnGet;
    private readonly UnityAction<T> m_ActionOnRelease;

    /// <summary>
    /// Gets the count all.
    /// </summary>
    /// <value>The count all.</value>
    public int countAll { get; private set; }

    /// <summary>
    /// Gets the count active.
    /// </summary>
    /// <value>The count active.</value>
    public int countActive { get { return countAll - countInactive; } }

    /// <summary>
    /// Gets the count inactive.
    /// </summary>
    /// <value>The count inactive.</value>
    public int countInactive { get { return m_Stack.Count; } }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectPool{T}"/> class.
    /// </summary>
    /// <param name="actionOnGet">The action on get.</param>
    /// <param name="actionOnRelease">The action on release.</param>
    public ObjectPool(UnityAction<T> actionOnGet, UnityAction<T> actionOnRelease)
    {
        m_ActionOnGet = actionOnGet;
        m_ActionOnRelease = actionOnRelease;
    }

    /// <summary>
    /// Gets this instance.
    /// </summary>
    /// <returns>T.</returns>
    public T Get()
    {
        T element;
        if (m_Stack.Count == 0)
        {
            element = new T();
            countAll++;
        }
        else
        {
            element = m_Stack.Pop();
        }
        if (m_ActionOnGet != null)
            m_ActionOnGet(element);
        return element;
    }

    /// <summary>
    /// Releases the specified element.
    /// </summary>
    /// <param name="element">The element.</param>
    public void Release(T element)
    {
        if (m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element))
            Debug.LogError("Internal error. Trying to destroy object that is already released to pool.");
        if (m_ActionOnRelease != null)
            m_ActionOnRelease(element);
        m_Stack.Push(element);
    }
}