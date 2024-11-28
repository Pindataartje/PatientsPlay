using UnityEngine;
using TMPro;

public class HeartbeatMonitor : MonoBehaviour
{
    public TextMeshProUGUI heartbeatText;
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

        Debug.Log($"{gameObject.name}'s heartbeat: {heartbeat} BPM");

        if (heartbeat <= 0)
        {
            LoseGame();
        }
    }

    void UpdateHeartbeatDisplay()
    {
        if (heartbeatText != null)
        {
            heartbeatText.text = $"{heartbeat} BPM";
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} does not have a heartbeatText assigned.");
        }
    }

    void LoseGame()
    {
        Debug.Log($"{gameObject.name} lost the game!");
        // Handle the end-game sequence here
    }
}
