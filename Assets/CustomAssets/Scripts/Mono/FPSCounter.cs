using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text fpsText; // Reference to the TMP_Text UI element
    [SerializeField] private float updateInterval = 0.5f; // How often the FPS updates (in seconds)

    private float accumulatedTime = 0f; // Accumulated time for FPS calculation
    private int frameCount = 0;         // Frame count since the last update
    private float timeSinceLastUpdate = 0f; // Time since the last FPS update

    private void Update()
    {
        timeSinceLastUpdate += Time.unscaledDeltaTime;
        accumulatedTime += Time.unscaledDeltaTime;
        frameCount++;

        if (timeSinceLastUpdate >= updateInterval)
        {
            // Calculate FPS
            float fps = frameCount / accumulatedTime;

            // Update the TMP_Text with the FPS value
            fpsText.text = $"FPS: {fps:0.0}";

            // Reset the counters
            timeSinceLastUpdate = 0f;
            accumulatedTime = 0f;
            frameCount = 0;
        }
    }
}
