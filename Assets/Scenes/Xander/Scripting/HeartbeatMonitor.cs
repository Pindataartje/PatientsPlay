using UnityEngine;
using TMPro; // Use TextMeshPro
using UnityEngine.UI;

public class HeartbeatMonitor : MonoBehaviour
{
    public TextMeshProUGUI heartbeatText; // Use TextMeshProUGUI for TMP text
    public AudioSource heartbeatAudioSource; // Audio source for heartbeat sound
    public AudioClip heartbeatSound; // Heartbeat sound clip
    public LineRenderer heartbeatLine; // Line renderer to display heartbeat visually

    private int heartbeat = 100;
    private float timeSinceLastBeat = 0f;
    private float beatInterval;
    private float beatLineAmplitude = 0.2f; // Height of the beat line peaks

    void Start()
    {
        UpdateHeartbeatDisplay();
        UpdateBeatInterval();
        DrawHeartbeatLine();
    }

    void Update()
    {
        timeSinceLastBeat += Time.deltaTime;
        if (timeSinceLastBeat >= beatInterval)
        {
            PlayHeartbeatSound();
            AnimateHeartbeatLine();
            timeSinceLastBeat = 0f;
        }
    }

    public void ModifyHeartbeat(int amount)
    {
        heartbeat += amount;
        heartbeat = Mathf.Clamp(heartbeat, 0, 100);
        UpdateHeartbeatDisplay();
        UpdateBeatInterval();

        if (heartbeat < 40)
        {
            LoseGame();
        }
    }

    void UpdateHeartbeatDisplay()
    {
        heartbeatText.text = $"{heartbeat} BPM";
    }

    void UpdateBeatInterval()
    {
        // The interval between beats is based on the heartbeat rate (in seconds per beat)
        beatInterval = 60f / heartbeat;
    }

    void PlayHeartbeatSound()
    {
        if (heartbeatAudioSource != null && heartbeatSound != null)
        {
            heartbeatAudioSource.PlayOneShot(heartbeatSound);
        }
    }

    void DrawHeartbeatLine()
    {
        // Set up the initial line points
        int pointsCount = 100;
        heartbeatLine.positionCount = pointsCount;
        for (int i = 0; i < pointsCount; i++)
        {
            float x = i * 0.1f;
            float y = Mathf.Sin(x) * beatLineAmplitude;
            heartbeatLine.SetPosition(i, new Vector3(x, y, 0));
        }
    }

    void AnimateHeartbeatLine()
    {
        // Animate the heartbeat line to create a visual peak when the heart beats
        int peakPoint = heartbeatLine.positionCount / 2;
        Vector3 peakPosition = heartbeatLine.GetPosition(peakPoint);
        peakPosition.y = beatLineAmplitude * 5f; // Create a peak
        heartbeatLine.SetPosition(peakPoint, peakPosition);

        // Reset the line back after a short duration
        Invoke(nameof(ResetHeartbeatLine), 0.1f);
    }

    void ResetHeartbeatLine()
    {
        DrawHeartbeatLine();
    }

    void LoseGame()
    {
        Debug.Log("You lost!");
        // Trigger end-game sequence here
    }
}
