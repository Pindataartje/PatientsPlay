using UnityEngine;
using TMPro;

public class VacuumItem : MonoBehaviour
{
    [Header("Vacuum Settings")]
    public TextMeshProUGUI vacuumFeedbackText;

    public void Use()
    {
        Debug.Log("Vacuum Gun activated! Skipping current bullet...");

        Revolver revolver = FindAnyObjectByType<Revolver>();
        if (revolver != null)
        {
            bool isLive = revolver.Chambers[revolver.CurrentChamber];
            string bulletType = isLive ? "LIVE bullet" : "BLANK bullet";

            Debug.Log($"Vacuum Gun skipped a {bulletType}!");

            // Use TextManager to show and clear the text
            if (TextManager.Instance != null)
            {
                TextManager.Instance.ShowTemporaryText(vacuumFeedbackText, $"Vacuum skipped a {bulletType}!", 4f);
            }

            revolver.ConsumeBullet();
        }
        else
        {
            Debug.LogWarning("No revolver found! Vacuum action failed.");
        }

        Destroy(gameObject);
    }
}
 