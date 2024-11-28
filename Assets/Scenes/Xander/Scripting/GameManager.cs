using UnityEngine;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Revolver revolver;
    public Transform revolverSpawnPoint; // Spawn location for the revolver
    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI aiHealthText;

    public AIController aiController;

    private bool isPlayerTurn = true;
    private int playerHealth = 100;
    private int aiHealth = 100;

    void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        Debug.Log("Game started, player's turn!");
        revolver.SetupChambers();
        UpdateHealthUI();
        StartTurn();
    }

    private void StartTurn()
    {
        if (isPlayerTurn)
        {
            Debug.Log("Player's turn!");
            revolver.EnablePickup(true); // Allow player to pick up the revolver
            revolver.canShoot = true;
        }
        else
        {
            Debug.Log("AI's turn!");
            revolver.EnablePickup(false); // Prevent player from picking up the revolver
            revolver.canShoot = false;

            // Start AI's turn using AIController
            if (aiController != null)
            {
                aiController.StartAITurn();
            }
        }
    }

    public void EndTurn(bool playerGetsAnotherTurn = false)
    {
        if (!playerGetsAnotherTurn)
        {
            isPlayerTurn = !isPlayerTurn;
        }

        if (playerHealth <= 0)
        {
            Debug.Log("Player has lost!");
            return; // End game if player has lost
        }

        if (aiHealth <= 0)
        {
            Debug.Log("AI has lost!");
            return; // End game if AI has lost
        }

        StartTurn();
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
            playerHealthText.text = $"{playerHealth}";
        }

        if (aiHealthText != null)
        {
            aiHealthText.text = $"{aiHealth}";
        }
    }
}
