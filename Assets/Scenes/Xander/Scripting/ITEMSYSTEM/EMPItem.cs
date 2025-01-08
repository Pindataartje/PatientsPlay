using UnityEngine;

public class EMPItem : MonoBehaviour
{
    [Header("EMP Settings")]
    public AudioClip empExplosionSound; // Optional sound effect

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object it collided with has the "Enemy" tag
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("EMP Grenade hit the enemy! Skipping their next turn.");

            // Find the GameManager and trigger the SkipEnemyTurn function
            GameManager gameManager = FindAnyObjectByType<GameManager>();
            if (gameManager != null)
            {
                gameManager.SkipEnemyTurn();
            }

            // Play explosion sound if assigned
            if (empExplosionSound != null)
            {
                AudioSource.PlayClipAtPoint(empExplosionSound, transform.position);
            }

            // Destroy the EMP object after collision
            Destroy(gameObject);
        }
    }
}
