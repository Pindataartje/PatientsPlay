using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
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

    public Transform barrelEnd; // Reference to the end of the barrel where the ray should be fired from

    // Layer mask for Player and Enemy layers
    private int layerMask;

    private int bulletsFired = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        grabInteractable = GetComponent<XRGrabInteractable>();
        gameManager = FindObjectOfType<GameManager>(); // Find the GameManager in the scene

        // Setup layer mask to include only Player and Enemy layers
        layerMask = LayerMask.GetMask("Player", "Enemy");

        UpdatePickupState();
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
            bool isLive = chambers[currentChamber];
            audioSource.PlayOneShot(isLive ? liveShotSound : blankShotSound);
            Debug.Log(isLive ? "Shot was LIVE." : "Shot was BLANK.");
            currentChamber = (currentChamber + 1) % totalBullets;
            bulletsFired++;

            // Perform raycast to determine hit
            Vector3 rayOrigin = barrelEnd != null ? barrelEnd.position : transform.position;
            Ray ray = new Ray(rayOrigin, transform.forward);
            RaycastHit hit;

            // Draw the ray for visualization purposes
            Debug.DrawRay(ray.origin, ray.direction * 50f, Color.red, 5.0f); // Draw the ray in red for 5 seconds

            // Use layer mask to only hit Player, Enemy, or Nothing layers
            if (Physics.Raycast(ray, out hit, 50f, layerMask, QueryTriggerInteraction.Ignore))
            {
                Debug.Log($"Shot hit: {hit.collider.name}");

                if (isLive)
                {
                    if (hit.collider.CompareTag("Enemy"))
                    {
                        Debug.Log("Live shot hit the Enemy (AI).");
                        HeartbeatMonitor enemyMonitor = hit.collider.GetComponent<HeartbeatMonitor>();
                        if (enemyMonitor != null)
                        {
                            enemyMonitor.ModifyHeartbeat(-60);
                        }
                    }
                    else if (hit.collider.CompareTag("Player"))
                    {
                        Debug.Log("Live shot hit the Player.");
                        HeartbeatMonitor playerMonitor = hit.collider.GetComponent<HeartbeatMonitor>();
                        if (playerMonitor != null)
                        {
                            playerMonitor.ModifyHeartbeat(-60);
                        }
                    }
                    else if (hit.collider.CompareTag("Nothing"))
                    {
                        Debug.Log("Live shot hit an object with 'Nothing' tag, ending turn.");
                    }
                    else
                    {
                        Debug.Log("Live shot hit something else, ending turn.");
                    }
                }
                else
                {
                    Debug.Log("Blank shot was fired.");
                    if (hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Player"))
                    {
                        Debug.Log("Blank shot landed, player keeps their turn.");
                    }
                    else
                    {
                        Debug.Log("Blank shot missed, ending player's turn.");
                    }
                }
            }
            else
            {
                Debug.Log("Shot missed, ending player's turn.");
            }

            PlaceBackOnTable();
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

        // Shuffle chambers to randomize bullet positions
        System.Random rng = new System.Random();
        chambers = chambers.OrderBy(x => rng.Next()).ToArray();
        currentChamber = 0;

        Debug.Log($"Revolver setup complete: {liveBullets} live bullets, {blanks} blank bullets.");
    }

    public void Reload()
    {
        audioSource.PlayOneShot(reloadSound);
        SetupChambers();
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
        audioSource.PlayOneShot(pickupSound);
    }
}
