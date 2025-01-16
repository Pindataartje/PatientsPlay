using UnityEngine;
using System.Collections;

public class EMPItem : MonoBehaviour
{
    [Header("EMP Settings")]
    public AudioClip empActivationSound;
    public AudioClip empExplosionSound;
    public ParticleSystem empEffect;

    public Transform originalPosition; // Assign the original position in the Inspector
    private Rigidbody rb;
    private bool isActivated = false;

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
        if (isActivated) return;

        isActivated = true;
        Debug.Log("EMP Grenade activated! Detonation in 1 second...");

        if (empActivationSound != null)
        {
            AudioSource.PlayClipAtPoint(empActivationSound, transform.position);
        }

        StartCoroutine(DetonateEMP());
    }

    private IEnumerator DetonateEMP()
    {
        yield return new WaitForSeconds(0.5f);
        if (empEffect != null)
        {
            empEffect.Play();
            Debug.Log("EMP particle effect triggered!");
        }

        yield return new WaitForSeconds(0.5f);

        Debug.Log("EMP Grenade detonated! Enemy's next turn will be skipped.");

        GameManager gameManager = FindAnyObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.SkipEnemyTurn();
        }

        if (empExplosionSound != null)
        {
            AudioSource.PlayClipAtPoint(empExplosionSound, transform.position);
        }

        Destroy(gameObject);
    }
}
