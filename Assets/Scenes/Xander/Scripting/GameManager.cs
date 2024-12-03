using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Revolver revolver;
    public Transform revolverSpawnPoint; // Spawn location for the revolver
    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI aiHealthText;

    public AIController aiController;

    public bool IsPlayerTurn => isPlayerTurn;
    private bool isPlayerTurn = true;
    private int playerHealth = 100;
    private int aiHealth = 100;

    void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        if (revolver == null)
        {
            Debug.LogError("Revolver is not assigned!");
            return;
        }

        Debug.Log("Game started, player's turn!");
        revolver.SetupChambers();
        UpdateHealthUI();
        isPlayerTurn = true;
        StartTurn();
    }

    public void ProcessShot(bool targetIsAI, bool isLiveShot)
    {
        if (targetIsAI)
        {
            ModifyHealth(false, isLiveShot ? -20 : 0); // AI takes damage
            Debug.Log($"AI was shot. Result: {(isLiveShot ? "LIVE shot, 20 damage" : "BLANK shot, no damage")}");
        }
        else
        {
            ModifyHealth(true, isLiveShot ? -20 : 0); // Player takes damage
            Debug.Log($"Player was shot. Result: {(isLiveShot ? "LIVE shot, 20 damage" : "BLANK shot, no damage")}");
        }

        EndTurn();
    }

    public void EndTurn(bool playerGetsAnotherTurn = false)
    {
        if (revolver.bulletsFired >= revolver.totalBullets)
        {
            Debug.Log("All bullets fired! Starting a new round.");
            revolver.SetupChambers();
            isPlayerTurn = true; // Reset to player's turn
            StartTurn();
            return;
        }

        if (!playerGetsAnotherTurn)
        {
            isPlayerTurn = !isPlayerTurn; // Alternate turns
        }

        if (playerHealth <= 0)
        {
            Debug.Log("Player has lost!");
            return; // Trigger game-over logic
        }

        if (aiHealth <= 0)
        {
            Debug.Log("AI has lost!");
            return; // Trigger game-over logic
        }

        StartTurn();
    }

    private void StartTurn()
    {
        if (isPlayerTurn)
        {
            Debug.Log("Player's turn!");
            revolver.EnablePickup(true);
            revolver.canShoot = true;
        }
        else
        {
            Debug.Log("AI's turn!");
            revolver.EnablePickup(false);
            revolver.canShoot = false;
            aiController?.StartAITurn();
        }
    }

    public void ModifyHealth(bool isPlayer, int amount)
    {
        if (isPlayer)
        {
            playerHealth += amount;
            playerHealth = Mathf.Clamp(playerHealth, 0, 100);
            Debug.Log($"Player's health: {playerHealth}");
        }
        else
        {
            aiHealth += amount;
            aiHealth = Mathf.Clamp(aiHealth, 0, 100);
            Debug.Log($"AI's health: {aiHealth}");
        }

        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (playerHealthText != null)
        {
            playerHealthText.text = playerHealth.ToString();
        }

        if (aiHealthText != null)
        {
            aiHealthText.text = aiHealth.ToString();
        }
    }
}
