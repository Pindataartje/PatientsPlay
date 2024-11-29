using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Revolver : MonoBehaviour
{
    [Header("Revolver Settings")]
    public int totalBullets = 6;
    private bool[] chambers; // Internal chamber state
    private int currentChamber; // Internal current chamber index
    public int bulletsFired;

    [Header("Shooting Logic")]
    public Collider shootingTrigger;
    public bool canShoot = false;

    [Header("Audio Clips")]
    public AudioClip blankShotSound;
    public AudioClip liveShotSound;
    public AudioClip reloadSound;
    public AudioClip pickupSound;

    [Header("References")]
    public AudioSource audioSource;
    public XRGrabInteractable grabInteractable;
    public GameManager gameManager;
    public Animator animator;

    // Public properties for AI access
    public bool[] Chambers => chambers; // Read-only access to chambers
    public int CurrentChamber => currentChamber; // Read-only access to current chamber

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        grabInteractable = GetComponent<XRGrabInteractable>();
        gameManager = FindAnyObjectByType<GameManager>();
        animator = GetComponent<Animator>();

        SetupChambers();
        grabInteractable.selectEntered.AddListener(OnPickup);
    }

    public void FireAction(bool aiShootSelf = false)
    {
        if (!canShoot)
        {
            Debug.LogWarning("Cannot shoot! Not your turn.");
            return;
        }

        if (bulletsFired >= totalBullets)
        {
            Debug.LogWarning("All bullets fired! Start a new round.");
            return;
        }

        if (chambers == null || chambers.Length == 0)
        {
            Debug.LogWarning("Revolver is not loaded.");
            return;
        }

        // Play shoot animation
        animator?.SetTrigger("Shoot");

        // Check current chamber for live or blank bullet
        bool isLive = chambers[currentChamber];
        audioSource?.PlayOneShot(isLive ? liveShotSound : blankShotSound);
        Debug.Log(isLive ? "Shot was LIVE." : "Shot was BLANK.");

        // Use the correct trigger collider to determine the target
        string target = "None";
        Collider[] hitTargets = Physics.OverlapBox(
            shootingTrigger.bounds.center,
            shootingTrigger.bounds.extents,
            shootingTrigger.transform.rotation
        );

        foreach (var hit in hitTargets)
        {
            Debug.Log($"Hit detected: {hit.name} with tag {hit.tag}");

            if (aiShootSelf && hit.CompareTag("Enemy"))
            {
                target = "AI";
            }
            else if (!aiShootSelf && hit.CompareTag("Player"))
            {
                target = "Player";
            }
            else if (!aiShootSelf && hit.CompareTag("Enemy"))
            {
                target = "Enemy";
            }
        }

        // Apply health damage and handle turn logic
        if (gameManager != null)
        {
            if (target == "AI")
            {
                gameManager.ModifyHealth(false, isLive ? -20 : 0);
                Debug.Log($"AI shot itself. Result: {(isLive ? "LIVE shot, 20 damage" : "BLANK shot, no damage")}");
            }
            else if (target == "Player")
            {
                gameManager.ModifyHealth(true, isLive ? -20 : 0);
                Debug.Log($"AI shot the player. Result: {(isLive ? "LIVE shot, 20 damage" : "BLANK shot, no damage")}");
            }
            else if (target == "Enemy")
            {
                gameManager.ModifyHealth(false, isLive ? -20 : 0);
                Debug.Log($"Player shot the enemy. Result: {(isLive ? "LIVE shot, 20 damage" : "BLANK shot, no damage")}");
            }
            else
            {
                Debug.LogWarning("Shot missed! No valid target. Check collider alignment or target tags.");
            }
        }

        // Mark the bullet as used
        chambers[currentChamber] = false; // Remove the bullet (live or blank) from the chamber
        Debug.Log($"Bullet removed from chamber {currentChamber}. Remaining chambers: {GetRemainingBullets()}");

        currentChamber = (currentChamber + 1) % totalBullets; // Increment the chamber after every shot
        bulletsFired++;

        // Confirm bullet consumption during AI turns
        Debug.Log($"Current chamber after shot: {currentChamber}. Bullets fired: {bulletsFired}");

        // Place back and manage turn
        if (!isLive && target == "Player")
        {
            Debug.Log("Player shot themselves with a BLANK. Player gets another turn.");
            gameManager.EndTurn(true); // Player gets another turn
        }
        else
        {
            PlaceBackOnTable();
            gameManager.EndTurn(); // End turn for live shot or other scenarios
        }
    }

    public void SetupChambers()
    {
        bulletsFired = 0; // Reset fired count
        chambers = new bool[totalBullets];

        // Randomize the number of live and blank bullets
        int blanks = Random.Range(1, totalBullets);
        int liveBullets = totalBullets - blanks;

        // Populate chambers
        for (int i = 0; i < blanks; i++) chambers[i] = false; // Blank bullets
        for (int i = blanks; i < totalBullets; i++) chambers[i] = true; // Live bullets

        // Shuffle the chambers
        ShuffleChambers();

        currentChamber = 0; // Start at the first chamber
        Debug.Log($"Revolver setup: {liveBullets} live bullets, {blanks} blank bullets.");
    }

    private void ShuffleChambers()
    {
        // Fisher-Yates shuffle
        System.Random rng = new System.Random();
        for (int i = chambers.Length - 1; i > 0; i--)
        {
            int swapIndex = rng.Next(i + 1);
            bool temp = chambers[i];
            chambers[i] = chambers[swapIndex];
            chambers[swapIndex] = temp;
        }
        Debug.Log("Chambers shuffled.");
    }

    private void OnPickup(SelectEnterEventArgs args)
    {
        PlayPickupSound();
        Debug.Log("Player picked up the revolver.");
    }

    public void PlaceBackOnTable()
    {
        if (gameManager != null)
        {
            Transform revolverSpawnPoint = gameManager.revolverSpawnPoint;
            transform.position = revolverSpawnPoint.position;
            transform.rotation = revolverSpawnPoint.rotation;

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = true;
                rb.constraints = RigidbodyConstraints.None;
            }

            EnablePickup(false);
            Debug.Log("Revolver placed back on the table.");
        }
    }

    public void PlayPickupSound()
    {
        if (audioSource != null && pickupSound != null)
        {
            audioSource.PlayOneShot(pickupSound);
            Debug.Log("Pickup sound played.");
        }
    }

    public void EnablePickup(bool enable)
    {
        if (grabInteractable != null)
        {
            grabInteractable.enabled = enable;
        }
    }

    private int GetRemainingBullets()
    {
        int remaining = 0;
        foreach (bool chamber in chambers)
        {
            if (chamber) remaining++;
        }
        return remaining;
    }
    public void ConsumeBullet()
    {
        // Mark the current chamber as empty and increment to the next chamber
        chambers[currentChamber] = false; // Consume the bullet
        Debug.Log($"Bullet in chamber {currentChamber} consumed.");
        currentChamber = (currentChamber + 1) % totalBullets; // Move to the next chamber
        bulletsFired++;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) // Check if the object has the "Ground" tag
        {
            Debug.Log("Revolver touched the ground. Returning to the table.");
            ReturnToTable();
        }
    }

    public void ReturnToTable()
    {
        if (gameManager != null)
        {
            Transform spawnPoint = gameManager.revolverSpawnPoint;
            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero; // Reset velocity to prevent sliding
                rb.angularVelocity = Vector3.zero; // Reset angular velocity
            }

            Debug.Log("Revolver returned to the table.");
        }
    }

}
