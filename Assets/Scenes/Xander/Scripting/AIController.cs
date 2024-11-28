using System.Collections;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public Revolver revolver;
    public Transform aiHandPoint; // Position where the revolver should be held by AI

    private GameManager gameManager;

    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }

    public void StartAITurn()
    {
        Debug.Log("AI's turn starting.");
        StartCoroutine(AITurnSequence());
    }

    private IEnumerator AITurnSequence()
    {
        // Move revolver to AI's hand
        if (revolver != null && aiHandPoint != null)
        {
            Debug.Log("AI is picking up the revolver.");
            revolver.transform.position = aiHandPoint.position;
            revolver.transform.rotation = aiHandPoint.rotation;
            revolver.PlayPickupSound();

            // Wait before shooting
            yield return new WaitForSeconds(2);
            Debug.Log("AI is ready to shoot.");
        }

        // Randomly decide whether AI shoots itself or the player
        bool shootSelf = Random.value > 0.5f;

        if (shootSelf)
        {
            Debug.Log("AI decides to shoot itself.");
            revolver.FireAction(true);
        }
        else
        {
            Debug.Log("AI decides to shoot the player.");
            revolver.FireAction(false);
        }

        // Wait 2 seconds after shooting
        yield return new WaitForSeconds(2);
        gameManager.EndTurn();
    }
}
