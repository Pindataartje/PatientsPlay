using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Linq;

public class Revolver : MonoBehaviour
{
    public int totalBullets = 6;
    private bool[] chambers; // true = live bullet, false = blank
    private int currentChamber;
    private bool canBePickedUp = false;
    public bool canShoot = false; // Bool to control if the player can shoot

    private AudioSource audioSource;
    public AudioClip blankShotSound;
    public AudioClip liveShotSound;
    public AudioClip reloadSound;
    public AudioClip pickupSound;

    private XRGrabInteractable grabInteractable;
    private GameManager gameManager;

    public Animator animator; // Reference to the Animator component

    private int bulletsFired = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        grabInteractable = GetComponent<XRGrabInteractable>();
        gameManager = FindAnyObjectByType<GameManager>(); // Updated for deprecated method

        animator = GetComponent<Animator>(); // Get the Animator component

        SetupChambers();
        grabInteractable.selectEntered.AddListener(OnPickup);
    }

    public void FireAction(bool aiShootSelf = false)
    {
        if (bulletsFired >= totalBullets)
        {
            Debug.LogWarning("No more bullets left to fire! Start a new round.");
            return;
        }

        if (chambers == null || chambers.Length == 0)
        {
            Debug.LogWarning("Revolver not loaded properly!");
            return;
        }

        if (canShoot)
        {
            // Trigger the shoot animation
            if (animator != null)
            {
                animator.SetTrigger("Shoot");
            }

            bool isLive = chambers[currentChamber];
            audioSource.PlayOneShot(isLive ? liveShotSound : blankShotSound);
            Debug.Log(isLive ? "Shot was LIVE." : "Shot was BLANK.");
            currentChamber = (currentChamber + 1) % totalBullets;
            bulletsFired++;

            // Delay placing the revolver back on the table to allow animation to play
            Invoke(nameof(PlaceBackOnTable), 1.0f);
        }
    }

    private void PlaceBackOnTable()
    {
        if (gameManager != null)
        {
            // Set revolver back to the table spawn point
            Transform revolverSpawnPoint = gameManager.revolverSpawnPoint;
            transform.position = revolverSpawnPoint.position;
            transform.rotation = revolverSpawnPoint.rotation;

            // Forcefully detach from player's hand
            if (grabInteractable.isSelected)
            {
                grabInteractable.interactionManager.SelectExit(grabInteractable.firstInteractorSelecting, grabInteractable);
            }

            // Disable pickup temporarily to avoid instant re-grab
            EnablePickup(false);
            Debug.Log("Revolver placed back on the table.");

            // Re-enable pickup after a short delay
            Invoke(nameof(EnableRevolverPickup), 0.5f);
        }
    }

    private void EnableRevolverPickup()
    {
        EnablePickup(true);
    }

    public void SetupChambers()
    {
        bulletsFired = 0;
        Debug.Log("Setting up chambers for new round.");

        chambers = new bool[totalBullets];
        int blanks = Random.Range(1, totalBullets);
        int liveBullets = totalBullets - blanks;

        // Populate chambers with blanks and live bullets
        for (int i = 0; i < blanks; i++)
        {
            chambers[i] = false; // Blank
        }
        for (int i = blanks; i < totalBullets; i++)
        {
            chambers[i] = true; // Live bullet
        }

        // Shuffle chambers using a custom shuffle method
        ShuffleChambers();

        currentChamber = 0;

        Debug.Log($"Revolver setup complete: {liveBullets} live bullets, {blanks} blank bullets.");
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
    }

    public void EnablePickup(bool enable)
    {
        canBePickedUp = enable;
        UpdatePickupState();
    }

    private void UpdatePickupState()
    {
        if (grabInteractable != null)
        {
            grabInteractable.enabled = canBePickedUp;
        }
    }

    private void OnPickup(SelectEnterEventArgs args)
    {
        PlayPickupSound();
        Debug.Log("Player picked up the revolver.");
    }

    public void PlayPickupSound()
    {
        audioSource.PlayOneShot(pickupSound);
        Debug.Log("Played pickup sound.");
    }
}
