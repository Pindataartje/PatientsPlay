using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Linq;
public class Revolver : MonoBehaviour
{
    public int totalBullets = 6;
    private bool[] chambers; // true = live bullet, false = blank
    private int currentChamber;
    private bool canBePickedUp = false;

    private AudioSource audioSource;
    public AudioClip blankShotSound;
    public AudioClip liveShotSound;

    private XRGrabInteractable grabInteractable;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        grabInteractable = GetComponent<XRGrabInteractable>();
        UpdatePickupState();
    }

    public void SetupChambers()
    {
        chambers = new bool[totalBullets];
        int blanks = Random.Range(1, totalBullets);
        int liveBullets = totalBullets - blanks;

        for (int i = 0; i < blanks; i++)
        {
            chambers[i] = false; // Blank
        }
        for (int i = blanks; i < totalBullets; i++)
        {
            chambers[i] = true; // Live bullet
        }

        // Shuffle chambers
        System.Random rng = new System.Random();
        chambers = chambers.OrderBy(x => rng.Next()).ToArray();
        currentChamber = 0;
    }

    public bool Fire()
    {
        bool isLive = chambers[currentChamber];
        audioSource.PlayOneShot(isLive ? liveShotSound : blankShotSound);
        currentChamber = (currentChamber + 1) % totalBullets;
        return isLive;
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
}