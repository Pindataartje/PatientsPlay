using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public Revolver revolver;
    public Transform revolverSpawnPoint; // Spawn location for the revolver
    public Transform aiHandTarget; // Where the revolver should be placed when AI holds it
    public Transform aiHeadTarget; // Where the revolver should be aimed if AI shoots itself

    public HeartbeatMonitor playerMonitor;
    public HeartbeatMonitor aiMonitor;

    private bool isPlayerTurn = true;

    void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        Debug.Log("Game started, player's turn!");
        StartNewRound();
    }

    public void StartNewRound()
    {
        Debug.Log("Starting new round.");
        revolver.SetupChambers();
        StartTurn();
    }

    private void StartTurn()
    {
        if (isPlayerTurn)
        {
            Debug.Log("Player's turn!");
            revolver.EnablePickup(true); // Allow player to pick up the revolver
        }
        else
        {
            Debug.Log("AI's turn!");
            revolver.EnablePickup(false); // Prevent player from picking up the revolver
            StartCoroutine(AIUseRevolver());
        }
    }

    public void EndTurn()
    {
        isPlayerTurn = !isPlayerTurn;
        StartTurn();
    }

    private IEnumerator AIUseRevolver()
    {
        Debug.Log("AI is starting its turn...");

        // Wait for AI to "decide" what to do
        yield return new WaitForSeconds(1f);

        // Move revolver to AI's hand position
        if (aiHandTarget != null)
        {
            Debug.Log("AI picks up the revolver.");
            revolver.transform.position = aiHandTarget.position;
            revolver.transform.rotation = aiHandTarget.rotation;
        }
        else
        {
            Debug.LogError("AI hand target not set!");
            yield break; // Exit if no target for AI hand
        }

        // Wait before deciding who to shoot
        yield return new WaitForSeconds(2f);

        bool aiShootSelf = Random.value > 0.5f;

        if (aiShootSelf)
        {
            Debug.Log("AI decided to shoot itself.");
            if (aiHeadTarget != null)
            {
                revolver.transform.rotation = Quaternion.LookRotation(aiHeadTarget.position - revolver.transform.position);
            }
        }
        else
        {
            Debug.Log("AI decided to shoot the player.");
            revolver.transform.rotation = Quaternion.LookRotation(playerMonitor.transform.position - revolver.transform.position);
        }

        // Wait before firing
        yield return new WaitForSeconds(1f);

        revolver.canShoot = true; // Allow AI to shoot
        Debug.Log("AI fires the revolver...");
        revolver.FireAction(aiShootSelf); // AI fires

        // Disable shooting after firing
        revolver.canShoot = false;

        // Wait before placing revolver back
        yield return new WaitForSeconds(1f);

        // Place revolver back on the table
        PlaceRevolverBackOnTable();

        // End AI turn
        EndTurn();
    }

    private void PlaceRevolverBackOnTable()
    {
        if (revolverSpawnPoint != null)
        {
            revolver.transform.position = revolverSpawnPoint.position;
            revolver.transform.rotation = revolverSpawnPoint.rotation;
            Debug.Log("Revolver placed back on the table.");
            revolver.EnablePickup(false); // Prevent picking up until next round
        }
        else
        {
            Debug.LogError("Revolver spawn point not set!");
        }
    }
}
