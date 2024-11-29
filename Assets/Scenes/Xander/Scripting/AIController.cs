using System.Collections;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public Revolver revolver;
    public Transform aiHandPoint; // Position where the revolver should be held by AI
    public Transform aimAtPlayerPoint; // Where AI aims when shooting at the player
    public Transform aimAtSelfPoint; // Where AI aims when shooting itself

    [Header("AI Sounds")]
    public AudioClip pickupSound;
    public AudioClip blankShotSound;
    public AudioClip liveShotSound;

    private GameManager gameManager;

    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }

    public void StartAITurn()
    {
        Debug.Log("AI's turn starting...");
        StartCoroutine(AITurnSequence());
    }

    public IEnumerator AITurnSequence()
    {
        if (revolver != null && aiHandPoint != null)
        {
            // Wait before picking up the revolver
            yield return new WaitForSeconds(2.0f);

            // AI picks up the revolver
            Debug.Log("AI is picking up the revolver.");
            revolver.transform.position = aiHandPoint.position;
            revolver.transform.rotation = aiHandPoint.rotation;

            Rigidbody rb = revolver.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }

            PlaySound(pickupSound); // Play pickup sound
            yield return new WaitForSeconds(1.0f);

            // Decide target
            bool shootSelf = DecideTarget();
            Transform aimTarget = shootSelf ? aimAtSelfPoint : aimAtPlayerPoint;

            // AI aims at the target
            if (aimTarget != null)
            {
                Debug.Log($"AI is aiming at {(shootSelf ? "itself" : "the player")}.");
                revolver.transform.LookAt(aimTarget.position);
                yield return new WaitForSeconds(1.5f);
            }

            // AI fires at the target
            Debug.Log($"AI is firing at {(shootSelf ? "itself" : "the player")}.");
            revolver.FireAction(shootSelf);

            // Handle bullet consumption explicitly
            Debug.Log($"AI shot a {(revolver.Chambers[revolver.CurrentChamber] ? "LIVE" : "BLANK")} bullet.");
            revolver.ConsumeBullet(); // Explicitly consume the bullet after shooting

            // Check the result of the shot
            bool isLive = revolver.Chambers[revolver.CurrentChamber];
            Debug.Log($"AI shot result: {(isLive ? "LIVE shot" : "BLANK shot")}");

            // Play the appropriate sound for the shot
            PlaySound(isLive ? liveShotSound : blankShotSound);

            // Apply damage if the AI shot the player with a live bullet
            if (!shootSelf && isLive)
            {
                gameManager.ModifyHealth(true, -20);
                Debug.Log("Player hit by AI with a LIVE bullet. 20 damage dealt.");
            }

            // Wait after shooting
            yield return new WaitForSeconds(1.5f);

            // Place revolver back on the table
            revolver.PlaceBackOnTable();

            // If AI shoots itself with a blank, give another turn
            if (shootSelf && !isLive)
            {
                Debug.Log("AI shot itself with a BLANK. AI gets another turn.");
                StartAITurn();
            }
            else
            {
                gameManager.EndTurn(); // End AI turn
            }
        }
    }

    private bool DecideTarget()
    {
        // Count blank and live bullets in the remaining chambers
        int blanks = 0, lives = 0;

        for (int i = revolver.CurrentChamber; i < revolver.Chambers.Length; i++)
        {
            if (revolver.Chambers[i]) lives++;
            else blanks++;
        }

        Debug.Log($"AI decision-making: {blanks} blanks, {lives} lives remaining.");

        // AI decides based on the blank-to-live ratio
        if (blanks > lives * 1.5f)
        {
            Debug.Log("AI decides to shoot itself (higher chance of blanks).");
            return true; // Shoot self
        }
        else
        {
            Debug.Log("AI decides to shoot the player (higher chance of lives).");
            return false; // Shoot player
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && revolver.audioSource != null)
        {
            revolver.audioSource.PlayOneShot(clip);
        }
    }
}
