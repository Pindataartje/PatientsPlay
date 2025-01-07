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

            PlaySound(pickupSound);
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
            revolver.ConsumeBullet();

            if (revolver.bulletsFired >= revolver.totalBullets)
            {
                Debug.Log("All bullets fired by AI. Starting a new round.");
                revolver.SetupChambers();
                gameManager.EndTurn();
                yield break;
            }

            yield return new WaitForSeconds(1.5f);
            revolver.PlaceBackOnTable();

            gameManager.EndTurn();
        }
    }

    private bool DecideTarget()
    {
        int blanks = 0, lives = 0;

        for (int i = revolver.CurrentChamber; i < revolver.Chambers.Length; i++)
        {
            if (revolver.Chambers[i]) lives++;
            else blanks++;
        }

        Debug.Log($"AI decision-making: {blanks} blanks, {lives} lives remaining.");

        return blanks > lives * 1.5f;
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && revolver.audioSource != null)
        {
            revolver.audioSource.PlayOneShot(clip);
        }
    }
}
