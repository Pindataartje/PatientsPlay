using UnityEngine;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainMenuCanvas;
    public GameObject playerWheelchair;
    public GameObject opponentWheelchair;
    public Transform[] playerWaypoints; // Array of waypoints for player movement
    public Transform[] opponentWaypoints; // Array of waypoints for opponent movement

    public AudioClip transitionSound;
    private AudioSource audioSource;

    // Booleans to trigger actions for testing
    public bool startGameTrigger = false;
    public bool startGamePhaseTrigger = false;
    public bool moveWheelchairsTrigger = false;

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

        if (moveWheelchairsTrigger)
        {
            moveWheelchairsTrigger = false;
            StartCoroutine(MoveWheelchairs());
        }
    }

    public void StartGame()
    {
        mainMenuCanvas.SetActive(false);
        audioSource.PlayOneShot(transitionSound);
        StartCoroutine(MoveWheelchairs());
    }

    private IEnumerator MoveWheelchairs()
    {
        // Move player and opponent simultaneously
        for (int i = 0; i < playerWaypoints.Length; i++)
        {
            float duration = 5f; // Adjust for how long the movement should take between waypoints
            Vector3 playerStart = playerWheelchair.transform.position;
            Vector3 playerEnd = playerWaypoints[i].position;
            Vector3 opponentStart = opponentWheelchair.transform.position;
            Vector3 opponentEnd = opponentWaypoints[i].position;

            float elapsed = 0f;
            while (elapsed < duration)
            {
                playerWheelchair.transform.position = Vector3.Lerp(playerStart, playerEnd, elapsed / duration);
                opponentWheelchair.transform.position = Vector3.Lerp(opponentStart, opponentEnd, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        StartGamePhase();
    }

    public void StartGamePhase()
    {
        Debug.Log("Arrived at morgue, game starts now!");
        // Initialize GameManager to start managing the gameplay
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.StartGame();
        }
    }
}
