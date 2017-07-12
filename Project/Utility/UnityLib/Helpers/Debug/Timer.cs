// Timer: get time elapsed in milliseconds
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace UnityLibrary
{
    /// <summary>
    /// Class Timer.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class Timer : MonoBehaviour
    {
        private void Start()
        {
            // init and start timer
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // put your function here..
            for (int i = 0; i < 1000000; i++)
            {
                var tmp = "asdf" + 1.ToString();
            }

            // get results in ms
            stopwatch.Stop();
            Debug.LogFormat("Timer: {0} ms", stopwatch.ElapsedMilliseconds);
            stopwatch.Reset();
        }
    }
}