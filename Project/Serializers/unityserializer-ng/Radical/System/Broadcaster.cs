using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

/// <summary>
/// Class Broadcaster.
/// </summary>
public static class Broadcaster
{
    /// <summary>
    /// Class Pair.
    /// </summary>
    public class Pair
    {
        /// <summary>
        /// The target
        /// </summary>
        public WeakReference target;
        /// <summary>
        /// The interest
        /// </summary>
        public WeakReference interest;
    }

    private static List<Pair> InterestList = new List<Pair>();

    /// <summary>
    /// Registers the interest.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="interestedParty">The interested party.</param>
    public static void RegisterInterest(this object target, object interestedParty)
    {
        Cleanup();
        InterestList.Add(new Pair() { target = new WeakReference(target), interest = new WeakReference(interestedParty) });
    }

    /// <summary>
    /// Unregisters the interest.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="interestedParty">The interested party.</param>
    public static void UnregisterInterest(this object target, object interestedParty)
    {
        Cleanup();
        InterestList.Remove(InterestList.FirstOrDefault(p => p.target.Target == target && p.interest.Target == interestedParty));
    }

    /// <summary>
    /// Broadcasts the specified message.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="message">The message.</param>
    public static void Broadcast(this object obj, string message)
    {
        Cleanup();

        foreach (var m in InterestList.Where(p => p.target.Target == obj).Select(p => p.interest).Where(r => r.IsAlive).ToList())
        {
            var mth = m.Target.GetType().GetMethod(message, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (mth.GetParameters().Length == 1)
            {
                mth.Invoke(m.Target, new[] { obj });
            }
            else
            {
                mth.Invoke(m.Target, null);
            }
        }
    }

    private static void Cleanup()
    {
        var list = InterestList.Where(k => !k.target.IsAlive || !k.interest.IsAlive).ToList();
        foreach (var e in list)
            InterestList.Remove(e);
    }
}