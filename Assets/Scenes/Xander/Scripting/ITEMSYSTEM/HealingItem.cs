using UnityEngine;

public class HealingItem : MonoBehaviour
{
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
        Debug.Log("Healing Item Used. +20 HP!");
        GameManager gameManager = FindAnyObjectByType<GameManager>();
        gameManager.ModifyHealth(true, 20);
        Destroy(gameObject);
    }
}
