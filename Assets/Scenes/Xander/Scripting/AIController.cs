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

            // Consume bullet immediately after firing
            Debug.Log("AI is consuming the bullet after firing.");
            revolver.ConsumeBullet();

            // Check if all bullets have been used
            if (revolver.bulletsFired >= revolver.totalBullets)
            {
                Debug.Log("All bullets fired by AI. Placing revolver back and starting a new round.");
                revolver.PlaceBackOnTable(); // Ensure revolver is reset to the table
                yield return new WaitForSeconds(1.0f);
                revolver.SetupChambers(); // Reset chambers for a new round

                // If it's still the AI's turn, start the turn again
                if (!gameManager.IsPlayerTurn)
                {
                    Debug.Log("AI starting the new round.");
                    StartAITurn();
                }
                else
                {
                    gameManager.EndTurn(); // Hand over the turn
                }

                yield break;
            }

            // Debug the result of the shot
            Debug.Log($"AI shot result: {(revolver.Chambers[revolver.CurrentChamber] ? "LIVE shot" : "BLANK shot")}");

            // Apply damage if the AI shot the player with a live bullet
            if (!shootSelf && revolver.Chambers[revolver.CurrentChamber])
            {
                gameManager.ModifyHealth(true, -20);
                Debug.Log("Player hit by AI with a LIVE bullet. 20 damage dealt.");
            }

            // Wait after shooting
            yield return new WaitForSeconds(1.5f);

            // Place revolver back on the table
            Debug.Log("AI is placing the revolver back on the table.");
            revolver.PlaceBackOnTable();

            // If AI shoots itself with a blank, give another turn
            if (shootSelf && !revolver.Chambers[revolver.CurrentChamber])
            {
                Debug.Log("AI shot itself with a BLANK. AI gets another turn.");
                yield return new WaitForSeconds(1.0f); // Small delay before restarting turn
                StartAITurn();
            }
            else
            {
                Debug.Log("AI turn ending normally.");
                gameManager.EndTurn(); // End AI turn
            }
        }
        else
        {
            Debug.LogError("AIController: Revolver or AIHandPoint is not assigned!");
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

