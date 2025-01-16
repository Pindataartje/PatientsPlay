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

    private bool skipEnemyTurn = false; // Added for EMP Item

    [Header("UI Elements")]
    public GameObject[] chamberCircles; // The 6 UI circles representing chambers
    public TextMeshProUGUI liveBulletText; // Text to show live bullets count
    public TextMeshProUGUI roundText; // Text to show the current round number
    private int currentRound = 1; // Tracks the round number


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
        UpdateRoundAndUI();
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

            // Increment the round counter
            currentRound++;

            // Reset revolver chambers and update UI
            revolver.SetupChambers();
            UpdateRoundAndUI();

            // Handle turn assignment for the new round
            if (playerGetsAnotherTurn && isPlayerTurn)
            {
                isPlayerTurn = true; // Ensure the player gets the first turn of the new round
            }
            else
            {
                isPlayerTurn = !isPlayerTurn; // Alternate turns otherwise
            }

            StartTurn();
            return;
        }

        // Handle normal turn switching
        if (!playerGetsAnotherTurn)
        {
            if (skipEnemyTurn && !isPlayerTurn)
            {
                Debug.Log("Skipping enemy turn due to EMP!");
                skipEnemyTurn = false; // Reset flag after skipping
                isPlayerTurn = true; // Force it back to the player's turn
            }
            else
            {
                // Properly toggle turns
                isPlayerTurn = !isPlayerTurn;
                Debug.Log($"Turn switched. isPlayerTurn: {isPlayerTurn}");
            }
        }
        else
        {
            Debug.Log("Player gets another turn due to blank or EMP.");
        }

        // Check for game-over conditions
        if (playerHealth <= 0)
        {
            Debug.Log("Player has lost!");
            GameEndFade fade = FindAnyObjectByType<GameEndFade>();
            fade?.StartFade();
            return; // Trigger game-over logic
        }

        if (aiHealth <= 0)
        {
            Debug.Log("AI has lost!");
            GameEndFade fade = FindAnyObjectByType<GameEndFade>();
            fade?.StartFade();
            return; // Trigger game-over logic
        }

        // Update UI for live bullets (optional live bullet check after every turn)
        UpdateLiveBulletText();

        // Start the next turn
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

            // Trigger the "Hit" animation whenever AI takes damage
            if (amount < 0)
            {
                aiController?.animator?.SetTrigger("Hit");
            }

            // Trigger "Death" animation if AI's health reaches zero
            if (aiHealth <= 0)
            {
                Debug.Log("AI has lost!");
                aiController?.animator?.SetTrigger("Death");
            }
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

    private bool skipAITurn = false;

    public void SkipEnemyTurn()
    {
        skipAITurn = true;
        Debug.Log("Enemy's next turn will be skipped!");
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
            if (skipAITurn)
            {
                Debug.Log("Enemy turn skipped due to EMP effect!");
                skipAITurn = false; // Reset after skipping
                isPlayerTurn = true; // Give turn back to player
                StartTurn();
                return;
            }

            Debug.Log("AI's turn!");
            revolver.EnablePickup(false);
            revolver.canShoot = false;
            aiController?.StartAITurn();
        }
    }
    public void UpdateRoundAndUI()
    {
        // Update round text
        if (roundText != null)
        {
            roundText.text = $"Round {currentRound}";
        }

        // Reactivate all chamber circles ONLY at the start of a new round
        if (revolver.bulletsFired == 0)
        {
            for (int i = 0; i < chamberCircles.Length; i++)
            {
                if (chamberCircles[i] != null && !chamberCircles[i].activeSelf)
                {
                    Debug.Log($"Reactivating circle {i} for a new round.");
                    chamberCircles[i].SetActive(true);
                }
            }
        }

        // Update the live bullet count
        UpdateLiveBulletText();
    }


    public void UpdateLiveBulletText()
    {
        if (liveBulletText != null)
        {
            int liveBullets = revolver.GetRemainingBullets();
            liveBulletText.text = $"Live: {liveBullets}/{revolver.totalBullets}";
        }
    }


}
