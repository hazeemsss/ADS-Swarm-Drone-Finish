using UnityEngine;
using TMPro;

public class FPSDisplay : MonoBehaviour
{
    private float deltaTime = 0.0f;
    private float updateInterval = 0.2f; // How often the FPS is updated (in seconds)

    [SerializeField] private TextMeshProUGUI fpsTitle; // Text element to display FPS

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f; // Smooths FPS measurement

        // Update FPS text at regular intervals
        updateInterval -= Time.deltaTime;
        if (updateInterval <= 0)
        {
            updateInterval = 0.2f; // Reset interval timer
            float fps = 1.0f / deltaTime; // Calculate FPS
            fpsTitle.text = $"FPS: {Mathf.RoundToInt(fps)}";
        }
    }
}
