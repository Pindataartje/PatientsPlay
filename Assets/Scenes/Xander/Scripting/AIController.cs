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
        if (!gameManager.IsPlayerTurn)
        {
            Debug.Log("AI's turn starting...");
            StartCoroutine(AITurnSequence());
        }
        else
        {
            Debug.Log("AI turn skipped due to EMP!");
        }
    }

     
    public IEnumerator AITurnSequence()
    {
        if (revolver != null && aiHandPoint != null)
        {
            yield return new WaitForSeconds(2.0f);

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

            bool shootSelf = DecideTarget();
            Transform aimTarget = shootSelf ? aimAtSelfPoint : aimAtPlayerPoint;

            if (aimTarget != null)
            {
                Debug.Log($"AI is aiming at {(shootSelf ? "itself" : "the player")}.");
                revolver.transform.LookAt(aimTarget.position);
                yield return new WaitForSeconds(1.5f);
            }

            Debug.Log($"AI is firing at {(shootSelf ? "itself" : "the player")}.");
            revolver.FireAction(shootSelf);

            // Play sound effect based on live or blank shot
            bool isLiveShot = revolver.Chambers[revolver.CurrentChamber];
            PlaySound(isLiveShot ? liveShotSound : blankShotSound);

            Debug.Log($"AI shot result: {(isLiveShot ? "LIVE shot" : "BLANK shot")}.");

            revolver.ConsumeBullet();

            if (revolver.bulletsFired >= revolver.totalBullets)
            {
                Debug.Log("All bullets fired by AI. Placing revolver back and starting a new round.");
                revolver.PlaceBackOnTable();
                yield return new WaitForSeconds(1.0f);
                revolver.SetupChambers();

                if (!gameManager.IsPlayerTurn)
                {
                    Debug.Log("AI starting the new round.");
                    StartAITurn();
                }
                else
                {
                    gameManager.EndTurn();
                }

                yield break;
            }

            if (!shootSelf && isLiveShot)
            {
                gameManager.ModifyHealth(true, -20);
                Debug.Log("Player hit by AI with a LIVE bullet. 20 damage dealt.");
            }

            yield return new WaitForSeconds(1.5f);

            Debug.Log("AI is placing the revolver back on the table.");
            revolver.PlaceBackOnTable();

            if (shootSelf && !isLiveShot)
            {
                Debug.Log("AI shot itself with a BLANK. AI gets another turn.");
                yield return new WaitForSeconds(1.0f);
                StartAITurn();
            }
            else
            {
                Debug.Log("AI turn ending normally.");
                gameManager.EndTurn();
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
