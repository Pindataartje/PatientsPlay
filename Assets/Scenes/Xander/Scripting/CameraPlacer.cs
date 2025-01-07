using System.Collections;
using UnityEngine;

public class CameraPlacer : MonoBehaviour
{
    [SerializeField] private Vector3 targetPosition = new Vector3(0f, 1.2f, 0f);
    [SerializeField] private float delay = 0.1f; // Delay in seconds before positioning

    void Start()
    {
        // Start the coroutine to ensure delayed positioning
        StartCoroutine(PlaceCameraAfterDelay());
    }

    IEnumerator PlaceCameraAfterDelay()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Place the camera at the desired position
        transform.position = targetPosition;

        Debug.Log($"Camera repositioned to {targetPosition} after {delay} seconds.");
    }
}
