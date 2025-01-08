using UnityEngine;
using System.Collections;

public class EMPItem : MonoBehaviour
{
    [Header("EMP Settings")]
    public AudioClip empActivationSound; // Optional activation sound
    public AudioClip empExplosionSound;  // Optional explosion sound
    public ParticleSystem empEffect;     // Optional particle effect

    private bool isActivated = false; // Prevent multiple activations

    public void Use()
    {
        if (isActivated) return; // Prevent duplicate activations

        isActivated = true; // Mark as activated
        Debug.Log("EMP Grenade activated! Detonation in 1 second...");

        // Play activation sound if assigned
        if (empActivationSound != null)
        {
            AudioSource.PlayClipAtPoint(empActivationSound, transform.position);
        } 

        // Start the activation sequence
        StartCoroutine(DetonateEMP());
    }

    private IEnumerator DetonateEMP()
    {
        // Optional particle effect after 0.5 seconds
        yield return new WaitForSeconds(0.5f);
        if (empEffect != null)
        {
            empEffect.Play(); // Play the one-shot particle effect
            Debug.Log("EMP particle effect triggered!");
        }

        // Continue waiting for the full 1-second delay
        yield return new WaitForSeconds(0.5f);

        Debug.Log("EMP Grenade detonated! Enemy's next turn will be skipped.");

        // Apply the SkipEnemyTurn function
        GameManager gameManager = FindAnyObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.SkipEnemyTurn(); // Ensure player's turn isn't overridden.
        }

        // Play explosion sound if assigned
        if (empExplosionSound != null)
        {
            AudioSource.PlayClipAtPoint(empExplosionSound, transform.position);
        }

        // Destroy the EMP grenade after use
        Destroy(gameObject);
    }
}
