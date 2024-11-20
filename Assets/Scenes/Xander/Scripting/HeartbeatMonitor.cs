using UnityEngine;
using TMPro; // Use TextMeshPro

public class HeartbeatMonitor : MonoBehaviour
{
    public TextMeshProUGUI heartbeatText; // Use TextMeshProUGUI for TMP text
    private int heartbeat = 100;

    void Start()
    {
        UpdateHeartbeatDisplay();
    }

    public void ModifyHeartbeat(int amount)
    {
        heartbeat += amount;
        heartbeat = Mathf.Clamp(heartbeat, 0, 100);
        UpdateHeartbeatDisplay();

        if (heartbeat < 40)
        {
            LoseGame();
        }
    }

    void UpdateHeartbeatDisplay()
    {
        heartbeatText.text = $"{heartbeat} BPM";
    }

    void LoseGame()
    {
        Debug.Log("You lost!");
        // Trigger end-game sequence here
    }
}
