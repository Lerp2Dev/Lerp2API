using System;
using System.Collections.Generic;

namespace Lerp2APIEditor.CustomDrawers
{
    static internal class MessengerInternal
    {
        /// <summary>
        /// The event table
        /// </summary>
        static public Dictionary<string, Delegate> eventTable = new Dictionary<string, Delegate>();

        /// <summary>
        /// The default mode
        /// </summary>
        static public readonly MessengerMode DEFAULT_MODE = MessengerMode.REQUIRE_LISTENER;

        /// <summary>
        /// Called when [listener adding].
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="listenerBeingAdded">The listener being added.</param>
        /// <exception cref="Lerp2APIEditor.CustomDrawers.MessengerInternal.ListenerException"></exception>
        static public void OnListenerAdding(string eventType, Delegate listenerBeingAdded)
        {
            if (!eventTable.ContainsKey(eventType))
            {
                eventTable.Add(eventType, null);
            }

            Delegate d = eventTable[eventType];
            if (d != null && d.GetType() != listenerBeingAdded.GetType())
            {
                throw new ListenerException(string.Format("Attempting to add listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being added has type {2}", eventType, d.GetType().Name, listenerBeingAdded.GetType().Name));
            }
        }

        /// <summary>
        /// Called when [listener removing].
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="listenerBeingRemoved">The listener being removed.</param>
        /// <exception cref="Lerp2APIEditor.CustomDrawers.MessengerInternal.ListenerException">
        /// </exception>
        static public void OnListenerRemoving(string eventType, Delegate listenerBeingRemoved)
        {
            if (eventTable.ContainsKey(eventType))
            {
                Delegate d = eventTable[eventType];

                if (d == null)
                {
                    throw new ListenerException(string.Format("Attempting to remove listener with for event type {0} but current listener is null.", eventType));
                }
                else if (d.GetType() != listenerBeingRemoved.GetType())
                {
                    throw new ListenerException(string.Format("Attempting to remove listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being removed has type {2}", eventType, d.GetType().Name, listenerBeingRemoved.GetType().Name));
                }
            }
            else
            {
                throw new ListenerException(string.Format("Attempting to remove listener for type {0} but Messenger doesn't know about this event type.", eventType));
            }
        }

        /// <summary>
        /// Called when [listener removed].
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        static public void OnListenerRemoved(string eventType)
        {
            if (eventTable[eventType] == null)
            {
                eventTable.Remove(eventType);
            }
        }

        /// <summary>
        /// Called when [broadcasting].
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="mode">The mode.</param>
        /// <exception cref="Lerp2APIEditor.CustomDrawers.MessengerInternal.BroadcastException"></exception>
        static public void OnBroadcasting(string eventType, MessengerMode mode)
        {
            if (mode == MessengerMode.REQUIRE_LISTENER && !eventTable.ContainsKey(eventType))
            {
                throw new MessengerInternal.BroadcastException(string.Format("Broadcasting message {0} but no listener found.", eventType));
            }
        }

        /// <summary>
        /// Creates the broadcast signature exception.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <returns>BroadcastException.</returns>
        static public BroadcastException CreateBroadcastSignatureException(string eventType)
        {
            return new BroadcastException(string.Format("Broadcasting message {0} but listeners have a different signature than the broadcaster.", eventType));
        }

        /// <summary>
        /// Class BroadcastException.
        /// </summary>
        /// <seealso cref="System.Exception" />
        public class BroadcastException : Exception
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="BroadcastException"/> class.
            /// </summary>
            /// <param name="msg">The MSG.</param>
            public BroadcastException(string msg)
                : base(msg)
            {
            }
        }

