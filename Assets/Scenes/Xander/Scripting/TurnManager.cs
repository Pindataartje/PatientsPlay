using UnityEngine;
using System.Collections;

public class TurnManager : MonoBehaviour
{
    public Revolver revolver;
    public HeartbeatMonitor playerMonitor;
    public HeartbeatMonitor aiMonitor;

    private bool isPlayerTurn = true;

    void Update()
    {
        // Player turn triggered by pressing the primary button on the right controller
        if (isPlayerTurn && Input.GetKeyDown(KeyCode.Space)) // Replace OVRInput with a simple key press for testing
        {
            PlayerTurn();
        }
    }

    public void StartPlayerTurn()
    {
        isPlayerTurn = true;
        Debug.Log("Player's turn started");
    }

    public void PlayerTurn()
    {
        if (isPlayerTurn)
        {
            bool shotLands = revolver.Fire();
            if (shotLands)
            {
                playerMonitor.ModifyHeartbeat(-60); // Live shot effect
                EndTurn();
            }
            else
            {
                // Blank shot
                EndTurn(true); // Give player another turn
            }
        }
    }

    public void EndTurn(bool keepTurn = false)
    {
        if (!keepTurn)
        {
            isPlayerTurn = !isPlayerTurn;
            if (!isPlayerTurn)
            {
                StartCoroutine(AITurn());
            }
        }
    }

    private IEnumerator AITurn()
    {
        yield return new WaitForSeconds(2); // Delay for AI action
        bool shotLands = revolver.Fire();
        if (shotLands)
        {
            aiMonitor.ModifyHeartbeat(-60); // AI shot effect
        }
        EndTurn();
    }
}
