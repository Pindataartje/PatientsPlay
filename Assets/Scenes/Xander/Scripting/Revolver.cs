using UnityEngine;
using System.Linq;

public class Revolver : MonoBehaviour
{
    public int totalBullets = 6;
    public int blanks;
    public int liveBullets;

    private int currentChamber;
    private bool[] chambers; // true = live bullet, false = blank
    private AudioSource audioSource;
    public AudioClip blankShotSound;
    public AudioClip liveShotSound;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        SetupChambers();
    }

    void Update()
    {
        // Use item (fire the gun) with the front trigger button
        if (Input.GetMouseButtonDown(0)) // Replace OVRInput with a simple mouse button press for testing
        {
            Fire();
        }
    }

    void SetupChambers()
    {
        chambers = new bool[totalBullets];
        blanks = Random.Range(1, totalBullets);
        liveBullets = totalBullets - blanks;

        // Randomly assign blanks and live bullets
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
}
