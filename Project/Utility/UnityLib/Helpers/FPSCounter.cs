using UnityEngine;

/// <summary>
/// Class FPSCounter.
/// </summary>
public class FPSCounter : MonoBehaviour
{
    [SerializeField]
    private float updateInterval = 0.1f;

    private float accum = 0.0f;
    private int frames = 0;
    private float timeleft;

    private int qty;

    private float fps;
    private float avgFps;

    private void Update()
    {
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;

        if (timeleft <= 0.0)
        {
            fps = (accum / frames);
            timeleft = updateInterval;
            accum = 0f;
            frames = 0;
        }

        qty++;

        avgFps += (fps - avgFps) / qty;
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(Screen.width - 150, 0, 150, 20), "FPS: " + fps.ToString("f2"));
        GUI.Label(new Rect(Screen.width - 150, 20, 150, 20), "Avg FPS: " + avgFps.ToString("f2"));
    }
}