using UnityEngine;
using TMPro;

public class VacuumItem : MonoBehaviour
{
    [Header("Vacuum Settings")]
    public TextMeshProUGUI vacuumFeedbackText;
    public Transform originalPosition; // Assign the original position in the Inspector

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            ResetPositionAndVelocity();
        }
    }

    private void ResetPositionAndVelocity()
    {
        if (originalPosition != null)
        {
            Debug.Log($"{gameObject.name} hit the ground. Resetting to original position...");
            transform.position = originalPosition.position;
            transform.rotation = originalPosition.rotation;

            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
        else
        {
            Debug.LogWarning($"Original position for {gameObject.name} is not set!");
        }
    }

    public void Use()
    {
        Debug.Log("Vacuum Gun activated! Skipping current bullet...");

        Revolver revolver = FindAnyObjectByType<Revolver>();
        if (revolver != null)
        {
            bool isLive = revolver.Chambers[revolver.CurrentChamber];
            string bulletType = isLive ? "LIVE bullet" : "BLANK bullet";

            Debug.Log($"Vacuum Gun skipped a {bulletType}!");

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
