using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Revolver : MonoBehaviour
{
    [Header("Revolver Settings")]
    public int totalBullets = 6;
    private bool[] chambers;
    private int currentChamber;
    public int bulletsFired;

    [Header("Shooting Logic")]
    public Collider shootingTrigger;
    bool isLive;
    bool aimingOnTarget;
    bool targetIsPlayer;
    bool targetIsEnemy;
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

    public bool[] Chambers => chambers;
    public int CurrentChamber => currentChamber;

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
            Debug.LogWarning("All bullets fired! Resetting for a new round.");
            SetupChambers(); // Reset chambers for a new round
            gameManager.EndTurn(); // Continue with the turn logic
            return;
        }

        if (chambers == null || chambers.Length == 0)
        {
            Debug.LogWarning("Revolver is not loaded.");
            return;
        }

        animator?.SetTrigger("Shoot");

        isLive = chambers[currentChamber];
        audioSource?.PlayOneShot(isLive ? liveShotSound : blankShotSound);
        Debug.Log(isLive ? "Shot was LIVE." : "Shot was BLANK.");

        if (aimingOnTarget)
        {
            if (targetIsPlayer)
            {
                PlayerShot();
            }
            if (targetIsEnemy)
            {
                EnemyShot();
            }
        }
        else
        {
            gameManager.EndTurn();
            Debug.Log("Shot missed! No valid target.");
        }

        //string target = "None";
        //Collider[] hitTargets = Physics.OverlapBox(
        //    shootingTrigger.bounds.center,
        //    shootingTrigger.bounds.extents,
        //    shootingTrigger.transform.rotation
        //);

        //foreach (var hit in hitTargets)
        //{
        //    // Detect "Player" and "Enemy" tags
        //    if (hit.CompareTag("Player"))
        //    {
        //        target = "Player";
        //        break;
        //    }
        //    else if (hit.CompareTag("Enemy"))
        //    {
        //        target = "Enemy";
        //        break;
        //    }
        //    else
        //    {
        //        target = "None";
        //        break;
        //    }
        //}

        //// Process results based on target and bullet type
        //if (gameManager != null)
        //{
        //    if (target == "Player")
        //    {
        //        gameManager.ModifyHealth(true, isNextLive ? -20 : 0);
        //        Debug.Log($"Player shot themselves. Result: {(isNextLive ? "LIVE shot, 20 damage" : "BLANK shot, no damage")}");

        //        // Allow another turn if it's a blank
        //        if (!isNextLive)
        //        {
        //            Debug.Log("Blank shot! Player gets another turn.");
        //            gameManager.EndTurn(true); // Retain the turn for the player
        //            ConsumeBullet();
        //            return;
        //        }
        //    }
        //    else if (target == "Enemy")
        //    {
        //        gameManager.ModifyHealth(false, isNextLive ? -20 : 0);
        //        Debug.Log($"Player shot the enemy. Result: {(isNextLive ? "LIVE shot, 20 damage" : "BLANK shot, no damage")}");
        //    }
        //    else
        //    {
        //        Debug.Log("Shot missed! No valid target.");
        //        Debug.Log(target);
        //    }
        //}

        if (bulletsFired >= totalBullets)
        {
            Debug.Log("All bullets used. Starting a new round.");
            SetupChambers(); // Reset for a new round
        }
    }


    public void SetupChambers()
    {
        bulletsFired = 0;
        chambers = new bool[totalBullets];

        int blanks = Random.Range(1, totalBullets);
        int liveBullets = totalBullets - blanks;

        for (int i = 0; i < blanks; i++) chambers[i] = false;
        for (int i = blanks; i < totalBullets; i++) chambers[i] = true;

        ShuffleChambers();

        currentChamber = 0;
        Debug.Log($"Revolver setup: {liveBullets} live bullets, {blanks} blank bullets.");
    }

    private void ShuffleChambers()
    {
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

    public int GetRemainingBullets()
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
        if (chambers == null || chambers.Length == 0)
        {
            Debug.LogWarning("Chambers are not initialized.");
            return;
        }

        Debug.Log($"Consuming bullet in chamber {currentChamber} (Live: {chambers[currentChamber]})");
        chambers[currentChamber] = false; // Mark the bullet as used
        currentChamber = (currentChamber + 1) % totalBullets; // Increment the chamber
        bulletsFired++;
        Debug.Log($"Bullet consumed. Current chamber: {currentChamber}, Bullets fired: {bulletsFired}");
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
    public void OnTriggerEnter(Collider other)
    {
        aimingOnTarget = true;

        if(other.tag == "Player")
        {
            targetIsPlayer = true;
        }
        if (other.tag == "Enemy")
        {
            targetIsEnemy = true;
        }
    }
    public void OnTriggerStay(Collider other)
    {
        aimingOnTarget = true;

        if (other.tag == "Player")
        {
            targetIsPlayer = true;
        }
        if (other.tag == "Enemy")
        {
            targetIsEnemy = true;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            aimingOnTarget = false;
            targetIsPlayer = false;
        }
        if (other.tag == "Enemy")
        {
            aimingOnTarget = false;
            targetIsEnemy = false;
        }
    }
    void PlayerShot()
    {
        ConsumeBullet();

        if (gameManager.IsPlayerTurn)
        {
            if (!isLive)
            {
                gameManager.EndTurn(true);
            }
            else
            {
                gameManager.ModifyHealth(true, -20);
                gameManager.EndTurn();
            }
        }
        else
        {
            if (!isLive)
            {
                gameManager.EndTurn(true);
            }
            else
            {
                gameManager.ModifyHealth(true, -20);
                gameManager.EndTurn(true);
            }
        }
    }
    void EnemyShot()
    {
        ConsumeBullet();

        if (gameManager.IsPlayerTurn)
        {
            if (!isLive)
            {
                gameManager.EndTurn();
            }
            else
            {
                gameManager.ModifyHealth(false, -20);
                gameManager.EndTurn();
            }
        }
    }
}
