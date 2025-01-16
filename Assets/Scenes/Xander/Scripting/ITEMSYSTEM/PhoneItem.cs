using UnityEngine;
using TMPro;

public class PhoneItem : MonoBehaviour
{
    public TextMeshProUGUI phoneText;
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
        Debug.Log("Phone Item Used. Checking chamber...");
        Revolver revolver = FindAnyObjectByType<Revolver>();

        string result = revolver.Chambers[revolver.CurrentChamber] ? "LIVE BULLET" : "BLANK BULLET";
        Debug.Log(result);

        if (TextManager.Instance != null)
        {
            TextManager.Instance.ShowTemporaryText(phoneText, result, 4f);
        }

        Destroy(gameObject);
    }
}
