using UnityEngine;
using TMPro;

public class VacuumItem : MonoBehaviour
{
    [Header("Vacuum Settings")]
    public TextMeshProUGUI vacuumFeedbackText; // Optional UI Text feedback for bullet type

    public void Use()
    {
        Debug.Log("Vacuum Gun activated! Skipping current bullet...");

        Revolver revolver = FindAnyObjectByType<Revolver>();
        if (revolver != null)
        {
            // Check the current chamber
            bool isLive = revolver.Chambers[revolver.CurrentChamber];
            string bulletType = isLive ? "LIVE bullet" : "BLANK bullet";

            Debug.Log($"Vacuum Gun skipped a {bulletType}!");

            if (vacuumFeedbackText != null)
            {
                vacuumFeedbackText.text = $"Vacuum skipped a {bulletType}!";
            }

            // Skip the current bullet
            revolver.ConsumeBullet();
        }
        else
        {
            Debug.LogWarning("No revolver found! Vacuum action failed.");
        }

        // Destroy the item after use
        Destroy(gameObject);
    }
}
 