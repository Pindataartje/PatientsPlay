using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq; // Added to use OrderBy

public class GameManager : MonoBehaviour
{
    public Revolver revolver;
    public Transform revolverSpawnPoint; // Spawn location for the revolver
    public Transform[] playerItemSpawnPoints; // Spawn locations for player's items
    public Transform[] opponentItemSpawnPoints; // Spawn locations for opponent's items
    public GameObject[] itemPrefabs; // Array of item prefabs to spawn

    public int maxItemsPerRound = 3;
    public int maxTotalItems = 8;

    private bool isPlayerTurn = true;
    private int currentRound = 1;
    private List<GameObject> spawnedItems = new List<GameObject>(); // Keep track of spawned items

    void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        Debug.Log("Game started, player's turn!");
        SpawnRevolver();
        StartNewRound();
    }

    public void StartNewRound()
    {
        Debug.Log("Starting new round: " + currentRound);
        revolver.SetupChambers();
        ClearOldItems();
        SpawnItemsForPlayer();
        SpawnItemsForOpponent();
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
            Debug.Log("Opponent's turn!");
            revolver.EnablePickup(false); // Prevent player from picking up the revolver
            StartCoroutine(AIUseRevolver());
        }
    }

    public void EndTurn()
    {
        isPlayerTurn = !isPlayerTurn;
        StartTurn();
    }

    private void SpawnItemsForPlayer()
    {
        int itemsToSpawn = Mathf.Min(maxItemsPerRound, maxTotalItems - GetPlayerCurrentItems());
        for (int i = 0; i < itemsToSpawn; i++)
        {
            if (playerItemSpawnPoints[i].childCount == 0) // Check if spawn point is empty
            {
                int randomIndex = Random.Range(0, itemPrefabs.Length);
                GameObject spawnedItem = Instantiate(itemPrefabs[randomIndex], playerItemSpawnPoints[i].position, Quaternion.identity);
                spawnedItem.transform.parent = playerItemSpawnPoints[i]; // Set parent to keep track
                spawnedItems.Add(spawnedItem);
            }
        }
    }

    private void SpawnItemsForOpponent()
    {
        int itemsToSpawn = Mathf.Min(maxItemsPerRound, maxTotalItems - GetOpponentCurrentItems());
        for (int i = 0; i < itemsToSpawn; i++)
        {
            if (opponentItemSpawnPoints[i].childCount == 0) // Check if spawn point is empty
            {
                int randomIndex = Random.Range(0, itemPrefabs.Length);
                GameObject spawnedItem = Instantiate(itemPrefabs[randomIndex], opponentItemSpawnPoints[i].position, Quaternion.identity);
                spawnedItem.transform.parent = opponentItemSpawnPoints[i]; // Set parent to keep track
                spawnedItems.Add(spawnedItem);
            }
        }
    }

    private void ClearOldItems()
    {
        foreach (GameObject item in spawnedItems)
        {
            if (item != null)
            {
                Destroy(item);
            }
        }
        spawnedItems.Clear();
    }

    private int GetPlayerCurrentItems()
    {
        // Placeholder for logic to count player items, assuming items are tagged or tracked
        return playerItemSpawnPoints.Count(t => t.childCount > 0);
    }

    private int GetOpponentCurrentItems()
    {
        // Placeholder for logic to count opponent items, assuming items are tagged or tracked
        return opponentItemSpawnPoints.Count(t => t.childCount > 0);
    }

    private void SpawnRevolver()
    {
        if (revolverSpawnPoint != null && revolver != null)
        {
            revolver.transform.position = revolverSpawnPoint.position;
            revolver.transform.rotation = revolverSpawnPoint.rotation;
        }
    }

    private IEnumerator AIUseRevolver()
    {
        yield return new WaitForSeconds(2); // Delay before AI uses the revolver
        bool shotLands = revolver.Fire();
        if (shotLands)
        {
            Debug.Log("AI fired a live bullet!");
            // Handle AI live shot logic
        }
        else
        {
            Debug.Log("AI fired a blank!");
            // Handle AI blank shot logic
        }
        EndTurn();
    }
}

