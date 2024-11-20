using UnityEngine;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainMenuCanvas;
    public GameObject wheelchair;
    public Transform[] waypoints; // Array of waypoints for wheelchair movement

    public AudioClip transitionSound;
    private AudioSource audioSource;

    // Booleans to trigger actions for testing
    public bool startGameTrigger = false;
    public bool startGamePhaseTrigger = false;
    public bool moveWheelchairTrigger = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        mainMenuCanvas.SetActive(true);
    }

    void Update()
    {
        // Start game with the primary button on the right controller
        if (Input.GetKeyDown(KeyCode.Space)) // Replace OVRInput with a simple key press for testing
        {
            StartGame();
        }

        // Trigger actions based on boolean values for testing in the Inspector
        if (startGameTrigger)
        {
            startGameTrigger = false;
            StartGame();
        }

        if (startGamePhaseTrigger)
        {
            startGamePhaseTrigger = false;
            StartGamePhase();
        }

        if (moveWheelchairTrigger)
        {
            moveWheelchairTrigger = false;
            StartCoroutine(MoveWheelchair());
        }
    }

    public void StartGame()
    {
        mainMenuCanvas.SetActive(false);
        audioSource.PlayOneShot(transitionSound);
        StartCoroutine(MoveWheelchair());
    }

    private IEnumerator MoveWheelchair()
    {
        foreach (Transform waypoint in waypoints)
        {
            float duration = 5f; // Adjust for how long the movement should take between waypoints
            Vector3 start = wheelchair.transform.position;
            Vector3 end = waypoint.position;

            float elapsed = 0f;
            while (elapsed < duration)
            {
                wheelchair.transform.position = Vector3.Lerp(start, end, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        StartGamePhase();
    }

    public void StartGamePhase()
    {
        Debug.Log("Arrived at morgue, game starts now!");
        // Initialize TurnManager to start the Russian roulette gameplay
        TurnManager turnManager = FindObjectOfType<TurnManager>();
        if (turnManager != null)
        {
            turnManager.StartPlayerTurn();
        }
    }
}