        /// <summary>
        /// Class ListenerException.
        /// </summary>
        /// <seealso cref="System.Exception" />
        public class ListenerException : Exception
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ListenerException"/> class.
            /// </summary>
            /// <param name="msg">The MSG.</param>
            public ListenerException(string msg)
                : base(msg)
            {
            }
        }
    }

    /// <summary>
    /// Class Messenger.
    /// </summary>
    static public class Messenger
    {
        private static Dictionary<string, Delegate> eventTable = MessengerInternal.eventTable;

        /// <summary>
        /// Adds the listener.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="handler">The handler.</param>
        static public void AddListener(string eventType, Callback handler)
        {
            MessengerInternal.OnListenerAdding(eventType, handler);
            eventTable[eventType] = (Callback)eventTable[eventType] + handler;
        }

        /// <summary>
        /// Removes the listener.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="handler">The handler.</param>
        static public void RemoveListener(string eventType, Callback handler)
        {
            MessengerInternal.OnListenerRemoving(eventType, handler);
            eventTable[eventType] = (Callback)eventTable[eventType] - handler;
            MessengerInternal.OnListenerRemoved(eventType);
        }

        /// <summary>
        /// Broadcasts the specified event type.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        static public void Broadcast(string eventType)
        {
            Broadcast(eventType, MessengerInternal.DEFAULT_MODE);
        }

        /// <summary>
        /// Broadcasts the specified event type.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="mode">The mode.</param>
        static public void Broadcast(string eventType, MessengerMode mode)
        {
            MessengerInternal.OnBroadcasting(eventType, mode);
            Delegate d;
            if (eventTable.TryGetValue(eventType, out d))
            {
                Callback callback = d as Callback;
                if (callback != null)
                {
                    callback();
                }
                else
                {
                    throw MessengerInternal.CreateBroadcastSignatureException(eventType);
                }
            }
        }
    }

    /// <summary>
    /// Class Messenger.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    static public class Messenger<T>
    {
        private static Dictionary<string, Delegate> eventTable = MessengerInternal.eventTable;

        /// <summary>
        /// Adds the listener.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="handler">The handler.</param>
        static public void AddListener(string eventType, Callback<T> handler)
        {
            MessengerInternal.OnListenerAdding(eventType, handler);
            eventTable[eventType] = (Callback<T>)eventTable[eventType] + handler;
        }

        /// <summary>
        /// Removes the listener.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="handler">The handler.</param>
        static public void RemoveListener(string eventType, Callback<T> handler)
        {
            MessengerInternal.OnListenerRemoving(eventType, handler);
            eventTable[eventType] = (Callback<T>)eventTable[eventType] - handler;
            MessengerInternal.OnListenerRemoved(eventType);
        }

        /// <summary>
        /// Broadcasts the specified event type.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="arg1">The arg1.</param>
        static public void Broadcast(string eventType, T arg1)
        {
            Broadcast(eventType, arg1, MessengerInternal.DEFAULT_MODE);
        }

        /// <summary>
        /// Broadcasts the specified event type.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="arg1">The arg1.</param>
        /// <param name="mode">The mode.</param>
        static public void Broadcast(string eventType, T arg1, MessengerMode mode)
        {
            MessengerInternal.OnBroadcasting(eventType, mode);
            Delegate d;
            if (eventTable.TryGetValue(eventType, out d))
            {
                Callback<T> callback = d as Callback<T>;
                if (callback != null)
                {
                    callback(arg1);
                }
                else
                {
                    throw MessengerInternal.CreateBroadcastSignatureException(eventType);
                }
            }
        }
    }

    /// <summary>
    /// Class Messenger.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    static public class Messenger<T, U>
    {
        private static Dictionary<string, Delegate> eventTable = MessengerInternal.eventTable;

        /// <summary>
        /// Adds the listener.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="handler">The handler.</param>
        static public void AddListener(string eventType, Callback<T, U> handler)
        {
            MessengerInternal.OnListenerAdding(eventType, handler);
            eventTable[eventType] = (Callback<T, U>)eventTable[eventType] + handler;
        }

        /// <summary>
        /// Removes the listener.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="handler">The handler.</param>
        static public void RemoveListener(string eventType, Callback<T, U> handler)
        {
            MessengerInternal.OnListenerRemoving(eventType, handler);
            eventTable[eventType] = (Callback<T, U>)eventTable[eventType] - handler;
            MessengerInternal.OnListenerRemoved(eventType);
        }

        /// <summary>
        /// Broadcasts the specified event type.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        static public void Broadcast(string eventType, T arg1, U arg2)
        {
            Broadcast(eventType, arg1, arg2, MessengerInternal.DEFAULT_MODE);
        }

        /// <summary>
        /// Broadcasts the specified event type.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        /// <param name="mode">The mode.</param>
        static public void Broadcast(string eventType, T arg1, U arg2, MessengerMode mode)
        {
            MessengerInternal.OnBroadcasting(eventType, mode);
            Delegate d;
            if (eventTable.TryGetValue(eventType, out d))
            {
                Callback<T, U> callback = d as Callback<T, U>;
                if (callback != null)
                {
                    callback(arg1, arg2);
                }
                else
                {
                    throw MessengerInternal.CreateBroadcastSignatureException(eventType);
                }
            }
        }
    }

    /// <summary>
    /// Class Messenger.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    /// <typeparam name="V"></typeparam>
    static public class Messenger<T, U, V>
    {
        private static Dictionary<string, Delegate> eventTable = MessengerInternal.eventTable;

        /// <summary>
        /// Adds the listener.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="handler">The handler.</param>
        static public void AddListener(string eventType, Callback<T, U, V> handler)
        {
            MessengerInternal.OnListenerAdding(eventType, handler);
            eventTable[eventType] = (Callback<T, U, V>)eventTable[eventType] + handler;
        }

        /// <summary>
        /// Removes the listener.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="handler">The handler.</param>
        static public void RemoveListener(string eventType, Callback<T, U, V> handler)
        {
            MessengerInternal.OnListenerRemoving(eventType, handler);
            eventTable[eventType] = (Callback<T, U, V>)eventTable[eventType] - handler;
            MessengerInternal.OnListenerRemoved(eventType);
        }

        /// <summary>
        /// Broadcasts the specified event type.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        /// <param name="arg3">The arg3.</param>
        static public void Broadcast(string eventType, T arg1, U arg2, V arg3)
        {
            Broadcast(eventType, arg1, arg2, arg3, MessengerInternal.DEFAULT_MODE);
        }

        /// <summary>
        /// Broadcasts the specified event type.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        /// <param name="arg3">The arg3.</param>
        /// <param name="mode">The mode.</param>
        static public void Broadcast(string eventType, T arg1, U arg2, V arg3, MessengerMode mode)
        {
            MessengerInternal.OnBroadcasting(eventType, mode);
            Delegate d;
            if (eventTable.TryGetValue(eventType, out d))
            {
                Callback<T, U, V> callback = d as Callback<T, U, V>;
                if (callback != null)
                {
                    callback(arg1, arg2, arg3);
                }
                else
                {
                    throw MessengerInternal.CreateBroadcastSignatureException(eventType);
                }
            }
        }
    }
}